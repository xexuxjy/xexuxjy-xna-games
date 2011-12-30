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
                QueueAction(Globals.ActionPool.GetActionIdle(this));
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
                        QueueAction(Globals.ActionPool.GetActionTravel(this, null, action.TargetLocation, Globals.s_monsterTravelSpeed));                           
                        break;
                    }
                case (ActionState.Idle):
                    {
                        // if we've finished an idle then look around for something
                        // to do.

                        BaseAction newAction;
                        // need to have a better way of sorting these weights but..
                        double choice = Globals.s_random.NextDouble();
                        if (choice < 0.2)
                        {
                            newAction = Globals.ActionPool.GetActionIdle(this);
                        }
                        else if (choice >= 0.2 && choice < 0.5)
                        {
                            newAction = Globals.ActionPool.GetActionFind(FindData.GetActionFindEnemy(this, Globals.s_monsterSearchRadius));
                        }
                        else
                        {
                            newAction = Globals.ActionPool.GetActionFind(FindData.GetActionFindLocation(this, Globals.s_monsterSearchRadius));
                        }
                        QueueAction(newAction);
                        break;
                    }
                case(ActionState.AttackingMelee):
                    {
                        if (action.Target.Alive && GameUtil.InRange(this,action.Target,Globals.s_monsterMeleeRange))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackMelee(this, action.Target, Globals.s_monsterMeleeRange, Globals.s_monsterMeleeDamage));
                        }
                        break;
                    }
                case (ActionState.AttackingRange):
                    {
                        // if we've finished attacking at range. then we need to see if 
                        // our target is dead or if we should do something else.

                        if (action.Target.Alive && GameUtil.InRange(this, action.Target, Globals.s_monsterRangedDamage))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackRange(this, action.Target, Globals.s_monsterRangedRange, Globals.s_monsterRangedDamage, SpellType.Fireball));
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
                        QueueAction(Globals.ActionPool.GetActionIdle(this));
                        break;
                    }
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override void Damaged(DamageData damageData)
        {
            base.Damaged(damageData);

            // if something attacks us then we need to hit back?

            float currentHealthPercentage = GetAttributePercentage(GameObjectAttributeType.Health);

            if (currentHealthPercentage > Globals.s_monsterFleeHealthPercentage)
            {
                ActionState currentActionState = CurrentActionState;
                // if we're in a passive state then maybe attack back?
                if (BaseAction.IsPassive(currentActionState))
                {
                    m_actionComponent.ClearAllActions(); 

                    if(damageData.m_damager.Alive)
                    {
                        if (GameUtil.InRange(this, damageData.m_damager, Globals.s_monsterMeleeRange))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackMelee(this, damageData.m_damager, Globals.s_monsterMeleeRange, Globals.s_monsterMeleeDamage));
                        }
                        else if (GameUtil.InRange(this, damageData.m_damager, Globals.s_monsterRangedDamage))
                        {
                            QueueAction(Globals.ActionPool.GetActionAttackRange(this, damageData.m_damager, Globals.s_monsterRangedRange, Globals.s_monsterRangedDamage, SpellType.Fireball));
                        }
                    }
                }
            }
            else
            {
                // if we're being attacked and damaged then run away if we're below 1/4 health.
                ClearAllActions();
                QueueAction(Globals.ActionPool.GetActionFlee(this, GetFleeDirection(), Globals.s_monsterFleeSpeed));
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        const float s_radius = 0.5f;
    }
}
