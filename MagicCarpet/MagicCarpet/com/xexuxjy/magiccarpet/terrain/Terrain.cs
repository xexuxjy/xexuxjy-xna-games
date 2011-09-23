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
using com.xexuxjy.magiccarpet.renderer;
using System.Collections.Generic;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework.Graphics;
using BulletXNA.BulletCollision;

namespace com.xexuxjy.magiccarpet.terrain
{
	public class Terrain : GameObject
	{
        ///////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain(Vector3 position,Game game)
            : base(position, game,GameObjectType.Terrain)
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
            m_heightMap = new float[Globals.WorldWidth * Globals.WorldWidth];
            m_heightMapTexture = new Texture2D(Game.GraphicsDevice, Globals.WorldWidth, Globals.WorldWidth, false, SurfaceFormat.Single);


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
            CollisionShape collisionShape = new HeightfieldTerrainShape(Globals.WorldWidth, Globals.WorldWidth, m_heightMap, 1f, -Globals.WorldHeight, Globals.WorldHeight, 1, true);
            m_collisionObject = new CollisionObject();
            m_collisionObject.SetCollisionShape(collisionShape);
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

            Vector3 lightDirection = new Vector3(0.5f,-1,0.5f);
            lightDirection.Normalize();
            Vector3 ambientLight = new Vector3(0.2f);
            Vector3 directionalLight = new Vector3(1f);

            //m_effect.Parameters["LightDirection"].SetValue(lightDirection);
            m_effect.Parameters["AmbientLight"].SetValue(ambientLight);
            m_effect.Parameters["DirectionalLight"].SetValue(directionalLight);


            m_effect.Parameters["LightPosition"].SetValue(new Vector3(1000, 40, 1000));

            BuildVertexBuffers();


            m_terrainRandom = new Random();

            InitialiseWorldGrid();

            //buildLandscape();
            LoadOrCreateHeighMap(null);
            //BuildTestTerrain1();
            //BuildSectionRenderers();
            BuildLandscape();
            base.Initialize();
        
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





            m_blockVertexBuffer = new VertexBuffer(Game.GraphicsDevice, PosOnlyVertex.VertexDeclaration, blockVertices.Length, BufferUsage.None);
            m_blockVertexBuffer.SetData<PosOnlyVertex>(blockVertices, 0, blockVertices.Length);
            m_blockIndexBuffer = new IndexBuffer(Game.GraphicsDevice, IndexElementSize.ThirtyTwoBits, blockIndices.Length, BufferUsage.None);
            m_blockIndexBuffer.SetData<int>(blockIndices);
        }

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
                m_heightMapTexture = new Texture2D(Game.GraphicsDevice, Globals.WorldWidth, Globals.WorldWidth, false, SurfaceFormat.Single);
                m_heightMapTexture.SetData<Single>(m_heightMap);

                m_effect.Parameters["HeightMapTexture"].SetValue(m_heightMapTexture);
                ClearTerrainChanged();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            UpdateHeightMapTexture();
            Matrix worldMatrix = Matrix.Identity;

            float a = (int)Math.Pow(3, m_numLevels);
            a *= (m_blockSize * 3);
            a *= -0.5f;

            //worldMatrix = Matrix.CreateTranslation(new Vector3(a, 0, a));

            Matrix viewProjection = Globals.Camera.ViewProjectionMatrix;
            BoundingFrustum boundingFrustrum = new BoundingFrustum(viewProjection);

            Game.GraphicsDevice.Indices = m_blockIndexBuffer;
            Game.GraphicsDevice.SetVertexBuffer(m_blockVertexBuffer);
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
                Matrix transform = worldMatrix * viewProjection;
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

            m_terrainSquareGrid = new TerrainSquare[Width][];
            for (int i = 0; i < m_terrainSquareGrid.Length; ++i)
            {
                m_terrainSquareGrid[i] = new TerrainSquare[Breadth];
            }

