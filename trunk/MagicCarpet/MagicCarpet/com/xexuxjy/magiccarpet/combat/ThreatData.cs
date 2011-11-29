using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.combat
{

    public struct ThreatData
    {
        public ThreatData(GameObject gameObject, int threatLevel, float lastTime)
        {
            m_gameObject = gameObject;
            m_threatLevel = threatLevel;
            m_lastTime = lastTime;
        }

        public static void UpdateThreatLevel(ref ThreatData threatData, int level)
        {
            threatData.m_threatLevel -= level;
        }


        public GameObject m_gameObject;
        public int m_threatLevel;
        public float m_lastTime;

        public const int s_initialThreatLevel = 50; // first time attacked.
        public const int s_threatIncrement = 20; // next time attacked.
        public const int s_threatDecay = 5; // per update

    }

}
