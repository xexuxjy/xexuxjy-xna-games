using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using BulletXNA.BulletCollision;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.terrain;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellFireball : MovingSpell
    {
        public SpellFireball(GameObject owner)
            : base(owner)
        {

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.spell;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)(GameObjectType.terrain | GameObjectType.magician | GameObjectType.balloon | GameObjectType.monster | GameObjectType.castle);

            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(1f, Matrix.CreateTranslation(Position), s_collisionShape, m_motionState, true, this, collisionFlags, collisionMask);
            m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() | CollisionFlags.CF_KINEMATIC_OBJECT);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ShouldCollideWith(ICollideable partner)
        {
            // stop on terrain
            if (partner is Terrain)
            {
                return true;
            }
            // of if we hit something that doesn't belong to us.
            if (partner.GetGameObject().Owner != Owner)
            {
                return true;
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void ProcessCollision(ICollideable partner, ManifoldPoint manifoldPoint)
        {
            // Double check?
            if (ShouldCollideWith(partner))
            {
                if (!(partner is Terrain))
                {
                    // look at doing this through attributes rather then specific call.
                    partner.GetGameObject().DoDamage(m_damage);
                }
                Cleanup();
            }

        }


        private float m_damage = 20;
    }
}
