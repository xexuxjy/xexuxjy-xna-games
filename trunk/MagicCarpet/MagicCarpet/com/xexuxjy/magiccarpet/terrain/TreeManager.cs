using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet;
using com.xexuxjy.magiccarpet.manager;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.renderer;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TreeManager : EmptyGameObject
    {

        public TreeManager()
            : base(GameObjectType.treemanager)
        {
        }

        public override void Initialize()
        {
            m_terrainEffect = Globals.MCContentManager.GetEffect("Terrain");
            m_treeTexture = Globals.MCContentManager.GetTexture("TreeBillboard");
            BuildTreeMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////


        public void BuildTreeMap()
        {
            // Based on example by Reimer at http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Region_growing.php
            int noiseTextureWidth = 128;

            RenderTarget2D treeTarget = new RenderTarget2D(Globals.GraphicsDevice, noiseTextureWidth, noiseTextureWidth, false, SurfaceFormat.Color, DepthFormat.Depth16);
            PerlinNoiseGenerator.GeneratePerlinNoise(noiseTextureWidth, treeTarget);
            Color[] pixels = new Color[noiseTextureWidth * noiseTextureWidth];
            treeTarget.GetData<Color>(pixels);
            int[,] noiseData = new int[noiseTextureWidth, noiseTextureWidth];
            for (int x = 0; x < noiseTextureWidth; x++)
            {
                for (int y = 0; y < noiseTextureWidth; y++)
                {
                    noiseData[x, y] = pixels[y + x * noiseTextureWidth].R;
                }
            }


            int stepSize = 4;


            Vector3 halfWidth = new Vector3(Globals.WorldWidth, 0, Globals.WorldWidth) / 2f;

            for (int z = 0; z < Globals.WorldWidth; z += stepSize)
            {
                for (int x = 0; x < Globals.WorldWidth; x += stepSize)
                {
                    //float height = GetHeightAtPointLocal(x, z);
                    float height = 0;
                    if (height > -4 && height < 20)
                    {
                        float relZ = (float)z / (float)Globals.WorldWidth;
                        float relX = (float)x / (float)Globals.WorldWidth;

                        float treeDensity = 0f;
                        float noiseAtPoint = noiseData[(int)(relX * noiseTextureWidth), (int)(relZ * noiseTextureWidth)];
                        if (noiseAtPoint > 200)
                        {
                            treeDensity = 5;
                        }
                        else if (noiseAtPoint > 150)
                        {
                            treeDensity = 4;
                        }
                        else if (noiseAtPoint > 100)
                        {
                            treeDensity = 3;
                        }
                        else
                        {
                            treeDensity = 0;
                        }


                        for (int currDetail = 0; currDetail < treeDensity; currDetail++)
                        {
                            float rand1 = (float)Globals.s_random.Next(1000) / 1000.0f;
                            float rand2 = (float)Globals.s_random.Next(1000) / 1000.0f;
                            float scale = (float)Globals.s_random.NextDouble();
                            //worldPosition
                            Vector3 tempPos = new Vector3((float)x - rand1, height, (float)z - rand2);
                            tempPos -= halfWidth;
                            Vector4 treePos = new Vector4(tempPos, scale);
                            m_treePositions.Add(treePos);
                        }
                    }

                }

            }
            CreateBillboardVerticesFromList(m_treePositions, out m_treeVertexBuffer);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void CreateBillboardVerticesFromList(List<Vector4> list, out VertexBuffer vertexBuffer)
        {
            int i = 0;

            float width = 1f;
            float height = 10f;
            // rotate through half to get a checkboard.
            //Matrix rotation = Matrix.CreateRotationY(MathUtil.SIMD_PI);


            int numIterations = 3;
            float stepSize = MathUtil.SIMD_2_PI / numIterations;

            VertexPositionScaleTexture[] billboardVertices = new VertexPositionScaleTexture[list.Count * 6 * numIterations];

            foreach (Vector4 currentV4 in list)
            {
                Vector3 left = new Vector3(-width, 0, 0);
                Vector3 right = new Vector3(width, 0, 0);
                Vector3 baseUp = new Vector3(0, height, 0);

                float scale2 = currentV4.W * (float)BulletGlobals.gRandom.NextDouble();

                left *= scale2;
                right *= scale2;
                Vector3 v = new Vector3(currentV4.X, currentV4.Y, currentV4.Z);
                float scale = currentV4.W;
                Vector3 up = baseUp * scale;


                // pick a random rotation for each tree as a base...

                float baseAngle = MathUtil.SIMD_2_PI * (float)Globals.s_random.NextDouble();


                // FIXME - update this to allow multiple textures to provide a more interesting looking tree..



                for (int j = 0; j < numIterations; ++j)
                {
                    Matrix rotation = Matrix.CreateRotationY(baseAngle + (j * stepSize));

                    billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(left, rotation), scale, new Vector2(0, 1));
                    billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(right, rotation), scale, new Vector2(1, 1));
                    billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(right + up, rotation), scale, new Vector2(1, 0));

                    billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(right + up, rotation), scale, new Vector2(1, 0));
                    billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(left + up, rotation), scale, new Vector2(0, 0));
                    billboardVertices[i++] = new VertexPositionScaleTexture(v + Vector3.TransformNormal(left, rotation), scale, new Vector2(0, 1));
                }
            }

            vertexBuffer = new VertexBuffer(Globals.GraphicsDevice, VertexPositionScaleTexture.VertexDeclaration, billboardVertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(billboardVertices);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            DrawTrees();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////
        
        public void DrawTrees()
        {
            m_terrainEffect.CurrentTechnique = m_terrainEffect.Techniques["BillboardTrees"];
            Globals.GraphicsDevice.SetVertexBuffer(m_treeVertexBuffer);

            RasterizerState oldState = Globals.GraphicsDevice.RasterizerState;
            //Globals.GraphicsDevice.RasterizerState = m_noCullState;

            Vector3 startPosition = Vector3.Zero;//new Vector3(-Globals.WorldWidth / 2f, 0, -Globals.WorldWidth / 2f);
            Matrix transform = Matrix.CreateTranslation(startPosition);
            Globals.MCContentManager.ApplyCommonEffectParameters(m_terrainEffect);

            m_terrainEffect.Parameters["WorldMatrix"].SetValue(transform);
            m_terrainEffect.Parameters["TreeTexture"].SetValue(m_treeTexture);

            //float oneOverTextureWidth = 1f / (m_textureWidth - 1);
            //m_terrainEffect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(oneOverTextureWidth, oneOverTextureWidth, 0, 0));

            //BlendState oldBlendState = Globals.GraphicsDevice.BlendState;
            //Globals.GraphicsDevice.BlendState = BlendState.AlphaBlend;


            foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
            {
                int noVertices = m_treeVertexBuffer.VertexCount;
                int noTriangles = noVertices / 3;
                pass.Apply();
                Globals.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
            }

            //Globals.GraphicsDevice.RasterizerState = oldState;
            //Globals.GraphicsDevice.BlendState = oldBlendState;          
        }

        private List<Vector4> m_treePositions = new List<Vector4>();
        private Texture2D m_treeTexture;
        private Effect m_terrainEffect;
        private VertexBuffer m_treeVertexBuffer;

    }
}