            Vector3 startPos = m_boundingBox.Min;

		}
		
		///////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(Vector3 point, float height)
        {
            AddPeak(point.X, point.Z, height);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////

        public void AddPeak(float x, float y, float height)
        {
            float defaultRadius = 10.0f;
            float maxHeight = 20.0f;
            AddPeak(x, y, defaultRadius, height, maxHeight);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void AddPeak(float x, float z, float radius, float height, float maxHeight)
        {

            TerrainUpdater terrainUpdate = new TerrainUpdater(new Vector3(x, 0, z), radius, s_terrainMoveTime, height, this);
            m_terrainUpdaters.Add(terrainUpdate);
        }		
        ///////////////////////////////////////////////////////////////////////////////////////////////

        //public void AddPeak(Vector3 position, float innerRadius, float outerRadius,float height,float maxHeight)
        //{
        //    float x = position.X;
        //    float y = position.Z;
        //    int innerLeftBound = (int)System.Math.Max(0, x - innerRadius);
        //    int innerRightBound = (int)System.Math.Min(Width, x + innerRadius);
        //    int innerUpBound = (int)System.Math.Max(0, y - innerRadius);
        //    int innerDownBound = (int)System.Math.Min(Breadth, y + innerRadius);

        //    int outerLeftBound = (int)System.Math.Max(0, x - outerRadius);
        //    int outerRightBound = (int)System.Math.Min(Width, x + outerRadius);
        //    int outerUpBound = (int)System.Math.Max(0, y - outerRadius);
        //    int outerDownBound = (int)System.Math.Min(Breadth, y + outerRadius);

        //    float innerSquared = innerRadius * innerRadius;

        //    // figure gradient.
        //    float gradient = 1f - ((outerRadius - innerRadius) / outerRadius);
        //    for (int i = outerLeftBound; i < outerRightBound; ++i)
        //    {
        //        for (int j = outerUpBound; j < outerDownBound; ++i)
        //        {
        //            TerrainSquare terrainSquare = GetTerrainSquareAtPoint(i, j);
        //            // for now only land squares can have their height changed in this way.
        //            if (terrainSquare.Type != TerrainType.immovable)
        //            {
        //                Vector2 lengthVector = new Vector2(Math.Abs(i - position.X), Math.Abs(j - position.Z));
        //                float distance = lengthVector.Length();
        //                float lerpValue = 1.0f - MathHelper.Lerp(outerRadius, innerRadius, distance);
        //                float oldHeight = GetHeightAtPoint(i, j, true);
        //                //float oldHeight = getHeightAtPoint(i, j);
        //                float newHeight = oldHeight + (height * lerpValue);
        //                newHeight = MathHelper.Clamp(-maxHeight, newHeight, maxHeight);
        //                SetHeightAtPoint(i, j, newHeight);
        //                SetTerrainType(terrainSquare, newHeight);
        //                Vector3 point = new Vector3(i, 0.0f, j);
        //                foreach (TerrainSection section in m_terrainSectionGrid)
        //                {
        //                    if (section.ContainsPoint(point))
        //                    {
        //                        section.SetDirty();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////

        //public virtual void UpdatePeak(float x, float y, float radius, float height, float maxHeight)
        //{
        //    int leftBound = (int)System.Math.Max(0, x - radius);
        //    int rightBound = (int)System.Math.Min(Width, x + radius);
        //    int upBound = (int)System.Math.Max(0, y - radius);
        //    int downBound = (int)System.Math.Min(Breadth, y + radius);


        //    float floatRadius = radius;

        //    for (int i = leftBound; i < rightBound; ++i)
        //    {
        //        for (int j = upBound; j < downBound; ++j)
        //        {
        //            TerrainSquare terrainSquare = GetTerrainSquareAtPoint(i, j);
        //            // for now only land squares can have their height changed in this way.
        //            if (terrainSquare.Type != TerrainType.immovable)
        //            {
        //                Vector2 vec2;
        //                vec2.X = System.Math.Abs(i - x);
        //                vec2.Y = System.Math.Abs(j - y);
        //                float distance = vec2.Length();
        //                float lerpValue = 1.0f - MathHelper.Lerp(0.0f, floatRadius, distance);
        //                // play with lerp value to smooth the terrain?
        //                //                          lerpValue = (float)Math.Sqrt(lerpValue);
        //                //lerpValue *= lerpValue;
        //                //                        lerpValue *= lerpValue;

        //                // ToDo - fractal hill generation.

        //                float oldHeight = GetHeightAtPoint(i, j, true);
        //                //float oldHeight = getHeightAtPoint(i, j);
        //                float newHeight = oldHeight + (height * lerpValue);
        //                newHeight = MathHelper.Clamp(-maxHeight, newHeight, maxHeight);
        //                SetHeightAtPoint(i, j, newHeight);
        //                SetTerrainType(terrainSquare, newHeight);
        //                Vector3 point = new Vector3(i, 0.0f, j);
        //                foreach (TerrainSection section in m_terrainSectionGrid)
        //                {
        //                    if (section.ContainsPoint(point))
        //                    {
        //                        section.SetDirty();
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

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
            Vector3 local = worldPoint - m_boundingBox.Min;
            int localX = (int)local.X;
            int localZ = (int)local.Z;
            m_terrainSquareGrid[localX][localZ].Height = worldPoint.Y;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPointLocal(int x,int y,float height)
        {
            m_heightMap[(y * Globals.WorldWidth) + x] = height;
        }


		///////////////////////////////////////////////////////////////////////////////////////////////
        public virtual float GetHeightAtPointLocal(float x, float z)
        {
            return 0f;
        }


		public virtual float GetHeightAtPointWorld(float x, float z)
		{
            float returnValue = float.MinValue;
            float ymid = (m_boundingBox.Max.Y + m_boundingBox.Min.Y) * 0.5f;
            Vector3 pos = new Vector3(x, ymid, z);
            // to local
            pos -= Position;
            if (m_boundingBox.Contains(pos) != ContainmentType.Disjoint)
            {
                Vector3 adjustedPos = pos - m_boundingBox.Min;
                int localX = (int)adjustedPos.X;
                int localZ = (int)adjustedPos.Z;
                returnValue = m_terrainSquareGrid[localX][localZ].Height;
            }
            return returnValue;
		}

		///////////////////////////////////////////////////////////////////////////////////////////////	
		
		public virtual void  BuildLandscape()
		{
            int counter = 0;
            int increment = 1;
            int maxHills = 1000;
            int maxInstanceHeight = 10;
            int maxOverallHeight = 20;
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
                int xpos = (int)((float)m_terrainRandom.NextDouble() * Width);
                int ypos = (int)((float)m_terrainRandom.NextDouble() * Breadth);
                float radius = ((float)m_terrainRandom.NextDouble() * maxRadius);
                float height = ((float)m_terrainRandom.NextDouble() * maxInstanceHeight);
                bool up = (float)m_terrainRandom.NextDouble() > 0.5;
                if (!up)
                {
                    height = -height;
                }
                AddPeak(xpos, ypos, radius, height, maxOverallHeight);
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

        public TerrainSquare GetTerrainSquareAtPoint(ref Vector3 worldPoint)
        {
            Vector3 local = worldPoint - m_boundingBox.Min;
            return m_terrainSquareGrid[(int)local.X][(int)local.Z];
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

        public void SetTerrainType(TerrainSquare terrainSquare,float val)
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
                    terrainUpdate.ApplyToTerrain(m_heightMap);
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        //public void DropRandomManaBall()
        //{
        //    int xpos = (int)(m_terrainRandom.NextDouble() * Width);
        //    int zpos = (int)(m_terrainRandom.NextDouble() * Breadth);
        //    double ypos = GetHeightAtPoint(xpos, zpos);
        //    Vector3 vec = new Vector3((float)xpos, (float)ypos, (float)zpos);
        //    ManaBall manaBall = (ManaBall)WorldObjectFactory.getInstance().getWorldObject(WorldObjectType.manaball,null, vec);
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare[][] TerrainSquares
        {
            get { return m_terrainSquareGrid; }
            set { m_terrainSquareGrid = value; }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 GetRandomWorldPositionXZ()
        {
            Vector3 result = new Vector3();
            result.X = ((float)m_terrainRandom.NextDouble() * Width);
            result.Z = ((float)m_terrainRandom.NextDouble() * Breadth);
            return result;
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public Vector3 GetRandomWorldPositionXZWithRange(Vector3 position,float distance)
        {
            Vector3 result = new Vector3();
            float sign = m_terrainRandom.NextDouble() > 0.5f ? 1.0f:-1.0f;
            result.X = position.X + (sign * ((float)m_terrainRandom.NextDouble() * distance));
            result.Z = position.Z + (sign * ((float)m_terrainRandom.NextDouble() * distance));

            // make sure it fits in bounds.
            result.X = MathHelper.Clamp(0.0f,result.X,Width);
            result.Z = MathHelper.Clamp(0.0f, result.Z, Width);

            return result;
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////	

        // simple terrain with two levels
        public void BuildTestTerrain1()
        {
            for (int z = 0; z < Globals.WorldWidth; ++z)
            {
                for (int x = 0; x < Globals.WorldWidth / 2; ++x)
                {
                    m_heightMap[(z * Globals.WorldWidth) + x] = 20.0f;
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



            public void ApplyToTerrain(float[] heightMap)
            {
                if (m_currentTime < m_totalTime)
                {
                    float floatRadius2 = m_radius * m_radius;

                    for (int i = m_minX; i < m_maxX; i++)
                    {
                        for (int j = m_minZ; j < m_maxZ; j++)
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
                                int index = (j * Globals.WorldWidth) + i;
                                float currentHeight = heightMap[index];
                                //float oldHeight = getHeightAtPoint(i, j);
                                float newHeight = currentHeight + (m_updateDeflection * lerpValue);
                                newHeight = MathHelper.Clamp(-m_terrain.s_maxTerrainHeight, newHeight, m_terrain.s_maxTerrainHeight);
                                heightMap[index] = newHeight;
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

        private TerrainSquare[][] m_terrainSquareGrid;
        private float[] m_heightMap;
 
        private List<TerrainUpdater> m_terrainUpdaters = new List<TerrainUpdater>();
        private List<TerrainUpdater> m_terrainUpdatersRemove = new List<TerrainUpdater>();

		private bool m_defaultHeightMethod = true;
        private bool m_terrainHasChanged;
        private float m_maxCoverage = 0.65f;
        private float m_minIslandSize = 5.0f;
        private float m_maxIslandSize = 15.0f;

        // the amount of space the terrain can move in a second.
        private float s_terrainMoveTime = 0.5f;
        private float s_maxTerrainHeight = 20f;

        const int m_numLevels = 2;
        const int m_blockVertices = 65;
        const int m_blockSize = m_blockVertices - 1;
        VertexBuffer m_blockVertexBuffer;
        IndexBuffer m_blockIndexBuffer;
        Effect m_effect;
        RasterizerState m_rasterizerState;
        Texture2D m_heightMapTexture;
        Texture2D m_baseTexture;
        Texture2D m_noiseTexture;

        private Random m_terrainRandom;
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