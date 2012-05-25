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

        public override bool ProcessCollision(ICollideable partner, ManifoldPoint manifoldPoint)
        {
            // Double check?
            if (ShouldCollideWith(partner))
            {
                if (partner is Terrain)
                {
                    Vector3 collisionPointWorld = manifoldPoint.GetPositionWorldOnA();

                    // adjust to nearest integer.
                    Vector3 roundedPoint = new Vector3((float)Math.Round(collisionPointWorld.X), collisionPointWorld.Y, (float)Math.Round(collisionPointWorld.Z));

                    if (Castle.CanPlaceLevel(collisionPointWorld, 0))
                    {
                        Castle castle = (Castle)Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.castle,collisionPointWorld);
                        if(castle != null)
                        {
                            castle.Owner = Owner;
                            castle.GrowToLevel(0);
                        }
                    }
                }
                Cleanup();
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override CollisionFilterGroups GetCollisionFlags()
        {
            return (CollisionFilterGroups)GameObjectType.spell;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override CollisionFilterGroups GetCollisionMask()
        {
            return (CollisionFilterGroups)(GameObjectType.terrain);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override CollisionShape BuildCollisionShape()
        {
            return s_collisionShape;
        }

   }
}
