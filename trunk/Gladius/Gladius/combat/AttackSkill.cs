using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Barrage.Content;

namespace Gladius.combat
{
    public class AttackSkill
    {
        public String Name;
        public int Cost;

        public DamageType DamageType;
        public DamageAffects DamageAffects;
        public float BaseDamage;
        public float DamageMultiplier;

        
        // todo - lots of extra abilities on here.



        public AttackSkill(String name,int cost,DamageType damageType,DamageAffects damageAffects,float baseDamage)
        {
            Name = name;
            Cost = cost;
            DamageType = damageType;
            DamageAffects = damageAffects;
            BaseDamage = baseDamage;
            DamageMultiplier = 1.0f;
        }
    }

    public class AttackSkillDictionary
    {
        public void Populate(ContentManager contentManager)
        {
            XmlSource xs = contentManager.Load<XmlSource>("CombatData/Skills");
            int ibreak = 0;

        }

        //public Dictionary<String, AttackSkill> AttackSkillDictionary = new Dictionary<String, AttackSkill>();
    }

    public enum DamageType
    {
        Physical,
        Air,
        Earth,
        Fire,
        Water,
        Light,
        Dark
    };

    public enum DamageAffects
    {
        Default,
        Self,
        Team
    }

    public enum AttackResultType
    {
        Miss,
        Hit,
        Critical
    }



}
