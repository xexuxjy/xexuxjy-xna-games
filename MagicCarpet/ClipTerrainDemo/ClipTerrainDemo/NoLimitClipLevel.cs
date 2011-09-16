using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Dhpoware;

namespace ClipTerrainDemo
{
    public class NoLimitClipLevel : DrawableGameComponent
    {
        public NoLimitClipLevel(Game game,CameraComponent camera)
            : base(game)
        {
            m_camera = camera;
            m_rasterizerState = new RasterizerState();
            //m_rasterizerState.FillMode = FillMode.WireFrame;
            m_graphicsDevice = Game.GraphicsDevice;
        }

        public override void Initialize()
        {
            m_effect = Game.Content.Load<Effect>("ClipTerrain");
            Texture2D wrongFormatTexture = Game.Content.Load<Texture2D>("heightmap");
            m_baseTexture = Game.Content.Load<Texture2D>("base");
            m_noiseTexture = Game.Content.Load<Texture2D>("noise");

            m_heightMapTexture = new Texture2D(Game.GraphicsDevice, wrongFormatTexture.Width, wrongFormatTexture.Height, false, SurfaceFormat.Single);
            Color[] colorData = new Color[wrongFormatTexture.Width * wrongFormatTexture.Height];
            wrongFormatTexture.GetData<Color>(colorData);

            Single[] adjustedData = new Single[colorData.Length];
            m_heightMapTexture.GetData<Single>(adjustedData);

            for (int i = 0; i < colorData.Length; ++i)
            {
                adjustedData[i] = colorData[i].R;
            }

            m_heightMapTexture.SetData<Single>(adjustedData);
            m_effect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);
            m_effect.Parameters["BaseTexture"].SetValue(m_baseTexture);
            m_effect.Parameters["NoiseTexture"].SetValue(m_noiseTexture);

            Vector3 lightDirection = new Vector3(0.5f,-1,0.5f);
            lightDirection.Normalize();
            Vector3 ambientLight = new Vector3(0.2f);
            Vector3 directionalLight = new Vector3(1f);

            //m_effect.Parameters["LightDirection"].SetValue(lightDirection);
            m_effect.Parameters["AmbientLight"].SetValue(ambientLight);
            m_effect.Parameters["DirectionalLight"].SetValue(directionalLight);


            m_effect.Parameters["LightPosition"].SetValue(new Vector3(1000, 40, 1000));


            BuildVertexBuffers();

        }


