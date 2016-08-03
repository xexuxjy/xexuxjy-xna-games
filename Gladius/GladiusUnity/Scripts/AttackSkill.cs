using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

public class AttackSkill
{
    public const int m_defaultMoveToAttackRange = 5;



    public String Name;
    public String Type;
    public String SubType;
    public SkillClass UseClass;
    public int DisplayNameId;
    public String DescriptionId;
    public int SkillLevel;
    public float SkillCosts1;
    public float SkillCosts2;
    public int JobPointCost;
    public int CombatMod1;
    public float CombatMod2;
    public int MoveToAttackModifier;
    public int SkillRange;
    public String SkillRangeName;
    public int SkillExcludeRange;
    public String SkillExcludeRangeName;

    public String SubSkillName;
    public AttackSkill SubSkill;

    public String MeterName;
    public String AnimName;
    public String FXName;

    public String Affinity;
    public int AffinityCost;
    public String TargetCondition;
    public String PreRequisite;

    public AnimationEnum Animation;

    public DamageType DamageType;

    public float AttackBonus = 1.0f;
    public float DefenseBonus = 1.0f;


    private List<String> m_targetConditions = new List<string>();
    private List<String> m_skillAttributes = new List<string>();

    public SkillStatus m_skillStatus1;
    public SkillStatus m_skillStatus2;

    public SkillEffect m_skillEffect = new SkillEffect();


    public bool ValidForTarget(BaseActor user, BaseActor target, Point targetPos)
    {
        // if there are any special rules...
        if (m_targetConditions.Count > 0)
        {
            bool valid = false;
            foreach (String condition in m_targetConditions)
            {
                valid |= CheckIndividualSkill(condition, user, target, targetPos);
            }
            return valid;
        }
        else
        {
            // standard rules
            return (user.TeamName != target.TeamName);
        }
    }

    public bool CausesStatus(String condition, out SkillStatus skillStatus)
    {
        if ("Status Only" == m_skillEffect.Name)
        {
            if (m_skillStatus1 != null && m_skillStatus1.skillStatusCondition.Contains(condition))
            {
                skillStatus = m_skillStatus1;
                return GladiusGlobals.Random100() < m_skillStatus1.statusVal1;
            }
            if (m_skillStatus2 != null && m_skillStatus2.skillStatusCondition.Contains(condition))
            {
                skillStatus = m_skillStatus2;
                return GladiusGlobals.Random100() < m_skillStatus2.statusVal1;
            }
        }
        skillStatus = null;
        return false;
    }


    private bool CheckIndividualSkill(String condition, BaseActor user, BaseActor target, Point targetPoint)
    {
        if ("any_square".Equals(condition))
        {
            return true;
        }
        if ("enemy_only".Equals(condition))
        {
            return target.TeamName != user.TeamName;
        }
        if ("self_only".Equals(condition))
        {
            return user == target;
        }
        if ("friend_only_not_self".Equals(condition))
        {
            return user != target && user.TeamName == target.TeamName;
        }
        if ("friend_only_not_self".Equals(condition))
        {
            return user != target && user.TeamName == target.TeamName;
        }
        if ("all_units_not_self".Equals(condition))
        {
            return user != target;
        }

        if ("empty_square".Equals(condition))
        {
        }



        if (condition.StartsWith("unitattr_req"))
        {
        }
        if (condition.StartsWith("unitattr_ign"))
        {
        }

        if (condition.StartsWith("target_has"))
        {
        }


        return false;
    }

    public void AddTargetCondition(String condition)
    {
        m_targetConditions.Add(condition);
    }

    public void AddAttribute(String attribute)
    {
        m_skillAttributes.Add(attribute);
    }

    public bool Available(BaseActor actor)
    {
        if (Name == "None")
        {
            return false;
        }

        // need to make sure actor has enough skillpoints or affinity points to use this.
        if (UseCost <= actor.ArenaSkillPoints)
        {
            return true;
        }
        return false;
    }

