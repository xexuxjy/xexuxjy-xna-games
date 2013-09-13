using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;

namespace Gladius.combat
{
    public class AttackSkill
    {
        public String Name;
        public int UseCost;
        public int PurchaseCost;
        public int SkillRow;

        public AttackType AttackType;
        public DamageType DamageType;
        public DamageAffects DamageAffects;
        public float BaseDamage;
        public float DamageMultiplier;

        public SkillIcon SkillIcon;

        
        // todo - lots of extra abilities on here.



        public AttackSkill(String name,int row,int useCost,int purchaseCost,AttackType attackType,DamageType damageType,DamageAffects damageAffects,float baseDamage)
        {
            Name = name;
            UseCost = useCost;
            PurchaseCost = purchaseCost;
            AttackType = attackType;
            DamageType = damageType;
            DamageAffects = damageAffects;
            BaseDamage = baseDamage;
            DamageMultiplier = 1.0f;
        }

        public AttackSkill(XmlElement node)
        {
            Name = node.Attributes["name"].Value;
            SkillRow = int.Parse(node.Attributes["skillRow"].Value);
            UseCost = int.Parse(node.Attributes["useCost"].Value);
            PurchaseCost = int.Parse(node.Attributes["purchaseCost"].Value);
            AttackType = (AttackType)Enum.Parse(typeof(AttackType), node.Attributes["attackType"].Value);
            DamageType = (DamageType)Enum.Parse(typeof(DamageType),node.Attributes["damageType"].Value);
            DamageAffects = (DamageAffects)Enum.Parse(typeof(DamageAffects), node.Attributes["damageAffects"].Value);
            SkillIcon = (SkillIcon)Enum.Parse(typeof(SkillIcon), node.Attributes["skillIcon"].Value);
            BaseDamage = float.Parse(node.Attributes["baseDamage"].Value);
            DamageMultiplier = 1.0f;
        }

    }

    public class AttackSkillDictionary
    {
        public void Populate(ContentManager contentManager)
        {
            using (StreamReader sr = new StreamReader(TitleContainer.OpenStream("Content/CombatData/Skills.xml")))
            {
                String result = sr.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                XmlNodeList nodes = doc.SelectNodes("//AttackSkill");
                foreach (XmlNode node in nodes)
                {
                    AttackSkill attackSkill = new AttackSkill(node as XmlElement);
                    Data[attackSkill.Name] = attackSkill;
                }

                //XmlSource xs = contentManager.Load<XmlSource>("CombatData/Skills");
                int ibreak = 0;
            }
        }

        public Dictionary<String, AttackSkill> Data = new Dictionary<String, AttackSkill>();
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
        None,
        Miss,
        Hit,
        Critical
    }

    public enum SkillIcon
    {
        Special = 0,
        Attack = 1,
        Defend = 2,
        Move = 3,

    }

    public enum AttackType
    {
        Move,
        Block,
        EndTurn,
        Single,
        AOE
    }


}
