using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionFindLocation : BaseAction
    {
        public ActionFindLocation(GameObject owner, float searchRadius)
            : base(owner, null, ActionState.Searching)
        {
            Duration = 1.0f;
            m_searchRadius = searchRadius;
        }

        protected override void InternalComplete()
        {
            // pick a random point within the search radius?
            float randomAngle = (float) Globals.s_random.NextDouble() * MathUtil.SIMD_2_PI;
            Vector3 newPosition = new Vector3((float)Math.Cos(randomAngle), 0, (float)Math.Sin(randomAngle));
            newPosition *= m_searchRadius;

            TargetLocation = Owner.Position + newPosition;
#if LOG_EVENT
            Globals.EventLogger.LogEvent(String.Format("ActionFindLocation[{0}][{1}] Target [{2}].", Owner.Id, Owner.GameObjectType, TargetLocation));
#endif

        }
        private float m_searchRadius;
    }
}
