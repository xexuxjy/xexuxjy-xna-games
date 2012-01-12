using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA.LinearMath;
using Microsoft.Xna.Framework;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionFind : BaseAction
    {
        public ActionFind(FindData findData)
            : base(findData.m_owner, findData.m_target, ActionState.Searching)
        {
            m_findData = findData;
            Duration = 1.0f;
        }

        // new clever version that lets us find a number of objects and give weights to the search?
        protected override void InternalComplete()
        {
            if (m_findData.m_position.HasValue)
            {
                ChosePosition();
            }
            else
            {
                List<GameObject> searchResults = new List<GameObject>();
                Globals.GameObjectManager.FindObjects(m_findData.m_findMask, Owner.Position, m_findData.m_findRadius, m_findData.m_owner, searchResults,m_findData.m_includeOwner);
                if (searchResults.Count > 0)
                {
                    int index = 0;
                    float heighestWeight = 0;
                    for (int i = 0; i < searchResults.Count; ++i)
                    {
                        float currentWeight = m_findData.GetWeightForType(searchResults[i].GameObjectType);
                        if (currentWeight > heighestWeight)
                        {
                            heighestWeight = currentWeight;
                            index = i;
                        }
                    }
                    Target = searchResults[index];
#if LOG_EVENT
                    Globals.EventLogger.LogEvent(String.Format("ActionFind : Object [{0}][{1}] TargetObject [{2}][{3}].", Owner.Id, Owner.GameObjectType, Target.Id, Target.GameObjectType));
#endif
                }
                else
                {
                    // couldn't find a target, chose a position instead?
                    ChosePosition();
                }
            }
        }

        private void ChosePosition()
        {
            // pick a random point within the search radius?
            float randomAngle = (float)Globals.s_random.NextDouble() * MathUtil.SIMD_2_PI;
            IndexedVector3 newPosition = new Vector3((float)Math.Cos(randomAngle), 0, (float)Math.Sin(randomAngle));
            newPosition *= m_findData.m_findRadius;

            TargetLocation = (m_findData.m_position.HasValue ? m_findData.m_position.Value : m_findData.m_owner.Position) + newPosition;
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("ActionFind : Location [{0}][{1}] TargetLocation [{2}].", Owner.Id, Owner.GameObjectType, TargetLocation));
#endif


        }

        public FindData m_findData;
    }
}
