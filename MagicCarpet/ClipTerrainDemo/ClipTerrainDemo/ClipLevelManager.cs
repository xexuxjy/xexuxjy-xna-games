using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;

namespace ClipTerrainDemo
{
    public class ClipLevelManager : DrawableGameComponent
    {
        public ClipLevelManager(Game game, CameraComponent camera, Vector3 position)
            : base(game)
        {
            m_camera = camera;
            m_position = position;
            m_clipMapSize = 31;
            m_blockSize = (m_clipMapSize + 1) / 4;
            m_fixupOffset = 3;
            
            m_numLevels = 2;
            m_maxHeight = 100;
        }


        public override void Initialize()
        {
            base.Initialize();
            m_effect = Game.Content.Load<Effect>("ClipTerrain");

            Texture2D wrongFormatTexture = Game.Content.Load<Texture2D>("heightmap");
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

            //m_heightMapTexture.GetData<Single>

            m_effect.Parameters["fineLevelTexture"].SetValue(m_heightMapTexture);
            m_normalTexture = new Texture2D(Game.GraphicsDevice, m_heightMapTexture.Width * 2, m_heightMapTexture.Height * 2);
            m_effect.Parameters["normalsTexture"].SetValue(m_normalTexture);
            m_effect.Parameters["ZScaleFactor"].SetValue(1.0f);

            BuildVertexBuffers(Game.GraphicsDevice);
            m_clipLevels = new ClipLevel[m_numLevels];
            for (int i = 0; i < m_numLevels; ++i)
            {
                m_clipLevels[i] = new ClipLevel(i, this);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            BoundingFrustum boundingFrustrum = new BoundingFrustum(m_camera.ViewProjectionMatrix);
            for(int i=0;i<m_clipLevels.Length;++i)
            {
                m_effect.Parameters["blockColor"].SetValue(ColorForRing(i));
                m_clipLevels[i].Draw(Game.GraphicsDevice,boundingFrustrum);
            }
        }

        public  void BuildVertexBuffers(GraphicsDevice graphicsDevice)
        {
            PosOnlyVertex[] blockVertices = new PosOnlyVertex[m_blockSize * m_blockSize];
            int[] blockIndices = new int[(m_blockSize - 1) * (m_blockSize - 1) * 6];
            int indexCounter = 0;
            int vertexCounter = 0;
            int stride = m_blockSize;

            for (int y = 0; y < m_blockSize; ++y)
            {
                for (int x = 0; x < m_blockSize; ++x)
                {
                    Vector2 v = new Vector2(x, y);
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    blockVertices[vertexCounter++] = vpnt;
                    if (x < m_blockSize - 1 && y < m_blockSize - 1)
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
            int fixupSize = m_blockSize * m_fixupOffset;
            PosOnlyVertex[] fixupVertices = new PosOnlyVertex[fixupSize];
            int[] fixupIndices = new int[(m_blockSize - 1) * (m_fixupOffset - 1) * 6];
            vertexCounter = 0;
            indexCounter = 0;

            stride = m_blockSize;

            for (int y = 0; y < m_blockSize; ++y)
            {
                for (int x = 0; x < m_fixupOffset; ++x)
                {
                    if (x < m_fixupOffset - 1 && y < m_blockSize - 1)
                    {
                        fixupIndices[indexCounter++] = (x + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));

                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + (y * stride));

                    }
                }
            }
            m_fixupIndexBufferH = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);
            m_fixupIndexBufferH.SetData<int>(fixupIndices);
            indexCounter = 0;

            stride = m_blockSize;

            for (int y = 0; y < m_fixupOffset; ++y)
            {
                for (int x = 0; x < m_blockSize; ++x)
                {
                    if (x < m_blockSize - 1 && y < m_fixupOffset - 1)
                    {
                        fixupIndices[indexCounter++] = (x + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + (y * stride));
                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));

                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + ((y + 1) * stride));
                        fixupIndices[indexCounter++] = (x + (y * stride));

                    }
                }
            }
            m_fixupIndexBufferV = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);
            m_fixupIndexBufferV.SetData<int>(fixupIndices);



            //int numDegenerateIndices = (((m_clipMapSize - 1) / 2) * 3) * 4;
            //indexCounter = 0;

            //PosOnlyVertex[] degenerates = new PosOnlyVertex[numDegenerateIndices/6];

            //for(int i=0;i<m_blockSize;++i)
            //{
            //    degenerates[indexCounter] = new PosOnlyVertex(new Vector2(i,0));
            //    degenerates[indexCounter+m_blockSize] = new PosOnlyVertex(new Vector2(0,i));
            //    degenerates[indexCounter+(2*m_blockSize)] = new PosOnlyVertex(new Vector2(i,m_blockSize));
            //    degenerates[indexCounter+(3*m_blockSize)] = new PosOnlyVertex(new Vector2(m_blockSize,i));
            //    indexCounter++;
            //}

            //int[] degenerateIndices = new int[numDegenerateIndices];
            //vertexCounter = 0;
            //indexCounter = 0;

            //for (int i=0;i<m_blockSize-2;i+=2)
            //{
            //    degenerateIndices[indexCounter++] = i;
            //    degenerateIndices[indexCounter++] = i + 2;
            //    degenerateIndices[indexCounter++] = i + 1;
            //}

            //for (int i = 0; i < m_clipMapSize - 2; i+=2 )
            //{
            //    degenerateIndices[indexCounter++] = i * m_clipMapSize + (m_clipMapSize-1);
            //    degenerateIndices[indexCounter++] = (i + 2) * m_clipMapSize + (m_clipMapSize-1);
            //    degenerateIndices[indexCounter++] = (i + 1) * m_clipMapSize + (m_clipMapSize-1);
            //}

            //for (int i =  m_clipMapSize-1; i > 1; i-=2 )
            //{
            //    degenerateIndices[indexCounter++] = (m_clipMapSize-1) * m_clipMapSize + i;
            //    degenerateIndices[indexCounter++] = (m_clipMapSize-1) * m_clipMapSize + (i - 2);
            //    degenerateIndices[indexCounter++] = (m_clipMapSize-1) * m_clipMapSize + (i - 1);
            //}

            //for (int i = m_clipMapSize-1; i > 1; i-=2 )
            //{
            //    degenerateIndices[indexCounter++] = i * m_clipMapSize;
            //    degenerateIndices[indexCounter++] = (i - 2) * m_clipMapSize;
            //    degenerateIndices[indexCounter++] = (i - 1) * m_clipMapSize;
            //}



            m_blockVertexBuffer = new VertexBuffer(graphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            //m_degenerateVertexBuffer = new VertexBuffer(graphicsDevice, PosOnlyVertex.VertexDeclaration, degenerates.Length, BufferUsage.None);

            m_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            //m_degenerateVertexBuffer.SetData<PosOnlyVertex>(degenerates, 0, degenerates.Length);

            m_blockIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            //m_fixupIndexBufferH = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);
            //m_degenerateIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, degenerateIndices.Length, BufferUsage.None);


            m_blockIndexBuffer.SetData<int>(blockIndices);
            //m_degenerateIndexBuffer.SetData(degenerateIndices);
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

        private ClipLevel[] m_clipLevels;


        public Effect m_effect;
        private RasterizerState m_rasterizerState;
        private GraphicsDevice m_graphicsDevice;
        public  CameraComponent m_camera;
        public Texture2D m_heightMapTexture;
        private Texture2D m_normalTexture;

        private float[][] m_heightMap;

        public VertexBuffer m_blockVertexBuffer;
        public VertexBuffer m_degenerateVertexBuffer;

        public IndexBuffer m_blockIndexBuffer;
        public IndexBuffer m_fixupIndexBufferH;
        public IndexBuffer m_fixupIndexBufferV;

        public IndexBuffer m_degenerateIndexBuffer;

        public Vector3 m_position;

        public int m_clipMapSize;
        public int m_blockSize;
        public int m_fixupOffset;
        public int m_numLevels;
        public int m_maxHeight;

    }
}
