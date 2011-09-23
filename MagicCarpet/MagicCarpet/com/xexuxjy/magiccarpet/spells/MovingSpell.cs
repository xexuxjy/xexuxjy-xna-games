using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BulletXNA;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.spells
{
    public abstract class MovingSpell : Spell, ICollideable
    {
        public MovingSpell(Game game)
            : base(game)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Initialize(SpellTemplate spellTemplate, GameObject owner)
        {
            base.Initialize(spellTemplate,owner);
            m_motionState = new DefaultMotionState();
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetInitialPositionAndDirection(Vector3 position, Vector3 direction)
        {
            StartPosition = position;
            Direction = direction;

            m_motionState.SetWorldTransform(Matrix.CreateTranslation(position));

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Update(GameTime gameTime)
        {
            Matrix outMatrix;
            m_motionState.GetWorldTransform(out outMatrix);
            Vector3 currentPosition = outMatrix.Translation;

            currentPosition += (m_direction * (float)gameTime.ElapsedGameTime.TotalSeconds);

            m_motionState.SetWorldTransform(Matrix.CreateTranslation(currentPosition));

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual GameObject GetGameObject()
        {
            return this;
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

        public Vector3 StartPosition
        {
            get { return m_startPosition; }
            set { m_startPosition = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Vector3 Direction
        {
            get { return m_direction; }
            set { m_direction = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public float Speed
        {
            get { return m_speed; }
            set { m_speed = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private Vector3 m_startPosition;
        private Vector3 m_direction;
        private float m_speed;
    }
}
