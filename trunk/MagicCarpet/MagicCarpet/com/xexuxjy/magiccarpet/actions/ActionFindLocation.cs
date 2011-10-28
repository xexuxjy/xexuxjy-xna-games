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
            float randomAngle = (float) Globals.random.NextDouble() * MathUtil.SIMD_2_PI;
            Vector3 newPosition = new Vector3((float)Math.Cos(randomAngle), 0, (float)Math.Sin(randomAngle));
            newPosition *= m_searchRadius;

            TargetLocation = Owner.Position += newPosition;
        
        }
        private float m_searchRadius;



    }
}
