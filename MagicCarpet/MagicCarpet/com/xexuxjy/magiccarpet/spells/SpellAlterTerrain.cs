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
    public class SpellAlterTerrain : MovingSpell
    {
        public SpellAlterTerrain(GameObject owner,bool up)
            : base(owner)
        {
            m_up = up;
            m_heightDelta = 10.0f;
            if (!m_up)
            {
                m_heightDelta *= -1f;
            }
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
                    ((Terrain)partner).AddPeak(collisionPointWorld,m_heightDelta);
                }
                Cleanup();
            }
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool m_up;
        private float m_heightDelta;
    }
}
