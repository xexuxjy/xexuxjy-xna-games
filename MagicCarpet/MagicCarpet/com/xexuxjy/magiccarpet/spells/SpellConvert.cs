using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.interfaces;
using com.xexuxjy.magiccarpet.gameobjects;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.terrain;

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

        public override void ProcessCollision(ICollideable partner)
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

    }
}
