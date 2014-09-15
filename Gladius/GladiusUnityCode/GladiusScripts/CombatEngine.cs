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

        public float CalculateExpectedDamage(BaseActor attacker, BaseActor defender, AttackSkill attackSkill)
        {
            //float totalDamage = attackSkill.BaseDamage;

            //DAMAGE  =  BASE POWER  x  ( ATTACK MULTIPLIER / 100 )  x  SITUATIONAL FACTORS
            float categoryMultiplier = GetCategoryDamageMultiplier(attacker.ActorClassData, defender.ActorClassData);

            // lots of other stats to adjust here..
            // base on facing
			float positionMultiplier = 1.0f;
            float angle = Vector3.Dot(attacker.transform.forward, defender.transform.forward);
			if(angle < 0)
			{
				positionMultiplier = 1.5f;
			}

			float heightDiff = attacker.Position.y - defender.Position.y;
			float heightMultiplier = 1.0f;
			if(heightDiff > 0.25f)
			{
				heightMultiplier = 1.5f;
			}
			else if(heightDiff < -0.25f)
			{
				heightMultiplier = 0.75f;
			}


			float damageTypeMultiplier = 1f;
			if(ImmuneToSkill(attacker,defender,attackSkill))
			{
				damageTypeMultiplier = 0f;
			}
			
			
			float attackSkillBonus = attackSkill.AttackBonus;
			float defenseSkillBonus = attackSkill.DefenseBonus;
			

            float totalDamage = attacker.PWR * attackSkill.DamageMultiplier * categoryMultiplier * positionMultiplier * heightMultiplier * damageTypeMultiplier * attackSkillBonus * defenseSkillBonus;
            return totalDamage;

        }


        public void ApplyCrowdBonus(BaseActor attacker, BaseActor defender, AttackSkill attackSkill)
        {
        }

		public bool ImmuneToSkill(BaseActor attacker, BaseActor defender, AttackSkill attackSkill)
		{
			DamageType dt = attackSkill.DamageType;
			List<AttackSkill> activeSkills = defender.m_activeSkills;	
			
			
			
			List<AttackSkill> passiveSkills = defender.m_innateSkills;
            return false;
		
		}


        public void ResolveAttack(BaseActor attacker, BaseActor defender, AttackSkill attackSkill)
        {
            AttackResult attackResult = new AttackResult();
            attackResult.damageCauser = attacker;

            float accuracyBonus = GetCategoryAccuracyBonus(attacker.ActorClassData, defender.ActorClassData);
            float totalDamage = CalculateExpectedDamage(attacker, defender, attackSkill);


        	http://www.gamefaqs.com/gamecube/561233-gladius/faqs/64758
            //DIFFERENCE = [ (ACC * 0.97) - DEF ]
            float totalAccuracy = attacker.ACC + accuracyBonus;
            float diff1 = (totalAccuracy * 0.97f) - defender.DEF;

            //MISS CHANCE = 10 * 1.5 ^ [ DIFFERENCE * (-16 / 100) ]
            float missChance = 10f * ((float)Math.Pow(1.5f, (diff1 * (-16 / 100))));
            missChance *= 0.01f; // (0->1)
            float hitChance = 1f - missChance;

            if (attackSkill.CantMiss)
            {
                hitChance = 1f;
            }



            //if (m_combatRandom.NextDouble() > hitChance)
            if(true)
            {
                //attackResult.resultType = defender.HasShield ? AttackResultType.Blocked : AttackResultType.Miss;
                attackResult.resultType = AttackResultType.Blocked;
            }
            else
            {
                attackResult.resultType = AttackResultType.Hit;
                attackResult.damageDone = (int)totalDamage;
            }


            //attackResult.resultType = AttackResultType.Blocked;

            defender.TakeDamage(attackResult);
            CheckAndApplyConditions(attacker,defender,attackSkill,attackResult);

            DrawCombatResult(attackSkill, attackResult, attacker, defender);

        }

		public void CheckAndApplyConditions(BaseActor attacker, BaseActor defender, AttackSkill attackSkill,AttackResult attackResult)
		{
            if (attackResult.resultType == AttackResultType.Hit || attackResult.resultType == AttackResultType.Critical)
            {
                for (int i = 0; i < StatusConditions.Conditions.Length; ++i)
                {
                    SkillStatus skillStatus;
                    if (attackSkill.CausesStatus(StatusConditions.Conditions[i], out skillStatus))
                    {
                        if (!defender.ImmuneToStatus(StatusConditions.Conditions[i]))
                        {
                            defender.CauseStatus(StatusConditions.Conditions[i], skillStatus);
                        }
                    }
                }
            }
		}


        private void DrawCombatResult(AttackSkill attackSkill, AttackResult attackResult, BaseActor attacker, BaseActor defender)
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


        private float GetCategoryDamageMultiplier(ActorClassData attacker, ActorClassData defender)
        {
            float categoryMultiplier = 1f;
            if (attacker != defender)
            {
                if (attacker.IsHeavy && defender.IsMedium)
                {
                    categoryMultiplier = 1.5f;
                }
                else if (attacker.IsHeavy && defender.IsLight)
                {
                    categoryMultiplier = 0.5f;
                }
                else if (attacker.IsMedium && defender.IsLight)
                {
                    categoryMultiplier = 1.5f;
                }
                else if (attacker.IsMedium&& defender.IsHeavy)
                {
                    categoryMultiplier = 0.5f;
                }
                else if (attacker.IsLight && defender.IsHeavy)
                {
                    categoryMultiplier = 1.5f;
                }
                else if (attacker.IsLight&& defender.IsMedium)
                {
                    categoryMultiplier = 0.5f;
                }
            }
            return categoryMultiplier;
        }


        private float GetCategoryAccuracyBonus(ActorClassData attacker, ActorClassData defender)
        {
            float bonus = 0f;
            if (attacker.IsHeavy && defender.IsLight)
            {
                bonus = -20f;
            }
            else if (attacker.IsLight && defender.IsHeavy)
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

            if (attacker.ActorClassData.IsHeavy && defender.ActorClassData.IsLight)
            {
                return -1;
            }
            if (attacker.ActorClassData.IsHeavy && defender.ActorClassData.IsMedium)
            {
                return 1;
            }
            if (attacker.ActorClassData.IsLight&& defender.ActorClassData.IsMedium)
            {
                return -1;
            }
            return 0;
        }


        public void CheckInnateSkills(BaseActor attacker, BaseActor defender, AttackSkill skill, AttackResult attackResult)
        {
        }

        public bool IsValidTarget(BaseActor attacker, BaseActor defender, AttackSkill skill)
        {
                if (attacker != null && defender != null)
                {
                    if (defender.Dead)
                    {
                        return false;
                    }
                    if (skill == null)
                    {
                        return true;
                    }
                    bool skillValid = skill.ValidForTarget(attacker, defender, new Point());
                    return skillValid;
            }
            return false;
        }

        public bool IsAttackerInRange(BaseActor attacker, BaseActor defender, bool cursorOnly = false)
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

                return len <= attacker.CurrentAttackSkill.m_skillEffect.Range;
            }
            return false;
        }

        public void ApplySkill(AttackSkill skill, BaseActor attacker, BaseActor defender, AttackResult result)
        {
            switch (skill.m_skillEffect.Name)
            {
                case "Shield Break":
                    if (GladiusGlobals.Random100() < skill.m_skillEffect.Modifier1)
                    {
                        defender.BreakShield();
                    }
                    break;
                case "Shield Restore":
                    if (GladiusGlobals.Random100() < skill.m_skillEffect.Modifier1)
                    {
                        defender.RestoreShield();
                    }
                    break;
                case "Helmet Break":
                    if (GladiusGlobals.Random100() < skill.m_skillEffect.Modifier1)
                    {
                        defender.BreakHelmet();
                    }
                    break;
                case "Knockback":
                    defender.Knockback(attacker);
                    break;
                case "Knockdown":
                    defender.Knockdown(2);
                    break;
                case "Counterattack":
                    defender.QueueCounterAttack(attacker);
                    break;
                case "Change Crowd Source":
                    GladiusGlobals.Crowd.UpdateTeamScore(attacker.TeamName, (int)skill.m_skillEffect.Modifier1);
                    break;
                case "Change Crowd Target":
                    GladiusGlobals.Crowd.UpdateTeamScore(defender.TeamName, (int)skill.m_skillEffect.Modifier1);
                    break;
                case "Avoid":
                    result.resultType = AttackResultType.Avoided;
                    break;
                case "Blocked":
                    result.resultType = AttackResultType.Blocked;
                    break;
                case "Retreat":
                    defender.Retreat(attacker);
                    break;
                case "Remove Condition":
                    break;
                case "Random Teleport":
                    break;
            
                case "Remove Status":
                    break;

            }

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