    public int SkillMoveRange
    {
        get
        {
            int val = 0;
            if (IsMoveToAttack)
            {
                val = m_defaultMoveToAttackRange;
                if (MoveToAttackModifier < 0)
                {
                    val = 0;
                }
                else
                {
                    val *= (1 + MoveToAttackModifier);
                }
            }
            return val;
        }
    }

    public bool InRange(int val)
    {
        return val < TotalSkillRange;
    }

    public int TotalSkillRange
    {
        get { return (SkillMoveRange + SkillRange); }
    }

    public SkillIcon SkillIcon
    {
        get { return SkillIcon.Attack; }
    }


    public void Initialise()
    {
        SetupSkillRow();
        if (m_skillStatus1 != null)
        {
            m_skillStatus1.Initialise();
        }
        if (m_skillStatus2 != null)
        {
            m_skillStatus2.Initialise();
        }
    }

    // "Move", "Attack", "Combo", "Affinity", "Special"
    private void SetupSkillRow()
    {
        if (SubSkill != null)
        {
            m_skillRow = 2;
        }
        else if (m_skillAttributes.Contains("affinity"))
        {
            m_skillRow = 3;
        }
        else if (SubType == "Support")
        {
            m_skillRow = 4;
        }
        else if (Type == "Attack" && SubType == "Standard")
        {
            m_skillRow = 0;
        }
        else
        {
            m_skillRow = -1;
        }
    }


    private int m_skillRow;
    public int SkillRow
    {
        get
        {
            return m_skillRow;
        }

    }

    public String GetPreRequisite()
    {
        return PreRequisite;
    }

    public int UseCost
    {
        get { return (int)SkillCosts2 / 10; }
    }

    public float DamageMultiplier
    {
        get { return 1.0f; }
    }

    public bool IsMoveToAttack
    {
        get { return m_skillAttributes.Contains("movetoattack"); }
    }

    public bool IsMultiHit
    {
        get { return m_skillAttributes.Contains("multihit"); }
    }

    public bool CantMiss
    {
        get { return m_skillAttributes.Contains("cantmiss"); }
    }

    public bool FaceOnAttack
    {
        get { return !m_skillAttributes.Contains("dontface"); }
    }

    public bool IsAffinity
    {
        get { return m_skillAttributes.Contains("affinity"); }
    }

    public bool IsBite
    {
        get { return m_skillAttributes.Contains("bite"); }
    }

    public bool IsCharge
    {
        get { return m_skillAttributes.Contains("charge"); }
    }

    public bool IsMelee
    {
        get { return m_skillAttributes.Contains("melee"); }
    }

    public bool IsOkWithNoTargets
    {
        get { return m_skillAttributes.Contains("okwithnotargets"); }
    }

    public bool IsPiercing
    {
        get { return m_skillAttributes.Contains("piercing"); }
    }

    public bool IsRanged
    {
        get { return m_skillAttributes.Contains("ranged"); }
    }

    public bool IsShield
    {
        get { return m_skillAttributes.Contains("shield"); }
    }

    public bool IsSpearAnim
    {
        get { return m_skillAttributes.Contains("spearanim"); }
    }

    public bool IsSpell
    {
        get { return m_skillAttributes.Contains("spell"); }
    }

    public bool IsSuicide
    {
        get { return m_skillAttributes.Contains("suicide"); }
    }

    public bool IsTeleport
    {
        get { return m_skillAttributes.Contains("teleport"); }
    }

    public bool IsWeapon
    {
        get { return m_skillAttributes.Contains("weapon"); }
    }

    public bool IsImmuneTo(String immunType)
    {
        bool result = false;
        if (m_skillStatus1 != null)
        {
            result = m_skillStatus1.ImmuneTo(immunType);
        }
        if (result == false && m_skillStatus2 != null)
        {
            result = m_skillStatus2.ImmuneTo(immunType);
        }
        return result;
    }


