using Microsoft.Xna.Framework;
using Dhpoware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ClipTerrainDemo
{
    public class ClipLevel : DrawableGameComponent
    {
        public ClipLevel(Game game, ICamera camera,Vector3 position,int scaleLevel) : base(game)
        {
            m_camera = camera;
            if (s_blockVertexBuffer == null)
            {
                BuildVertexBuffers(game.GraphicsDevice);
            }

            m_rasterizerState = new RasterizerState();
            m_rasterizerState.FillMode = FillMode.WireFrame;
            m_scaleLevel = scaleLevel;
            m_position = position; 

        }


        public override void Initialize()
        {
            base.Initialize();
            m_effect = Game.Content.Load<Effect>("ClipTerrain");

            Texture2D wrongFormatTexture = Game.Content.Load<Texture2D>("heightmap");
            m_heightMapTexture = new Texture2D(Game.GraphicsDevice,wrongFormatTexture.Width,wrongFormatTexture.Height,false,SurfaceFormat.Single);
            Color[] colorData = new Color[wrongFormatTexture.Width*wrongFormatTexture.Height];
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

        }


        public override void Draw(GameTime gameTime)
        {
            BoundingFrustum boundingFrustrum = new BoundingFrustum(m_camera.ViewProjectionMatrix);
            // cornera from top left so  go from -2 -> 1
            DrawAtOffset(-2, -2, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(-1, -2, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(0, -2, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(1, -2, m_scaleLevel, boundingFrustrum);

            DrawAtOffset(-2, -1, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(1, -1, m_scaleLevel, boundingFrustrum);

            DrawAtOffset(-2, 0, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(1, 0, m_scaleLevel, boundingFrustrum);

            DrawAtOffset(-2, 1, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(-1, 1, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(0, 1, m_scaleLevel, boundingFrustrum);
            DrawAtOffset(1, 1, m_scaleLevel, boundingFrustrum);

        }



        public void DrawAtOffset(int x, int y, int scaleLevel,BoundingFrustum boundingFrustrum)
        {
            int scaleFactor = (int)Math.Pow(2, scaleLevel);

            x *= (scaleFactor * (s_blockSize - 1));
            y *= (scaleFactor * (s_blockSize - 1));

            //x *= (s_blockSize - 1);
            //y *= (s_blockSize - 1);

             //offset for fixups
            if (x < 0)
            {
                x -= scaleFactor;
            }
            else
            {
                x += scaleFactor;
            }

            if (y < 0)
            {
                y -= scaleFactor;
            }
            else
            {
                y += scaleFactor;
            }
            
            x += (int)m_position.X;
            y += (int)m_position.Z;

            BoundingBox box = new BoundingBox(new Vector3(x, -s_maxHeight, y), new Vector3(x + ((s_blockSize - 1) * scaleFactor), s_maxHeight, y + ((s_blockSize - 1) * scaleFactor)));
            if (boundingFrustrum.Intersects(box))
            {

                Matrix worldMatrix = Matrix.Identity;
                m_effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * m_camera.ViewMatrix * m_camera.ProjectionMatrix);

                RasterizerState oldState = Game.GraphicsDevice.RasterizerState;
                Game.GraphicsDevice.RasterizerState = m_rasterizerState;
                foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    m_effect.Parameters["ScaleFactor"].SetValue(new Vector4(scaleFactor, scaleFactor, x, y));
                    m_effect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(1.0f / m_heightMapTexture.Width, 1.0f / m_heightMapTexture.Height, 0, 0));

                    m_effect.Parameters["blockColor"].SetValue(ColorForRing(scaleLevel));
                    Game.GraphicsDevice.Indices = s_blockIndexBuffer;
                    Game.GraphicsDevice.SetVertexBuffer(s_blockVertexBuffer);
                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, s_blockVertexBuffer.VertexCount, 0, s_blockIndexBuffer.IndexCount / 3);
                }
            }
            //Game.GraphicsDevice.RasterizerState = oldState;


        }



        public static void BuildVertexBuffers(GraphicsDevice graphicsDevice)
        {
            PosOnlyVertex[] blockVertices = new PosOnlyVertex[s_blockSize * s_blockSize];
            int[] blockIndices = new int[(s_blockSize-1) * (s_blockSize-1) * 6];
            int indexCounter = 0;
            int vertexCounter = 0;
            for (int x = 0; x < s_blockSize; ++x)
            {
                for (int y = 0; y < s_blockSize; ++y)
                {
                    Vector2 v = new Vector2(x,y);
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    blockVertices[vertexCounter++] = vpnt;
                    if (x < s_blockSize-1 && y < s_blockSize-1)
                    {
                        blockIndices[indexCounter++] = (x + (y * s_blockSize));
                        blockIndices[indexCounter++] = (x + ((y + 1) * s_blockSize));
                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * s_blockSize));

                        blockIndices[indexCounter++] = (x + 1 + ((y + 1) * s_blockSize));
                        blockIndices[indexCounter++] = (x + 1 + (y * s_blockSize));
                        blockIndices[indexCounter++] = (x + (y * s_blockSize));
                    }
                }
            }
            int fixupSize = s_blockSize * s_fixupOffset;
            PosOnlyVertex[] fixupVertices = new PosOnlyVertex[fixupSize];
            int[] fixupIndices = new int[(s_blockSize -1) * (s_fixupOffset -1) * 6];
            vertexCounter = 0;
            indexCounter = 0;
            for (int x = 0; x < s_fixupOffset; ++x)
            {
                for (int y = 0; y < s_blockSize; ++y)
                {
                    Vector2 v = new Vector2(x, y);
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    fixupVertices[vertexCounter++] = vpnt;
                    if (x < s_fixupOffset-1 && y < s_blockSize-1)
                    {
                        fixupIndices[indexCounter++] = (x + (y * s_fixupOffset));
                        fixupIndices[indexCounter++] = (x + ((y + 1) * s_fixupOffset));
                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * s_fixupOffset));

                        fixupIndices[indexCounter++] = (x + 1 + ((y + 1) * s_fixupOffset));
                        fixupIndices[indexCounter++] = (x + 1 + (y * s_fixupOffset));
                        fixupIndices[indexCounter++] = (x + (y * s_fixupOffset));
                    }
                }
            }


            s_blockVertexBuffer = new VertexBuffer(graphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            s_fixupVertexBuffer = new VertexBuffer(graphicsDevice, PosOnlyVertex.VertexDeclaration, fixupVertices.Length, BufferUsage.None);

            s_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            s_fixupVertexBuffer.SetData<PosOnlyVertex>(fixupVertices, 0, fixupVertices.Length);

            s_blockIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            s_fixupIndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, fixupIndices.Length, BufferUsage.None);


            s_blockIndexBuffer.SetData<int>(blockIndices);
            s_fixupIndexBuffer.SetData<int>(fixupIndices);

        }


        public Vector4 ColorForRing(int ring)
        {
            switch (ring)
            {
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


        private int m_scaleLevel;

        private Vector3 m_position;
        private Vector3 m_offset;

        private Effect m_effect;
        private RasterizerState m_rasterizerState;
        private GraphicsDevice m_graphicsDevice;
        private ICamera m_camera;
        private Texture2D m_heightMapTexture;
        private Texture2D m_normalTexture;

        private float[][] m_heightMap;



        private static VertexBuffer s_blockVertexBuffer;
        private static VertexBuffer s_fixupVertexBuffer;

        private static IndexBuffer s_blockIndexBuffer;
        private static IndexBuffer s_fixupIndexBuffer;


        private static int s_clipMapSize = 15;
        private static int s_blockSize = (s_clipMapSize + 1) / 4;
        private static int s_fixupOffset = 3;

        private static int s_maxHeight = 100;


    }
}
