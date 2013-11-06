using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.IO;
using System.Xml;
using xexuxjy.Gladius.util;
using Gladius.renderer.animation;

namespace Gladius.combat
{
    public class AttackSkill
    {
        public int Id;
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

        public AnimationEnum Animation = AnimationEnum.None;

        public int MinRange;
        public int MaxRange;
        public int Radius;
        public bool EightSquare;
        // todo - lots of extra abilities on here.

        public List<GameObjectAttributeModifier> StatModifiers = new List<GameObjectAttributeModifier>();


        public bool HasMovementPath()
        {
            return MaxRange > 1 && !RangedAttack;
        }


        public bool RangedAttack
        {
            get { return Animation == AnimationEnum.BowShot; }
        }


        public AttackSkill(String name,int row,int useCost,int purchaseCost,DamageType damageType,DamageAffects damageAffects,float baseDamage)
        {
            Name = name;
            UseCost = useCost;
            PurchaseCost = purchaseCost;
            DamageType = damageType;
            DamageAffects = damageAffects;
            BaseDamage = baseDamage;
            DamageMultiplier = 1.0f;
            Radius = 1;
        }

        public AttackSkill(XmlElement node)
        {
            Id = int.Parse(node.Attributes["id"].Value);
            Name = node.Attributes["name"].Value;
            SkillRow = int.Parse(node.Attributes["skillRow"].Value);
            UseCost = int.Parse(node.Attributes["useCost"].Value);
            PurchaseCost = int.Parse(node.Attributes["purchaseCost"].Value);
            AttackType = (AttackType)Enum.Parse(typeof(AttackType), node.Attributes["attackType"].Value);
            DamageType = (DamageType)Enum.Parse(typeof(DamageType),node.Attributes["damageType"].Value);
            DamageAffects = (DamageAffects)Enum.Parse(typeof(DamageAffects), node.Attributes["damageAffects"].Value);
            SkillIcon = (SkillIcon)Enum.Parse(typeof(SkillIcon), node.Attributes["skillIcon"].Value);
            BaseDamage = float.Parse(node.Attributes["baseDamage"].Value);
            if(node.HasAttribute("minRange"))
            {
                MinRange = int.Parse(node.Attributes["minRange"].Value);
            }
            else
            {
                MinRange = 1;
            }
            if(node.HasAttribute("maxRange"))
            {
                MaxRange = int.Parse(node.Attributes["maxRange"].Value);
            }
            else
            {
                MaxRange = 1;
            }
            if (node.HasAttribute("radius"))
            {
                Radius = int.Parse(node.Attributes["radius"].Value);
            }
            else
            {
                Radius = 1;
            }

            if (node.HasAttribute("animation"))
            {
                Animation = (AnimationEnum)Enum.Parse(typeof(AnimationEnum), node.Attributes["animation"].Value);
            }

            DamageMultiplier = 1.0f;

            if (node.HasChildNodes)
            {
                XmlNodeList modifiers = node.GetElementsByTagName("Modifier");
                foreach (XmlNode modifier in modifiers)
                {
                    String sval = modifier.Attributes["stat"].Value;
                    GameObjectAttributeType goat = (GameObjectAttributeType)Enum.Parse(typeof(GameObjectAttributeType), sval);
                    int val = int.Parse(modifier.Attributes["amount"].Value);
                    GameObjectAttributeModifier statModifier = new GameObjectAttributeModifier(goat,val);
                    StatModifiers.Add(statModifier);
                }

            }
        }

        public bool HasModifiers()
        {
            return StatModifiers.Count > 0;
        }

        //public static bool IsAttackSkill(AttackSkill attackSkill)
        //{
        //    return attackSkill.AttackType == AttackType.SingleOrtho || attackSkill.AttackType == AttackType.SingleSurround;
        //}

        //public static bool IsMoveSkill(AttackSkill attackSkill)
        //{
        //    return attackSkill.AttackType == AttackType.Move;
        //}

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
                    Data[attackSkill.Id] = attackSkill;
                }

                //XmlSource xs = contentManager.Load<XmlSource>("CombatData/Skills");
                int ibreak = 0;
            }
        }

        public Dictionary<int, AttackSkill> Data = new Dictionary<int, AttackSkill>();
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
        Critical,
        Blocked
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
        Attack,
        Block,
        SingleOrtho,
        Move,
        EndTurn
    }


}
