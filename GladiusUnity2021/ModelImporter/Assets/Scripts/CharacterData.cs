using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : IEquatable<CharacterData>
{

    public String Name
    {get;set;}

    public string ClassDefName
    {get; set;}

    public int Cost
    { get; set; }

    public int HireCost
    { get; set; }

    public float CON
    { get; set; }

    public float PWR
    { get; set; }

    public float ACC
    { get; set; }

    public float DEF
    { get; set; }

    public float INI
    { get; set; }

    public float MOVEMENTRATE
    { get; set; }

    public int HP
    { get; set; }

    public int DAM
    { get; set; }

    public int Level
    {get;set;}

    public int Experience
    {get;set;}

    public int JobPoints
    {get;set;}

    public int XP
    {get;set;}

    public int NEXTXP
    {get;set;}




    public ActorClassDef CurrentClassDef
    {
        get
        {
            return ShiftedActorClassDef != null ? ShiftedActorClassDef : ActorClassDef;
        }
    }


    public GladiatorSchool School
    { get; set; } 

    private ActorClassDef m_actorClassDef;


    public ActorClassDef ActorClassDef
    {
        get
        {
            if (m_actorClassDef == null)
            {
                m_actorClassDef = ActorGenerator.ActorClassDefs.Find(x => x.name == ClassDefName);
            }
            return m_actorClassDef;
        }

    }


    public Point StartPosition
    { get; set; }
    
    public void ResetStartPosition()
    {
        StartPosition = GladiusGlobals.InvalidPoint;
    }



    public ActorClassDef ShiftedActorClassDef
    {
        get;
        set;
    }


    public String ThumbnailName
    {
        get
        {
            if (!String.IsNullOrEmpty(CurrentClassDef.headIcon))
            {
                return CurrentClassDef.headIcon;
            }
            return CurrentClassDef.name;
        }
    }


    public bool Selected
    {get;set;}


    public int RequiredMask
    { get; set; }


    public int MinLevel
    { get; set; }


    public int MaxLevel
    { get; set; }


    public string SkillSetName
    { get; set; }

    public string StatsSetName
    { get; set; }

    public List<AttackSkill> NonInnateSkills
    {
        get { return m_nonInnateSkills; }
    }

    public List<AttackSkill> InnateSkills
    {
        get { return m_innateSkills; }
    }


    public bool PassSkillOnly
    {
        get
        {
            if (KnownSkillNames.Count == 1 && KnownSkillNames[0] == "Pass")
            {
                //CurrentAttackSkill = m_knownSkills[0];
                return true;
            }
            return false;
        }
    }


    public DamageType WeaponAffinityType
    {
        get
        {
            Item item = GetItemAtLocation(ItemLocation.Weapon);
            //return item != null ? item.DamageType : DamageType.None;
            return item != null ? item.AffinityType : DamageType.None;
        }
    }

    public DamageType ArmourAffinityType
    {
        get
        {
            Item item = GetItemAtLocation(ItemLocation.Armor);
            return item != null ? item.AffinityType : DamageType.None;
        }
    }



    public float ValFromName(string name)
    {
        switch (name)
        {
            case "LVL": return Level;
            case "XP": return XP;
            case "Next": return NEXTXP;
            case "HP": return HP;
            case "DAM": return DAM;
            case "PWR": return PWR;
            case "ACC": return DEF;
            case "INI": return INI;
            case "CON": return CON;
            case "MOV": return (int)MOVEMENTRATE;
            case "Arm": return 0;
            case "Wpn": return 0;
            default:
                return 0;
        }
    }

    public float ValFromGoat(GameObjectAttributeType type)
    {
        switch (type)
        {
            case GameObjectAttributeType.Level: return Level;
            case GameObjectAttributeType.Xp: return XP;
            case GameObjectAttributeType.NextXp: return NEXTXP;
            case GameObjectAttributeType.Health: return HP;
            case GameObjectAttributeType.Damage: return DAM;
            case GameObjectAttributeType.Power: return PWR;
            case GameObjectAttributeType.Accuracy: return DEF;
            case GameObjectAttributeType.Initiative: return INI;
            case GameObjectAttributeType.Constitution: return CON;
            case GameObjectAttributeType.Movement: return (int)MOVEMENTRATE;
            default:
                return 0;
        }

    }


    public void InitValues()
    {
        ACC = 10;
        PWR = 10;
        DEF = 10;
        CON = 10;
        XP = 10;
    }

    public void CopyModCoreStat(StatsSet mcs)
    {
        CON = mcs.CON;
        PWR = mcs.PWR;
        ACC = mcs.ACC;
        DEF = mcs.DEF;
        INI = mcs.INI;
        MOVEMENTRATE = mcs.MOV;
    }



    public void ShiftCharacter(AttackSkill attackSkill)
    {
        FreeSkillNames.Clear();
        if (attackSkill != null)
        {
            ShiftedActorClassDef = ActorGenerator.ActorClassDefs.Find(x => x.name == attackSkill.SkillShiftData.ShiftClass);
            AttackSkill shiftSkill = attackSkill;
            while (shiftSkill != null)
            {
                FreeSkillNames.AddRange(shiftSkill.SkilllFreeList);
                shiftSkill = shiftSkill.PreReqSkill;
            }

            


        }
        else
        {
            ShiftedActorClassDef = null;
        }
    }



    public Item GetItemAtLocation(ItemLocation location)
    {
        String itemName = m_itemNames[(int)location];
        if (m_items[(int)location] == null && itemName != null)
        {
            Item item = null;
            if (ItemManager.Items.TryGetValue(itemName, out item))
            {
                m_items[(int)location] = item;
            }
        }

        return m_items[(int)location];
    }

    //            	CON, PWR, ACC, DEF, INT, MOVE


    public void ReplaceItem(String itemKey)
    {
        Item item;
        int location = -1;
        if (ItemManager.Items.TryGetValue(itemKey, out item))
        {
            location = (int)item.ItemLocation;
        }
        if (m_items[location] != null)
        {
            // return current item to the school
            if (School != null)
            {
                School.AddItem(m_items[location].Name);
            }
            AddItem(itemKey);
        }
    }


    public void AddItem(String itemKey)
    {
        Item item;
        if (ItemManager.Items.TryGetValue(itemKey, out item))
        {
            m_items[(int)item.ItemLocation] = item;
            UpdateStats();
        }
    }

    public void RemoveItem(String itemKey)
    {
        Item item;
        if (ItemManager.Items.TryGetValue(itemKey, out item))
        {
            m_items[(int)item.ItemLocation] = null;
            UpdateStats();
        }
    }




    private void UpdateStats()
    {
    }



    public String GetInfoString()
    {
        if (m_infoString == null)
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("*** {0} ***\n", Name);
            //sb.Append("\nClass : " + ActorClass);
            //sb.Append("\nLevel : " + Level);
            //sb.Append("\nPower : " + m_attributeDictionary[GameObjectAttributeType.Power].CurrentValue);
            //sb.Append("\nAccuracy : " + m_attributeDictionary[GameObjectAttributeType.Accuracy].CurrentValue);
            //sb.Append("\nDefense : " + m_attributeDictionary[GameObjectAttributeType.Defense].CurrentValue);
            //sb.Append("\nConstitution : " + m_attributeDictionary[GameObjectAttributeType.Constitution].CurrentValue);
            //sb.Append("\nXP : " + m_attributeDictionary[GameObjectAttributeType.Experience].CurrentValue);

            m_infoString = sb.ToString();
        }
        return m_infoString;
    }

    public void LearnSkill(String skillName)
    {
        KnownSkillNames.Add(skillName);
    }

    public void AddItemByNameAndLoc(String itemName, ItemLocation loc)
    {
        m_itemNames[(int)loc] = itemName;
    }

    public String GetItemNameAtLoc(ItemLocation loc)
    {
        return m_itemNames[(int)loc];
    }

    public bool Equals(CharacterData other)
    {
        if (other == null)
        {
            return false;
        }
        return String.Equals(Name, other.Name);
    }


    static List<AttackSkill> AllSkills = new List<AttackSkill>();
    static List<AttackSkill> PreReqSkills = new List<AttackSkill>();

    public void SetupSkills()
    {
        AllSkills.Clear();
        PreReqSkills.Clear();

        m_nonInnateSkills.Clear();
        m_innateSkills.Clear();


        List<String> skillNames = FreeSkillNames.Count > 0 ? FreeSkillNames : KnownSkillNames;

        foreach (string name in skillNames)
        {
            AttackSkill skill = AttackSkillDictionary.Data[name];
            AllSkills.Add(skill);
        }

        for (int i = AllSkills.Count - 1; i >= 0; i--)
        {
            if (!String.IsNullOrEmpty(AllSkills[i].PreReqName))
            {
                AttackSkill preReq = AttackSkillDictionary.Data[AllSkills[i].PreReqName];
                PreReqSkills.Add(preReq);
            }

        }

        //AllSkills.RemoveAll(x => PreReqSkills.Contains(x));


        foreach (AttackSkill skill in AllSkills)
        {
           if (skill.IsInnate)
            {
                m_innateSkills.Add(skill);
            }
            else
            {
                if (skill.IsUsable(this))
                {
                    //if (skill.IsOneTime)
                    //{
                    //    if (!String.IsNullOrEmpty(skill.PreReqName))
                    //    {
                    //        AttackSkill preReq = AttackSkillDictionary.Data[skill.PreReqName];
                    //        m_innateSkills.Remove(preReq);
                    //        m_innateSkills.Add(skill);
                    //    }
                    //}

                    m_nonInnateSkills.Add(skill);
                }
            }
        }
    }



    private Item[] m_items = new Item[(int)ItemLocation.NumItems];
    private String[] m_itemNames = new String[(int)ItemLocation.NumItems];
    private String m_infoString;

    private List<AttackSkill> m_innateSkills = new List<AttackSkill>();
    private List<AttackSkill> m_nonInnateSkills = new List<AttackSkill>();

    // These are ones that we've learnt. 
    public List<string> KnownSkillNames = new List<string>();
    // Frees Skills are ones given my another skill such as Bear Shift
    public List<string> FreeSkillNames = new List<string>();

    //private List<String> m_knownSkillNames = new List<String>();
    //private List<String> m_freeSkillNames = new List<string>();
}
