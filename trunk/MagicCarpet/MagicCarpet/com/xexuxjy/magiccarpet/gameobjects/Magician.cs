using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.spells;
using Microsoft.Xna.Framework;
using com.xexuxjy.magiccarpet.util;
using com.xexuxjy.magiccarpet.actions;
using com.xexuxjy.magiccarpet.combat;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Magician : GameObject
    {
        public Magician(Vector3 startPosition)
            : base(startPosition, GameObjectType.magician)
        {
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public override float GetStartOffsetHeight()
        {
            return 0.5f;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////


        public SpellType SelectedSpell1
        {
            get { return m_selectedSpell1; }
            set { m_selectedSpell1 = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public SpellType SelectedSpell2
        {
            get { return m_selectedSpell2; }
            set { m_selectedSpell2 = value; }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell1(Vector3 start, Vector3 direction)
        {
            CastSpell(m_selectedSpell1, start, direction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

        public void CastSpell2(Vector3 start, Vector3 direction)
        {
            CastSpell(m_selectedSpell2, start, direction);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////

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
                        //QueueAction(Globals.ActionPool.GetActionTravel(this, null, action.TargetLocation, Globals.s_monsterTravelSpeed));
                        if(action.Target != null)
                        {
                            if(action.Target.Alive)
                            {
                                if (GameUtil.InRange(this, action.Target, Globals.s_monsterMeleeRange))
                                {
                                    QueueAction(Globals.ActionPool.GetActionAttackMelee(this, action.Target, Globals.s_monsterMeleeRange, Globals.s_monsterMeleeDamage));
                                }
                                else if (GameUtil.InRange(this, action.Target, Globals.s_monsterRangedDamage))
                                {
                                    QueueAction(Globals.ActionPool.GetActionAttackRange(this, action.Target, Globals.s_monsterRangedRange, Globals.s_monsterRangedDamage, SpellType.Fireball));
                                }
                                else
                                {
                                    //QueueAction(Globals.ActionPool.GetActionTravel());
                                }
                            }
                        }

                        break;
                    }
                case (ActionState.Idle):
                    {
                        // if we don't have a castle then travel to a location and create one.
                        // need to make sure we don't enqueue this multiple times.
                        if (m_castles.Count == 0)
                        {
                            Vector3 castlePosition = FindCastleLocation();
                            QueueAction(Globals.ActionPool.GetActionTravel(this, null, castlePosition, Globals.s_magicianTravelSpeed));
                            QueueAction(Globals.ActionPool.GetActionCastSpell(this, null, castlePosition, SpellType.Castle));
                        }
                        else
                        {
                            FindData findData = new FindData();
                            findData.m_owner = this;
                            findData.m_findMask = GameObjectType.magician | GameObjectType.castle | GameObjectType.balloon | GameObjectType.monster;
                            findData.m_magicianWeight = 1.0f;
                            findData.m_monsterWeight = 0.8f;
                            findData.m_balloonWeight = 0.6f;
                            findData.m_castleWeight = 0.4f;

                            QueueAction(Globals.ActionPool.GetActionFind(findData));

                        }
                        break;
                    }
                case (ActionState.AttackingMelee):
                    {
                        if (action.Target.Alive && GameUtil.InRange(this, action.Target, Globals.s_monsterMeleeRange))
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
                case (ActionState.Dieing):
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

        public Vector3 FindCastleLocation()
        {
            return Globals.Terrain.GetRandomWorldPositionXZ();
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

                    if (damageData.m_damager.Alive)
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
                // FIXME - shouldn't really clear action if we're dead / dieing?
                ClearAllActions();
                QueueAction(Globals.ActionPool.GetActionFlee(this, GetFleeDirection(), Globals.s_magicianFleeSpeed));
            }
        }



        public override String DebugText
        {
            get
            {
                return String.Format("Magician Id [{0}] Pos[{1}] Health[{2}] Mana[{3}] Spell1[{4}] Spell2[{5}.", Id, Position, GetAttribute(GameObjectAttributeType.Health).CurrentValue, GetAttribute(GameObjectAttributeType.Mana).CurrentValue, SelectedSpell1, SelectedSpell2);
            }
        }

        public override void NotifyOwnershipGained(GameObject gameObject)
        {
            base.NotifyOwnershipGained(gameObject);
            Castle tryCastle = gameObject as Castle;
            if (tryCastle != null)
            {
                m_castles.Add(tryCastle);
            }

            Balloon tryBalloon = gameObject as Balloon;
            if (tryBalloon != null)
            {
                m_balloons.Add(tryBalloon);
            }
        }

        public override void NotifyOwnershipLost(GameObject gameObject)
        {
            base.NotifyOwnershipLost(gameObject);
            Castle tryCastle = gameObject as Castle;
            if (tryCastle != null)
            {
                m_castles.Remove(tryCastle);
            }

            Balloon tryBalloon = gameObject as Balloon;
            if (tryBalloon != null)
            {
                m_balloons.Remove(tryBalloon);
            }

        }



        private List<Castle> m_castles = new List<Castle>();
        private List<Balloon> m_balloons = new List<Balloon>();


        private SpellType m_selectedSpell1 = SpellType.Convert;
        private SpellType m_selectedSpell2 = SpellType.Castle;


    }
}
