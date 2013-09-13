using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using xexuxjy.Gladius.util;
using Microsoft.Xna.Framework;

namespace Gladius.combat
{
    public class CombatEngine
    {
        public CombatEngine()
        {
            m_combatRandom = new Random();
        }


        public AttackResult ResolveAttack(BaseActor attacker, BaseActor defender,AttackSkill attackData)
        {
            AttackResult attackResult = new AttackResult();

            float totalDamage = attackData.BaseDamage;

            // chance for evade.
            // each 10 points of agility gives 1% evade chance?
            float defenderAgility = defender.GetAttributeValue(GameObjectAttributeType.Agility);
            float chance = defenderAgility / 10;
            chance *= 0.01f;

            if (m_combatRandom.NextDouble() < chance)
            {
                attackResult.resultType = AttackResultType.Miss;
            }
            else
            {
                attackResult.resultType = AttackResultType.Hit;
                attackResult.damageDone = totalDamage;
            }
            return attackResult;
        }

        public bool IsValidTarget(BaseActor attacker, BaseActor defender)
        {
            return attacker != null && defender != null && attacker != defender;
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
        

        private Random m_combatRandom;
    }

    public class AttackDataInstance
    {
     //   public AttackData AttackData;
    }





    public class AttackResult
    {
        public AttackResultType resultType;
        public float damageDone;
        public BaseActor damageCauser;
    }



}
