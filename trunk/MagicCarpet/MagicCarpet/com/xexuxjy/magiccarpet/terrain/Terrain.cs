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

namespace com.xexuxjy.magiccarpet.terrain
{
    public class Terrain : GameObject, ICollideable
	{
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain(Vector3 position,Game game)
            : base(position, game,GameObjectType.terrain)
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int Width
		{
			get
			{
				return (int)(m_boundingBox.Max.X - m_boundingBox.Min.X);
			}
		}
        
        ///////////////////////////////////////////////////////////////////////////////////////////////
        
        virtual public int Breadth
		{
			get
			{
				return (int)(m_boundingBox.Max.Z - m_boundingBox.Min.Z);
			}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void LoadOrCreateHeighMap(String textureName)
        {
            m_heightMap = new float[m_textureWidth * m_textureWidth];
            m_heightMapTexture = new Texture2D(Game.GraphicsDevice, m_textureWidth, m_textureWidth, false, SurfaceFormat.Single);


            if (!String.IsNullOrEmpty(textureName))
            {
                Texture2D wrongFormatTexture = Game.Content.Load<Texture2D>("Textures\\Terrain\\"+textureName);
                //m_heightMapTexture = new Texture2D(Game.GraphicsDevice, wrongFormatTexture.Width, wrongFormatTexture.Height, false, SurfaceFormat.Single);
                Color[] colorData = new Color[wrongFormatTexture.Width * wrongFormatTexture.Height];
                wrongFormatTexture.GetData<Color>(colorData);

                //m_heightMapTexture.GetData<Single>(m_heightMap);

                for (int i = 0; i < colorData.Length; ++i)
                {
                    m_heightMap[i] = colorData[i].R;
                }

                //m_heightMapTexture.SetData<Single>(m_heightMap);
            }
            UpdateHeightMap();
            
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        protected override void BuildCollisionObject()
        {
            // Should really 
            CollisionShape collisionShape = new HeightfieldTerrainShape(m_textureWidth, m_textureWidth, m_heightMap, 1f, -Globals.WorldHeight, Globals.WorldHeight, 1, true);
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.terrain;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)GameObjectType.spell;
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(0f, Matrix.CreateTranslation(Position), collisionShape, m_motionState, true, this, collisionFlags, collisionMask);
            
            //m_collisionObject = new CollisionObject();
            //m_collisionObject.SetCollisionShape(collisionShape);
            //m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() | CollisionFlags.CF_KINEMATIC_OBJECT);


            //CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.terrain;
            //CollisionFilterGroups collisionMask = (CollisionFilterGroups)GameObjectType.spell;


            //Globals.CollisionManager.AddToWorld(m_collisionObject,collisionFlags,collisionMask);
            //m_collisionObject = 
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////
        
        public override void Initialize()
        {
            m_effect = Game.Content.Load<Effect>("Effects\\Terrain\\ClipTerrain");
            m_baseTexture = Game.Content.Load<Texture2D>("Textures\\Terrain\\base");
            m_noiseTexture = Game.Content.Load<Texture2D>("Textures\\Terrain\\noise");

            m_effect.Parameters["BaseTexture"].SetValue(m_baseTexture);
            m_effect.Parameters["NoiseTexture"].SetValue(m_noiseTexture);

            Vector3 ambientLight = new Vector3(0.3f);
            Vector3 directionalLight = new Vector3(0.5f);

            m_effect.Parameters["AmbientLight"].SetValue(ambientLight);
            m_effect.Parameters["DirectionalLight"].SetValue(directionalLight);
            m_effect.Parameters["LightPosition"].SetValue(new Vector3(0, 40, 0));


            BuildVertexBuffers();


            Globals.random = new Random();

            InitialiseWorldGrid();

            //buildLandscape();
            LoadOrCreateHeighMap(null);
            //BuildTestTerrain1();
            //BuildSectionRenderers();
            BuildLandscape();
            base.Initialize();
        
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

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
                //// ugly way to guess the texture slots.
                //try
                //{
                //    for (int i = 0; i < 8; ++i)
                //    {
                //        //if (Game.GraphicsDevice.Textures[i] == m_heightMapTexture)
                //        {
                //            Game.GraphicsDevice.Textures[i] = null;
                //        }
                //    }
                //}
                //catch (System.Exception ex)
                //{
                //}
                m_heightMapTexture = new Texture2D(Game.GraphicsDevice, m_textureWidth, m_textureWidth, false, SurfaceFormat.Single);
                m_heightMapTexture.SetData<Single>(m_heightMap);

                m_effect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);
                ClearTerrainChanged();
            }
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
            float oneOverTextureWidth = 1f / (m_textureWidth-1);

            //Vector3 maxPos = Vector3.Zero;

            Vector3 lastStartPosition = Vector3.Zero;

            int numSpans = Globals.WorldWidth / m_blockSize;
            Vector3 blockSize = new Vector3(m_blockSize, 0, m_blockSize);
            Vector3 startPosition = new Vector3(-Globals.WorldWidth * 0.5f, 0, -Globals.WorldWidth * 0.5f);

            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                for(int j=0;j<numSpans;++j)
                {
                    for(int i=0;i<numSpans;++i)
                    {
                        Vector3 localPosition = new Vector3((m_blockSize) * i, 0, (m_blockSize) * j);

                        Vector3 worldPosition = localPosition + startPosition;

                        Vector3 minbb = new Vector3(worldPosition.X, -Globals.WorldHeight, worldPosition.Z);
                        Vector3 maxbb = minbb + blockSize;
                        maxbb.Y = Globals.WorldHeight;

                        BoundingBox bb = new BoundingBox(minbb,maxbb);

                        if (boundingFrustrum.Intersects(bb))
                        {
                            Matrix transform = Matrix.CreateTranslation(startPosition) * viewProjection;
                            //Matrix transform = viewProjection * Matrix.CreateTranslation(worldPosition);
                            //Matrix transform = viewProjection;
                            m_effect.Parameters["WorldViewProjMatrix"].SetValue(transform);
                            m_effect.Parameters["FineTextureBlockOrigin"].SetValue(new Vector4(oneOverTextureWidth, oneOverTextureWidth, localPosition.X, localPosition.Z));

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

                Game.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_blockVertexBuffer.VertexCount, 0, m_blockIndexBuffer.IndexCount / 3);
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
            m_boundingBox = new BoundingBox(Position+Globals.worldMinPos,Position+Globals.worldMaxPos);
            m_terrainSquareGrid = new TerrainSquare[Globals.WorldWidth * Globals.WorldWidth];
            for (int i = 0; i < m_terrainSquareGrid.Length; ++i)
            {
                m_terrainSquareGrid[i] = new TerrainSquare();
            }

		}

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(Vector3 point, float height)
        {
            point = WorldToLocal(point);
            AddPeak(point.X, point.Z, height);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(float x, float y, float height)
        {
            float defaultRadius = 10.0f;
            AddPeak(x, y, defaultRadius, height);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void AddPeak(float x, float z, float radius, float height)
        {

            TerrainUpdater terrainUpdate = new TerrainUpdater(new Vector3(x, 0, z), radius, s_terrainMoveTime, height, this);
            m_terrainUpdaters.Add(terrainUpdate);
        }		
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPointWorld(Vector3 point)
        {
            // straight down
            float result = GetHeightAtPointWorld(point.X, point.Z);
            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointWorld(ref Vector3 worldPoint)
        {
            Vector3 local = WorldToLocal(worldPoint);
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            SetHeightAtPointLocal(localX,localZ,worldPoint.Y);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointLocal(int localX,int localZ,float height)
        {
            Debug.Assert(localX >= 0 && localX <= Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ <= Globals.WorldWidth);
            //m_heightMap[(localZ * Globals.WorldWidth) + localX] = height;
            m_heightMap[(localZ * m_textureWidth)+localX] = height;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

		public virtual float GetHeightAtPointWorld(float x, float z)
		{
            Vector3 local = WorldToLocal(new Vector3(x,0,z));
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

        public virtual void  BuildLandscape()
		{
            int counter = 0;
            int increment = 1;
            int maxHills = 10;
            int maxInstanceHeight = 10;
            int maxRadius = 20;
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
                int xpos = (int)((float)Globals.random.NextDouble() * Width);
                int ypos = (int)((float)Globals.random.NextDouble() * Breadth);
                float radius = ((float)Globals.random.NextDouble() * maxRadius);
                float height = ((float)Globals.random.NextDouble() * maxInstanceHeight);
                bool up = (float)Globals.random.NextDouble() > 0.5;
                if (!up)
                {
                    height = -height;
                }
                AddPeak(xpos, ypos, radius, height);
            }

            UpdateHeightMap();
		}
		
		///////////////////////////////////////////////////////////////////////////////////////////////
		
		public virtual void  ToggleHeightMethod()
		{
			m_defaultHeightMethod = !m_defaultHeightMethod;
		}
		
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        //// used to put a castle on the land, this will take the center of the castle and figure out 
        //// how much space it will need , flatten the land etc.
        //public void addCastle(Castle theCastle)
        //{
        //    Vector3 castlePosition = theCastle.Position;
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
        //        Vector3 castlePosition2 = new Vector3();
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

        public TerrainSquare GetTerrainSquareAtPointWorld(ref Vector3 worldPoint)
        {
            Vector3 local = worldPoint - m_boundingBox.Min;
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            return GetTerrainSquareAtPointLocal(localX, localZ);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare GetTerrainSquareAtPointLocal(int localX,int localZ)
        {
            Debug.Assert(localX >= 0 && localX < Globals.WorldWidth);
            Debug.Assert(localZ >= 0 && localZ < Globals.WorldWidth);
            return m_terrainSquareGrid[(localZ * Globals.WorldWidth)+localX];
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

        public void SetTerrainTypeAndOccupier(Vector3 position, TerrainType terrainType, GameObject occupier)
        {
            TerrainSquare square = GetTerrainSquareAtPointWorld(ref position);
            square.Type = terrainType;
            square.Occupier = occupier;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void SetTerrainTypeOld(TerrainSquare terrainSquare,float val)
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

        public bool IsPointInTerrain(ref Vector3 point)
           {
            return ((point.X >= 0.0f && point.X <= Width) && (point.Z >= 0.0f && point.Z <= Breadth));

        }        
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void AssertPointInTerrain(ref Vector3 point)
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

        public Vector3 GetRandomWorldPositionXZ()
        {
            Vector3 result = new Vector3();
            result.X = ((float)Globals.random.NextDouble() * Width);
            result.Z = ((float)Globals.random.NextDouble() * Breadth);
            result.Y = GetHeightAtPointLocal((int)result.X, (int)result.Z);


            return LocalToWorld(result);
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 GetRandomWorldPositionXZWithRange(Vector3 position,float distance)
        {
            Vector3 result = new Vector3();
            float sign = Globals.random.NextDouble() > 0.5f ? 1.0f:-1.0f;
            result.X = position.X + (sign * ((float)Globals.random.NextDouble() * distance));
            result.Z = position.Z + (sign * ((float)Globals.random.NextDouble() * distance));

            // make sure it fits in bounds.
            result.X = MathHelper.Clamp(result.X,0.0f,Width);
            result.Z = MathHelper.Clamp(result.Z,0.0f, Width);

            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public void ClampToTerrain(ref Vector3 position)
        {
            Vector3 local = WorldToLocal(position);

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
            get{return s_terrainMoveTime;}
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private class TerrainUpdater
        {
            public TerrainUpdater(Vector3 position, float radius, float totalTime, float totalDeflection,Terrain terrain)
            {
                m_terrain = terrain;
                m_positionLocal = position;

                BoundingBox terrainBB = m_terrain.BoundingBox;

                // need to adjust position based on midpoint of terrain
                //m_position -= new Vector3(CommonSettings.worldWidth / 2, 0, CommonSettings.worldBreadth / 2);
                m_radius = radius;
                m_totalTime = totalTime;
                m_totalDeflection = totalDeflection;
                m_currentTime = 0f;


                m_minX = (int)System.Math.Max(0, position.X - radius);
                m_maxX = (int)System.Math.Min(Globals.WorldWidth, position.X + radius);
                m_minZ = (int)System.Math.Max(0, position.Z - radius);
                m_maxZ = (int)System.Math.Min(Globals.WorldWidth, position.Z + radius);
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
                            Vector3 worldPoint = new Vector3(i, 0, j);
                            Vector3 diff = worldPoint - m_positionLocal;
                            float diffLength2 = diff.LengthSquared();
                            if (diffLength2 < floatRadius2)
                            {
                                float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                                // play with lerp value to smooth the terrain?
                                //                          lerpValue = (float)Math.Sqrt(lerpValue);
                                //lerpValue *= lerpValue;
                                //                        lerpValue *= lerpValue;

                                // ToDo - fractal hill generation.
                                float currentHeight = m_terrain.GetHeightAtPointLocal(i, j);
                                //float oldHeight = getHeightAtPoint(i, j);
                                float newHeight = currentHeight + (m_updateDeflection * lerpValue);

                                newHeight = MathHelper.Clamp(newHeight, -Globals.WorldHeight,Globals.WorldHeight);
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
            private Vector3 m_positionLocal;
            private Vector3 m_midPoint;

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
        const int m_blockVertices = 33;
        const int m_blockSize = m_blockVertices - 1;
        const int m_textureWidth = Globals.WorldWidth + 1;
        
        VertexBuffer m_blockVertexBuffer;
        IndexBuffer m_blockIndexBuffer;
        Effect m_effect;
        RasterizerState m_rasterizerState;
        Texture2D m_heightMapTexture;
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

        public void ProcessCollision(ICollideable partner, ManifoldPoint manifoldPoint)
        {
            // terrain shouldn't do anything. up to other objects to collide.
        }

        public GameObject GetGameObject()
        {
            return this;
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