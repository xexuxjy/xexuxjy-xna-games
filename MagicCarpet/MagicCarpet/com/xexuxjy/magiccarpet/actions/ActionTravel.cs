using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using BulletXNA;

namespace com.xexuxjy.magiccarpet.actions
{
    public class ActionTravel : BaseAction
    {
        public ActionTravel(GameObject owner, GameObject target,Vector3 position,float speed)
            : base(owner, target, ActionState.Travelling)
        {
            m_position = position;
            m_speed = speed;
            owner.TargetSpeed = m_speed;
        }

        public override void Update(GameTime gameTime)
        {
            // whatever target we had is no longer valid.
            if (Target != null && !Globals.GameObjectManager.ObjectAvailable(Target))
            {
                ActionComplete();
            }
            else
            {
                Vector3 currentPosition = Owner.Position;
                Vector3 targetPosition = (Target != null) ? Target.Position : m_position;

                float dist2 = MathUtil.Vector3Distance2XZ(currentPosition, targetPosition);
                if (dist2 <= s_nearnessCheck)
                {
                    ActionComplete();
                }
                else
                {
                    // 
                    Vector3 direction = targetPosition - currentPosition;
                    direction.Y = 0;
                    Owner.Direction = direction;
                }
            }
        }

        protected override void InternalComplete()
        {
            Owner.TargetSpeed = 0f;
        }


        private Vector3 m_position;
        private float m_speed;
        private const float s_nearnessCheck = 0.1f;

    }
}
