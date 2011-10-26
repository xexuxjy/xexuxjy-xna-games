using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;
using com.xexuxjy.magiccarpet.terrain;
using com.xexuxjy.magiccarpet.interfaces;
using BulletXNA.BulletCollision;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.spells
{
    public class SpellCastle : MovingSpell
    {
        public SpellCastle(GameObject owner)
            : base(owner)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override bool ShouldCollideWith(ICollideable partner)
        {
            return partner is Terrain;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override int GetCollisionMask()
        {
            return 0;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void ProcessCollision(ICollideable partner, ManifoldPoint manifoldPoint)
        {
            // Double check?
            if (ShouldCollideWith(partner))
            {
                if (partner is Terrain)
                {
                    Vector3 collisionPointWorld = manifoldPoint.GetPositionWorldOnA();
                    if (Castle.CanPlaceSize(collisionPointWorld, 0))
                    {
                        Castle castle = (Castle)Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.castle,collisionPointWorld);
                        if(castle != null)
                        {
                            castle.Owner = Owner;
                            castle.GrowToSize(0);
                        }
                    }
                }
                Cleanup();
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        protected override void BuildCollisionObject()
        {
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.spell;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)(GameObjectType.terrain);
            // needs to be kinematic to preserve and use motion states?
            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(1f, Matrix.CreateTranslation(Position), s_collisionShape, m_motionState, true, this, collisionFlags, collisionMask);
            m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() | CollisionFlags.CF_KINEMATIC_OBJECT);
        }
    }
}
