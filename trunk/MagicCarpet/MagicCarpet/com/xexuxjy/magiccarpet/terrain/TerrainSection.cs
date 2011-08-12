using System;
using System.Collections.Generic;
using System.Text;
using com.xexuxjy.magiccarpet.renderer;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.collision;
using System.Diagnostics;
using com.xexuxjy.magiccarpet;
using BulletXNA.BulletCollision;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TerrainSection : GameObject
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public TerrainSection(Terrain terrain, int sectorX, int sectorZ, int stepSize,Vector3 minBounds,Vector3 maxBounds, Game game)
            : base(new Vector3(), game)
        {
            // cheat and build once as they should all be uniform.
            BuildSectionIndices(minBounds, maxBounds,stepSize);
            
            m_terrain = terrain;
            m_sectorX = sectorX;
            m_sectorZ = sectorZ;
            m_stepSize = stepSize;
            m_boundingBox = new BoundingBox(minBounds, maxBounds);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            //Position = (m_boundingBox.Max - m_boundingBox.Min) * 0.5f ;
            BuildRenderer();
            BuildCollisionObject();

            base.Initialize();

            Id = String.Format("TerrainSection([{0}],[{1}])", m_sectorX, m_sectorZ);
            SetDirty();


        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override Vector3 Position
        {
            get
            {
                return m_terrain.Position;
            }
            set { }
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

        public void UpdatePlainVerticesAndCollisionShape(TerrainSectionRenderer.MorphingTerrainVertexFormatStruct[] morphingVertices)
        {
            Vector3 min = m_boundingBox.Min;
            min.Y = Globals.worldMinPos.Y;
            Vector3 max = m_boundingBox.Max;
            max.Y = Globals.worldMaxPos.Y;
            if (m_plainVertices == null)
            {
                m_plainVertices = new ObjectArray<Vector3>(morphingVertices.Length);
            }

            for (int i = 0; i < morphingVertices.Length; ++i)
            {
                m_plainVertices[i] = morphingVertices[i].Position;
            }
            if (m_rigidBody != null)
            {
                ((BvhTriangleMeshShape)m_rigidBody.GetCollisionShape()).RefitTree(ref min, ref max);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        
        public override void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * m_terrain.TerrainMoveTime;


            m_terrainMoveTime += delta;
            m_terrainMoveTime = Math.Min(m_terrainMoveTime, 1.0f);
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
            int totalTriangles = s_indices.Count / 3;

            TriangleIndexVertexArray indexVertexArrays = new TriangleIndexVertexArray(totalTriangles,
                s_indices, 1, m_plainVertices.Count, m_plainVertices, 1);

            CollisionShape heightFieldTerrain = new BvhTriangleMeshShape(indexVertexArrays, true, true);
            m_rigidBody = Globals.CollisionManager.LocalCreateRigidBody(0f, Matrix.CreateTranslation(Position), heightFieldTerrain,null, true);

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


        public static void BuildSectionIndices(Vector3 min,Vector3 max,int stepSize)
        {
            int numVerticesX = (int)(max.X - min.X) ;
            numVerticesX /= stepSize;
            int numVerticesZ = (int)(max.Z - min.Z);
            numVerticesZ /= stepSize;
            int numberOfQuadsX = numVerticesX - 1;
            int numberOfQuadsZ = numVerticesZ - 1;
            int stride = 6;

            if (s_indices == null)
            {
                s_indices = new ObjectArray<int>(numberOfQuadsX * numberOfQuadsZ * stride);
                for (int x = 0; x < numberOfQuadsX; x++)
                {
                    for (int y = 0; y < numberOfQuadsZ; y++)
                    {
                        int index = (x + y * (numberOfQuadsX)) * stride;
                        s_indices[index] = (x + y * numVerticesX);
                        s_indices[index + 1] = ((x + 1) + y * numVerticesX);
                        s_indices[index + 2] = ((x + 1) + (y + 1) * numVerticesX);

                        s_indices[index + 3] = (x + (y + 1) * numVerticesX);
                        s_indices[index + 4] = (x + y * numVerticesX);
                        s_indices[index + 5] = ((x + 1) + (y + 1) * numVerticesX);
                    }
                }
            }
        }

        protected Terrain m_terrain;
        protected TerrainSectionRenderer m_terrainSectionRenderer;
        public int m_sectorX;
        public int m_sectorZ;
        public int m_stepSize; // allows us to skip to avoid 1:1
        protected float m_terrainMoveTime; // counter used when adjusting terrain heights
        protected bool m_isDirty = false;
        protected ObjectArray<Vector3> m_plainVertices;
        protected static ObjectArray<int> s_indices;



    }
}
