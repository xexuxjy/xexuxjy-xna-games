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

        protected override void BuildCollisionObject()
        {
            m_collisionObject = new CollisionObject();
            m_collisionObject.SetCollisionShape(s_collisionShape);
            m_collisionObject.SetUserPointer(this);
            Globals.CollisionManager.AddToWorld(m_collisionObject);
        }
        
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetInitialPositionAndDirection(Vector3 position, Vector3 direction)
        {
            Position = position;
            Direction = direction;

            m_motionState.SetWorldTransform(Matrix.CreateTranslation(position));
            TargetSpeed = s_defaultSpellSpeed;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual int GetCollisionMask()
        {
            return 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public virtual bool ShouldCollideWith(ICollideable partner)
        {
            return false;
        }
        
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public virtual void ProcessCollision(ICollideable partner)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public GameObject GetGameObject()
        {
            return this;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected static CollisionShape s_collisionShape = new SphereShape(0.2f);
        protected const float s_defaultSpellSpeed = 5f;


    }
}