    public static SkillClass ForActorClass(ActorClass actorClass)
    {
        switch (actorClass)
        {
            case ActorClass.AMAZON:
                return SkillClass.AMAZON;
            case ActorClass.ARCHER:
            case ActorClass.ARCHERF:
                return SkillClass.ARCHER;
            case ActorClass.BANDITA:
            case ActorClass.BANDITAF:
            case ActorClass.BANDITB:
                return SkillClass.BANDIT;
            case ActorClass.BARBARIAN:
            case ActorClass.BARBARIANF:
                return SkillClass.BARBARIAN;
            case ActorClass.BEAR:
                return SkillClass.BEAR;
            case ActorClass.BEARGREATER:
                return SkillClass.BEARGREATER;
            case ActorClass.BERSERKER:
            case ActorClass.BERSERKERF:
                return SkillClass.BERSERKER;
            case ActorClass.CAT:
                return SkillClass.CAT;
            case ActorClass.CATGREATER:
                return SkillClass.CATGREATER;
            case ActorClass.CENTURION:
            case ActorClass.CENTURIONF:
                return SkillClass.CENTURION;
            case ActorClass.CHANNELERIMP:
                return SkillClass.CHANNELER;
            case ActorClass.CYCLOPS:
                return SkillClass.CYCLOPS;
            case ActorClass.CYCLOPSGREATER:
                return SkillClass.CYCLOPSGREATER;
            case ActorClass.DARKLEGIONNAIRE:
                return SkillClass.DARKLEGIONNAIRE;
            case ActorClass.DERVISH:
            case ActorClass.DERVISHF:
                return SkillClass.DERVISH;
            case ActorClass.DUMMY:
                return SkillClass.DUMMY;
            case ActorClass.EIJI:
                return SkillClass.EIJI;
            case ActorClass.GALDR:
                return SkillClass.GALDR;
            case ActorClass.GALVERG:
                return SkillClass.GALVERG;
            case ActorClass.GUNGNIR:
            case ActorClass.GUNGNIRF:
                return SkillClass.GUNGNIR;
            case ActorClass.GWAZI:
                return SkillClass.GWAZI;
            case ActorClass.LEGIONNAIRE:
            case ActorClass.LEGIONNAIREF:
                return SkillClass.LEGIONNAIRE;
            case ActorClass.LUDO:
                return SkillClass.LUDO;
            case ActorClass.MINOTAUR:
                return SkillClass.MINOTAUR;
            case ActorClass.MONGREL:
                return SkillClass.MONGREL;
            case ActorClass.MONGRELSHAMAN:
                return SkillClass.MONGRELSHAMAN;
            case ActorClass.MURMILLO:
            case ActorClass.MURMILLOF:
                return SkillClass.MURMILLO;
            case ActorClass.MUTUUS:
                return SkillClass.MUTUUS;
            case ActorClass.OGRE:
                return SkillClass.OGRE;
            case ActorClass.PELTAST:
            case ActorClass.PELTASTF:
                return SkillClass.PELTAST;
            case ActorClass.SAMNITEEXP:
            case ActorClass.SAMNITEEXPF:
            case ActorClass.SAMNITEIMP:
            case ActorClass.SAMNITEIMPF:
            case ActorClass.SAMNITESTE:
            case ActorClass.SAMNITESTEF:
                return SkillClass.SAMNITE;
            case ActorClass.SATYR:
                return SkillClass.SATYR;
            case ActorClass.URLAN:
                return SkillClass.URLAN;
            case ActorClass.URSULA:
                return SkillClass.URSULA;
            case ActorClass.VALENS:
                return SkillClass.VALENS;
            case ActorClass.SCARAB:
                return SkillClass.SCARAB;
            case ActorClass.SCORPION:
                return SkillClass.SCORPION;
            case ActorClass.SECUTORIMP:
            case ActorClass.SECUTORIMPF:
            case ActorClass.SECUTORSTE:
            case ActorClass.SECUTORSTEF:
                return SkillClass.SECUTOR;
            case ActorClass.SUMMONER:
                return SkillClass.SUMMONER;
            case ActorClass.UNDEADCASTERA:
                return SkillClass.UNDEADCASTER;
            case ActorClass.UNDEADMELEEIMPA:
                return SkillClass.UNDEADMELEE;
            case ActorClass.URSULACOSTUMEA:
            case ActorClass.URSULACOSTUMEB:
                return SkillClass.URSULA;
            case ActorClass.VALENSCOSTUMEA:
            case ActorClass.VALENSCOSTUMEB:
                return SkillClass.VALENS;
            case ActorClass.WOLF:
                return SkillClass.WOLF;
            case ActorClass.WOLFGREATER:
                return SkillClass.WOLFGREATER;
            case ActorClass.YETI:
                return SkillClass.YETI;
            default:
                return SkillClass.NONE;
        }
    }

}