        public void BuildVertexBuffers()
        {
            PosOnlyVertex[] blockVertices = new PosOnlyVertex[m_blockVertices * m_blockVertices];
            int[] blockIndices = new int[(m_blockSize) * (m_blockSize) * 6];
            int indexCounter = 0;
            int vertexCounter = 0;
            int stride = m_blockVertices;

            for (int y = 0; y < m_blockVertices; ++y)
            {
                for (int x = 0; x < m_blockVertices; ++x)
                {
                    Vector2 v = new Vector2(x, y);
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    blockVertices[vertexCounter++] = vpnt;
                    if (x < m_blockSize && y < m_blockSize)
                    {
                        blockIndices[indexCounter++] = (x + (y * stride));
                        blockIndices[indexCounter++] = (x + 1 + (y * stride));
                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));

                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));
                        blockIndices[indexCounter++] = (x + ((y + 1) * stride));
                        blockIndices[indexCounter++] = (x + (y * stride));
                    }
                }
            }





            m_blockVertexBuffer = new VertexBuffer(m_graphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            //m_degenerateVertexBuffer = new VertexBuffer(graphicsDevice, PosOnlyVertex.VertexDeclaration, degenerates.Length, BufferUsage.None);

            m_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            //m_degenerateVertexBuffer.SetData<PosOnlyVertex>(degenerates, 0, degenerates.Length);

            m_blockIndexBuffer = new IndexBuffer(m_graphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            //m_fixupIndexBufferH = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);
            //m_degenerateIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, degenerateIndices.Length, BufferUsage.None);


            m_blockIndexBuffer.SetData<int>(blockIndices);
            //m_degenerateIndexBuffer.SetData(degenerateIndices);

            //CreateLShapes();
        }

        public override void Draw(GameTime gameTime)
        {
            
            Matrix worldMatrix = Matrix.Identity;

            float a = (int)Math.Pow(3, m_numLevels);
            a *= (m_blockSize * 3);
            a *= -0.5f;

            //worldMatrix = Matrix.CreateTranslation(new Vector3(a, 0, a));


            BoundingFrustum boundingFrustrum = new BoundingFrustum(m_camera.ViewProjectionMatrix);

            RasterizerState oldState = Game.GraphicsDevice.RasterizerState;
            m_graphicsDevice.RasterizerState = m_rasterizerState;
            m_graphicsDevice.Indices = m_blockIndexBuffer;
            m_graphicsDevice.SetVertexBuffer(m_blockVertexBuffer);
            float oneOverTextureWidth = 1f/1024f;
            m_effect.Parameters["ZScaleFactor"].SetValue(0.1f);

            float maxHeight = 100;

            float maxSpan2 = (float)Math.Pow(3, m_numLevels) * m_blockVertices;


            // need to figure out a window on the height map texture.
            float visibleTerrainFraction = 1.0f;
            m_effect.Parameters["TerrainTextureWindow"].SetValue(new Vector2(0, 0));
            maxSpan2 *= visibleTerrainFraction;

            m_effect.Parameters["OneOverMaxExtents"].SetValue(1 / maxSpan2);


            Vector3 maxPos = Vector3.Zero;

            Vector3 lastStartPosition = Vector3.Zero;

            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                // Draw Center
                Vector3 position = Vector3.Zero;
                Vector3 scale = new Vector3(1, 1, 1);
                m_effect.Parameters["ScaleFactor"].SetValue(new Vector4(scale.X, scale.Z, position.X, position.Z));
                Matrix transform = worldMatrix * m_camera.ViewMatrix * m_camera.ProjectionMatrix;
                m_effect.Parameters["WorldViewProjMatrix"].SetValue(transform);

                // need apply on inner level to make sure latest vals copied across
                pass.Apply();
                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_blockVertexBuffer.VertexCount, 0, m_blockIndexBuffer.IndexCount / 3);


                for (int level = 0; level < m_numLevels; ++level)
                {
                    m_effect.Parameters["BlockColor"].SetValue(ColorForRing(level));
                    Vector3 blockSize = new Vector3(m_blockSize,0,m_blockSize);
                    blockSize *= scale;

                    lastStartPosition -= blockSize;

                    for (int j = 0; j < 3; ++j)
                    {
                        for (int k = 0; k < 3; ++k)
                        {
                            // skip center
                            if (!(j == 1 && k == 1))
                            {
                                position = new Vector3((m_blockSize) * k,0,(m_blockSize)*j);
                                position *= scale;
                                position += lastStartPosition;

                                BoundingBox bb = new BoundingBox(position,position+blockSize);

                                if (bb.Max.X > maxPos.X)
                                {
                                    maxPos.X = bb.Max.X;
                                }

                                if (bb.Max.Z > maxPos.Z)
                                {
                                    maxPos.Z = bb.Max.Z;
                                }

                                if (boundingFrustrum.Intersects(bb))
                                {
                                    m_effect.Parameters["ScaleFactor"].SetValue(new Vector4(scale.X, scale.Z, position.X, position.Z));
                                    m_effect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(oneOverTextureWidth, oneOverTextureWidth, 0, 0));

                                    // need apply on inner level to make sure latest vals copied across
                                    pass.Apply();
                                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_blockVertexBuffer.VertexCount, 0, m_blockIndexBuffer.IndexCount / 3);

                                }
                                else
                                {
                                    int ibreak = 0;
                                }
                            }
                        }
                    }
                    scale *= new Vector3(3, 1, 3);

                }
            }
            Game.GraphicsDevice.RasterizerState = oldState;
        }



        public Vector4 ColorForRing(int ring)
        {
            switch (ring)
            {
                case (0):
                    return Color.White.ToVector4();
                case (1):
                    return Color.Yellow.ToVector4();
                case (2):
                    return Color.Red.ToVector4();
                case (3):
                    return Color.Green.ToVector4();
                case (4):
                    return Color.Blue.ToVector4();
                case (5):
                    return Color.Magenta.ToVector4();
                case (6):
                    return Color.Olive.ToVector4();

                default:
                    return Color.Black.ToVector4();
            }
        }


        const int m_numLevels = 2;
        const int m_blockVertices = 65;
        const int m_blockSize = m_blockVertices-1;
        VertexBuffer m_blockVertexBuffer;
        IndexBuffer m_blockIndexBuffer;
        CameraComponent m_camera;
        Effect m_effect;
        RasterizerState m_rasterizerState;
        Texture2D m_heightMapTexture;
        Texture2D m_baseTexture;
        Texture2D m_noiseTexture;

        GraphicsDevice m_graphicsDevice;
    }


}
