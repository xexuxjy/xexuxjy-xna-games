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

namespace com.xexuxjy.magiccarpet.terrain
{
	public class Terrain:WorldObject
	{
        ///////////////////////////////////////////////////////////////////////////////////////////////


        public Terrain(Vector3 position,Game game)
            : base(position, game)
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            m_collider = false;

            m_terrainRandom = new Random();

            InitialiseWorldGrid();

            //buildLandscape();
            BuildTestTerrain1();
            //BuildSectionRenderers();
            //buildLandscape();
            QuadTree.GetInstance().GetRootNode().BuildTerrainIndicies();
            base.Initialize();

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        public TerrainSection GetSectionForBoundingBox(ref BoundingBox boundingBox)
        {
            // quick test for now, go through and see which section contains it.
            // again may need a solution for splitting the box, but need to have that in one place
            // so test is against containts, not intersects.
            TerrainSection[][] sections = m_terrainSectionGrid;
            for (int i = 0; i < sections.Length; ++i)
            {
                for (int j = 0; j < sections[i].Length; ++j)
                if (sections[i][j].BoundingBox.Contains(boundingBox) == ContainmentType.Contains)
                {
                    return sections[i][j];
                }
            }

            return null;
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

        virtual public int NumSectionsX
        {
            get { return m_numTerrainSectionsX; }
            set { m_numTerrainSectionsX = value; }
        }

        
        ///////////////////////////////////////////////////////////////////////////////////////////////

        virtual public int NumSectionsZ
        {
            get { return m_numTerrainSectionsZ; }
            set { m_numTerrainSectionsZ = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

        
        protected void BuildSectionRenderers()
        {
            int totalSections = m_numTerrainSectionsX * m_numTerrainSectionsZ;
            TerrainSection[][] sections = m_terrainSectionGrid;
            for (int i = 0; i < sections.Length; ++i)
            {
                for (int j = 0; j < sections[i].Length; ++j)
                {
                    new TerrainSectionRenderer((MagicCarpet)Game, sections[i][j], this);
                }
            }
        }		
		///////////////////////////////////////////////////////////////////////////////////////////////
		
		protected virtual void InitialiseWorldGrid()
		{
            Vector3 halfExtents = new Vector3(32, s_maxTerrainHeight, 32);
            m_boundingBox = new BoundingBox(-halfExtents + Position, halfExtents + Position);

            m_numTerrainSectionsX = 1;//8;
            m_numTerrainSectionsZ = 1;//8;
            m_stepSize = 1;
            int spanPerSectionX = Width / m_numTerrainSectionsX;
            int spanPerSectionZ = Breadth / m_numTerrainSectionsZ;
            QuadTreeNode rootNode = QuadTree.GetInstance().GetRootNode();
            m_terrainSectionGrid = new TerrainSection[m_numTerrainSectionsX][];
            for(int i=0;i<m_terrainSectionGrid.Length;++i)
            {
                m_terrainSectionGrid[i] = new TerrainSection[m_numTerrainSectionsZ];
            }

            m_terrainSquareGrid = new TerrainSquare[Width][];
            for (int i = 0; i < m_terrainSquareGrid.Length; ++i)
            {
                m_terrainSquareGrid[i] = new TerrainSquare[Breadth];
            }

            Vector3 startPos = m_boundingBox.Min;

            for (int i = 0; i < m_numTerrainSectionsX; ++i)
            {
                for (int j = 0; j < m_numTerrainSectionsZ; ++j)
                {
                    Vector3 min = startPos + new Vector3(i * spanPerSectionX, 0, j * spanPerSectionZ);
                    Vector3 max = min+ new Vector3(spanPerSectionX,0,spanPerSectionZ);
                    min.Y = -s_maxTerrainHeight;
                    max.Y = s_maxTerrainHeight;
                    m_terrainSectionGrid[i][j] = new TerrainSection(this, i, j, m_stepSize,min,max, Game);
                    m_terrainSectionGrid[i][j].Initialize();
                    Console.WriteLine("[{0}] min[{1}] max[{2}].", m_terrainSectionGrid[i][ j].Id, min, max);
                }
            }
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


        public virtual float GetHeightAtPoint(Vector3 point)
        {
            // straight down
            float result = GetHeightAtPoint(point.X, point.Z);
            return result;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPoint(ref Vector3 worldPoint)
        {
            Vector3 local = worldPoint - m_boundingBox.Min;
            m_terrainSquareGrid[(int)local.X][(int)local.Z].Height = worldPoint.Y;
        }

		///////////////////////////////////////////////////////////////////////////////////////////////
        

        ///////////////////////////////////////////////////////////////////////////////////////////////

		public virtual float GetHeightAtPoint(float x, float z)
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

        private TerrainSection GetSectionAtPoint(float worldPosX, float worldPosZ , out float sectionXPct, out float sectionZPct)
        {
            int xdiff = Width / m_numTerrainSectionsX;
            int zdiff = Breadth / m_numTerrainSectionsZ;

            int x = (int)worldPosX / xdiff;
            int z = (int)worldPosZ / zdiff;

            x = Math.Min(x, m_numTerrainSectionsX - 1);
            z = Math.Min(z, m_numTerrainSectionsZ - 1);

            float xrem = worldPosX - (x * xdiff);
            float zrem = worldPosZ - (z * zdiff);
            
            sectionXPct = xrem;
            sectionZPct = zrem;
            return m_terrainSectionGrid[x][z];
        }

		///////////////////////////////////////////////////////////////////////////////////////////////	
		
		public virtual void  BuildLandscape()
		{
            //int counter = 0;
            //int increment = 1;
            //int maxHills = 1000;
            //int maxInstanceHeight = 10;
            //int maxOverallHeight = 20;
            //int maxRadius = 20;
            //int currentHills = 0;
            //while (currentHills++ < maxHills)
            //{
            //    if (counter == 5)
            //    {
            //        increment = - 1;
            //    }
            //    else if (counter == 0)
            //    {
            //        increment = 1;
            //    }
            //    counter += increment;
            //    int xpos = (int)((float)m_terrainRandom.NextDouble() * Width);
            //    int ypos = (int)((float)m_terrainRandom.NextDouble() * Breadth);
            //    float radius = ((float)m_terrainRandom.NextDouble() * maxRadius);
            //    float height = ((float)m_terrainRandom.NextDouble() * maxInstanceHeight);
            //    bool up = (float)m_terrainRandom.NextDouble() > 0.5;
            //    if (!up)
            //    {
            //        height = -height;
            //    }
            //    AddPeak(xpos, ypos, radius, height,maxOverallHeight);
            //}
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

            foreach (TerrainUpdater terrainUpdate in m_terrainUpdaters)
            {
                terrainUpdate.Update(gameTime);
                if (!terrainUpdate.Complete())
                {
                    terrainUpdate.ApplyToTerrain(m_terrainSquareGrid);
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
            m_terrainUpdatersRemove.Clear();
            // FIXME - avoid going through this entire list searching and use a list of updates instead.
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
        
        public TerrainSection[][] TerrainSections
        {
            get { return m_terrainSectionGrid; }
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
            Vector3 min = m_boundingBox.Min;
            for (int i = 0; i < Width; ++i)
            {
                for (int j = Breadth / 2; j < Breadth; ++j)
                {
                    Vector3 point = min + new Vector3(i, 20, j);
                    SetHeightAtPoint(ref point);
                }
            }
            TerrainSection[][] sections = m_terrainSectionGrid;
            for (int i = 0; i < sections.Length; ++i)
            {
                for (int j = 0; j < sections[i].Length; ++j)
                {
                    sections[i][j].SetDirty();
                }
            }
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
                m_position = position;
                // need to adjust position based on midpoint of terrain
                //m_position -= new Vector3(CommonSettings.worldWidth / 2, 0, CommonSettings.worldBreadth / 2);
                m_radius = radius;
                m_totalTime = totalTime;
                m_totalDeflection = totalDeflection;
                m_currentTime = 0f;

                BoundingBox terrainBB = m_terrain.BoundingBox;

                m_minX = (int)System.Math.Max(terrainBB.Min.X, position.X - radius);
                m_maxX = (int)System.Math.Min(terrainBB.Max.X, position.X + radius);
                m_minZ = (int)System.Math.Max(terrainBB.Min.Z, position.Z - radius);
                m_maxZ = (int)System.Math.Min(terrainBB.Max.Z, position.Z + radius);

                // build a list of terrain sections that will be affected by this updater so we can get them to 
                // refresh their vertices
                m_affectedSections = new List<TerrainSection>();
                BoundingSphere boundingSphere = new BoundingSphere(m_position, m_radius);

                TerrainSection[][] sections = m_terrain.TerrainSections;
                for (int i = 0; i < sections.Length; ++i)
                {
                    for (int j = 0; j < sections[i].Length; ++j)
                    {
                        TerrainSection terrainSection = sections[i][j];
                        if (terrainSection.BoundingBox.Intersects(boundingSphere))
                        {
                            m_affectedSections.Add(terrainSection);
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

            public void ApplyToTerrain(TerrainSquare[][] heightMap)
            {
                if (m_currentTime < m_totalTime)
                {
                    float floatRadius2 = m_radius * m_radius;

                    for (int i = m_minX; i < m_maxX; i++)
                    {
                        for (int j = m_minZ; j < m_maxZ; j++)
                        {
                            Vector3 worldPoint = new Vector3(i, 0, j);
                            Vector3 diff = worldPoint - m_position;
                            float diffLength2 = diff.LengthSquared();
                            if (diffLength2 < floatRadius2)
                            {
                                TerrainSquare terrainSquare = m_terrain.GetTerrainSquareAtPoint(ref worldPoint);
                                float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                                // play with lerp value to smooth the terrain?
                                //                          lerpValue = (float)Math.Sqrt(lerpValue);
                                //lerpValue *= lerpValue;
                                //                        lerpValue *= lerpValue;

                                // ToDo - fractal hill generation.

                                float currentHeight = terrainSquare.Height;
                                //float oldHeight = getHeightAtPoint(i, j);
                                float newHeight = currentHeight + (m_updateDeflection * lerpValue);
                                newHeight = MathHelper.Clamp(-m_terrain.s_maxTerrainHeight, newHeight, m_terrain.s_maxTerrainHeight);
                                Vector3 newPos = new Vector3(i, newHeight, j);
                                m_terrain.SetHeightAtPoint(ref newPos);
                            }
                        }
                    }
                    foreach (TerrainSection terrainSection in m_affectedSections)
                    {
                        terrainSection.SetDirty();
                    }
                }
            }

            private Terrain m_terrain;
            private Vector3 m_position;
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
            private List<TerrainSection> m_affectedSections; // this list of sections that this will overlap.
        }





        private int m_numTerrainSectionsX;
        private int m_numTerrainSectionsZ;
        private int m_stepSize;

        private TerrainSection[][] m_terrainSectionGrid;
        private TerrainSquare[][] m_terrainSquareGrid;

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

        private Random m_terrainRandom;
	}
}