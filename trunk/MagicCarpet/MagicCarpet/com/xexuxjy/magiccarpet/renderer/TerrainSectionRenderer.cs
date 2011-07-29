using System;
using com.xexuxjy.magiccarpet.objects;
using com.xexuxjy.magiccarpet.terrain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace com.xexuxjy.magiccarpet.renderers
{
    public class TerrainSectionRenderer 
    {
        static TerrainSectionRenderer()
        {
            buildVertexDecleration();
            buildTerrainTextures();
        }

        public TerrainSectionRenderer(TerrainSection terrainSection,Terrain terrain)
        {
            m_terrainSection = terrainSection;
            m_terrain = terrain;
            m_sectorX = terrainSection.m_sectorX;
            m_sectorZ = terrainSection.m_sectorZ;
            
            computeValues((int)terrainSection.m_worldSpanX, (int)terrainSection.m_worldSpanZ);
            loadEffectFile();

            buildVertexBuffer();
            buildIndexBuffer();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            if (m_terrainSection.IsDirty())
            {
                buildVertexBuffer();
            }

            GraphicsDevice device = ((IGraphicsDeviceService)GlobalUtils.GameServices.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            //device.VertexDeclaration = m_vertexDecleration;
            //device.Vertices[0].SetSource(m_vertexBuffer, 0, MorphingTerrainVertexFormatStruct.SizeInBytes);
            //device.Indices = m_indexBuffer;
            Camera camera = ((ICameraService)(GlobalUtils.GameServices.GetService(typeof(ICameraService)))).Camera;

            Matrix identity = Matrix.Identity;
            //Matrix translation = Matrix.CreateTranslation(m_terrainSection.Position);
            Matrix translation = Matrix.CreateTranslation(new Vector3());
            Matrix view = camera.View;
            Matrix world = Matrix.Multiply(translation, identity);

            Matrix projection = camera.PerspectiveMatrix;

            Matrix worldViewProjection = world * view * projection;

            // only one of these should be active.

            drawBasicEffect(device,ref view,ref world,ref projection);
            drawEffect(device, ref view, ref world, ref projection);

            drawDebugAxes(device);
            if (shouldDrawBoundingBox())
            {
                drawBoundingBox(device);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override int DrawOrder
        {
            get { return CommonSettings.TerrainDrawOrder; }
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void drawEffect(GraphicsDevice device, ref Matrix view, ref Matrix world, ref Matrix projection)
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

                foreach (EffectPass effectPass in m_effect.CurrentTechnique.Passes)
                {
                    effectPass.Apply();
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_totalNumberOfVertices, 0, (m_numberOfQuadsX * m_numberOfQuadsZ * 2));
                }
            }

        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        bool buildVertexBuffer()
        {
            bool result = true;
            GraphicsDevice device = ((IGraphicsDeviceService)GlobalUtils.GameServices.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            if (null != device)
            {
                //
                // Build a vertex buffer and determine
                // the min\max size of the sector
                //
                String tempName = String.Format("terrain_section_[0]_[1]", m_sectorX, m_sectorZ);

                buildTextureForGrid();


                //if (m_vertexBuffer == null)
                if(m_vertices == null)
                {
                    //m_vertexBuffer = new VertexBuffer(device, MorphingTerrainVertexFormatStruct.SizeInBytes * m_totalNumberOfVertices);

                    // All the vertices's are stored in a 1D array
                    m_vertices = new MorphingTerrainVertexFormatStruct[m_totalNumberOfVertices];
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
                m_vertexBuffer.SetData(m_vertices);     
                m_terrainSection.ClearDirty();
            }
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void buildIndexBuffer()
        {
            GraphicsDevice device = ((IGraphicsDeviceService)GlobalUtils.GameServices.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            m_indices = new int[m_numberOfQuadsX * m_numberOfQuadsZ * 6];
            for (int x = 0; x < m_numberOfQuadsX; x++)
            {
                for (int y = 0; y < m_numberOfQuadsZ; y++)
                {
                    int index = (x + y * (m_numberOfQuadsX)) * 6;
                    m_indices[index] = (x + y * m_numberOfVerticesX);
                    m_indices[index + 1] = ((x + 1) + y * m_numberOfVerticesX);
                    m_indices[index + 2] = ((x + 1) + (y + 1) * m_numberOfVerticesX);

                    m_indices[index + 3] = (x + (y + 1) * m_numberOfVerticesX);
                    m_indices[index + 4] = (x + y * m_numberOfVerticesX);
                    m_indices[index + 5] = ((x + 1) + (y + 1) * m_numberOfVerticesX);
                }
            }
            // this is the index buffer we are going to store the indices in
            m_indexBuffer = new IndexBuffer(device, typeof(int), m_indices.Length, BufferUsage.None);
            m_indexBuffer.SetData(m_indices, 0, m_indices.Length);
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int[] getOffsetIndices(int minX, int minZ, int maxX, int maxZ)
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

        protected void computeValues(int width, int height)
        {
            // Vertices
            m_numberOfVerticesX = width+1;
            m_numberOfVerticesZ = height+1;
            m_totalNumberOfVertices = m_numberOfVerticesX * m_numberOfVerticesZ;

            // Quads
            m_numberOfQuadsX = width;
            m_numberOfQuadsZ = height;
            m_totalNumberOfQuads = m_numberOfQuadsX * m_numberOfQuadsZ;

            m_totalNumberOfTriangles = m_totalNumberOfQuads * 2;
            m_totalNumberOfIndicies = m_totalNumberOfQuads * 6;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void computeNormals()
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

        private void buildTextureForGrid()
        {
            GraphicsDevice device = ((IGraphicsDeviceService)GlobalUtils.GameServices.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            if (null != device)
            {
                unsafe
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
                            Color color = getColourForTerrainType(m_terrain.GetTerrainSquareAtPoint(squareOffsetX+
xoffset, squareOffsetZ+zoffset).Type);
                            textureData[(i * textureWidth) + j] = color.PackedValue;
                        }
                    }
                    m_texture.SetData(textureData); 
           
                
                }
            }
            //m_texture.Save(@"c:\tmp\test.jpg", ImageFileFormat.Jpg);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void loadEffectFile()
        {
            GraphicsDevice device = ((IGraphicsDeviceService)GlobalUtils.GameServices.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

            m_effect = s_contentManager.Load<Effect>(CommonSettings.terrainEffect);
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
        
        public static System.Drawing.Brush getBrushForTerrainType(TerrainType terrainType)
        {
            if (s_brushes[(int)terrainType] == null)
            {
                Color color = getColourForTerrainType(terrainType);
                s_brushes[(int)terrainType] = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(color.A,color.R,color.G,color.B));
            }
            return s_brushes[(int)terrainType];
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 getNormalAtPoint(float xpct, float zpct,int worldSpanX,int worldSpanZ)
        {
            int x = (int)(xpct);
            int z = (int)(zpct);

            x = MathHelper.Clamp(0, x, worldSpanX - 1);
            z = MathHelper.Clamp(0, z, worldSpanZ - 1);

            return m_vertices[(z * m_numberOfVerticesZ) + x].Normal; 
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static Color getColourForTerrainType(TerrainType terrainType)
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

        private static void buildVertexDecleration()
        {
            VertexElement[] vertexElements = new VertexElement[]
            {
                new VertexElement(0,VertexElementFormat.Vector3,VertexElementUsage.Position,0),
                new VertexElement(sizeof(float)*3,VertexElementFormat.Vector2,VertexElementUsage.TextureCoordinate,0),
                new VertexElement(sizeof(float)*5,VertexElementFormat.Vector3,VertexElementUsage.Normal,0),
                new VertexElement(sizeof(float)*8,VertexElementFormat.Single,VertexElementUsage.TextureCoordinate,1)
            };
            GraphicsDevice device = ((IGraphicsDeviceService)GlobalUtils.GameServices.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
            
            s_vertexDecleration = new VertexDeclaration(vertexElements);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private enum TerrainTextureSlot
        {
            deepWaterTexture, shallowWaterTexture, sandTexture, grassTexture, screeTexture, iceTexture
        };

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static void buildTerrainTextures()
        {
            s_terrainTextures = new Texture2D[6];
            s_terrainTextures[(int)TerrainTextureSlot.deepWaterTexture] = s_contentManager.Load<Texture2D>(CommonSettings.deepWaterTextureId);
            s_terrainTextures[(int)TerrainTextureSlot.shallowWaterTexture] = s_contentManager.Load<Texture2D>(CommonSettings.shallowWaterTextureId);
            s_terrainTextures[(int)TerrainTextureSlot.sandTexture] = s_contentManager.Load<Texture2D>(CommonSettings.sandTextureId);
            s_terrainTextures[(int)TerrainTextureSlot.grassTexture] = s_contentManager.Load<Texture2D>(CommonSettings.grassTextureId);
            s_terrainTextures[(int)TerrainTextureSlot.screeTexture] = s_contentManager.Load<Texture2D>(CommonSettings.screeTextureId);
            s_terrainTextures[(int)TerrainTextureSlot.iceTexture] = s_contentManager.Load<Texture2D>(CommonSettings.iceTextureId);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public MorphingTerrainVertexFormatStruct[] Vertices
        {
            get { return m_vertices; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public int[] Indices
        {
            get { return m_indices; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static System.Drawing.Brush[] s_brushes = new System.Drawing.Brush[Enum.GetValues(typeof(TerrainType)).Length];
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
        private BasicEffect m_basicEffect;
        private Texture2D m_heightMap;
        private static VertexDeclaration s_vertexDecleration;
        private static Texture2D[] s_terrainTextures;
        private int[] m_indices;
        private Vector3[] m_vertices;
    }
}