public static class AttackSkillDictionary
{
    static AttackSkillDictionary()
    {
        TextAsset textAsset = (TextAsset)Resources.Load(GladiusGlobals.DataRoot + "SkillData");
        String data = textAsset.text;
        Parse(data);
        AssignSubSkills();

        AttackSkill noneSkill = new AttackSkill();
        noneSkill.Name = "None";
        Data[noneSkill.Name] = noneSkill;

        foreach (AttackSkill attackSkill in Data.Values)
        {
            attackSkill.Initialise();
        }

    }

    static HashSet<String> skillTypeSet = new HashSet<String>();
    static HashSet<String> skillTypeSet2 = new HashSet<String>();

    // move/strike
    // attack
    // combo
    // special
    // affinty


    static private void AssignSubSkills()
    {
        foreach (AttackSkill attackSkill in Data.Values)
        {
            if ("DarkLegionnaire Combo I" == attackSkill.Name)
            {
                int ibreak = 0;
            }

            if (attackSkill.SubSkillName != null)
            {
                if (Data.ContainsKey(attackSkill.SubSkillName))
                {
                    AttackSkill childSkill = Data[attackSkill.SubSkillName];
                    attackSkill.SubSkill = childSkill;
                    int ibreak = 0;
                }
            }
        }
    }


