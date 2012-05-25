using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BulletXNA;
using BulletXNA.LinearMath;
using BulletXNA.BulletDynamics;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.collision
{
    public class SimpleMotionState : DefaultMotionState
    {
        public SimpleMotionState() : this(IndexedMatrix.Identity, IndexedMatrix.Identity)
        {
        }

        public SimpleMotionState(IndexedMatrix startTrans, IndexedMatrix centerOfMassOffset) : base(startTrans,centerOfMassOffset)
        {
        }

        public override void SetWorldTransform(IndexedMatrix centerOfMassWorldTrans)
        {
            SetWorldTransform(ref centerOfMassWorldTrans);
        }

        public override void SetWorldTransform(ref IndexedMatrix centerOfMassWorldTrans)
        {
            // these don't see to go through motion states so set it directly.
            if (RigidBody != null && RigidBody.IsStaticObject())
            {
                RigidBody.SetWorldTransform(ref centerOfMassWorldTrans);
            }
            else
            {
                base.SetWorldTransform(ref centerOfMassWorldTrans);
            }
        }

        public RigidBody RigidBody
        {
            get{return m_rigidBody;}
            set { m_rigidBody = value; }
        }

        // use this to help provide a consistent way of setting stuff up in physics.. as bullet will handle
        // differently for static,kinematic,dynamic etc.
        private RigidBody m_rigidBody;

    }
}
