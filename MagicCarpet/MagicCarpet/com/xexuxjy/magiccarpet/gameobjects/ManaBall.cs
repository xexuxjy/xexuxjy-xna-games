using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA.BulletCollision;
using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class ManaBall : GameObject
    {

        public ManaBall(Vector3 startPosition)
            : base( startPosition,GameObjectType.manaball)
        {
            ManaValue = 10;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize()
        {
            base.Initialize();
            m_scaleTransform = Matrix.CreateScale(0.5f);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override float GetStartOffsetHeight()
        {
            return 0.5f;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            if (s_collisionShape == null)
            {
                s_collisionShape = new SphereShape(GetStartOffsetHeight());
            }
            float mass = 2f;
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(mass, IndexedMatrix.CreateTranslation(Position), s_collisionShape,m_motionState,true,this);
            BeingLoaded = false;
            // set a custom material here as we want a fairly damped response.
            //rb.SetCollisionFlags(rb.GetCollisionFlags() | CollisionFlags.CF_CUSTOM_MATERIAL_CALLBACK);

            //m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() | CollisionFlags.CF_KINEMATIC_OBJECT);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool BeingLoaded
        {
            get { return m_beingLoaded; }
            set
            {
                m_beingLoaded = value;
                if (m_beingLoaded)
                {
                    RigidBody rb = (RigidBody)m_collisionObject;
                    rb.SetFlags(rb.GetFlags() & ~RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY);
                    rb.SetActivationState(ActivationState.ACTIVE_TAG);
                    rb.SetDamping(0.5f, 0.1f);
                }
                else
                {
                    RigidBody rb = (RigidBody)m_collisionObject;
                    rb.SetFlags(rb.GetFlags() & ~RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY);
                    rb.SetActivationState(ActivationState.ACTIVE_TAG);
                    rb.SetDamping(0.5f, 0.1f);

                }
            }
        }



        //////////////////////////////////////////////////////////////////////////////////////////////////////

        
        public float ManaValue
        {
            get { return m_manaValue; }
            set { m_manaValue = value; }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private float m_manaValue;
        private bool m_beingLoaded;

        protected static CollisionShape s_collisionShape;
    }
}