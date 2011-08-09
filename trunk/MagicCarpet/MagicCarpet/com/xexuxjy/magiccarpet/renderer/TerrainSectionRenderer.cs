using System;
using com.xexuxjy.magiccarpet.terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using com.xexuxjy.magiccarpet.util;
using System.IO;
using BulletXNA.LinearMath;
using Dhpoware;


namespace com.xexuxjy.magiccarpet.renderer
{
    public class TerrainSectionRenderer : DefaultRenderer
    {

        public TerrainSectionRenderer(MagicCarpet game,TerrainSection terrainSection,Terrain terrain) : base(game)
        {
            m_terrainSection = terrainSection;
            m_terrain = terrain;
            m_sectorX = terrainSection.m_sectorX;
            m_sectorZ = terrainSection.m_sectorZ;
            
            ComputeValues();
            LoadEffectFile();
            BuildIndexBuffer(game.GraphicsDevice);
            BuildVertexBuffer(game.GraphicsDevice);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////



        public override void Draw(GameTime gameTime)
        {
            if (m_terrainSection.IsDirty())
            {
                BuildVertexBuffer(Game.GraphicsDevice);
            }

            ICamera camera = Globals.Camera;

            Matrix identity = Matrix.Identity;
            //Matrix translation = Matrix.CreateTranslation(m_terrainSection.Position);
            Matrix translation = Matrix.CreateTranslation(new Vector3());
            Matrix view = camera.ViewMatrix;
            Matrix world = Matrix.Multiply(translation, identity);

            Matrix projection = camera.ProjectionMatrix;

            Matrix worldViewProjection = world * view * projection;

            // only one of these should be active.

            DrawBasicEffect(Game.GraphicsDevice, ref view, ref world, ref projection);
            DrawEffect(Game.GraphicsDevice, ref view, ref world, ref projection);

            DrawDebugAxes(Game.GraphicsDevice);
            if (ShouldDrawBoundingBox())
            {
                DrawBoundingBox(Game.GraphicsDevice);
            }
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void DrawEffect(GraphicsDevice device, ref Matrix view, ref Matrix world, ref Matrix projection)
        {
            if (null != m_effect)
            {
                Matrix worldViewProjection = world * view * projection;
                m_effect.Parameters["worldViewProjection"].SetValue(worldViewProjection);
                m_effect.Parameters["baseTexture"].SetValue(m_texture);
                m_effect.Parameters["deepWaterHeightValue"].SetValue(-5.0f);
                m_effect.Parameters["shallowWaterHeightValue"].SetValue(-3.0f);
                m_effect.Parameters["sandHeightValue"].SetValue(1.0f);
                m_effect.Parameters["grassHeightValue"].SetValue(5.0f);
                m_effect.Parameters["screeHeightValue"].SetValue(10.0f);
                m_effect.Parameters["iceHeightValue"].SetValue(20.0f);
                
                // running into problems casting enums in this case, so workaround
                int count = 0;
                foreach (Enum textureSlot in Enum.GetValues(typeof(TerrainTextureSlot)))
                {
                    String slotName = Enum.GetName(typeof(TerrainTextureSlot), textureSlot);
                    m_effect.Parameters[slotName].SetValue(s_terrainTextures[count++]);
                }

                float moveTime = m_terrainSection.TerrainMoveTime;
                m_effect.Parameters["timeStep"].SetValue(moveTime);

                device.SetVertexBuffer(m_vertexBuffer, 0);
                device.Indices = s_indexBuffer;
                foreach (EffectPass effectPass in m_effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_vertices.Length, 0, (m_numberOfQuadsX * m_numberOfQuadsZ * 2));
                }
                
            }

        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool BuildVertexBuffer(GraphicsDevice device)
        {
            bool result = true;

            if (null != device)
            {
                //
                // Build a vertex buffer and determine
                // the min\max size of the sector
                //
                String tempName = String.Format("terrain_section_[0]_[1]", m_sectorX, m_sectorZ);

                BuildTextureForGrid(Game.GraphicsDevice);

                if (s_vertexDecleration == null)
                {
                    BuildVertexDecleration();
                }

                if (m_vertexBuffer == null)
                {
                    // All the vertices's are stored in a 1D array
                    m_vertices = new MorphingTerrainVertexFormatStruct[m_numberOfVerticesX * m_numberOfVerticesZ];
                    m_vertexBuffer = new VertexBuffer(device, s_vertexDecleration, m_vertices.Length, BufferUsage.None);
                }
                // Load vertices's into the buffer one by one
                Vector3 offset = m_terrainSection.BoundingBox.Min;
                for (int x = 0; x < m_numberOfVerticesX; x++)
                {
                    for (int z = 0; z < m_numberOfVerticesZ; z++)
                    {
                        MorphingTerrainVertexFormatStruct vertex = new MorphingTerrainVertexFormatStruct();
                        vertex.Position = offset;
                        vertex.Position.X += x;
                        vertex.Position.Z += z;
                        vertex.Position.Y = m_terrain.GetHeightAtPoint(vertex.Position.X, vertex.Position.Z);
                        vertex.TargetHeight = m_terrain.GetHeightAtPoint(vertex.Position.X, vertex.Position.Z, true);

                        if (Math.Abs(vertex.Position.Y - vertex.TargetHeight) > 0.3f)
                        {
                            int ibreak = 0;
                            vertex.Position.Y = vertex.TargetHeight;
                        }

                        // Set the u,v values so one texture covers the entire terrain
                        vertex.TextureCoordinate = new Vector2((float)z / m_numberOfQuadsZ * 4, (float)x / m_numberOfQuadsX *4);
                        //vertex.TextureCoordinate = new Vector2((float)z % 2, (float)x % 2);
                        vertex.Normal = Vector3.Up;
                        //int index = (x * m_numberOfQuadsX) + z;
                        int index = x + z * m_numberOfVerticesX;
                        m_vertices[index] = vertex;
                    }
                }

                //computeNormals();

                // 
                //MorphingTerrainVertexFormatStruct[] copyOfClassData = new MorphingTerrainVertexFormatStruct[m_vertices.Length];
                //for (int i = 0; i < m_vertices.Length; ++i)
                //{
                //    vertexFormatClassToStruct(m_vertices[i], ref copyOfClassData[i]);
                //}
                m_terrainSection.ClearDirty();
                m_vertexBuffer.SetData<MorphingTerrainVertexFormatStruct>(m_vertices);
            }
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void BuildIndexBuffer(GraphicsDevice device)
        {
            if (s_indexBuffer == null)
            {
                ObjectArray<int> indices = TerrainSection.GetSectionIndices();
                s_indexBuffer = new IndexBuffer(device, typeof(int), indices.Count, BufferUsage.None);
                s_indexBuffer.SetData(indices.GetRawArray(), 0, indices.Count);
            }
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int[] GetOffsetIndices(int minX, int minZ, int maxX, int maxZ)
        {
            Vector3 offset = m_terrainSection.BoundingBox.Min;

            // adjust the offsets so they fit our local coord scheme.
            minX = minX - (int)(offset.X);
            maxX = maxX - (int)(offset.X);

            minZ = minZ - (int)(offset.Z);
            maxZ = maxZ - (int)(offset.Z);
                
            int quadsX = (maxX - minX);
            int quadsZ = (maxZ - minZ);
            
            int size = quadsX * quadsZ * 6;
            int stepSize = m_numberOfVerticesX;
            int[] returnArray = new int[size];
            // Working with quads so it's (max -1)
            for (int x = minX; x < maxX; x++)
            {
                for (int y = minZ; y < maxZ; y++)
                {
                    int index = ((x-minX) + ((y-minZ) * quadsZ))* 6;
                    returnArray[index] = (x + (y * stepSize));
                    returnArray[index + 1] = ((x + 1) + (y * stepSize));
                    returnArray[index + 2] = ((x + 1) + ((y + 1) * stepSize));

                    returnArray[index + 3] = (x + ((y + 1) * stepSize));
                    returnArray[index + 4] = (x + (y * stepSize));
                    returnArray[index + 5] = ((x + 1) + ((y + 1) * stepSize));
                }
            }
            return returnArray;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected void ComputeValues()
        {
            // Vertices
            m_numberOfVerticesX = m_terrainSection.Width;
            m_numberOfVerticesZ = m_terrainSection.Breadth;

            // Quads
            m_numberOfQuadsX = m_numberOfVerticesX-1;
            m_numberOfQuadsZ = m_numberOfVerticesZ-1;
            m_totalNumberOfQuads = m_numberOfQuadsX * m_numberOfQuadsZ;

            m_totalNumberOfTriangles = m_totalNumberOfQuads * 2;
            m_totalNumberOfIndicies = m_totalNumberOfQuads * 6;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void ComputeNormals()
        {
            // compute normals
            for (int z = 1; z < m_numberOfQuadsZ; z++)
            {
                for (int x = 1; x < m_numberOfQuadsX; x++)
                {
                    Vector3 X = Vector3.Subtract(m_vertices[z * m_numberOfVerticesZ + x + 1].Position, m_vertices[z * m_numberOfVerticesZ + x - 1].Position);
                    Vector3 Z = Vector3.Subtract(m_vertices[(z + 1) * m_numberOfVerticesZ + x].Position, m_vertices[(z - 1) * m_numberOfVerticesZ + x].Position);

                    Vector3 Normal = Vector3.Cross(Z, X);
                    Normal.Normalize();
                    m_vertices[(z * m_numberOfVerticesZ) + x].Normal = Normal;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void BuildTextureForGrid(GraphicsDevice device)
        {
            if (null != device)
            {
                int textureWidth = 256;
                int textureBreadth = 256;
                if (m_texture == null)
                {
                    m_texture = new Texture2D(device, textureWidth, textureBreadth, true, SurfaceFormat.Color);
                }
                uint[] textureData = new uint[textureBreadth * textureWidth];
                m_texture.GetData<uint>(textureData);

                int stepSizeX = textureWidth / m_numberOfQuadsX;
                int stepSizeZ = textureBreadth / m_numberOfQuadsZ;

                // got top left corner.
                Vector3 foo = m_terrainSection.BoundingBox.Min;
                int squareOffsetX = (int)foo.X;
                int squareOffsetZ = (int)foo.Z;

                for (int i = 0; i < textureWidth; ++i)
                {
                    int xoffset = i / stepSizeX;
                    for (int j = 0; j < textureBreadth; ++j)
                    {
                        int zoffset = j / stepSizeZ;
                        Color color = GetColourForTerrainType(m_terrain.GetTerrainSquareAtPoint(squareOffsetX+
xoffset, squareOffsetZ+zoffset).Type);
                        textureData[(i * textureWidth) + j] = color.PackedValue;
                    }
                }
                m_texture.SetData(textureData); 
           
                
            }
            String filename = "d:/tmp/mc-test.jpg";
            //using (FileStream fileStream = File.OpenWrite(filename))  
            //{  
            //    m_texture.SaveAsJpeg(fileStream,m_texture.Width,m_texture.Height);
            //    fileStream.Close();  
            //}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void LoadEffectFile()
        {

            m_effect = Game.Content.Load<Effect>(Globals.terrainEffect);
            // dummy values for now, need reconciling with map
            m_effect.Parameters["deepWaterHeightValue"].SetValue(-5.0f);
            m_effect.Parameters["shallowWaterHeightValue"].SetValue(-3.0f);
            m_effect.Parameters["sandHeightValue"].SetValue(1.0f);
            m_effect.Parameters["grassHeightValue"].SetValue(5.0f);
            m_effect.Parameters["screeHeightValue"].SetValue(10.0f);
            m_effect.Parameters["iceHeightValue"].SetValue(20.0f);

            //m_basicEffect = new BasicEffect(device, null);
            //m_basicEffect.AmbientLightColor = Color.White.ToVector3();
            //m_basicEffect.DirectionalLight0.Direction = new Vector3(0f, -1f, 0f);
            //m_basicEffect.DirectionalLight0.Enabled = true;
            //m_basicEffect.DirectionalLight0.DiffuseColor = Color.White.ToVector3();
            //m_basicEffect.TextureEnabled = true;
            //m_basicEffect.LightingEnabled = true;
            //m_basicEffect.VertexColorEnabled = false;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public Vector3 GetNormalAtPoint(Vector3 point)
        //{
        //    if(!m_
        //    int x = (int)(point.X);
        //    int z = (int)(point.X);

        //    x = MathHelperExtension.Clamp(0, x, worldSpanX - 1);
        //    z = MathHelperExtension.Clamp(0, z, worldSpanZ - 1);


        //    return m_vertices[(z * m_numberOfVerticesZ) + x].Normal; 
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Color GetColourForTerrainType(TerrainType terrainType)
        {
            switch (terrainType)
            {
                case TerrainType.beach:
                    {
                        return Color.Yellow;
                    }
                case TerrainType.castle:
                    {
                        return Color.Indigo;
                    }
                case TerrainType.grass:
                    {
                        return Color.GreenYellow;
                    }

                case TerrainType.grass2:
                    {
                        return Color.Green;
                    }
                case TerrainType.ice:
                    {
                        return Color.Ivory;
                    }
                case TerrainType.immovable:
                    {
                        return Color.Black;
                    }
                case TerrainType.rock:
                    {
                        return Color.Gray;
                    }
                case TerrainType.water:
                    {
                        return Color.Blue;
                    }
                case TerrainType.shallowwater:
                    {
                        return Color.Aquamarine;
                    }
                case TerrainType.deepwater:
                    {
                        return Color.DarkBlue;
                    }
            }
            return Color.Black;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public struct MorphingTerrainVertexFormatStruct
        {
            public Vector3 Position;
            public Vector2 TextureCoordinate;
            public Vector3 Normal;
            public float TargetHeight;
            public static int SizeInBytes { get { return (sizeof(float) * 8) + 4; } }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        //public class MorphingTerrainVertexFormatClass
        //{
        //    public Vector3 Position;
        //    public Vector2 TextureCoordinate;
        //    public Vector3 Normal;
        //    public float TargetHeight;
        //    public static int SizeInBytes { get { return (sizeof(float) * 8) + 4; } }
        //    public void positionAsRef(ref Vector3 inVec)
        //    {
        //        inVec = Position;
        //    }
        //};

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //private void vertexFormatClassToStruct(MorphingTerrainVertexFormatClass clazz, ref MorphingTerrainVertexFormatStruct strukt)
        //{
        //    strukt.Position = clazz.Position;
        //    strukt.Normal = clazz.Normal;
        //    strukt.TextureCoordinate = clazz.TextureCoordinate;
        //    strukt.TargetHeight = clazz.TargetHeight;
        //}
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void BuildVertexDecleration()
        {
            VertexElement[] vertexElements = new VertexElement[]
            {
                new VertexElement(0,VertexElementFormat.Vector3,VertexElementUsage.Position,0),
                new VertexElement(sizeof(float)*3,VertexElementFormat.Vector2,VertexElementUsage.TextureCoordinate,0),
                new VertexElement(sizeof(float)*5,VertexElementFormat.Vector3,VertexElementUsage.Normal,0),
                new VertexElement(sizeof(float)*8,VertexElementFormat.Single,VertexElementUsage.TextureCoordinate,1)
            };
            
            s_vertexDecleration = new VertexDeclaration(vertexElements);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum TerrainTextureSlot
        {
            deepWaterTexture, shallowWaterTexture, sandTexture, grassTexture, screeTexture, iceTexture
        };

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void LoadContent()
        {
            base.LoadContent();
            if (s_terrainTextures == null)
            {
                s_terrainTextures = new Texture2D[6];

                DirectoryInfo dir = new DirectoryInfo(Game.Content.RootDirectory);

                s_terrainTextures[(int)TerrainTextureSlot.deepWaterTexture] = Game.Content.Load<Texture2D>(Globals.deepWaterTextureId);
                s_terrainTextures[(int)TerrainTextureSlot.shallowWaterTexture] = Game.Content.Load<Texture2D>(Globals.shallowWaterTextureId);
                s_terrainTextures[(int)TerrainTextureSlot.sandTexture] = Game.Content.Load<Texture2D>(Globals.sandTextureId);
                s_terrainTextures[(int)TerrainTextureSlot.grassTexture] = Game.Content.Load<Texture2D>(Globals.grassTextureId);
                s_terrainTextures[(int)TerrainTextureSlot.screeTexture] = Game.Content.Load<Texture2D>(Globals.screeTextureId);
                s_terrainTextures[(int)TerrainTextureSlot.iceTexture] = Game.Content.Load<Texture2D>(Globals.iceTextureId);

            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public MorphingTerrainVertexFormatStruct[] Vertices
        {
            get { return m_vertices; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private TerrainSection m_terrainSection;
        protected MorphingTerrainVertexFormatStruct[] m_vertices;
        int m_sectorX;
        int m_numberOfVerticesX;
        int m_sectorZ;
        int m_numberOfVerticesZ;
        int m_numberOfQuadsX;
        int m_numberOfQuadsZ;
        int m_totalNumberOfQuads;
        int m_totalNumberOfIndicies;
        int m_totalNumberOfTriangles;
        Terrain m_terrain;
        private Effect m_effect;
        private Texture2D m_heightMap;
        private VertexBuffer m_vertexBuffer;

        private static IndexBuffer s_indexBuffer;
        private static VertexDeclaration s_vertexDecleration;
        private static Texture2D[] s_terrainTextures;
    }
}
