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
using Gladius.actors;

namespace Gladius.combat
{
    public class AttackSkill
    {
        public int Id;
        public String Name;
        public int UseCost; // how many points needed to use
        public int UseGain; // how many points you gain on using - normally only for affinity skills.
        public int PurchaseCost;
        public int SkillRow;

        public AttackType AttackType;
        public DamageType DamageType;
        public DamageAffects DamageAffects;
        public int BaseDamage;
        public float DamageMultiplier;

        public SkillIcon SkillIcon;

        public AnimationEnum Animation = AnimationEnum.None;

        public int MinRange;
        public int MaxRange;
        // how far we can move to do the attack.
        public int MovementRange;

        public int Radius;
        public bool EightSquare;
        // todo - lots of extra abilities on here.

        public List<GameObjectAttributeModifier> StatModifiers = new List<GameObjectAttributeModifier>();


        public bool HasMovementPath()
        {
            return MovementRange > 0;
        }


        public bool RangedAttack
        {
            get { return MinRange > 1; }
        }

        public AttackSkill(String name,int row,int useCost,int purchaseCost,DamageType damageType,DamageAffects damageAffects,int baseDamage)
        {
            Name = name;
            UseCost = useCost;
            PurchaseCost = purchaseCost;
            DamageType = damageType;
            DamageAffects = damageAffects;
            BaseDamage = baseDamage;
            DamageMultiplier = 1.0f;
            Radius = 0;
        }

        public AttackSkill(XmlElement node)
        {
            Id = int.Parse(node.Attributes["id"].Value);
            Name = node.Attributes["name"].Value;
            SkillRow = int.Parse(node.Attributes["skillRow"].Value);
            UseCost = GetIntAttribute(node, "useCost", 1); 
            UseGain = GetIntAttribute(node, "useGain", 0);
            PurchaseCost = int.Parse(node.Attributes["purchaseCost"].Value);
            AttackType = GetAttackType(node);
            DamageType = GetDamageType(node);
            DamageAffects = GetDamageAffects(node);
            SkillIcon = (SkillIcon)Enum.Parse(typeof(SkillIcon), node.Attributes["skillIcon"].Value);
            BaseDamage = GetIntAttribute(node, "baseDamage", 0);
            MinRange = GetIntAttribute(node, "minRange", 0);
            MaxRange = GetIntAttribute(node, "maxRange", 0);
            Radius = GetIntAttribute(node, "radius", 0);
            MovementRange = GetIntAttribute(node, "movementRange", 0);


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

        public DamageType GetDamageType(XmlElement node)
        {
            if (node.HasAttribute("damageType"))
            {
                return (DamageType)Enum.Parse(typeof(DamageType), node.Attributes["damageType"].Value);
            }
            return DamageType.Physical;
        }

        public DamageAffects GetDamageAffects(XmlElement node)
        {
            if (node.HasAttribute("damageAffects"))
            {
                return (DamageAffects)Enum.Parse(typeof(DamageAffects), node.Attributes["damageAffects"].Value);
            }
            return DamageAffects.Default;
        }

        public AttackType GetAttackType(XmlElement node)
        {
            if (node.HasAttribute("attackType"))
            {
                return (AttackType)Enum.Parse(typeof(AttackType), node.Attributes["attackType"].Value);
            }
            return AttackType.SingleOrtho;
        }

        private int GetIntAttribute(XmlElement node, string name, int defaultVal)
        {
            if (node.HasAttribute(name))
            {
                return  int.Parse(node.Attributes[name].Value);
            }
            return defaultVal;
        }

        public bool HasModifiers()
        {
            return StatModifiers.Count > 0;
        }

        public bool NeedsGrid
        {
            get { return AttackType != AttackType.EndTurn; }
        }

        public bool InRange(int dist)
        {
            return (dist >= MinRange && dist <= (MovementRange + MaxRange));
        }

        public bool Available(BaseActor actor)
        {
            // need to make sure actor has enough skillpoints or affinity points to use this.
            if (UseCost <= actor.ArenaSkillPoints)
            {
                return true;
            }
            return false;
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
