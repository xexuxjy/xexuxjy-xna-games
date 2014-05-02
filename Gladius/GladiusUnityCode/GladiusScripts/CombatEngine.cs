using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using StringLeakTest;

namespace Gladius.combat
{
    public class CombatEngine
    {
        public CombatEngine()
        {
            m_combatRandom = new System.Random();
        }

        public float CalculateExpectedDamage(BaseActor attacker, BaseActor defender,AttackSkill attackSkill)
        {
            //float totalDamage = attackSkill.BaseDamage;

            //DAMAGE  =  BASE POWER  x  ( ATTACK MULTIPLIER / 100 )  x  SITUATIONAL FACTORS
            float categoryMultiplier = GetCategoryDamageMultiplier(ActorGenerator.CategoryClass[attacker.ActorClass], ActorGenerator.CategoryClass[defender.ActorClass]);
            float totalDamage = attacker.GetAttributeValue(GameObjectAttributeType.Power) * attackSkill.DamageMultiplier * categoryMultiplier;
            // lots of other stats to adjust here..
            return totalDamage;

        }

        public void ResolveAttack(BaseActor attacker, BaseActor defender,AttackSkill attackSkill)
        {
            AttackResult attackResult = new AttackResult();

            float accuracyBonus = GetCategoryAccuracyBonus(ActorGenerator.CategoryClass[attacker.ActorClass], ActorGenerator.CategoryClass[defender.ActorClass]);
            float totalDamage = CalculateExpectedDamage(attacker, defender, attackSkill);

            
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





            //attackResult.resultType = AttackResultType.Blocked;

            if (attackResult.resultType == AttackResultType.Blocked)
            {
                defender.StartBlock(attacker);
            }
            else
            {
                defender.TakeDamage(attackResult);
            }

            DrawCombatResult(attackSkill,attackResult,attacker,defender);
            
        }

        private void DrawCombatResult(AttackSkill attackSkill, AttackResult attackResult,BaseActor attacker,BaseActor defender)
        {
            String text = null;
            Color color = Color.white;
            if (attackResult.resultType == AttackResultType.Miss)
            {
                text = "Missed";
            }
            else if (attackResult.resultType == AttackResultType.Blocked)
            {
                text = "Blocked";
            }
            else 
            {
                text = "" + attackResult.damageDone;
                if (attackResult.resultType == AttackResultType.Critical)
                {
                    color = Color.red;
                }
            }

            GladiusGlobals.CombatEngineUI.DrawFloatingText(defender.CameraFocusPoint, color, text, 2f);

            m_lastCombatResult.Length = 0;
            m_lastCombatResult.ConcatFormat("Attacker [{0}] Defender[{1}] Skill [{2}] ", attacker.Name, defender.Name, attackSkill.Name);
            m_lastCombatResult.ConcatFormat("Result [{0}] Damage [{1}]", "" + attackResult.resultType, attackResult.damageDone);
            Debug.Log(m_lastCombatResult.ToString());

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


        public int GetClassAdvantage(BaseActor attacker, BaseActor defender)
        {
            ActorCategory aCat = ActorGenerator.CategoryClass[attacker.ActorClass];
            ActorCategory dCat = ActorGenerator.CategoryClass[defender.ActorClass];

            if (aCat == ActorCategory.Heavy && dCat == ActorCategory.Light)
            {
                return -1;
            }
            if (aCat == ActorCategory.Heavy && dCat == ActorCategory.Medium)
            {
                return 1;
            }
            if (aCat == ActorCategory.Light && dCat == ActorCategory.Medium)
            {
                return -1;
            }



            return 0;
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

        public bool IsAttackerInRange(BaseActor attacker, BaseActor defender,bool cursorOnly=false)
        {
            if (defender != null && attacker != null)
            {
                Point fp = attacker.ArenaPoint;
                Point tp = defender.ArenaPoint;
                Vector3 diff = new Vector3(tp.X, 0, tp.Y) - new Vector3(fp.X, 0, fp.Y);

                // we're next to the target already.
                //int len = (int)diff.LengthSquared();
                int len = (int)diff.magnitude;
                if (len == 1)
                {
                    return true;
                }

                if (attacker.CurrentAttackSkill.DamageAffects == DamageAffects.Self)
                {
                    return true;
                }

                if (attacker.CurrentAttackSkill.InRange(len))
                {
                    if(cursorOnly ||attacker.CurrentAttackSkill.RangedAttack )
                    {
                        return true;
                    }
                }

            }
            return false;
        }

        private StringBuilder m_lastCombatResult = new StringBuilder();
        private System.Random m_combatRandom;
    }

    

    public class AttackResult
    {
        public AttackResultType resultType;
        public int damageDone;
        public BaseActor damageCauser;
    }



}
