using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Dhpoware;
using System.Diagnostics;

namespace ClipTerrainDemo
{
    public class ClipTerrain : DrawableGameComponent
    {

        public ClipTerrain(Game game,ICamera camera)
            : base(game)
        {
            m_rasterizerState = new RasterizerState();
            m_rasterizerState.FillMode = FillMode.WireFrame;
            m_camera = camera;
        }

        public override void Initialize()
        {
            GenerateTextureForHeightMap();
            BuildRings();

        }



        public void GenerateTextureForHeightMap()
        {

        }

        public void BuildRings()
        {
            m_effect = Game.Content.Load<Effect>("ClipTerrain");
            //m_basicEffect.TextureEnabled = true;
            //m_basicEffect.VertexColorEnabled = true;
            m_numRings = 4;

            Vector2 center = new Vector2();
            Vector2[] ringDims = new Vector2[m_numRings];

            ringDims[0] = new Vector2(33, 33);
            ringDims[1] = ringDims[0]*2;
            ringDims[2] = ringDims[1]*4;
            ringDims[3] = ringDims[2] * 8;
            //ringDims[3] = new Vector2(1024, 1024);

            int numSteps = (int)ringDims[0].X * 2;

            List<PosOnlyVertex>[] ringVertices = new List<PosOnlyVertex>[m_numRings];
            List<int>[] ringIndices= new List<int>[m_numRings];
            for (int i = 0; i < m_numRings; ++i)
            {
                ringVertices[i] = new List<PosOnlyVertex>();
                ringIndices[i] = new List<int>();
            }
            for (int i = 0; i < m_numRings; ++i)
            {
                if (i == 0)
                {
                    BuildRing(i,center - ringDims[i], center + ringDims[i], center, center, numSteps, ringVertices[i], ringIndices[i]);
                }
                else
                {
                    BuildRing(i,center - ringDims[i], center + ringDims[i], center - ringDims[i - 1], center + ringDims[i - 1], numSteps, ringVertices[i], ringIndices[i]);
                }
            }

            m_ringVertexBuffers = new VertexBuffer[m_numRings];
            m_ringIndexBuffers = new IndexBuffer[m_numRings];
            m_ringDemoTextures = new Texture2D[m_numRings];

            for (int i = 0; i < m_numRings; i++)
            {
                m_ringVertexBuffers[i] = new VertexBuffer(Game.GraphicsDevice, PosOnlyVertex.VertexDeclaration, ringVertices[i].Count, BufferUsage.None);
                m_ringVertexBuffers[i].SetData<PosOnlyVertex>(ringVertices[i].ToArray());
                m_ringIndexBuffers[i] = new IndexBuffer(Game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, ringIndices[i].Count, BufferUsage.None);
                m_ringIndexBuffers[i].SetData<int>(ringIndices[i].ToArray());
            }

            m_ringDemoTextures[0] = GetTexture(Color.Yellow.ToVector3());
            m_ringDemoTextures[1] = GetTexture(Color.Red.ToVector3());
            //m_ringDemoTextures[2] = GetTexture(Color.Green.ToVector3());
            //m_ringDemoTextures[3] = GetTexture(Color.Blue.ToVector3());



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

        private Texture2D GetTexture(Vector3 color)
        {
            if (!m_colorMap.ContainsKey(color))
            {
                Texture2D newTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
                Color[] colorData = new Color[1];
                newTexture.GetData<Color>(colorData);
                colorData[0] = new Color(color);
                newTexture.SetData(colorData);
                m_colorMap[color] = newTexture;
            }
            return m_colorMap[color];
        }




        public void BuildRing(int ring, Vector2 min, Vector2 max, Vector2 holeMin, Vector2 holeMax, int numSteps, List<PosOnlyVertex> vertices, List<int> indices)
        {
            //int xstep = (int)((max.X - min.X) / numSteps);
            //int ystep = (int)((max.Y - min.Y) / numSteps);

            //// 4 rectangles , top,left,right,bottom.
            //Vector2 holeStep = ((max - min) - (holeMax - holeMin));

            //int holeX = (int)holeStep.X/2;
            //int holeY = (int)holeStep.Y/2;

            //int numVertices = numSteps + 1;

            // lowest level - dont need to split
            if (ring == 0)
            {
                BuildSubSquare(min, max, numSteps, numSteps, vertices, indices,Color.White);
            }
            else
            {
                int subStepSize = numSteps /3;
                //TopLeft
                Vector2 subMin = min;
                Vector2 subMax = holeMin;
                BuildSubSquare(subMin, subMax, subStepSize,subStepSize, vertices, indices,Color.Red);
               
                ////TopMiddle
                subMin = new Vector2(holeMin.X, min.Y);
                subMax = new Vector2(holeMax.X, holeMin.Y);
                BuildSubSquare(subMin, subMax, subStepSize, subStepSize, vertices, indices, Color.Yellow);

                //TopRight
                subMin = new Vector2(holeMax.X, min.Y);
                subMax = new Vector2(max.X, holeMin.Y);
                BuildSubSquare(subMin, subMax, subStepSize, subStepSize, vertices, indices, Color.Green);

                //MiddleLeft
                subMin = new Vector2(min.X, holeMin.Y);
                subMax = new Vector2(holeMin.X, holeMax.Y);
                BuildSubSquare(subMin, subMax, subStepSize, subStepSize, vertices, indices, Color.Blue);

                //MiddleRight
                subMin = new Vector2(holeMax.X, holeMin.Y);
                subMax = new Vector2(max.X, holeMax.Y);
                BuildSubSquare(subMin, subMax, subStepSize, subStepSize, vertices, indices, Color.MidnightBlue);

                //BottomLeft
                subMin = new Vector2(min.X, holeMax.Y);
                subMax = new Vector2(holeMin.X, max.Y);
                BuildSubSquare(subMin, subMax, subStepSize, subStepSize, vertices, indices, Color.Orange);

                //BottomMiddle
                subMin = new Vector2(holeMin.X, holeMax.Y);
                subMax = new Vector2(holeMax.X, max.Y);
                BuildSubSquare(subMin, subMax, subStepSize, subStepSize, vertices, indices, Color.Lavender);

                //BottomRight
                subMin = new Vector2(holeMax.X, holeMax.Y);
                subMax = new Vector2(max.X, max.Y);
                BuildSubSquare(subMin, subMax, subStepSize, subStepSize, vertices, indices, Color.HotPink);



            }

        }

        private void BuildSubSquare(Vector2 min, Vector2 max, int xSpan, int ySpan, List<PosOnlyVertex> vertices, List<int> indices, Color color)
        {
            float xstep = ((max.X - min.X) / xSpan);
            float ystep = ((max.Y - min.Y) / ySpan);

            int vertsX = xSpan+1;
            int vertsY = ySpan+1;

            int vertexBase = vertices.Count;

            for (int x = 0; x < vertsX; ++x)
            {
                for (int y = 0; y < vertsY; ++y)
                {
                    Vector2 v = new Vector2(min.X + (x * xstep), min.Y + (y * ystep));
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    vertices.Add(vpnt);
                    if (x < xSpan && y < ySpan)
                    {
                        indices.Add(vertexBase+x + (y * vertsX));
                        indices.Add(vertexBase + x + ((y + 1) * vertsX));
                        indices.Add(vertexBase + x + 1 + ((y + 1) * vertsX));

                        indices.Add(vertexBase + x + 1 + ((y + 1) * vertsX));
                        indices.Add(vertexBase + x + 1 + (y * vertsX));
                        indices.Add(vertexBase + x + (y * vertsX));
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix worldMatrix = Matrix.Identity;
            m_effect.Parameters["WorldViewProjMatrix"].SetValue(worldMatrix * m_camera.ViewMatrix * m_camera.ProjectionMatrix);

            RasterizerState oldState = Game.GraphicsDevice.RasterizerState;
            Game.GraphicsDevice.RasterizerState = m_rasterizerState;
            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                for(int i=0;i<m_numRings;++i)
                {
                    m_effect.Parameters["ScaleFactor"].SetValue(new Vector4(1, 1, 0, 0));
                    m_effect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(1.0f / m_heightMapTexture.Width, 1.0f / m_heightMapTexture.Height, 0, 0));

                    m_effect.Parameters["BlockColor"].SetValue(ColorForRing(i));
                    Game.GraphicsDevice.Indices = m_ringIndexBuffers[i];
                    Game.GraphicsDevice.SetVertexBuffer(m_ringVertexBuffers[i]);
                    Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_ringVertexBuffers[i].VertexCount, 0, m_ringIndexBuffers[i].IndexCount/ 3);                
                }
            }
            Game.GraphicsDevice.RasterizerState = oldState;
        }

        public void UpdateHeight(float[] p, int index, float new_val)
        {
            p[index] = new_val;
        }


        public Vector4 ColorForRing(int ring)
        {
            switch (ring)
            {
                case (0):
                    return Color.Yellow.ToVector4();
                case (1):
                    return Color.Red.ToVector4();
                default:
                    return Color.Black.ToVector4();
            }
        }


        private Effect m_effect;
        
        // simple one vb per ring to begin with
        private VertexBuffer[] m_ringVertexBuffers;
        private IndexBuffer[] m_ringIndexBuffers;
        private Texture2D[] m_ringDemoTextures;

        private const int m_length = 1024;
        private int m_numRings;

        private Texture2D m_heightMapTexture;
        private Texture2D m_normalTexture;

        private float[][] m_heightMap;
        private ICamera m_camera;
        private Dictionary<Vector3, Texture2D> m_colorMap = new Dictionary<Vector3, Texture2D>();
        private RasterizerState m_rasterizerState;

        private Random m_random = new Random();
        const int m_randomMax = 0x7fff;
        const int s_gridSize = 32 + 1;  // must be (2^N) + 1
        const float s_gridSpacing = 5.0f;

        const float s_gridHeightScale = 0.2f;

        
    }

    public struct PosOnlyVertex : IVertexType
    {

        public PosOnlyVertex(Vector2 v)
        {
            Position = v;
        }

        public Vector2 Position;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }
    };

    

}
