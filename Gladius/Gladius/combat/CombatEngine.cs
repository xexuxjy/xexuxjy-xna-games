using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using xexuxjy.Gladius.util;
using Microsoft.Xna.Framework;
using StringLeakTest;

namespace Gladius.combat
{
    public class CombatEngine
    {
        public CombatEngine()
        {
            m_combatRandom = new Random();
        }


        public void ResolveAttack(BaseActor attacker, BaseActor defender,AttackSkill attackData)
        {
            AttackResult attackResult = new AttackResult();

            float totalDamage = attackData.BaseDamage;

            float strengthBonus = attacker.GetAttributeValue(GameObjectAttributeType.Accuracy);

            float accuracyBonus = GetCategoryAccuracyBonus(attacker.ActorClass, defender.ActorClass);
            float categoryMultuplier = GetCategoryDamageMultiplier(attacker.ActorClass,defender.ActorClass);

        http://www.gamefaqs.com/gamecube/561233-gladius/faqs/64758
            //DIFFERENCE = [ (ACC * 0.97) - DEF ]
            float totalAccuracy = attacker.GetAttributeValue(GameObjectAttributeType.Accuracy) + accuracyBonus;
            float diff1 = ( totalAccuracy * 0.97f) - defender.GetAttributeValue(GameObjectAttributeType.Defense);

            //MISS CHANCE = 10 * 1.5 ^ [ DIFFERENCE * (-16 / 100) ]
            float missChance = 10f * ((float)Math.Pow(1.5f, (diff1 * (-16/100) ) ));
            missChance *= 0.01f; // (0->1)
            float hitChance = 1f - missChance;


            if (m_combatRandom.NextDouble() > hitChance)
            {
                attackResult.resultType = defender.HasShield?AttackResultType.Blocked:AttackResultType.Miss;
            }
            else
            {
                attackResult.resultType = AttackResultType.Hit;
                attackResult.damageDone = (int)totalDamage;
            }



            //DAMAGE  =  BASE POWER  x  ( ATTACK MULTIPLIER / 100 )  x  SITUATIONAL FACTORS
            float baseDamage = attacker.GetAttributeValue(GameObjectAttributeType.Power) * attackData.DamageMultiplier;


            //attackResult.resultType = AttackResultType.Blocked;

            if (attackResult.resultType == AttackResultType.Blocked)
            {
                defender.StartBlock(attacker);
            }
            else
            {
                defender.TakeDamage(attackResult);
            }

            m_lastCombatResult.Clear();
            m_lastCombatResult.ConcatFormat("Attacker [{0}]\nDefender[{1}]\nSkill [{2}]\n", attacker.DebugName, defender.DebugName, attackData.Name);
            m_lastCombatResult.ConcatFormat("Result [{0}]\nDamage [{1}]", ""+attackResult.resultType, attackResult.damageDone);
        }

        public StringBuilder LastCombatResult
        {
            get { return m_lastCombatResult; }
        }


        private float GetCategoryDamageMultiplier(ActorCategory attacker, ActorCategory defender)
        {
            float categoryMultiplier = 1f;
            if (attacker != defender)
            {
                if (attacker == ActorCategory.Heavy && defender == ActorCategory.Medium)
                {
                    categoryMultiplier = 1.5f;
                }
                else if (attacker == ActorCategory.Heavy && defender == ActorCategory.Light)
                {
                    categoryMultiplier = 0.5f;
                }
                else if (attacker == ActorCategory.Medium && defender == ActorCategory.Light)
                {
                    categoryMultiplier = 1.5f;
                }
                else if (attacker == ActorCategory.Medium && defender == ActorCategory.Heavy)
                {
                    categoryMultiplier = 0.5f;
                }
                else if (attacker == ActorCategory.Light && defender == ActorCategory.Heavy)
                {
                    categoryMultiplier = 1.5f;
                }
                else if (attacker == ActorCategory.Light && defender == ActorCategory.Medium)
                {
                    categoryMultiplier = 0.5f;
                }
            }
            return categoryMultiplier;
        }


        private float GetCategoryAccuracyBonus(ActorCategory attacker, ActorCategory defender)
        {
            float bonus = 0f;
            if (attacker == ActorCategory.Heavy && defender == ActorCategory.Light)
            {
                bonus = -20f;
            }
            else if (attacker == ActorCategory.Light && defender == ActorCategory.Heavy)
            {
                bonus = 38f;
            }
            return bonus;
        }

        public bool IsNearDeath(BaseActor actor)
        {
            return actor.Health / actor.MaxHealth < 0.2f;
        }



        public bool IsValidTarget(BaseActor attacker, BaseActor defender,AttackSkill skill)
        {
            if (attacker != null && defender != null)
            {
                if (defender.Dead)
                {
                    return false;
                }

                if (attacker == defender && skill != null && skill.DamageAffects == DamageAffects.Self)
                {
                    return true;
                }
                if (attacker.Team == defender.Team && (skill == null || skill.HasMovementPath()))
                {
                    return false;
                }
                if (attacker.Team == defender.Team && skill.DamageAffects == DamageAffects.Team)
                {
                    return true;
                }
                if (attacker != defender)
                {
                    return true;
                }

            }
            return false;
        }

        public bool IsAttackNextTo(BaseActor attacker, BaseActor defender)
        {
            if (defender != null && attacker != null)
            {
                Point fp = attacker.CurrentPosition;
                Point tp = defender.CurrentPosition;
                Vector3 diff = new Vector3(tp.X, 0, tp.Y) - new Vector3(fp.X, 0, fp.Y);

                // we're next to the target already.
                int len = (int)diff.LengthSquared();
                if (len == 1)
                {
                    return true;
                }

                if (attacker.CurrentAttackSkill.DamageAffects == DamageAffects.Self)
                {
                    return true;
                }
            }
            return false;
        }

        private StringBuilder m_lastCombatResult = new StringBuilder();
        private Random m_combatRandom;
    }

    public class AttackDataInstance
    {
     //   public AttackData AttackData;
    }





    public class AttackResult
    {
        public AttackResultType resultType;
        public int damageDone;
        public BaseActor damageCauser;
    }



}
