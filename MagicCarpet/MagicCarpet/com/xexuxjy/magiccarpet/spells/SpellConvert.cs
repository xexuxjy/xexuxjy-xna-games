using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.terrain;
using BulletXNA.BulletCollision;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellConvert : MovingSpell 
    {

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpellConvert(GameObject owner)
            : base(owner)
        {
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ShouldCollideWith(ICollideable partner)
        {
            bool shouldCollide = false;
            GameObject target = partner.GetGameObject();
            if (target is ManaBall)
            {
                // if it's owned by someone else or is non-aligned
                if (target.Owner != Owner)
                {
                    shouldCollide = true;
                }
            }
            else if(target is Terrain)
            {
                shouldCollide = true;
            }
            return shouldCollide;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override int GetCollisionMask()
        {
            return 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void ProcessCollision(ICollideable partner,ManifoldPoint manifoldPoint)
        {
            // Double check?
            if (ShouldCollideWith(partner))
            {
                // only 2 valid things should be manaballs and terrain.
                if (partner is ManaBall)
                {
                    GameObject target = partner.GetGameObject();
                    target.Owner = Owner;
                }
                Cleanup();
            }
        
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void BuildCollisionObject()
        {
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.spell;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)(GameObjectType.manaball | GameObjectType.terrain);

            //m_collisionObject = new CollisionObject();
            //m_collisionObject.SetCollisionShape(s_collisionShape);

            //Globals.CollisionManager.AddToWorld(m_collisionObject,collisionFlags,collisionMask);
            // needs to be kinematic to preserve and use motion states?
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(1f, Matrix.CreateTranslation(Position), s_collisionShape, m_motionState, true, this, collisionFlags, collisionMask);
            m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() | CollisionFlags.CF_KINEMATIC_OBJECT);

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
