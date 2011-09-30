using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA.BulletCollision;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class ManaBall : GameObject
    {

        public ManaBall(Vector3 startPosition,Game game)
            : base( startPosition,game,GameObjectType.manaball)
        {
            ManaValue = 10;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void BuildCollisionObject()
        {
            if (s_collisionShape == null)
            {
                s_collisionShape = new SphereShape(1f);
            }

            //m_rigidBody = Globals.CollisionManager.LocalCreateRigidBody(1f, Matrix.CreateTranslation(Position), s_collisionShape,m_motionState,true);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float ManaValue
        {
            get { return m_manaValue; }
            set { m_manaValue = value; }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private float m_manaValue;

        protected static CollisionShape s_collisionShape;
    }
}