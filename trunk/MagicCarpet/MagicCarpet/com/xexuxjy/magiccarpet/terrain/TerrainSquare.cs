using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TerrainSquare
    {
        /////////////////////////////////////////////////////////////////////////////////////

        public Vector3 Position
        {
            get{return m_position;}
            set{m_position = value;}
        }

        /////////////////////////////////////////////////////////////////////////////////////

        public TerrainType Type
        {
            get{return m_terrainType;}
            set{m_terrainType = value;}
        }

        /////////////////////////////////////////////////////////////////////////////////////

        public float GetHeight()
        {
            return m_position.Y;
        }

        /////////////////////////////////////////////////////////////////////////////////////

        public void SetHeight(float height)
        {
            m_position.Y = height;
        }

        /////////////////////////////////////////////////////////////////////////////////////

        public void SetTargetHeight(float height)
        {
            m_targetHeight = height;
        }

        /////////////////////////////////////////////////////////////////////////////////////

        public float GetTargetHeight()
        {
            return m_targetHeight;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        // no-args ctor for serialize
        public TerrainSquare() { }
        public TerrainSquare(float xpos, float ypos, float zpos)
        {
            m_position.X = xpos;
            m_position.Y = ypos;
            m_position.Z = zpos;

            m_terrainType = TerrainType.water;
        }

        /////////////////////////////////////////////////////////////////////////////////////

        Vector3 m_position;
        float m_targetHeight;
        TerrainType m_terrainType;
    }
}
