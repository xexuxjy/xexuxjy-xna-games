using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA.BulletCollision;

namespace com.xexuxjy.magiccarpet.spells
{
    public abstract class MovingSpell : Spell, ICollideable
    {
        public MovingSpell(GameObject owner)
            : base(owner)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize(SpellTemplate spellTemplate)
        {
            base.Initialize(spellTemplate);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            //m_collisionObject = new CollisionObject();
            //m_collisionObject.SetCollisionShape(s_collisionShape);
            //m_collisionObject.SetUserPointer(this);
            //m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() & (~CollisionFlags.CF_STATIC_OBJECT));

            //Globals.CollisionManager.AddToWorld(m_collisionObject);
            //m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(1f, Matrix.CreateTranslation(Position), s_collisionShape, m_motionState, true, this);
        }
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetInitialPositionAndDirection(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;

            //m_motionState.SetWorldTransform(Matrix.CreateTranslation(position));
            TargetSpeed = s_defaultSpellSpeed;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected static CollisionShape s_collisionShape = new SphereShape(0.2f);
        protected const float s_defaultSpellSpeed = 5f;


    }
}
