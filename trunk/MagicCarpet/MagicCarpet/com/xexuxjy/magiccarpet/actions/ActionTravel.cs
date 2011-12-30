using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using BulletXNA;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionTravel : BaseAction
    {
        public ActionTravel(GameObject owner, GameObject target, IndexedVector3? position, float speed) : 
            this(owner, target, position, speed, 0f, float.MaxValue)
        {
        }


        public ActionTravel(GameObject owner, GameObject target, IndexedVector3? position, float speed,float minDistance,float maxDistance)
            : base(owner, target, ActionState.Travelling)
        {
            m_position = position;
            m_speed = speed;m_minDistance2 = minDistance * minDistance;
            m_maxDistance2 = maxDistance * maxDistance;

            m_travelActionResult = TravelActionResult.none;
        }

        public override void Start()
        {
            base.Start();
            Owner.TargetSpeed = m_speed;
        }

        public override void Update(GameTime gameTime)
        {
            // whatever target we had is no longer valid.
            if (Target != null && !Globals.GameObjectManager.ObjectAvailable(Target))
            {
                m_travelActionResult = TravelActionResult.died;
                ActionComplete();
            }
            else
            {

                IndexedVector3 currentPosition = Owner.Position;
                IndexedVector3 targetPosition = (Target != null) ? Target.Position : m_position.Value;
                float dist2 = MathUtil.Vector3Distance2XZ(currentPosition, targetPosition);

                if (dist2 <= s_nearnessCheck + m_minDistance2)
                {
                    m_complete = true;
                    m_travelActionResult = TravelActionResult.found;
                    ActionComplete();
                }
                else if (dist2 > m_maxDistance2)
                {
                    m_complete = true;
                    m_travelActionResult = TravelActionResult.outOfRange;
                    ActionComplete();
                }
                else
                {
                    // 
                    IndexedVector3 direction = targetPosition - currentPosition;
                    direction.Y = 0;
                    direction.Normalize();
                    Owner.Direction = direction;
                }
            }
        }



        protected override void InternalComplete()
        {
            Owner.TargetSpeed = 0f;
        }

        public override bool Complete
        {
            get
            {
                return m_complete;
            }

        }


        private IndexedVector3? m_position;
        private float m_speed;
        private float m_minDistance2;
        private float m_maxDistance2;
        private const float s_nearnessCheck = 0.1f;

        private bool m_complete;

        public TravelActionResult m_travelActionResult;

    }


    public enum TravelActionResult
    {
        none,
        found,
        outOfRange,
        died
    }


}
