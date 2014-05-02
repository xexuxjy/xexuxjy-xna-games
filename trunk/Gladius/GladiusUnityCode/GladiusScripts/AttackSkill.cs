using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Gladius
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

        public bool RequiresLOS
        { get; set; }

        public bool RangedAttack
        {
            get { return MinRange > 1; }
        }

        public AttackSkill()
        {
        }

        public AttackSkill(String name, int row, int useCost, int purchaseCost, DamageType damageType, DamageAffects damageAffects, int baseDamage)
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


            if (node.HasAttribute("animation:"))
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
                    GameObjectAttributeModifier statModifier = new GameObjectAttributeModifier(goat, val);
                    StatModifiers.Add(statModifier);
                }

            }
        }

        public DamageType GetDamageType(XmlElement node)
        {
            if (node.HasAttribute("damageType:"))
            {
                return (DamageType)Enum.Parse(typeof(DamageType), node.Attributes["damageType"].Value);
            }
            return DamageType.Physical;
        }

        public DamageAffects GetDamageAffects(XmlElement node)
        {
            if (node.HasAttribute("damageAffects:"))
            {
                return (DamageAffects)Enum.Parse(typeof(DamageAffects), node.Attributes["damageAffects"].Value);
            }
            return DamageAffects.Default;
        }

        public AttackType GetAttackType(XmlElement node)
        {
            if (node.HasAttribute("attackType:"))
            {
                return (AttackType)Enum.Parse(typeof(AttackType), node.Attributes["attackType"].Value);
            }
            return AttackType.SingleOrtho;
        }

        private int GetIntAttribute(XmlElement node, string name, int defaultVal)
        {
            if (node.HasAttribute(name))
            {
                return int.Parse(node.Attributes[name].Value);
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

        public bool RequiresLocationTarget
        {
            get
            {
                return true;
            }
        }

        public bool RequiresActorTarget
        {
            get
            {
                return true;
            }
        }


    }

    public class AttackSkill2
    {
        public String Name;
        public String Type;
        public String SubType;
        public String UseClass;
        public String DisplayNameId;
        public String DescriptionId;
        public int SkillLevel;
        public float SkillCosts1;
        public float SkillCosts2;
        public int JobPointCost;
        public String Attribute;
        public int CombatMod1;
        public float CombatMod2;
        public int SkillRange;
        public String SkillRangeName;
        public int SkillExcludeRange;
        public String SkillExcludeRangeName;

        public String MeterName;
        public String AnimName;
        public String FXName;
        public String SkillEffectName;
        public float SkillEffectModifier1;
        public float SkillEffectModifier2;
        public int SkillEffectRange;
        public String SkillEffectRangeName;
        public String SkillEffectCondition;

        public String Affinity;
        public String TargetCondition;
    
    }

    public class AttackSkillDictionary
    {
        public void Populate(String filename)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("AttackSkills");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(textAsset.text);
            XmlNodeList nodes = doc.SelectNodes("//AttackSkill");
            foreach (XmlNode node in nodes)
            {
                AttackSkill attackSkill = new AttackSkill(node as XmlElement);
                Data[attackSkill.Id] = attackSkill;
            }
        }


        public void LoadExtractedData(String path)
        {
            TextAsset textAsset = (TextAsset)Resources.Load("ExtractedSkillData");
            String data = textAsset.text;
            ParseExtractedData(data);
        }

        public void ParseExtractedData(String data)
        {
                AttackSkill2 currentSkill = null;
                String[] lines = data.Split('\n');
                for(int counter=0;counter<lines.Length;counter++)
                {
                    String line = lines[counter];
                    if (!line.StartsWith("SKILL"))
                    {
                        continue;
                    }

                    String[] lineTokens = line.Split(new char[]{',',':'});
         

                    if(line.StartsWith("SKILLCREATE:"))
                    {
                        currentSkill = new AttackSkill2();
                        if (lineTokens.Length == 4)
                        {
                            currentSkill.Name = lineTokens[1];
                            currentSkill.Type = lineTokens[2];
                            currentSkill.SubType = lineTokens[3];
                            Data2[currentSkill.Name] = currentSkill;

                        }

                    }
                    else if(line.StartsWith("SKILLUSECLASS:"))
                    {
                        currentSkill.UseClass = lineTokens[1];
                    }
                    else if(line.StartsWith("SKILLDISPLAYNAMEID:"))
                    {
                        currentSkill.DisplayNameId = lineTokens[1];
                    }
                    else if(line.StartsWith("SKILLDESCRIPTIONID:"))
                    {
                        currentSkill.DescriptionId = lineTokens[1];
                    }
                    else if(line.StartsWith("SKILLLEVEL:"))
                    {
                        currentSkill.SkillLevel = int.Parse(lineTokens[1]);
                    }
                    else if(line.StartsWith("SKILLJOBPOINTCOST:"))
                    {
                        currentSkill.JobPointCost= int.Parse(lineTokens[1]);
                    }
                    else if(line.StartsWith("SKILLCOSTS:"))
                    {
                        currentSkill.SkillCosts1 = float.Parse(lineTokens[1]);
                        currentSkill.SkillCosts2 = float.Parse(lineTokens[2]);
                    }
                    else if(line.StartsWith("SKILLATTRIBUTE:"))
                    {
                        currentSkill.Attribute = lineTokens[1];
                    }
                    else if(line.StartsWith("SKILLCOMBATMODS:"))
                    {
                        currentSkill.CombatMod1 = int.Parse(lineTokens[1]);
                        currentSkill.CombatMod2 = float.Parse(lineTokens[2]);
                    }
                    else if(line.StartsWith("SKILLRANGE:"))
                    {
                        currentSkill.SkillRange = int.Parse(lineTokens[1]);
                        currentSkill.SkillRangeName = lineTokens[2];
                    }
                    else if(line.StartsWith("SKILLEXCLUDERANGE:"))
                    {
                        currentSkill.SkillExcludeRange= int.Parse(lineTokens[1]);
                        currentSkill.SkillExcludeRangeName = lineTokens[2];

                    }
                    else if(line.StartsWith("SKILLMETER:"))
                    {
                        currentSkill.MeterName= lineTokens[1];
                    }
                    else if(line.StartsWith("SKILLANIMSPEED:"))
                    {

                    }
                    else if(line.StartsWith("SKILLFXSWING:"))
                    {

                    }
                    else if(line.StartsWith("SKILLEFFECTRANGE:"))
                    {
                        currentSkill.SkillEffectRange = int.Parse(lineTokens[1]);
                        currentSkill.SkillEffectRangeName = lineTokens[2];

                    }
                    else if(line.StartsWith("SKILLEFFECT:"))
                    {
                        if (lineTokens.Length != 4)
                        {
                            int ibreak = 0;
                        }

                        currentSkill.SkillEffectName = lineTokens[1];
                        currentSkill.SkillEffectModifier1 = float.Parse(lineTokens[2]);
                        currentSkill.SkillEffectModifier2 = float.Parse(lineTokens[3]);
                    }
                    else if(line.StartsWith("SKILLEFFECTCONDITION:"))
                    {
                        currentSkill.SkillEffectCondition = lineTokens[1];
                    }
                    else if(line.StartsWith("SKILLSTATUS:"))
                    {

                    }
                    else if(line.StartsWith("SKILLSTATUSDURATION:"))
                    {

                    }
                    else if(line.StartsWith("SKILLSTATUSSITUATIONSTATUSCONDITION:"))
                    {                             

                    }
                    else if(line.StartsWith("SKILLSTATUSCHANCE:"))
                    {                             

                    }
                    else if(line.StartsWith("SKILLSTATUS2:"))
                    {                             

                    }
                    else if(line.StartsWith("SKILLSTATUSDURATION2:"))
                    {                             

                    }
                    else if(line.StartsWith("SKILLSTATUSSITUATIONSTATUSCONDITION2:"))
                    {                             

                    }
                    else if(line.StartsWith("SKILLSTATUSCHANCE2:"))
                    {                             

                    }
                    else if(line.StartsWith("SKILLSTATUSTARGET:"))
                    {

                    }
                    else if(line.StartsWith("SKILLSTATUSCONDITION:"))
                    {

                    }
                    else if(line.StartsWith("SKILLANIM:"))
                    {

                    }
                    else if(line.StartsWith("SKILLFX:"))
                    {

                    }
                    else if(line.StartsWith("SKILLSTATUSSITUATIONAFFINITYCONDITION:"))
                    {

                    }
                    else if(line.StartsWith("SKILLSTATUSSITUATIONAFFINITYCONDITION2:"))
                    {

                    }
                    else if(line.StartsWith("SKILLPROJECTILE:"))
                    {

                    }
                    else if(line.StartsWith("SKILLPROJECTILESEQUENCE:"))
                    {

                    }
                    else if(line.StartsWith("SKILLFXPROJECTILE:"))
                    {

                    }
                    else if(line.StartsWith("SKILLSTATUSSITUATIONUNITCONDITION:"))
                    {

                    }
                    else if (line.StartsWith("SKILLAFFINITY:"))
                    {

                    }
                    else if (line.StartsWith("SKILLTARGETCONDITION:"))
                    {

                    }

                    else
                    {

                    //err
                        //Debug.LogError("Unknown token : "+line);
                    }
                }


        }



        public Dictionary<int, AttackSkill> Data = new Dictionary<int, AttackSkill>();
        public Dictionary<String, AttackSkill2> Data2 = new Dictionary<String, AttackSkill2>();
    }




}
