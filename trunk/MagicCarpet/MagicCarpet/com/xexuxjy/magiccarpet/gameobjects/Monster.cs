using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.actions;
using GameStateManagement;
using com.xexuxjy.magiccarpet.spells;
using com.xexuxjy.magiccarpet.util;
using BulletXNA.BulletCollision;
using com.xexuxjy.magiccarpet.combat;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Monster : GameObject
    {
        public Monster()
            : base(GameObjectType.monster)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public Monster(Vector3 startPosition)
            : base(startPosition,GameObjectType.monster)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void BuildCollisionObject()
        {
            CollisionFilterGroups collisionFlags = (CollisionFilterGroups)GameObjectType.monster;
            CollisionFilterGroups collisionMask = (CollisionFilterGroups)(GameObjectType.terrain | GameObjectType.magician | GameObjectType.balloon | GameObjectType.monster | GameObjectType.castle | GameObjectType.spell);

            m_collisionObject = Globals.CollisionManager.LocalCreateRigidBody(1f, Matrix.CreateTranslation(Position), new SphereShape(s_radius), m_motionState, true, this, collisionFlags, collisionMask);
            m_collisionObject.SetCollisionFlags(m_collisionObject.GetCollisionFlags() | CollisionFlags.CF_KINEMATIC_OBJECT);
            m_scaleTransform = Matrix.CreateScale(s_radius);

        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////

        /************************************************************************/
        /* 
         * if we're not doing anything, then 
         *  look to see if there is anything nearby that poses a threat
         *  
         * if there is then start attacking it
         * 
         * if there isn't then find somewhere else to go
         
         */
        /************************************************************************/

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ActionState == ActionState.None)
            {
                QueueAction(new ActionIdle(this));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public override void ActionComplete(BaseAction action)
        {
            switch (action.ActionState)
            {
                case (ActionState.Searching):
                    {
                        // Decide what to do



                        QueueAction(new ActionTravel(Owner, null, action.TargetLocation, s_monsterSpeed));                           
                        break;
                    }
                case (ActionState.Idle):
                    {



                        break;
                    }
                case(ActionState.AttackingMelee):
                    {
                        if (action.Target.Alive && GameUtil.InRange(Owner,action.Target,s_monsterMeleeRange))
                        {
                            QueueAction(new ActionAttackRange(Owner, action.Target, s_monsterMeleeRange, s_monsterRangeDamage, SpellType.Fireball));
                        }
                        break;
                    }
                case (ActionState.AttackingRange):
                    {
                        // if we've finished attacking at range. then we need to see if 
                        // our target is dead or if we should do something else.

                        if (action.Target.Alive && GameUtil.InRange(Owner,action.Target,s_monsterRangeDamage))
                        {
                            QueueAction(new ActionAttackRange(Owner, action.Target, s_monsterRangeRange,s_monsterRangeDamage, SpellType.Fireball));
                        }
                        break;
                    }

                case (ActionState.Travelling):
                    {
                        TargetSpeed = 0f;
                        break;
                    }
                case(ActionState.Dieing):
                    {
                        // when we've finished dieing then we want to spawn a manaball here.
                        Globals.GameObjectManager.CreateAndInitialiseGameObject(GameObjectType.manaball, Position);
                        Cleanup();
                        break;
                    }

                default:
                    {
                        QueueAction(new ActionIdle(this));
                        break;
                    }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Damaged(DamageData damageData)
        {
            base.Damaged(damageData);

            if (GetAttributePercentage(GameObjectAttributeType.Health) < 0.25f)
            {
                // if we're being attacked and damaged then run away if we're below 1/4 health.
                // FIXME - shouldn't really clear action if we're dead / dieing?
                m_actionPool.ClearAllActions(); 
                QueueAction(new ActionFlee(this, GetFleeDirection()));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        private GameObject m_currentTarget;
        private const float s_searchRadius = 50f;
        private const float s_monsterSpeed = 5f;
        private const float s_monsterMeleeRange = 0.5f;
        private const float s_monsterMeleeDamage = 5f;

        private const float s_monsterRangeRange = 5f;
        private const float s_monsterRangeDamage = 5f;

        private const float s_radius = 0.5f;
    }
}
