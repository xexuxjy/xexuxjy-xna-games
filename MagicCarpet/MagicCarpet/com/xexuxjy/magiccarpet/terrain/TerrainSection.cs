using System;
using System.Collections.Generic;
using System.Text;
using com.xexuxjy.magiccarpet.renderer;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.collision;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.terrain;
using com.xexuxjy.magiccarpet;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TerrainSection : WorldObject
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public TerrainSection(Terrain terrain, int sectorX, int sectorZ, Vector3 minBounds,Vector3 maxBounds, Game game)
            : base(new Vector3(), game)
        {
            // cheat and build once as they should all be uniform.
            BuildSectionIndices(minBounds, maxBounds);
            
            m_terrain = terrain;
            m_sectorX = sectorX;
            m_sectorZ = sectorZ;
            m_boundingBox = new BoundingBox(minBounds, maxBounds);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            // do this before base initialise otherwise we'll be added to the collision system.
            //m_collider = false;

            float sectionWidth = m_terrain.Width / m_terrain.NumSectionsX;
            float sectionBreadth = m_terrain.Breadth / m_terrain.NumSectionsZ;

            Vector3 transformVector = new Vector3();
            transformVector.X = m_sectorX * sectionWidth;
            transformVector.Z = m_sectorZ * sectionBreadth;
            // up the bounds of the object.
            Vector3 minBound = transformVector;
            minBound.Y = Globals.containmentMinHeight;
            Vector3 maxBound = transformVector;
            maxBound.X += sectionWidth;
            maxBound.Z += sectionBreadth;
            maxBound.Y = Globals.containmentMaxHeight;

            // need position to be the middle of it
            transformVector.X += sectionWidth / 2.0f;
            transformVector.Z += sectionBreadth / 2.0f;


            Position = transformVector;
            BuildRenderer();
            BuildCollisionObject();

            base.Initialize();

            Id = String.Format("TerrainSection([{0}],[{1}])", m_sectorX, m_sectorZ);
            SetDirty();


        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public DefaultRenderer Renderer
        {
            get { return m_terrainSectionRenderer; }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public void BuildRenderer()
        {
            m_terrainSectionRenderer = new TerrainSectionRenderer((MagicCarpet)Game, this, m_terrain);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetDirty()
        {
            m_isDirty = true;
            // reset this here - though may cause oddness with multiple height events.
            m_terrainMoveTime = 0.0f;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void ClearDirty()
        {
            m_isDirty = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool IsDirty()
        {
            return m_isDirty;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // FIXME - Sort out this god awful system of percentages and world points!
        //public Vector3 GetNormalAtPoint(int x, int z)
        //{
        //    if (null != m_terrainSectionRenderer)
        //    {
        //        return m_terrainSectionRenderer.GetNormalAtPoint(x, z, m_worldSpanX, m_worldSpanZ);
        //    }
        //    return Vector3.Zero;
        //}

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * m_terrain.TerrainMoveTime;


            m_terrainMoveTime += delta;
            m_terrainMoveTime = Math.Min(m_terrainMoveTime, 1.0f);
            if (IsDirty())
            {
                Vector3 min = m_boundingBox.Min;
                min.Y = Globals.worldMinPos.Y;
                Vector3 max = m_boundingBox.Max;
                max.Y = Globals.worldMaxPos.Y;

                ((BvhTriangleMeshShape)m_rigidBody.GetCollisionShape()).RefitTree(ref min,ref max);
            }


            base.Update(gameTime);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public float TerrainMoveTime { get { return m_terrainMoveTime; } }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool ContainsPoint(Vector3 point)
        {
            // FIXME - Do a proper check.
            return true;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void BuildCollisionObject()
        {
            TerrainSectionRenderer.MorphingTerrainVertexFormatStruct[] morphingVertices = m_terrainSectionRenderer.Vertices;
            m_plainVertices = new ObjectArray<Vector3>(morphingVertices.Length);
            for (int i = 0; i < morphingVertices.Length; ++i)
            {
                m_plainVertices[i] = morphingVertices[i].Position;
            }
            int totalTriangles = (Width-1) * (Breadth-1) * 2;

            TriangleIndexVertexArray indexVertexArrays = new TriangleIndexVertexArray(totalTriangles,
                s_indices, 1, m_plainVertices.Count, m_plainVertices, 1);

            CollisionShape heightFieldTerrain = new BvhTriangleMeshShape(indexVertexArrays, true, true);
            m_rigidBody = Globals.CollisionManager.LocalCreateRigidBody(0f, Matrix.CreateTranslation(m_position), heightFieldTerrain, true);

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ObjectArray<int> GetSectionIndices()
        {
            return s_indices;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

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


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public static void BuildSectionIndices(Vector3 min,Vector3 max)
        {
            int numX = (int)(max.X - min.X);
            int numZ = (int)(max.Z - min.Z);
            int numberOfQuadsX = numX - 1;
            int numberOfQuadsZ = numZ - 1;
            int stepSize = 6;

            if (s_indices == null)
            {
                s_indices = new ObjectArray<int>(numberOfQuadsX * numberOfQuadsZ * stepSize);
                for (int x = 0; x < numberOfQuadsX; x++)
                {
                    for (int y = 0; y < numberOfQuadsZ; y++)
                    {
                        int index = (x + y * (numberOfQuadsX)) * stepSize;
                        s_indices[index] = (x + y * numX);
                        s_indices[index + 1] = ((x + 1) + y * numX);
                        s_indices[index + 2] = ((x + 1) + (y + 1) * numX);

                        s_indices[index + 3] = (x + (y + 1) * numX);
                        s_indices[index + 4] = (x + y * numX);
                        s_indices[index + 5] = ((x + 1) + (y + 1) * numX);
                    }
                }
            }
        }

        protected Terrain m_terrain;
        protected TerrainSectionRenderer m_terrainSectionRenderer;
        public int m_sectorX;
        public int m_sectorZ;
        protected float m_terrainMoveTime; // counter used when adjusting terrain heights
        protected bool m_isDirty = false;
        protected ObjectArray<Vector3> m_plainVertices;
        protected static ObjectArray<int> s_indices;



    }
}
