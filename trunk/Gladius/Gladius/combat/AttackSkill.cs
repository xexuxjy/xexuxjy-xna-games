using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Barrage.Content;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;

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

        public AttackSkill(XmlElement node)
        {
            Name = node.Attributes["name"].Value;
            Cost = int.Parse(node.Attributes["cost"].Value);
            DamageType = (DamageType)Enum.Parse(typeof(DamageType),node.Attributes["damageType"].Value);
            DamageAffects = (DamageAffects)Enum.Parse(typeof(DamageAffects), node.Attributes["damageAffects"].Value);
            BaseDamage = float.Parse(node.Attributes["baseDamage"].Value);
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
        Miss,
        Hit,
        Critical
    }



}