    static public void Parse(String data)
    {
        AttackSkill currentSkill = null;
        String[] lines = data.Split('\n');
        char[] splitTokens = new char[] { ',', ':' };
        for (int counter = 0; counter < lines.Length; counter++)
        {
            String line = lines[counter];
            line = line.Trim();

            if (!line.StartsWith("SKILL"))
            {
                continue;
            }

            String[] lineTokens = GladiusGlobals.SplitAndTidyString(line, splitTokens);


            if (line.StartsWith("SKILLCREATE:"))
            {
                //if(currentSkill != null)
                //{
                //	currentSkill.Initialise();
                //}
                currentSkill = new AttackSkill();
                if (lineTokens.Length == 4)
                {
                    currentSkill.Name = lineTokens[1];
                    if (currentSkill.Name == "Pass")
                    {
                        int ibreak1 = 0;
                    }

                    currentSkill.Type = lineTokens[2];
                    skillTypeSet.Add(currentSkill.Type);

                    currentSkill.SubType = lineTokens[3];
                    skillTypeSet2.Add(currentSkill.SubType);

                    Data[currentSkill.Name] = currentSkill;
                }

            }
            else if (line.StartsWith("SKILLUSECLASS:"))
            {
                SkillClass skillClass = (SkillClass)Enum.Parse(typeof(SkillClass), lineTokens[1].ToUpper(), true);
                currentSkill.UseClass = skillClass;
                
            }
            else if (line.StartsWith("SKILLDISPLAYNAMEID:"))
            {
                currentSkill.DisplayNameId = int.Parse(lineTokens[1]);
            }
            else if (line.StartsWith("SKILLDESCRIPTIONID:"))
            {
                currentSkill.DescriptionId = lineTokens[1];
            }
            else if (line.StartsWith("SKILLLEVEL:"))
            {
                currentSkill.SkillLevel = int.Parse(lineTokens[1]);
            }
            else if (line.StartsWith("SKILLJOBPOINTCOST:"))
            {
                currentSkill.JobPointCost = int.Parse(lineTokens[1]);
            }
            else if (line.StartsWith("SKILLCOSTS:"))
            {
                currentSkill.SkillCosts1 = float.Parse(lineTokens[1]);
                currentSkill.SkillCosts2 = float.Parse(lineTokens[2]);
            }
            else if (line.StartsWith("SKILLATTRIBUTE:"))
            {
                currentSkill.AddAttribute(lineTokens[1]);
            }
            else if (line.StartsWith("SKILLCOMBATMODS:"))
            {
                currentSkill.CombatMod1 = int.Parse(lineTokens[1]);
                currentSkill.CombatMod2 = float.Parse(lineTokens[2]);
            }
            else if (line.StartsWith("SKILLSUBSKILL:"))
            {
                currentSkill.SubSkillName = lineTokens[1];
            }

            else if (line.StartsWith("SKILLRANGE:"))
            {
                currentSkill.SkillRange = int.Parse(lineTokens[1]);
                currentSkill.SkillRangeName = lineTokens[2];
            }
            else if (line.StartsWith("SKILLEXCLUDERANGE:"))
            {
                currentSkill.SkillExcludeRange = int.Parse(lineTokens[1]);
                currentSkill.SkillExcludeRangeName = lineTokens[2];

            }
            else if (line.StartsWith("SKILLAFFCOST:"))
            {
                currentSkill.AffinityCost = int.Parse(lineTokens[1]);
            }
            else if (line.StartsWith("SKILLMOVETOATTACKMOD:"))
            {
                currentSkill.MoveToAttackModifier = int.Parse(lineTokens[1]);
            }
            else if (line.StartsWith("SKILLMETER:"))
            {
                currentSkill.MeterName = lineTokens[1];
            }
            else if (line.StartsWith("SKILLANIMSPEED:"))
            {

            }
            else if (line.StartsWith("SKILLFXSWING:"))
            {

            }
            else if (line.StartsWith("SKILLPREREQ:"))
            {
                currentSkill.PreRequisite = lineTokens[1];
            }
            else if (line.StartsWith("SKILLEFFECTRANGE:"))
            {
                currentSkill.m_skillEffect.Range = int.Parse(lineTokens[1]);
                //currentSkill.m_skillEffect.EffectRangeType = (SkillEffectRangeType)Enum.Parse(typeof(SkillEffectRangeType),lineTokens[2]);
            }
            else if (line.StartsWith("SKILLEFFECT:"))
            {
                currentSkill.m_skillEffect.Name = lineTokens[1];
                currentSkill.m_skillEffect.Modifier1 = float.Parse(lineTokens[2]);
                currentSkill.m_skillEffect.Modifier2 = float.Parse(lineTokens[3]);
            }
            else if (line.StartsWith("SKILLEFFECTCONDITION:"))
            {
                currentSkill.m_skillEffect.SkillEffectCondition = lineTokens[1];
            }
            else if (line.Contains("SKILLSTATUS"))
            {
                HandleSkillStatus(currentSkill, lineTokens[0], lineTokens);
            }
            else if (line.StartsWith("SKILLANIM:"))
            {
                // can't use these directly.
            }
            else if (line.StartsWith("SKILLFX:"))
            {

            }
            else if (line.StartsWith("SKILLPROJECTILE:"))
            {

            }
            else if (line.StartsWith("SKILLPROJECTILESEQUENCE:"))
            {

            }
            else if (line.StartsWith("SKILLFXPROJECTILE:"))
            {

            }
            else if (line.StartsWith("SKILLAFFINITY:"))
            {
                currentSkill.Affinity = lineTokens[1];
            }
            else if (line.StartsWith("SKILLTARGETCONDITION:"))
            {
                currentSkill.AddTargetCondition(lineTokens[1]);

            }

            else
            {

                //err
                //Debug.LogError("Unknown token : "+line);
            }
        }
        int ibreak = 0;
    }

