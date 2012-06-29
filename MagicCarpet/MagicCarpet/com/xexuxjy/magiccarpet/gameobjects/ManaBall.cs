using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA.BulletCollision;
using BulletXNA.BulletDynamics;
using BulletXNA.LinearMath;
using Microsoft.Xna.Framework.Graphics;

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

        public override CollisionShape BuildCollisionShape()
        {
            if (s_collisionShape == null)
            {
                s_collisionShape = new SphereShape(Globals.s_manaBallSize);
            }
            return s_collisionShape;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            base.BuildCollisionObject();
            BeingLoaded = false;
            // set a custom material here as we want a fairly damped response.
            //rb.SetCollisionFlags(rb.GetCollisionFlags() | CollisionFlags.CF_CUSTOM_MATERIAL_CALLBACK);
            // manaballs should be allowed to move and go into sleep tates.
            m_collisionObject.SetActivationState(ActivationState.ACTIVE_TAG);
            RigidBody rb = m_collisionObject as RigidBody;
            rb.SetFlags(rb.GetFlags() &~ RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY);
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
                    StickToGround = false;
                }
                else
                {
                    RigidBody rb = (RigidBody)m_collisionObject;
                    rb.SetFlags(rb.GetFlags() & RigidBodyFlags.BT_DISABLE_WORLD_GRAVITY);
                    rb.SetActivationState(ActivationState.ACTIVE_TAG);
                    rb.SetDamping(0.5f, 0.1f);
                    StickToGround = true;

                }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        protected override void DrawEffect(GraphicsDevice graphicsDevice, Matrix view, Matrix world, Matrix projection)
        {
            if (BeingLoaded)
            {
                Vector3 pos = Position;
                int ibreak = 0;
            }

            base.DrawEffect(graphicsDevice, view, world, projection);
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