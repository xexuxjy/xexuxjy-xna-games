using System;
using System.Collections.Generic;
using System.Text;
using com.xexuxjy.magiccarpet.renderers;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.collision;
using System.Diagnostics;
using com.xexuxjy.magiccarpet.terrain;
using MagicCarpet.com.xexuxjy.magiccarpet;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TerrainSection : WorldObject
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public TerrainSection(Terrain terrain, int sectorX, int sectorZ, int worldSpanX, int worldSpanZ, Game game)
            : base(new Vector3(), game)
        {
            m_terrain = terrain;
            m_sectorX = sectorX;
            m_sectorZ = sectorZ;
            m_worldSpanX = worldSpanX;
            m_worldSpanZ = worldSpanZ;
            m_xVerts = m_worldSpanX;
            m_zVerts = m_worldSpanZ;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            // do this before base initialise otherwise we'll be added to the collision system.
            //m_collider = false;

            int stepSizeX = m_worldSpanX / m_xVerts;
            int stepSizeZ = m_worldSpanZ / m_zVerts;

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

            base.Initialize();

            Id = String.Format("TerrainSection([{0}],[{1}])", m_sectorX, m_sectorZ);
            SetDirty();
  

        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public void BuildRenderer()
        {
            m_terrainSectionRenderer = new TerrainSectionRenderer(this,m_terrain);
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
        public Vector3 GetNormalAtPoint(int x, int z)
        {
            if (null != m_terrainSectionRenderer)
            {
                return m_terrainSectionRenderer.getNormalAtPoint(x, z, m_worldSpanX, m_worldSpanZ);
            }
            return Vector3.Zero;
        }
        
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime updateTime)
        {
            float delta = (updateTime.ElapsedGameTime.Milliseconds / 1000.0f) * m_terrain.TerrainMoveTime;
            m_terrainMoveTime += delta;
            m_terrainMoveTime = Math.Min(m_terrainMoveTime, 1.0f);
            base.Update(updateTime);
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

        private void BuildCollisionObject()
        {
            TerrainSectionRenderer.MorphingTerrainVertexFormatStruct[] morphingVertices = m_terrainSectionRenderer.Vertices;
            Vector3[] plainVertices = new Vector3[morphingVertices.Length];
            for(int i=0;i<morphingVertices.Length;++i)
            {
                plainVertices[i] = morphingVertices[i].Position;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public Terrain m_terrain;
        TerrainSectionRenderer m_terrainSectionRenderer;
        private TerrainSquare[][] m_terrainSquares;
        public int m_sectorX;
        public int m_sectorZ;
        public int m_xVerts;
        public int m_zVerts;
        public int m_worldSpanX;
        public int m_worldSpanZ;
        private float m_terrainMoveTime; // counter used when adjusting terrain heights
        private bool m_isDirty = false;

    }
}
