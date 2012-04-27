using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletXNA.LinearMath;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.util;
using BulletXNA;


namespace com.xexuxjy.magiccarpet.combat
{
    public class ThreatComponent
    {
        public ThreatComponent(GameObject owner)
        {
            m_gameObject = owner;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void UpdateThreats()
        {
            for (int i = 0; i < m_recentThreats.Count; ++i)
            {
                if (m_recentThreats[i].m_threatLevel > 0)
                {
                    ThreatData.UpdateThreatLevel(ref m_recentThreats.GetRawArray()[i], -ThreatData.s_threatDecay);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void AddOrUpdateThreat(GameObject gameObject)
        {

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] AddThreat [{2}][{3}].", m_gameObject.Id, m_gameObject.GameObjectType, gameObject.Id, gameObject.GameObjectType));
#endif

            int index = FindThreatIndex(gameObject);
            if (index >= 0)
            {
                ThreatData.UpdateThreatLevel(ref m_recentThreats.GetRawArray()[index], ThreatData.s_threatIncrement);
            }
            else
            {
                m_recentThreats.Add(new ThreatData(gameObject, ThreatData.s_initialThreatLevel, 0));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected int FindThreatIndex(GameObject gameObject)
        {
            int index = -1;
            for (int i = 0; i < m_recentThreats.Count; ++i)
            {
                if (m_recentThreats[i].m_gameObject == gameObject)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void RemoveThreat(GameObject gameObject)
        {

            int index = FindThreatIndex(gameObject);
            if (index >= 0)
            {
#if LOG_EVENT
                Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] RemoveThreat [{2}][{3}].", m_gameObject.Id, m_gameObject.GameObjectType, gameObject.Id, gameObject.GameObjectType));
#endif
                m_recentThreats.RemoveAt(index);
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 GetFleeDirection()
        {
            // find out which is the 
            float totalThreatLevel = 0f;
            Vector3 threatDirection = Vector3.Zero;

            foreach (ThreatData threatData in m_recentThreats)
            {
                totalThreatLevel += threatData.m_threatLevel;
            }

            // if we have a threat level.
            if (totalThreatLevel > 0)
            {

                // now go through and find the direction of greatest threat?
                foreach (ThreatData threatData in m_recentThreats)
                {
                    float contribution = (float)threatData.m_threatLevel / totalThreatLevel;
                    threatDirection += (GameUtil.DirectionToTarget(m_gameObject.Position, threatData.m_gameObject) * contribution);
                }
                // flee away from the area of greatest threats...
                threatDirection.Y = 0f;
                threatDirection = -threatDirection;
            }
            if (MathUtil.FuzzyZero(threatDirection.LengthSquared()))
            {
                // either equal threats around us or no threats, in which case chose random direction
                Vector3 randomPosition = Globals.Terrain.GetRandomWorldPositionXZ();
                threatDirection = GameUtil.DirectionToTarget(m_gameObject, randomPosition);
            }
            threatDirection.Normalize();

#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("GameObject[{0}][{1}] FleeDirection [{2}].", m_gameObject.Id, m_gameObject.GameObjectType, threatDirection));
#endif
            return threatDirection;



        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private ObjectArray<ThreatData> m_recentThreats = new ObjectArray<ThreatData>();
        private GameObject m_gameObject;
    }
}
