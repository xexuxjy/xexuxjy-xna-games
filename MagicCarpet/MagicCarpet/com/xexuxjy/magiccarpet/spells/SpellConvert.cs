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

        public override bool ProcessCollision(ICollideable partner,ManifoldPoint manifoldPoint)
        {
            bool didCollide = false;
            // Double check?
            if (ShouldCollideWith(partner))
            {
                // only 2 valid things should be manaballs and terrain.
                if (partner is ManaBall)
                {
                    GameObject target = partner.GetGameObject();
                    target.Owner = Owner;
                    didCollide = true;
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
            return (CollisionFilterGroups)(GameObjectType.manaball | GameObjectType.terrain);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////	

        public override float Mass
        {
            get
            {
                return 0.0001f; 
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}
