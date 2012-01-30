/*
* Created on 11-Jan-2006
*
* To change the template for this generated file go to
* Window - Preferences - Java - Code Generation - Code and Comments
*/
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.collision;
using System.Collections.Generic;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.interfaces;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.renderer;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class Terrain : GameObject, ICollideable
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain(IndexedVector3 position)
            : base(position, GameObjectType.terrain)
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int Width
        {
            get
            {
                return (int)(BoundingBox.Max.X - BoundingBox.Min.X);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int Breadth
        {
            get
            {
                return (int)(BoundingBox.Max.Z - BoundingBox.Min.Z);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void LoadOrCreateHeighMap(String textureName)
        {
            m_heightMap = new float[m_textureWidth * m_textureWidth];
            m_heightMapTexture = new Texture2D(Game.GraphicsDevice, m_textureWidth, m_textureWidth, false, SurfaceFormat.Single);
            //m_normalMapTexture = new Texture2D(Game.GraphicsDevice, m_textureWidth, m_textureWidth, false, SurfaceFormat.Color);
            m_normalMapRenderTarget = new RenderTarget2D(Game.GraphicsDevice, m_textureWidth, m_textureWidth, false, SurfaceFormat.Color, DepthFormat.None);

            if (!String.IsNullOrEmpty(textureName))
            {
                Texture2D wrongFormatTexture = Game.Content.Load<Texture2D>("Textures\\Terrain\\" + textureName);
                Color[] colorData = new Color[wrongFormatTexture.Width * wrongFormatTexture.Height];
                wrongFormatTexture.GetData<Color>(colorData);

                for (int i = 0; i < colorData.Length; ++i)
                {
                    m_heightMap[i] = colorData[i].R;
                }

            }
            UpdateHeightMap();

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            CollisionShape collisionShape = new HeightfieldTerrainShape(m_textureWidth, m_textureWidth, m_heightMap, 1f, -Globals.WorldHeight, Globals.WorldHeight, 1, true);
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.terrain;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)(GameObjectType.spell | GameObjectType.manaball | GameObjectType.camera);
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(0f, IndexedMatrix.CreateTranslation(Position), collisionShape, m_motionState, true, this, collisionFlags, collisionMask);

        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            m_terrainEffect = Game.Content.Load<Effect>("Effects\\Terrain\\ClipTerrain");
            m_normalsEffect = Game.Content.Load<Effect>("Effects\\Terrain\\TerrainNormalMap");
            m_baseTexture = Game.Content.Load<Texture2D>("Textures\\Terrain\\base");
            m_noiseTexture = Game.Content.Load<Texture2D>("Textures\\Terrain\\noise");

            m_terrainEffect.Parameters["BaseTexture"].SetValue(m_baseTexture);
            m_terrainEffect.Parameters["NoiseTexture"].SetValue(m_noiseTexture);

            IndexedVector3 ambientLight = new IndexedVector3(0.1f);
            IndexedVector3 directionalLight = new IndexedVector3(0.4f);

            m_terrainEffect.Parameters["AmbientLight"].SetValue(ambientLight);
            m_terrainEffect.Parameters["DirectionalLight"].SetValue(directionalLight);
            m_terrainEffect.Parameters["LightPosition"].SetValue(new IndexedVector3(0, 40, 0));
            IndexedVector3 lightDirection = new IndexedVector3(10, -10, 0);
            lightDirection.Normalize();

            m_terrainEffect.Parameters["LightDirection"].SetValue(lightDirection);

            m_helperScreenQuad = new ScreenQuad(Game);
            m_helperScreenQuad.Initialize();

            BuildVertexBuffers();

            InitialiseWorldGrid();

            LoadOrCreateHeighMap(null);
            base.Initialize();

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void BuildVertexBuffers()
        {
            int adjustedVertices = (m_blockSize * m_multiplier) + 1;
            int adjustedIndices = (m_blockSize * m_multiplier);


            PosOnlyVertex[] blockVertices = new PosOnlyVertex[adjustedVertices * adjustedVertices];
            int[] blockIndices = new int[adjustedIndices * adjustedIndices * 6];
            int indexCounter = 0;
            int vertexCounter = 0;
            int stride = adjustedVertices;


            for (int y = 0; y < adjustedVertices; ++y)
            {
                for (int x = 0; x < adjustedVertices; ++x)
                {
                    Vector2 v = new Vector2(x, y) * m_oneOver;
                    PosOnlyVertex vpnt = new PosOnlyVertex(v);

                    blockVertices[vertexCounter++] = vpnt;
                    if (x < adjustedIndices && y < adjustedIndices)
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

            m_blockVertexBuffer = new VertexBuffer(Game.GraphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            m_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            m_blockIndexBuffer = new IndexBuffer(Game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            m_blockIndexBuffer.SetData<int>(blockIndices);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateHeightMapTexture()
        {
            if (HasTerrainChanged())
            {
                m_normalsEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);
                m_terrainEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);

                // need these here to unset the textures so I can change them.
                foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                foreach (EffectPass pass in m_normalsEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                }

                m_heightMapTexture.SetData<Single>(m_heightMap);
                m_terrainEffect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);

                UpdateNormalMap();


                ClearTerrainChanged();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateNormalMap()
        {
            //m_terrainEffect.Parameters["NormalMapTexture"].SetValue((Texture)null);

            int width = m_heightMapTexture.Width;

            Game.GraphicsDevice.SetRenderTarget(m_normalMapRenderTarget);
            Game.GraphicsDevice.Clear(Color.White);

            m_normalsEffect.CurrentTechnique = m_normalsEffect.Techniques["ComputeNormals"];
            m_normalsEffect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);
            float texelWidth = 1f / (float)width;
            m_normalsEffect.Parameters["texelWidth"].SetValue(texelWidth);

            Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, width, 0, 0, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);

            m_normalsEffect.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);

            foreach (EffectPass pass in m_normalsEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                m_helperScreenQuad.Draw();
            }

            //m_normalsEffect.Parameters["HeightMapTexture"].SetValue((Texture)null);

            Game.GraphicsDevice.SetRenderTarget(null);
            m_terrainEffect.Parameters["NormalMapTexture"].SetValue(m_normalMapRenderTarget);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Draw(GameTime gameTime)
        {
            UpdateHeightMapTexture();


            Matrix viewProjection = Globals.Camera.ViewProjectionMatrix;
            BoundingFrustum boundingFrustrum = new BoundingFrustum(viewProjection);

            Game.GraphicsDevice.Indices = m_blockIndexBuffer;
            Game.GraphicsDevice.SetVertexBuffer(m_blockVertexBuffer);
            //float oneOverTextureWidth = 1f/m_textureWidth;
            float oneOverTextureWidth = 1f / (m_textureWidth - 1);

            //IndexedVector3 maxPos = IndexedVector3.Zero;

            IndexedVector3 lastStartPosition = IndexedVector3.Zero;

            int numSpans = Globals.WorldWidth / m_blockSize;
            IndexedVector3 blockSize = new IndexedVector3(m_blockSize, 0, m_blockSize);
            IndexedVector3 startPosition = new IndexedVector3(-Globals.WorldWidth * 0.5f, 0, -Globals.WorldWidth * 0.5f);

            m_terrainEffect.CurrentTechnique = m_terrainEffect.Techniques["TileTerrain"];

            foreach (EffectPass pass in m_terrainEffect.CurrentTechnique.Passes)
            {
                for (int j = 0; j < numSpans; ++j)
                {
                    for (int i = 0; i < numSpans; ++i)
                    {
                        IndexedVector3 localPosition = new IndexedVector3((m_blockSize) * i, 0, (m_blockSize) * j);

                        IndexedVector3 worldPosition = localPosition + startPosition;

                        IndexedVector3 minbb = new IndexedVector3(worldPosition.X, -Globals.WorldHeight, worldPosition.Z);
                        IndexedVector3 maxbb = minbb + blockSize;
                        maxbb.Y = Globals.WorldHeight;

                        BoundingBox bb = new BoundingBox(minbb, maxbb);

                        if (boundingFrustrum.Intersects(bb))
                        {

                            Matrix transform = Matrix.CreateTranslation(startPosition) * viewProjection;

                            m_terrainEffect.Parameters["CameraPosition"].SetValue(Globals.Camera.Position);
                            m_terrainEffect.Parameters["WorldViewProjMatrix"].SetValue(transform);
                            Vector4 scaleFactor = new Vector4(m_oneOver);
                            m_terrainEffect.Parameters["ScaleFactor"].SetValue(scaleFactor);
                            m_terrainEffect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(oneOverTextureWidth, oneOverTextureWidth, localPosition.X, localPosition.Z));

                            m_terrainEffect.Parameters["FogEnabled"].SetValue(true);
                            m_terrainEffect.Parameters["FogStart"].SetValue(20);
                            m_terrainEffect.Parameters["FogEnd"].SetValue(200);



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
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

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

        ///////////////////////////////////////////////////////////////////////////////////////////////

        protected virtual void InitialiseWorldGrid()
        {
            m_terrainSquareGrid = new TerrainSquare[Globals.WorldWidth * Globals.WorldWidth];
            for (int i = 0; i < m_terrainSquareGrid.Length; ++i)
            {
                m_terrainSquareGrid[i] = new TerrainSquare();
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(IndexedVector3 point, float height)
        {
            point = WorldToLocal(point);
            AddPeak(point.X, point.Z, height);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(float x, float y, float height)
        {
            float defaultRadius = 15.0f;
            AddPeak(x, y, defaultRadius, height);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void AddPeak(float x, float z, float radius, float height)
        {
            //TerrainUpdater terrainUpdate = new TerrainUpdater(new IndexedVector3(x, 0, z), radius, s_terrainMoveTime, height, this);
            //m_terrainUpdaters.Add(terrainUpdate);
            TerrainUpdater.ApplyImmediate(new IndexedVector3(x, 0, z), radius, height, this);
            UpdateHeightMap();

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorld(IndexedVector3 point)
        {
            // straight down
            float result = GetHeightAtPointWorld(point.X, point.Z);
            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointWorld(ref IndexedVector3 worldPoint)
        {
            IndexedVector3 local = WorldToLocal(worldPoint);
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            SetHeightAtPointLocal(localX, localZ, worldPoint.Y);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointLocal(int localX, int localZ, float height)
        {
            Debug.Assert(localX >= 0 && localX <= Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ <= Globals.WorldWidth);
            //m_heightMap[(localZ * Globals.WorldWidth) + localX] = height;
            m_heightMap[(localZ * m_textureWidth) + localX] = height;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorld(float x, float z)
        {
            IndexedVector3 local = WorldToLocal(new IndexedVector3(x, 0, z));
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            return GetHeightAtPointLocal(localX, localZ);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual float GetHeightAtPointLocal(int localX, int localZ)
        {
            Debug.Assert(localX >= 0 && localX <= Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ <= Globals.WorldWidth);
            //return m_heightMap[(localZ * Globals.WorldWidth) + localX];
            return m_heightMap[(localZ * m_textureWidth) + localX];
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public virtual void BuildRandomLandscape()
        {
            int counter = 0;
            int increment = 1;
            int maxHills = Width;
            int maxInstanceHeight = 10;
            int maxRadius = 50;
            int currentHills = 0;
            while (currentHills++ < maxHills)
            {
                if (counter == 5)
                {
                    increment = -1;
                }
                else if (counter == 0)
                {
                    increment = 1;
                }
                counter += increment;
                int xpos = (int)((float)Globals.s_random.NextDouble() * Width);
                int ypos = (int)((float)Globals.s_random.NextDouble() * Breadth);
                float radius = ((float)Globals.s_random.NextDouble() * maxRadius);
                // don't want too small a radius
                if (radius < 5f)
                {
                    radius = 5f;
                }


                float height = ((float)Globals.s_random.NextDouble() * maxInstanceHeight);
                bool up = (float)Globals.s_random.NextDouble() > 0.5;
                if (!up)
                {
                    height = -height;
                }


                TerrainUpdater.ApplyImmediate(new IndexedVector3(xpos, 0, ypos), radius, height, this);
            }

            UpdateHeightMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void ToggleHeightMethod()
        {
            m_defaultHeightMethod = !m_defaultHeightMethod;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        //// used to put a castle on the land, this will take the center of the castle and figure out 
        //// how much space it will need , flatten the land etc.
        //public void addCastle(Castle theCastle)
        //{
        //    IndexedVector3 castlePosition = theCastle.Position;
        //    BoundingBox box = theCastle.BoundingBox;
        //    int left = (int)box.Min.X;
        //    int right = (int)box.Max.X;
        //    int top = (int)box.Min.Z;
        //    int bottom = (int)box.Max.Z;

        //    // If it will fit then add it. otherwise no
        //    if((left > 0 ) && (right < Width) && (top > 0 ) && (bottom < Breadth))
        //    {
        //        // hmm what should the height be? height at 'centre' of castle
        //        float height = getHeightAtPoint(castlePosition);
        //        // go through and set the height on the squares
        //        for(int i=left;i<right;++i)
        //        {
        //            for(int j=top;j<bottom;++j)
        //            {
        //                TerrainSquare terrainSquare = getTerrainSquareAtPoint(i,j);
        //                terrainSquare.setTargetHeight(height);
        //                terrainSquare.Type = TerrainType.castle;
        //            }
        //        }
        //        IndexedVector3 castlePosition2 = new IndexedVector3();
        //        castlePosition2.X = theCastle.Position.X;
        //        castlePosition2.Z = theCastle.Position.Z;
        //        castlePosition2.Y = height;

        //        theCastle.Position = castlePosition2;
        //        m_terrainHasChanged = true;
        //    }
        //    else
        //    {
        //        // won't fit.
        //    }
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare GetTerrainSquareAtPointWorld(ref IndexedVector3 worldPoint)
        {
            IndexedVector3 local = worldPoint - BoundingBox.Min;
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            return GetTerrainSquareAtPointLocal(localX, localZ);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare GetTerrainSquareAtPointLocal(int localX, int localZ)
        {
            Debug.Assert(localX >= 0 && localX < Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ < Globals.WorldWidth);
            return m_terrainSquareGrid[(localZ * Globals.WorldWidth) + localX];
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool HasTerrainChanged()
        {
            return m_terrainHasChanged;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void ClearTerrainChanged()
        {
            m_terrainHasChanged = false;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetTerrainTypeAndOccupier(IndexedVector3 position, TerrainType terrainType, GameObject occupier)
        {
            TerrainSquare square = GetTerrainSquareAtPointWorld(ref position);
            square.Type = terrainType;
            square.Occupier = occupier;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetTerrainTypeOld(TerrainSquare terrainSquare, float val)
        {
            TerrainType result = TerrainType.water;
            if (val <= -5.0f)
            {
                result = TerrainType.deepwater;
            }
            else if (val > -5.0f && val <= -2.0f)
            {
                result = TerrainType.water;
            }
            else if (val > -2.0f && val <= 0.0f)
            {
                result = TerrainType.shallowwater;
            }
            else
                if (val > 0.0f && val <= 1.0f)
                {
                    result = TerrainType.beach;
                }
                else
                    if (val > 1.0f && val <= 3.0f)
                    {
                        result = TerrainType.grass;
                    }
                    else
                        if (val > 3.0f && val <= 5.0f)
                        {
                            result = TerrainType.grass2;
                        }
                        else
                            if (val > 5.0f && val <= 8.0f)
                            {
                                result = TerrainType.rock;
                            }
                            else
                                if (val > 8.0f)
                                {
                                    result = TerrainType.ice;
                                }
            terrainSquare.Type = result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public bool IsPointInTerrain(ref IndexedVector3 point)
        {
            return ((point.X >= 0.0f && point.X <= Width) && (point.Z >= 0.0f && point.Z <= Breadth));

        }
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void AssertPointInTerrain(ref IndexedVector3 point)
        {
            //Debug.Assert(isPointInTerrain(ref point), "Point not in terrain");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override void Update(GameTime gameTime)
        {
            // go through and adjust our current and target heights to make the landscape move nicely.
            base.Update(gameTime);
            bool terrainChanged = false; ;
            foreach (TerrainUpdater terrainUpdate in m_terrainUpdaters)
            {
                terrainUpdate.Update(gameTime);
                if (!terrainUpdate.Complete())
                {
                    terrainUpdate.ApplyToTerrain();
                    terrainChanged = true;
                }
                else
                {
                    m_terrainUpdatersRemove.Add(terrainUpdate);
                }
            }

            foreach (TerrainUpdater terrainUpdate in m_terrainUpdatersRemove)
            {
                m_terrainUpdaters.Remove(terrainUpdate);
            }

            if (terrainChanged)
            {
                UpdateHeightMap();
            }

            m_terrainUpdatersRemove.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare[] TerrainSquares
        {
            get { return m_terrainSquareGrid; }
            set { m_terrainSquareGrid = value; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public IndexedVector3 GetRandomWorldPositionXZ()
        {
            IndexedVector3 result = new IndexedVector3();
            result.X = ((float)Globals.s_random.NextDouble() * Width);
            result.Z = ((float)Globals.s_random.NextDouble() * Breadth);
            result.Y = GetHeightAtPointLocal((int)result.X, (int)result.Z);


            return LocalToWorld(result);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public IndexedVector3 GetRandomWorldPositionXZWithRange(IndexedVector3 position, float distance)
        {
            IndexedVector3 result = new IndexedVector3();
            float sign = Globals.s_random.NextDouble() > 0.5f ? 1.0f : -1.0f;
            result.X = position.X + (sign * ((float)Globals.s_random.NextDouble() * distance));
            result.Z = position.Z + (sign * ((float)Globals.s_random.NextDouble() * distance));

            // make sure it fits in bounds.
            result.X = MathHelper.Clamp(result.X, 0.0f, Width);
            result.Z = MathHelper.Clamp(result.Z, 0.0f, Width);

            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void ClampToTerrain(ref IndexedVector3 position)
        {
            IndexedVector3 local = WorldToLocal(position);

            local.X = MathHelper.Clamp(local.X, 0, Width);
            local.Z = MathHelper.Clamp(local.Z, 0, Width);

            position = LocalToWorld(local);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        // simple terrain with two levels
        public void BuildTestTerrain1()
        {
            for (int z = 0; z < m_textureWidth; ++z)
            {
                for (int x = 0; x < m_textureWidth / 2; ++x)
                {
                    m_heightMap[(z * m_textureWidth) + x] = 3.0f;
                }
            }
            UpdateHeightMap();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void UpdateHeightMap()
        {
            m_terrainHasChanged = true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public float TerrainMoveTime
        {
            get { return s_terrainMoveTime; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	


        // The following fractal height functions were taken from a MineCraft clone (reference needed)
        // but are attributed else where.

        public void BuildFractalLandscape()
        {
            m_heightMap = GetRandomHeightData(m_textureWidth);
        }


        public static int[] GenerateFractalTerrainMap(int width, int maxHeight)
        {

            float[] heightData = GetRandomHeightData(width);
            int[] heights = FillHeights(heightData, width, maxHeight);

            return heights;
        }

        /// <summary>
        /// Converts a noise map into an integer based height map.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="width">Width in number of cubes</param>
        /// <param name="maxHeight">Maximum number of stacked cubes</param>
        /// <returns></returns>
        private static int[] FillHeights(float[] heightData, int width, int maxHeight)
        {
            float min = float.MaxValue;
            float max = float.MinValue;

            //heightData includes an extra row and column that we don't want. 
            //By using x and y instead of looping directly through the array, 
            //we can easily avoid that extra row and column.

            //Noise map won't be from 0.0 to 1.0, but we need it that way.  So,
            //start by finding the minimum and maximum value of the map.
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int index = y * width + x;
                    if (heightData[index] > max)
                    {
                        max = heightData[index];
                    }
                    if (heightData[index] < min)
                    {
                        min = heightData[index];
                    }
                }
            }

            //To make sure the range is all positive and starts with 0, we will
            //use an adjustment value to shift all values by.
            float adjust = -min;

            //To make sure the highest value is 1.0, we will divide all values
            //by the difference between minimum and maximum.
            float spread = (max + adjust) - (min + adjust);

            int[] heights = new int[width * width];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    int index = y * width + x;
                    float heightRatio = (heightData[index] + adjust) / spread;
                    heights[index] = (int)(heightRatio * maxHeight);
                }
            }

            return heights;
        }

        /// <summary>
        /// Method uses the Diamond-Square algorithm to populate a floating
        /// point noise map.
        /// </summary>
        /// <param name="width">The width of the map in cubes - must be a power
        /// of 2</param>
        /// <returns></returns>
        private static float[] GetRandomHeightData(int width)
        {
            //The width needs to be a power of 2 + 1 for the fractal algorithm to work.
            //width = width + 1;

            float[] heightData = new float[width * width];

            //Later on there will be overlap with the algorithm.  Filling the
            //array with max value to start will help use identify which values
            //have already been set.
            for (int index = 0; index < heightData.Length; index++)
            {
                heightData[index] = float.MaxValue;
            }

            //Initialize the 4 corners to seed values.
            int x = 0;
            int y = 0;
            heightData[y * width + x] = 0.0f;
            x = width - 1;
            y = 0;
            heightData[y * width + x] = 0.0f;
            x = width - 1;
            y = width - 1;
            heightData[y * width + x] = 0.0f;
            x = 0;
            y = width - 1;
            heightData[y * width + x] = 0.0f;

            //Iterate through diamond and square passes until the entire array
            //is populated.
            for (int power = 1; power < width; power *= 2)
            {
                DiamondPass(heightData, power, width);
                SquarePass(heightData, power, width);
            }

            return heightData;
        }

        /// <summary>
        /// Completes a diamond pass on the entire array at the given power.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="power">A power of 2 number that helps to adjust stride
        /// values appropriate to the pass number</param>
        /// <param name="width">Width of the map in number of cubes - must be a
        /// power of 2</param>
        private static void DiamondPass(float[] heightData, int power, int width)
        {
            int jump = (width - 1) / power;
            float heightMultiplier = 5.0f;


            for (int x = 0; x < width - jump; x += jump)
            {
                for (int y = 0; y < width - jump; y += jump)
                {
                    int centerX = x + jump / 2;
                    int centerY = y + jump / 2;

                    if (heightData[centerY * width + centerX] == float.MaxValue)
                    {
                        float corner1 = heightData[y * width + x];
                        float corner2 = heightData[y * width + x + jump];
                        float corner3 = heightData[(y + jump) * width + x];
                        float corner4 = heightData[(y + jump) * width + x + jump];

                        float average = (corner1 + corner2 + corner3 + corner4) / 4.0f;

                        float value = average + (float)((Globals.s_random.NextDouble() - 0.5) * heightMultiplier);

                        heightData[centerY * width + centerX] = value;
                    }
                }
            }
        }

        /// <summary>
        /// Completes a square pass on the entire array at the given power.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="power">A power of 2 number that helps to adjust stride
        /// values appropriate to the pass number</param>
        /// <param name="width">Width of the map in number of cubes - must be a
        /// power of 2</param>
        private static void SquarePass(float[] heightData, int power, int width)
        {
            int jump = (width - 1) / power;

            for (int x = 0; x < width - jump; x += jump)
            {
                for (int y = 0; y < width - jump; y += jump)
                {
                    int stride = jump / 2;

                    int midX;
                    int midY;

                    //Top
                    midX = x + jump / 2;
                    midY = y;
                    SquareFill(heightData, stride, midX, midY, width);

                    //Bottom
                    midX = x + stride;
                    midY = y + jump;
                    SquareFill(heightData, stride, midX, midY, width);

                    //Left
                    midX = x;
                    midY = y + stride;
                    SquareFill(heightData, stride, midX, midY, width);

                    //Right
                    midX = x + jump;
                    midY = y + stride;
                    SquareFill(heightData, stride, midX, midY, width);
                }
            }
        }

        /// <summary>
        /// Helper method for the square pass that does the actual filling in
        /// of data.
        /// </summary>
        /// <param name="heightData">Noise map to be converted</param>
        /// <param name="stride">Distance from the center to look for seed
        /// values</param>
        /// <param name="x">The x coordinate for the center value to be changed</param>
        /// <param name="y">The y coordinate for the center value to be changed</param>
        /// <param name="width">The width of the map in number of cubes</param>
        private static void SquareFill(float[] heightData, int stride, int x, int y, int width)
        {
            if (heightData[y * width + x] == float.MaxValue)
            {
                int topX = x;
                int topY = y + stride;

                int bottomX = x;
                int bottomY = y - stride;

                int leftX = x - stride;
                int leftY = y;

                int rightX = x + stride;
                int rightY = y;

                //There will be out of bounds issues at the borders, so make the values "wrap"
                if (topY >= width)
                {
                    topY = stride;
                }

                if (bottomY < 0)
                {
                    bottomY = (width - 1) - stride;
                }

                if (leftX < 0)
                {
                    leftX = (width - 1) - stride;
                }

                if (rightX >= width)
                {
                    rightX = 0;
                }

                float topValue = heightData[topY * width + topX];
                float bottomValue = heightData[bottomY * width + bottomX];
                float leftValue = heightData[leftY * width + leftX];
                float rightValue = heightData[rightY * width + rightX];

                float average = (topValue + bottomValue + leftValue + rightValue) / 4.0f;

                float value = average + (float)((Globals.s_random.NextDouble() - 0.5) * 2.0);
                heightData[y * width + x] = value;
            }
        }
    


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private struct TerrainUpdater
        {
            public TerrainUpdater(IndexedVector3 position, float radius, float totalTime, float totalDeflection, Terrain terrain)
            {
                m_terrain = terrain;
                m_positionLocal = position;
                m_updateDeflection = 0f;

                BoundingBox terrainBB = m_terrain.BoundingBox;

                // need to adjust position based on midpoint of terrain
                //m_position -= new IndexedVector3(CommonSettings.worldWidth / 2, 0, CommonSettings.worldBreadth / 2);
                m_radius = radius;
                m_totalTime = totalTime;
                m_totalDeflection = totalDeflection;
                m_currentTime = 0f;


                m_minX = (int)System.Math.Max(0, position.X - radius);
                m_maxX = (int)System.Math.Min(Globals.WorldWidth, position.X + radius);
                m_minZ = (int)System.Math.Max(0, position.Z - radius);
                m_maxZ = (int)System.Math.Min(Globals.WorldWidth, position.Z + radius);
            }


            public static void ApplyImmediate(IndexedVector3 position, float radius, float totalDeflection, Terrain terrain)
            {
                int minX = (int)System.Math.Max(0, position.X - radius);
                int maxX = (int)System.Math.Min(Globals.WorldWidth, position.X + radius);
                int minZ = (int)System.Math.Max(0, position.Z - radius);
                int maxZ = (int)System.Math.Min(Globals.WorldWidth, position.Z + radius);

                float floatRadius2 = radius * radius;
                for (int j = minZ; j < maxZ; j++)
                {
                    for (int i = minX; i < maxX; i++)
                    {
                        IndexedVector3 worldPoint = new IndexedVector3(i, 0, j);
                        IndexedVector3 diff = worldPoint - position;
                        float diffLength2 = diff.LengthSquared();
                        if (diffLength2 < floatRadius2)
                        {
                            float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                            lerpValue = (float)Math.Pow(lerpValue, 4f);
                            lerpValue = (float)Math.Sin(MathUtil.SIMD_HALF_PI * lerpValue);
                            //lerpValue += 1.0f;

                            // play with lerp value to smooth the terrain?
                            //                          lerpValue = (float)Math.Sqrt(lerpValue);
                            //lerpValue *= lerpValue;
                            //lerpValue = (float)Math.Pow(lerpValue, 4f);
                            // ToDo - fractal hill generation.
                            float currentHeight = terrain.GetHeightAtPointLocal(i, j);
                            //float oldHeight = getHeightAtPoint(i, j);
                            float newHeight = currentHeight + (totalDeflection * lerpValue);

                            newHeight = MathHelper.Clamp(newHeight, -Globals.WorldHeight, Globals.WorldHeight);
                            terrain.SetHeightAtPointLocal(i, j, newHeight);
                        }
                    }
                }
            }


            public void ApplyToTerrain()
            {
                if (m_currentTime < m_totalTime)
                {
                    float floatRadius2 = m_radius * m_radius;
                    for (int j = m_minZ; j < m_maxZ; j++)
                    {
                        for (int i = m_minX; i < m_maxX; i++)
                        {
                            IndexedVector3 worldPoint = new IndexedVector3(i, 0, j);
                            IndexedVector3 diff = worldPoint - m_positionLocal;
                            float diffLength2 = diff.LengthSquared();
                            if (diffLength2 < floatRadius2)
                            {
                                float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                                lerpValue = (float)Math.Pow(lerpValue, 1.5f);
                                float currentHeight = m_terrain.GetHeightAtPointLocal(i, j);
                                float newHeight = currentHeight + (m_updateDeflection * lerpValue);

                                newHeight = MathHelper.Clamp(newHeight, -Globals.WorldHeight, Globals.WorldHeight);
                                m_terrain.SetHeightAtPointLocal(i, j, newHeight);
                            }
                        }
                    }
                }
            }


            public void Update(GameTime gameTime)
            {
                float timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;
                m_currentTime += timeStep;
                m_updateDeflection = (timeStep / m_totalTime) * m_totalDeflection;
            }

            public bool Complete()
            {
                return m_currentTime > m_totalTime;
            }



            private Terrain m_terrain;
            private IndexedVector3 m_positionLocal;
            //private IndexedVector3 m_midPoint;

            private float m_radius;
            private float m_totalTime;
            private float m_currentTime;
            private float m_totalDeflection;
            private float m_updateDeflection;
            int m_minX;
            int m_maxX;
            int m_minZ;
            int m_maxZ;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private TerrainSquare[] m_terrainSquareGrid;
        private float[] m_heightMap;

        private List<TerrainUpdater> m_terrainUpdaters = new List<TerrainUpdater>();
        private List<TerrainUpdater> m_terrainUpdatersRemove = new List<TerrainUpdater>();

        private bool m_defaultHeightMethod = true;
        private bool m_terrainHasChanged;
        private float m_maxCoverage = 0.65f;
        private float m_minIslandSize = 5.0f;
        private float m_maxIslandSize = 15.0f;

        // the time taken for the complete terrain move.
        private float s_terrainMoveTime = 5.0f;

        const int m_numLevels = 1;
        const int m_blockSize = 32;
        const int m_textureWidth = Globals.WorldWidth + 1;

        const int m_multiplier = 1;
        const float m_oneOver = 1f / (float)(m_multiplier);


        VertexBuffer m_blockVertexBuffer;
        IndexBuffer m_blockIndexBuffer;
        Effect m_terrainEffect;
        Effect m_normalsEffect;

        RasterizerState m_rasterizerState;
        Texture2D m_heightMapTexture;

        //Texture2D m_normalMapTexture;
        RenderTarget2D m_normalMapRenderTarget;

        ScreenQuad m_helperScreenQuad;

        Texture2D m_baseTexture;
        Texture2D m_noiseTexture;

        public int GetCollisionMask()
        {
            throw new NotImplementedException();
        }

        public bool ShouldCollideWith(ICollideable partner)
        {
            return true;
        }


        public Texture2D BaseTexture
        {
            get { return m_baseTexture; }
        }
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