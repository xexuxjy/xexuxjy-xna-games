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
            // register or re-initialise this terrain


            Vector3 halfExtents = new Vector3(32,1,32);
            m_boundingBox = new BoundingBox(-halfExtents+Position, halfExtents+Position);

            m_terrainRandom = new Random();

            BuildTerrainSquareGrid();
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
            foreach (TerrainSection section in m_terrainSectionGrid)
            {
                if (section.BoundingBox.Contains(boundingBox) == ContainmentType.Contains)
                {
                    return section;
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
            foreach (TerrainSection section in m_terrainSectionGrid)
            {
                new TerrainSectionRenderer((MagicCarpet)Game,section,this);
            }
        }		
		///////////////////////////////////////////////////////////////////////////////////////////////
		
		protected virtual void InitialiseWorldGrid()
		{
          // increase these to represent vertices

            m_numTerrainSectionsX = 1;//8;
            m_numTerrainSectionsZ = 1;//8;

            int spanPerSectionX = Width / m_numTerrainSectionsX;
            int spanPerSectionZ = Breadth / m_numTerrainSectionsZ;
            QuadTreeNode rootNode = QuadTree.GetInstance().GetRootNode();
            m_terrainSectionGrid = new TerrainSection[m_numTerrainSectionsX,m_numTerrainSectionsZ];

            Vector3 startPos = m_boundingBox.Min;
            int stepSize = 4;

            for (int i = 0; i < m_numTerrainSectionsX; ++i)
            {
                for (int j = 0; j < m_numTerrainSectionsZ; ++j)
                {
                    Vector3 min = startPos + new Vector3(i * spanPerSectionX, 0, j * spanPerSectionZ);
                    Vector3 max = min+ new Vector3(spanPerSectionX,0,spanPerSectionZ);
                    max.Y = m_boundingBox.Max.Y;
                    m_terrainSectionGrid[i,j] = new TerrainSection(this, i, j, 2,min,max, Game);
                    m_terrainSectionGrid[i,j].Initialize();
                    Console.WriteLine("[{0}] min[{1}] max[{2}].", m_terrainSectionGrid[i, j].Id, min, max);
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

            int leftBound = (int)System.Math.Max(m_boundingBox.Min.X, x - radius);
            int rightBound = (int)System.Math.Min(m_boundingBox.Max.X, x + radius);
            int upBound = (int)System.Math.Max(m_boundingBox.Min.Z, z - radius);
            int downBound = (int)System.Math.Min(m_boundingBox.Max.Z, z + radius);

            float xspan = rightBound - leftBound;
            float zspan = downBound - upBound;

            float floatRadius2 = radius * radius;

            Vector3 center = new Vector3(x,0,z);

            for (int i = leftBound; i < rightBound; ++i)
            {
                for (int j = upBound; j < downBound; ++j)
                {
                    Vector3 worldPoint = new Vector3(i, 0, j);
                    TerrainSquare terrainSquare = GetTerrainSquareAtPoint(ref worldPoint);
                    // for now only land squares can have their height changed in this way.
                    if (terrainSquare.Type != TerrainType.immovable)
                    {
                        Vector3 diff = worldPoint - center;
                        float diffLength2 = diff.LengthSquared();
                        if (diffLength2 < floatRadius2)
                        {
                            float lerpValue = (floatRadius2 - diffLength2) / floatRadius2;
                            // play with lerp value to smooth the terrain?
                            //                          lerpValue = (float)Math.Sqrt(lerpValue);
                            //lerpValue *= lerpValue;
                            //                        lerpValue *= lerpValue;

                            // ToDo - fractal hill generation.

                            float oldHeight = terrainSquare.Height;
                            //float oldHeight = getHeightAtPoint(i, j);
                            float newHeight = oldHeight + (height * lerpValue);
                            newHeight = MathHelper.Clamp(-maxHeight, newHeight, maxHeight);
                            Vector3 newPos = new Vector3(i, newHeight, j);
                            SetHeightAtPoint(ref newPos);
                            foreach (TerrainSection section in m_terrainSectionGrid)
                            {
                                if (section.ContainsPoint(worldPoint))
                                {
                                    section.SetDirty();
                                }
                            }
                        }

                    }
                }
            }
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
            float result = GetHeightAtPoint(point.X, point.Z,false);
			return result;
		}
		

        /////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual float GetHeightAtPoint(Vector3 point, bool getTargetHeight)
        {
            // straight down
            float result = GetHeightAtPoint(point.X, point.Z, getTargetHeight);
            return result;
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////
        //public Vector3 GetNormalAtPoint(Vector3 worldPoint)
        //{
        //    return GetNormalAtPointInternal(worldPoint);
        //}


        /////////////////////////////////////////////////////////////////////////////////////////////////

        //private Vector3 GetNormalAtPointInternal(Vector3 worldPoint)
        //{
        //    Vector3 result = Vector3.Zero;
        //    if (!(m_boundingBox.Contains(worldPoint) == ContainmentType.Disjoint))
        //    {
        //        Vector3 localPosition = worldPoint - Position;

        //        Vector3 lerpResult = MathUtil.Vector3InverseLerp(BoundingBox.Min, localPosition, BoundingBox.Max);

        //        // read 4 map values and get an average from them.
        //        Vector3 v0 = GetNormalAtPoint(iMapX0, iMapZ0);
        //        Vector3 v1 = GetNormalAtPoint(iMapX1, iMapZ0);
        //        Vector3 v2 = GetNormalAtPoint(iMapX0, iMapZ1);
        //        Vector3 v3 = GetNormalAtPoint(iMapX1, iMapZ1);

        //        Vector3 avgLo = (v1 * fMapX) + (v0 * (1.0f - fMapX));
        //        Vector3 avgHi = (v3 * fMapX) + (v2 * (1.0f - fMapX));
        //        Vector3 result = (avgHi * fMapZ) + (avgLo * (1.0f - fMapZ)); ;
        //    }
        //    return result;
        //}
        
        /////////////////////////////////////////////////////////////////////////////////////////////////

        //private Vector3 GetNormalAtPoint(int x, int z)
        //{
        //    float outX;
        //    float outZ;
        //    TerrainSection terrainSection = GetSectionAtPoint(x, z, out outX, out outZ);
        //    return terrainSection.GetNormalAtPoint(x, z);
        //}


        ///////////////////////////////////////////////////////////////////////////////////////////////

        public void SetHeightAtPoint(ref Vector3 worldPoint)
        {
            Vector3 local = worldPoint - m_boundingBox.Min;
            m_terrainSquareGrid[(int)local.X, (int)local.Z].TargetHeight = worldPoint.Y;
        }

		///////////////////////////////////////////////////////////////////////////////////////////////
        
        public virtual float GetHeightAtPoint(int x, int z)
        {
            return GetHeightAtPoint(x, z, false);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////

		public virtual float GetHeightAtPoint(float x, float z,bool getTargetHeight)
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
                returnValue = getTargetHeight ? m_terrainSquareGrid[localX, localZ].TargetHeight : m_terrainSquareGrid[localX, localZ].Height;
            }
            else
            {
                int ibreak = 0;
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
            return m_terrainSectionGrid[x,z];
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
            return m_terrainSquareGrid[(int)local.X, (int)local.Z];
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
        //public void SetTerrainType(int x, int z)
        //{
        //    TerrainSquare terrainSquare = GetTerrainSquareAtPoint(x, z);
        //    float val = terrainSquare.Position.Y;
        //    SetTerrainType(terrainSquare, val);
        //}

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
            for (int i = 0; i < m_terrainSquareGrid.GetLength(0); ++i)
            {
                for (int j = 0; j < m_terrainSquareGrid.GetLength(1); ++j)
                {
                    //int index = (int)((x * m_worldExtents.Width) + z);
                    float currentHeight = m_terrainSquareGrid[i, j].Height;
                    float targetHeight = m_terrainSquareGrid[i, j].TargetHeight;
                    if (!MathHelperExtension.CompareFloat(currentHeight, targetHeight))
                    {
                        bool down = targetHeight < currentHeight;

                        // adjust the height by move time and clamp it.
                        //float delta = (updateTime.ElapsedGameTime.Milliseconds / 1000.0f) * TerrainMoveTime;
                        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * TerrainMoveTime;
                        if (delta > 0f)
                        {
                            int ibreak = 0;
                        }
                        float diff = Math.Abs(targetHeight - currentHeight);
                        if (delta > diff)
                        {
                            delta = diff;
                        }

                        if (down)
                        {
                            delta *= -1.0f;
                        }

                        //m_terrainSquareGrid[i,j].SetHeight(currentHeight + delta);
                        m_terrainSquareGrid[i, j].Height = (currentHeight + delta);
                    }
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private void BuildTerrainSquareGrid()
        {
            m_terrainSquareGrid = new TerrainSquare[Width,Breadth];
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

        //public void ToggleVisibility()
        //{
        //    for (int i = 0; i < m_terrainSectionGrid.Length; ++i)
        //    {
        //        ((DefaultRenderer)m_terrainSectionGrid[i].Renderer).setVisible(!m_terrainSectionGrid[i].Renderer.Visible);
        //    }
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public TerrainSquare[,] TerrainSquares
        {
            get { return m_terrainSquareGrid; }
            set { m_terrainSquareGrid = value; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	
        
        public TerrainSection[,] TerrainSections
        {
            get { return m_terrainSectionGrid; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private void UpdateTerrainHeights(GameTime gameTime)
        {
            //for(int i=0;i<m_terrainHeightList.Length;++i)
            //{
            //    // set a height value that lerps between current height
            //    float currentHeight = getHeightAtPoint();
            //    addPeak(Position,height);
            //}
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
            foreach (TerrainSection section in m_terrainSectionGrid)
            {
                section.SetDirty();
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////	

        private struct TerrainHeightUpdate
        {
            Vector3 position;
            float height;
            float radius;
            long currentTime;
            long maxTime;
        };


        ///////////////////////////////////////////////////////////////////////////////////////////////	


        public float TerrainMoveTime{get{return s_terrainMoveTime;}}
        
        private Vector3 m_worldHalfdExtents;
        private Vector3 m_position;

        private int m_numTerrainSectionsX;
        private int m_numTerrainSectionsZ;

        private TerrainSection[,] m_terrainSectionGrid;
        private TerrainSquare[,] m_terrainSquareGrid;

		private bool m_defaultHeightMethod = true;
        private bool m_terrainHasChanged;
        private float m_maxCoverage = 0.65f;
        private float m_minIslandSize = 5.0f;
        private float m_maxIslandSize = 15.0f;

        // the amount of space the terrain can move in a second.
        private float s_terrainMoveTime = 5;

        private List<TerrainHeightUpdate> m_terrainHeightList;
        private Random m_terrainRandom;
	}
}