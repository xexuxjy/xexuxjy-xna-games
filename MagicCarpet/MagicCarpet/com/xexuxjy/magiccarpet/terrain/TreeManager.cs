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
using LTreesLibrary.Trees;
using BulletXNA.LinearMath;

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
            List<Vector4> treePositions = new List<Vector4>();
            BuildTreeMap(treePositions);
            CreateBillboardVerticesFromList(treePositions, out m_treeVertexBuffer);
            BuildPrettyTrees(treePositions);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        // first pass brute force.
        public override void Update(GameTime gameTime)
        {
        }




        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region PrettyTrees

        public void BuildPrettyTrees(List<Vector4> treePositions)
        {
            //m_simpleTree = Globals.MCContentManager.GetSimpleTree("Pine");
            m_simpleTree = Globals.MCContentManager.GetSimpleLowPolyTree("Pine",4); 

            m_instanceTrunkEffect = Globals.MCContentManager.GetEffect("InstancedTree");
            m_simpleTree.TrunkEffect = m_instanceTrunkEffect;
            m_simpleTree.LeafEffect = m_instanceTrunkEffect;

            //m_instanceTreeMatrices = new Matrix[m_treePositions.Count];

            BoundingSphere bs = m_simpleTree.TrunkMesh.BoundingSphere;
            float desiredSize = 3f;
            float scaledSize = desiredSize / bs.Radius;

            for (int i = 0; i < m_bindingMatrices.Length; ++i)
            {
                m_bindingMatrices[i] = Matrix.Identity;
            }

            for (int i = 0; i < m_bindingQuaternions.Length; ++i)
            {
                m_bindingQuaternions[i] = Quaternion.Identity;
            }


            int counter = 0;
            foreach (Vector4 currentV4 in treePositions)
            {

                float scale2 = currentV4.W;// *(float)BulletGlobals.gRandom.NextDouble();

                Vector3 v = new Vector3(currentV4.X, currentV4.Y, currentV4.Z);

                // pick a random rotation for each tree as a base...

                float baseAngle = MathUtil.SIMD_2_PI * (float)Globals.s_random.NextDouble();
                Matrix rotationMatrix = Matrix.CreateRotationY(baseAngle);
                Matrix scaleMatrix = Matrix.CreateScale(scale2 * scaledSize);

                Matrix resultMatrix = scaleMatrix * rotationMatrix;
                resultMatrix.Translation = v;

                TreeBounds treeBounds = new TreeBounds(resultMatrix);

                m_treeBoundsQuadTree.AddObject(treeBounds);

            }

            //m_instanceVertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, s_instanceVertexDeclaration,
            //                                                   500, BufferUsage.WriteOnly);
            //m_instanceVertexBuffer.SetData<Matrix>(m_instanceTreeMatrices, 0, m_instanceTreeMatrices.Length,SetDataOptions.Discard);

            //m_instanceVertexBuffer.SetData(0, m_instanceTreeMatrices, 0, m_instanceTreeMatrices.Length, s_instanceVertexDeclaration.VertexStride);

        }


        public void DrawPrettyTrees()
        {
            if (Globals.Terrain.HeightMapTexture == null)
            { 
                return;
            }
            

            Globals.MCContentManager.SaveBlendState();

            Globals.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            Globals.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Set up the instance rendering effect.
            Effect effect = m_simpleTree.TrunkEffect;

            effect.CurrentTechnique = effect.Techniques["TrunkHardwareInstancing"];

            Globals.MCContentManager.ApplyCommonEffectParameters(m_instanceTrunkEffect);

            effect.Parameters["Texture"].SetValue(m_simpleTree.TrunkTexture);
            effect.Parameters["LeafTexture"].SetValue(m_simpleTree.LeafTexture);

            m_simpleTree.Skeleton.CopyBoneBindingMatricesTo(m_bindingMatrices, m_bindingQuaternions);
            effect.Parameters["Bones"].SetValue(m_bindingMatrices);

            effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            effect.Parameters["HeightMapTexelWidth"].SetValue(Globals.Terrain.OneOverTextureWidth);
            effect.Parameters["HeightMapTexture"].SetValue(Globals.Terrain.HeightMapTexture);

            // Draw trunks....

            bool drawTrunks = true;

            if (drawTrunks)
            {
                float visibiltyDistance2 = 400;

                m_instanceTreeMatricesList.Clear();

                List<QuadTree<TreeBounds>> activeBounds = m_treeBoundsQuadTree.GetLeavesInsideFrustrum(Globals.s_currentCameraFrustrum);
                foreach (QuadTree<TreeBounds> quadTreeNode in activeBounds)
                {
                    foreach (TreeBounds treeBounds in quadTreeNode.Objects)
                    {
                        m_instanceTreeMatricesList.Add(treeBounds.m_matrix);
                    }
                }

                if (m_instanceTreeMatricesList.Count > 0)
                {

                    // If we have more instances than room in our vertex buffer, grow it to the neccessary size.
                    if ((m_instanceVertexBuffer == null) ||
                        (m_instanceTreeMatricesList.Count > m_instanceVertexBuffer.VertexCount))
                    {
                        if (m_instanceVertexBuffer != null)
                        {
                            m_instanceVertexBuffer.Dispose();
                        }

                        m_instanceVertexBuffer = new DynamicVertexBuffer(Globals.GraphicsDevice, s_instanceVertexDeclaration,
                                                                       m_instanceTreeMatricesList.Count, BufferUsage.WriteOnly);
                    }


                    // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                    Globals.GraphicsDevice.SetVertexBuffers(
                        new VertexBufferBinding(m_simpleTree.TrunkMesh.VertexBuffer, 0, 0),
                        new VertexBufferBinding(m_instanceVertexBuffer, 0, 1)
                    );

                    Globals.GraphicsDevice.Indices = m_simpleTree.TrunkMesh.IndexBuffer;

                    
                    m_instanceVertexBuffer.SetData<Matrix>(m_instanceTreeMatricesList.GetRawArray(), 0, m_instanceTreeMatricesList.Count, SetDataOptions.Discard);
                    //m_instanceVertexBuffer.SetData<Matrix>(m_instanceTreeMatricesList.GetRawArray(), 0, 1, SetDataOptions.Discard);


                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Globals.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               m_simpleTree.TrunkMesh.NumberOfVertices, 0,
                                                               m_simpleTree.TrunkMesh.NumberOfTriangles, m_instanceTreeMatricesList.Count);
                    }
                }
            }
            bool drawLeaves = false;
            if (drawLeaves)
            {
                // And leaves
                effect.CurrentTechnique = effect.Techniques["LeafHardwareInstancing"];
                effect.Parameters["LeafScale"].SetValue(0.1f);
                effect.Parameters["Bones"].SetValue(m_bindingMatrices);

                effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);

                effect.Parameters["BillboardRight"].SetValue(Vector3.Right);
                effect.Parameters["BillboardUp"].SetValue(Vector3.Up);


                // Tell the GPU to read from both the model vertex buffer plus our instanceVertexBuffer.
                Globals.GraphicsDevice.SetVertexBuffers(
                    new VertexBufferBinding(m_simpleTree.LeafCloud.VertexBuffer, 0, 0),
                    new VertexBufferBinding(m_instanceVertexBuffer, 0, 1)
                );

                Globals.GraphicsDevice.Indices = m_simpleTree.LeafCloud.IndexBuffer;


                try
                {
                    // Draw all the instance copies in a single call.
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();

                        Globals.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0,
                                                               m_simpleTree.LeafCloud.NumberOfVertices, 0,
                                                               m_simpleTree.LeafCloud.NumberOfTriangles, m_instanceTreeMatricesList.Count);
                    }
                }
                catch (System.Exception ex)
                {
                    int ibreak = 0;                
                }

            }
            effect.Parameters["HeightMapTexture"].SetValue((Texture)null);

            Globals.MCContentManager.RestoreBlendState();
        }

        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////
        #region BillboardTrees
        public void BuildTreeMap(List<Vector4> treePositions)
        {
            // Based on example by Reimer at http://www.riemers.net/eng/Tutorials/XNA/Csharp/Series4/Region_growing.php
            int noiseTextureWidth = 64;

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

                        treeDensity *= 10;


                        for (int currDetail = 0; currDetail < treeDensity; currDetail++)
                        {
                            //float rand1 = (float)Globals.s_random.Next(1000) / 1000.0f;
                            //float rand2 = (float)Globals.s_random.Next(1000) / 1000.0f;
                            float rand1 = (float)Globals.s_random.NextDouble() * stepSize;
                            float rand2 = (float)Globals.s_random.NextDouble() * stepSize;

                            rand1 *= Globals.s_random.NextDouble() < 0.5 ? 1 : -1;
                            rand2 *= Globals.s_random.NextDouble() < 0.5 ? 1 : -1;


                            float scale = (float)Globals.s_random.NextDouble();
                            
                            //worldPosition
                            Vector3 tempPos = new Vector3((float)x - rand1, height, (float)z - rand2);
                            tempPos -= halfWidth;
                            Vector4 treePos = new Vector4(tempPos, scale);
                            treePositions.Add(treePos);
                        }
                    }

                }

            }
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
            //   DrawTrees();
            DrawPrettyTrees();
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void DrawTrees()
        {
            m_terrainEffect.CurrentTechnique = m_terrainEffect.Techniques["BillboardTrees"];
            Globals.GraphicsDevice.SetVertexBuffer(m_treeVertexBuffer);


            Vector3 startPosition = Vector3.Zero;
            Matrix transform = Matrix.CreateTranslation(startPosition);
            Globals.MCContentManager.ApplyCommonEffectParameters(m_terrainEffect);

            m_terrainEffect.Parameters["WorldMatrix"].SetValue(transform);
            m_terrainEffect.Parameters["TreeTexture"].SetValue(m_treeTexture);


            foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
            {
                int noVertices = m_treeVertexBuffer.VertexCount;
                int noTriangles = noVertices / 3;
                pass.Apply();
                Globals.GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, noTriangles);
            }
        }
        #endregion


        // To store instance transform matrices in a vertex buffer, we use this custom
        // vertex type which encodes 4x4 matrices as a set of four Vector4 values.
        static VertexDeclaration s_instanceVertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 1),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 2),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 3)
        );


        private SimpleTree m_simpleTree;
        //private List<Vector4> m_treePositions = new List<Vector4>();
        //private Matrix[] m_instanceTreeMatrices = null;
        private ObjectArray<Matrix> m_instanceTreeMatricesList = new ObjectArray<Matrix>();


        private Texture2D m_treeTexture;
        private Texture2D m_heightMapTexture;

        private Effect m_terrainEffect;
        private Effect m_instanceTrunkEffect;
        private Effect m_instanceLeafEffect;

        private VertexBuffer m_treeVertexBuffer;
        private DynamicVertexBuffer m_instanceVertexBuffer;
        private Matrix[] m_bindingMatrices = new Matrix[4];
        private Quaternion[] m_bindingQuaternions = new Quaternion[4];

        private QuadTree<TreeBounds> m_treeBoundsQuadTree = new QuadTree<TreeBounds>(4,Vector3.Zero,(Globals.worldMaxPos - Globals.worldMinPos)/2f);




    }
        public class TreeBounds : ISpatialNode
        {
            public Matrix m_matrix = Matrix.Identity;

            public TreeBounds(Matrix m)
            {
                m_matrix = m;
            }

            public Vector3 Position
            {
                get{return m_matrix.Translation;}
            }

            public BoundingSphere BoundSphere
            {
                // need to get scale?
                get
                {
                    return new BoundingSphere(m_matrix.Translation,1f);
                }
            }
                    

        }
}
