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
            StickToGround = false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void SetInitialPositionAndDirection(Vector3 position, Vector3 direction)
        {
            Position = position;
            Heading = direction;
            TargetSpeed = Globals.s_defaultSpellSpeed;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected static CollisionShape s_collisionShape = new SphereShape(Spell.s_objectSize);


    }
}