    static public void HandleSkillStatus(AttackSkill skill, String attrName, String[] lineTokens)
    {
        SkillStatus status = null;
        if (attrName.EndsWith("2:"))
        {
            if (skill.m_skillStatus2 == null)
            {
                skill.m_skillStatus2 = new SkillStatus();
            }
            status = skill.m_skillStatus2;
        }
        else
        {
            if (skill.m_skillStatus1 == null)
            {
                skill.m_skillStatus1 = new SkillStatus();
            }
            status = skill.m_skillStatus1;
        }


        if (attrName.Contains("SKILLSTATUSDURATION"))
        {
            status.statusDurationType = lineTokens[1];
            status.statusDuration = float.Parse(lineTokens[2]);
        }
        else if (attrName.Contains("SKILLSTATUSTARGET"))
        {
            status.statusTargetName = lineTokens[1];
            status.statusTargetRange = int.Parse(lineTokens[2]);
            status.statusTargetShape = lineTokens[3];


        }
        else if (attrName.Contains("SKILLSTATUSCONDITION"))
        {
            status.skillStatusCondition.Add(lineTokens[1]);

        }
        else if (attrName.Contains("SKILLSTATUSINTERVAL"))
        {
            status.statusIntervalName = lineTokens[1];
            status.statusIntervalDuration = int.Parse(lineTokens[2]);

        }
        else if (attrName.Contains("SKILLSTATUSCHANCE"))
        {
            status.statusChance = int.Parse(lineTokens[1]);
        }
        else if (attrName.Contains("STATUSSITUATIONUNITCONDITION"))
        {
            //status.skillStatusSituationUnitCondition.Add(lineTokens[1]);
        }
        else if (attrName.Contains("STATUSSITUATIONAFFINITYCONDITION"))
        {
            //status.skillStatusSituationUnitCondition.Add(lineTokens[1]);
        }
        else if (attrName.Contains("SKILLSTATUSSITUATIONSKILLCONDITION"))
        {
            //status.skillStatusSituationUnitCondition.Add(lineTokens[1]);
        }
        else if (attrName.Contains("SKILLSTATUSSITUATIONSTATUSCONDITION"))
        {
        }
        else if (attrName.Contains("SKILLSTATUSUSELIMIT"))
        {
        }
        else if (attrName.Contains("SKILLSTATUS"))
        {
            // FIXME - something needs to happen to parse the status names as they contrain rules/modifications
            status.statusName = lineTokens[1];
            status.statusVal1 = float.Parse(lineTokens[2]);
            status.statusVal2 = float.Parse(lineTokens[3]);
        }
        else
        {
            int ibreak = 0;
        }

    }

    public static Dictionary<String, AttackSkill> Data = new Dictionary<String, AttackSkill>();
}


public class SkillEffect
{
    public String Name;
    public float Modifier1;
    public float Modifier2;
    public int Range;
    public String SkillEffectRangeName;
    public String SkillEffectCondition;
    public SkillEffectRangeType EffectRangeType;
}

public class SkillStatus
{
    public String statusName;
    public float statusVal1;
    public float statusVal2;
    public String statusDurationType;
    public float statusDuration;
    public String statusTargetName;
    public int statusTargetRange;
    public String statusTargetShape;
    public List<String> skillStatusCondition = new List<String>();
    public List<String> skillStatusSituationUnitCondition = new List<String>();

    public String statusSituationAffinityCondition;
    public int statusChance;
    public String statusIntervalName;
    public int statusIntervalDuration;


    public float AccuracyBonus
    {
        get
        {
            float val = 0.0f;
            if (statusName == "change accuracy")
            {
                val = statusVal2;
            }
            return val;
        }
    }

    public float DefenseBonus
    {
        get
        {
            float val = 0.0f;
            if (statusName == "change defense")
            {
                val = statusVal2;
            }
            return val;
        }
    }

    //public float AccuracyBonus
    //{
    //    get
    //    {
    //        float val = 0.0f;
    //        if (statusName == "change initiative percent")
    //        {
    //            val = statusVal2;
    //        }
    //        else if (statusName == "change initiative")
    //        {
    //            val = statusVal2;
    //        }

    //        return val;
    //    }
    //}

    public float MoveRateBonus
    {
        get
        {
            float val = 0.0f;
            if (statusName == "change moverate percent")
            {
                val = statusVal2;
            }
            else if (statusName == "change moverate")
            {
                val = statusVal2;
            }

            return val;
        }
    }


    public bool CombatDefend;


    //public bool IsImmunityStatus()
    //{
    //    return "immunity status" == statusName;
    //}

    public bool ImmuneTo(String condition)
    {
        return "immunity status" == statusName && skillStatusCondition.Contains(condition);
    }

    public void Initialise()
    {

    }

}

public class Resistance
{
    String type;
    float value;
}
