using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class AttackSkill
{
    public const float NoModifierValue = -1000f;

    // chear to get name at top
    public string AName
    { get { return Name; } }

    public string Name;
    public string SkillType;
    public string Classificiation;
    public String UseClassString;

    public int Level;
    public int JobPointCost;
    public float Costs1;
    public float Costs2;
    public int AffinityCost;

    public int DisplayNameId = -1;
    public int DescriptionId = -1;

    public float CombatAccuracyMod = 0;
    public float CombatDamageMultiplierMod = 1.0f;

    public string Affinity;

    public string Anim;
    public string AnimDefend;
    public string AnimCharge;
    public string AnimMove;
    public string AnimLoop;
    public string AnimLow;

    public float AnimSpeed;
    public float AnimTimeStart;

    public string ComboButton;

    public float DistanceDelay1;
    public float DistanceDelay2;

    public string EffectFX;

    public int EffectRange;
    public string EffectRangeName;

    public int ExcludeRange;
    public string ExcludeRangeName;

    public string FX;
    public string FXCTAG;
    public string FXMove;
    public string FXProjectile;
    public string FXProjectileImpact;
    public string FXSwing;

    public string FXProjectileSequenceVal1;
    public string FXProjectileSequenceVal2;
    public string FXProjectileSequenceVal3;
    public int FXProjectileSequenceVal4;

    public string MeterName;
    public float MeterVal1;
    public float ComboLength;

    public int MoveRange;
    public string MoveRangePattern;

    public int MoveToAttackMod = 1;

    public string MultiHitData;

    public string PreReqName;
    public AttackSkill PreReqSkill;

    public string ProjectileWeapon;
    public string ProjectileAxis;
    public float ProjectileSpeed;

    public int ProjectileRotationVal1;
    public int ProjectileRotationVal2;
    public int ProjectileRotationVal3;
    public int ProjectileRotationVal4;

    public string ProjectileSequenceVal1;
    public string ProjectileSequenceVal2;
    public string ProjectileSequenceVal3;
    public int ProjectileSequenceVal4;

    public string ProxyVal1;
    public string ProxyVal2;

    public int Range;
    public string RangeName;

    public string Replaces;

    public SkillShiftData SkillShiftData;

    public string Sound;

    public string SubSkillName;
    public AttackSkill SubSkill;

    public string SummonDataDescription;
    public string SummonDataType;
    public int SummonDataVal;

    public string WeaponReq;

    public DamageType DamageType = DamageType.Physical;


    public List<string> AttributesList = new List<string>();
    public List<string> TargetConditionsList = new List<string>();
    public List<string> EffectStatusConditionList = new List<string>();

    public List<string> EffectSkillConditionList = new List<string>();
    public List<string> MoveRangeConditionList = new List<string>();
    public List<string> ProjectileAttributesList = new List<string>();
    public List<string> SkilllFreeList = new List<string>();
    public List<string> UsabilityConditionList = new List<string>();

    public List<AttackSkillStatus> StatusList = new List<AttackSkillStatus>();
    public List<SkillEffect> EffectList = new List<SkillEffect>();
    public List<SkillSplitEffect> SplitEffectList = new List<SkillSplitEffect>();

    public String DisplayName
    {
        get
        {
            if (DisplayNameId > 0)
            {
                return LocalisationData.GetValue(DisplayNameId);
            }
            return "No Name";
        }
    }

    public String DisplayDescription
    {
        get
        {
            if (DescriptionId > 0)
            {
                return LocalisationData.GetValue(DescriptionId);
            }
            return "No Description";
        }
    }

    public bool IsAttack
    {
        get { return "Attack" == SkillType; }
    }

    public bool IsInnate
    {
        get { return "Innate" == SkillType; }
    }

    public bool IsSupport
    {
        get{ return "Support" == SkillType;}
    }

    public bool IsStatus
    {
        get { return "Status" == Classificiation; }
    }


    public bool IsCommand
    {
        get{ return "Command" == SkillType;}
    }


    public bool IsDefense
    {
        get { return "Defend" == Classificiation; }
    }

    public bool IsHitReturn
    {
        get { return "Hit Return" == Classificiation; }
    }

    public bool IsMissReturn
    {
        get { return "Miss Return" == Classificiation; }
    }

    public bool IsBreakout
    {
        get { return "Breakout" == Classificiation; }
    }


    public bool IsProjectile
    {
        get { return !String.IsNullOrEmpty(ProjectileWeapon); }
    }


    public bool IsCounterAttack
    {
        get
        {
            foreach (SkillEffect se in EffectList)
            {
                if (se.Effects != null && (se.Effects.Contains("Counterattack")))
                {
                    return true;
                }
            }
            return false;
        }
    }


    public bool HasConditionalActivate
    {
        get
        {
            return StatusList.Exists(x => (x.Description == "conditional activate"));
        }
    }

    public bool IsTargetIgnoreFriendly
    {
        get
        {
            return IsAttack;
        }
    }

    public bool IsTargetIgnoreSelf
    {
        get
        {
            return IsAttack;
        }
    }

    public bool IsTargetIgnoreEmptyGrid
    {
        get
        {
            return IsAttack;
        }
    }


    public bool IsEffectIgnoreFriendly
    {
        get
        {
            return IsAttack;
        }
    }

    public bool IsEffectIgnoreSelf
    {
        get
        {
            return IsAttack;
        }
    }

    public bool IsEffectIgnoreEmptyGrid
    {
        get
        {
            return IsAttack || IsSupport;
        }
    }

    public bool CenteredOnUser
    {
        get 
        {
            return RangeName == "Self" && Range == 0;
            //SKILLRANGE: 0, "Self"
        }
    }


    public bool IsCombo
    {
        //get { return SubSkill != null; }
        get { return MeterName != null && MeterName.StartsWith("Chain"); }
    }

    public bool IsNonInterface
    {
        get { return AttributesList.Contains("noninterface"); }
    }
    public bool IsMoveToAttack
    {
        get { return AttributesList.Contains("movetoattack"); }
    }

    public bool IsMove
    { get { return IsMoveToAttack || IsTeleport; } }


    public bool IsMultiHit
    {
        get { return AttributesList.Contains("multihit"); }
    }

    public bool CantMiss
    {
        get { return AttributesList.Contains("cantmiss"); }
    }

    public bool FaceOnAttack
    {
        get { return !AttributesList.Contains("dontface"); }
    }

    public bool DoesntFaceOnDefend
    {
        get { return AttributesList.Contains("doesntfaceondefend"); }
    }


    public bool IsAffinity
    {
        get { return AttributesList.Contains("affinity"); }
    }

    public bool IsBite
    {
        get { return AttributesList.Contains("bite"); }
    }

    public bool IsCharge
    {
        get { return AttributesList.Contains("charge"); }
    }

    public bool IsMelee
    {
        get { return AttributesList.Contains("melee"); }
    }

    public bool IsOkWithNoTargets
    {
        get { return AttributesList.Contains("okwithnotargets"); }
    }

    public bool IsPiercing
    {
        get { return AttributesList.Contains("piercing"); }
    }

    public bool IsRanged
    {
        get { return AttributesList.Contains("ranged"); }
    }

    public bool IsShield
    {
        get { return AttributesList.Contains("shield"); }
    }

    public bool IsSpearAnim
    {
        get { return AttributesList.Contains("spearanim"); }
    }

    public bool IsSpell
    {
        get { return AttributesList.Contains("spell"); }
    }

    public bool IsSuicide
    {
        get { return AttributesList.Contains("suicide"); }
    }

    public bool IsTeleport
    {
        get { return AttributesList.Contains("teleport"); }
    }

    public bool IsWeapon
    {
        get { return AttributesList.Contains("weapon"); }
    }

    public bool IsShift
    {
        get
        {
            return IsShiftInto || IsShiftAway;
        }
    }

    public bool IsShiftInto
    {
        get
        {
            if(Name.Contains("ShiftReturn"))
            {
                return false;
            }
            return String.Equals("changeforminto", Anim);
        }
    }


    public bool IsShiftAway
    {
        get
        {
            if (Name.Contains("ShiftReturn"))
            {
                return true;
            }
            return false;
            //return String.Equals("changeFormAway", AnimDefend);
        }
    }

    public bool IsImmuneTo(String immunType)
    {
        bool result = false;
        //foreach (AttackSkillStatus status in StatusList)
        //{
        //    if (status.ImmuneTo(immunType))
        //    {
        //        result = true;
        //        break;
        //    }
        //}
        return result;
    }

    //public bool CausesStatuses
    //{
    //    foreach (AttackSkillStatus status in StatusList)
        
    //}


    public bool CausesStatus(String condition, out AttackSkillStatus skillStatus)
    {
        if ("Status Only" == Name)
        {
            foreach (AttackSkillStatus status in StatusList)
            {
                //if (status.CausesStatus(condition))
                //{
                //    skillStatus = status;
                //    return GladiusGlobals.Random100() < status.Chance;
                //}
            }
        }
        skillStatus = null;
        return false;
    }

    public bool RemovesStatus(String condition, out AttackSkillStatus skillStatus)
    {
        if ("Removes Status" == Name)
        {
            foreach (AttackSkillStatus status in StatusList)
            {
                ///if (status.CausesStatus(condition))
                //{
                //    skillStatus = status;
                //    return GladiusGlobals.Random100() < status.Chance;
                //}
            }
        }
        skillStatus = null;
        return false;
    }


    public bool CausesKnockback
    {
        get
        {
            return EffectList.Find(x => x.Effects == "Knockback") != null;
        }
    }


    public bool CausesKnockdown
    {
        get
        {
            return EffectList.Find(x => x.Effects == "Knockdown") != null;
        }
    }



    public int UseCost
    {
        get { return (int)Costs2 / 10; }
    }

    // "Move", "Attack", "Combo", "Affinity", "Special"
    public void SetupSkillData()
    {
        if (IsMove || IsCommand)
        {
            m_skillRow = 0;
        }
        else if (IsCombo)
        {
            m_skillRow = 2;
        }
        else if (IsAffinity)
        {
            m_skillRow = 3;
        }
        else if (IsAttack)
        {
            m_skillRow = 1;
        }
        else if(IsSupport)
        {
            m_skillRow = 4;
        }
        else
        {
            m_skillRow = -1;
        }
        
        //foreach (AttackSkillStatus status in StatusList)
        //{
        //    status.BuildSkillStatModifier(this);
        //}

        if(!String.IsNullOrWhiteSpace(PreReqName))
        {
            AttackSkillDictionary.Data.TryGetValue(PreReqName, out PreReqSkill);
        }

        if (!String.IsNullOrWhiteSpace(SubSkillName))
        {
            AttackSkillDictionary.Data.TryGetValue(SubSkillName, out SubSkill);
        }
    }


    public int SkillRow
    {
        get
        {
            return m_skillRow;
        }
    }

    public AttackSkillStatus GetSkillStatus(string status)
    {
        foreach (AttackSkillStatus skillStatus in StatusList)
        {
            if (skillStatus.Description == status)
            {
                return skillStatus;
            }
        }
        return null;
    }

    public int m_skillRow;


    //public bool TargetsUnit(ArenaActor attacker,ArenaActor target)
    //{
    //    if(CausesKnockback)
    //    {
    //        int distance = EffectRange;
    //        if(!CombatEngine.UnitKnockbackValid(attacker,distance,target))
    //        {
    //            return false;
    //        }
    //    }

    //    if(IsOverrun)
    //    {
    //        if (target.CharacterData.ActorClassDef.IsNoKnockBack)
    //        {
    //            return false;
    //        }
    //    }

    //    if (IsBreakout || AttributesList.Contains("translateanim"))
    //    {
    //        // check map flatness??

    //    }

    //    return ValidTargetConditions(attacker, target,target.ArenaPoint,null);
        
    //}



    public bool IsOverrun
    {
        get { return Name == "Samnite Overrun"; }
    }

    public bool IsOneTime
    {
        get { return Classificiation == "One Time"; }
    }



    public bool IsUsable(CharacterData characterData)
    {
        if(!String.IsNullOrEmpty(WeaponReq))
        {
            Item item = characterData.GetItemAtLocation(ItemLocation.Weapon);
            if(item != null)
            {
                if(!String.Equals(item.ItemSubType,WeaponReq,StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }
            }
        }

        if (string.Equals(characterData.CurrentClassDef.skillUse,UseClassString,StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }
        return false;
    }

    public bool IsPurchasable
    {
        get
        {
            //return (Classificiation == "Standard" || IsSupport);
            return JobPointCost > 0;
        }

    }
}


public class AttackSkillStatus
{
    public AttackSkill Owner;
    public string Description;
    public int Val1;
    public float ModifierValue;
    public int Chance;

    public string DurationDescription;
    public float Duration;

    public string IntervalDescription;
    public int Interval;

    public string TargetType;
    public int TargetVal;
    public string TargetRestriction;

    public int UseLimit;

    public List<string> ConditionList = new List<string>();
    public List<string> SituationAffinityConditionList = new List<string>();
    public List<string> SituationStatusConditionList = new List<string>();
    public List<string> SituationUnitConditionList = new List<string>();
    public List<string> SituationSkillConditionList = new List<string>();
    

}

public class SkillEffect
{
    public string Effects;
    public float Chance;
    public float Val2;

    public List<string> ConditionList = new List<string>();
    public List<string> StatusConditionList = new List<string>();

}

public class SkillSplitEffect
{
   public List<string> ConditionList = new List<string>();
}


public class SkillShiftData
{
    public string ShiftClass;
    public string Appearance;
    public int ConstitutionAdd;
    public int PowerAdd;
    public int AccuracyAdd;
    public int DefenseAdd;
    public int InitiativeAdd;
    public int MovementAdd;

}



public static class AttackSkillDictionary
{
    static AttackSkillDictionary()
    {
        String path = GladiusGlobals.DataRoot + "SkillData";

        TextAsset ta = Resources.Load<TextAsset>(path);
        if (ta != null)
        {
            var lexer = new AttackSkillLexer(new Antlr4.Runtime.AntlrInputStream(ta.text));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new AttackSkillParser(commonTokenStream);
            IParseTree parseTree = parser.root();
            MyAttackSkillParser listener = new MyAttackSkillParser(Data);
            ParseTreeWalker.Default.Walk(listener, parseTree);
            foreach (AttackSkill skill in Data.Values)
            {
                skill.SetupSkillData();

            }
        }
    }

    public static AttackSkill PassSkill
    {
        get { return Data["Pass"]; }
    }

    public static Dictionary<String, AttackSkill> Data = new Dictionary<String, AttackSkill>();

}


public class MyAttackSkillParser : AttackSkillBaseListener
{
    public MyAttackSkillParser(Dictionary<String, AttackSkill> dictionary)
    {
        m_dictionary = dictionary;
    }

    public String GetStringVal(ITerminalNode node)
    {
        return node.GetText().Replace("\"", "");
    }

    public int GetIntVal(ITerminalNode node)
    {
        int result = 0;
        if (!int.TryParse(node.GetText(), out result))
        {
            String text = node.GetText();
            int ibreak = 0;
        }
        return result;
    }

    public float GetFloatVal(ITerminalNode node)
    {
        return float.Parse(node.GetText());
    }

    public override void EnterCreateDef(AttackSkillParser.CreateDefContext context)
    {
        current = new AttackSkill();
        current.Name = GetStringVal(context.STRING(0));
        current.SkillType = GetStringVal(context.STRING(1));
        current.Classificiation = GetStringVal(context.STRING(2));

        m_dictionary[current.Name] = current;

        base.EnterCreateDef(context);
    }

    public override void EnterUseClass(AttackSkillParser.UseClassContext context)
    {
        current.UseClassString = GetStringVal(context.STRING());
        base.EnterUseClass(context);
    }

    public override void EnterAttribute(AttackSkillParser.AttributeContext context)
    {
        current.AttributesList.Add(GetStringVal(context.STRING()));
        base.EnterAttribute(context);
    }

    public override void EnterDisplayNameId(AttackSkillParser.DisplayNameIdContext context)
    {
        current.DisplayNameId = GetIntVal(context.FLOAT());
        base.EnterDisplayNameId(context);
    }

    public override void EnterDescriptionId(AttackSkillParser.DescriptionIdContext context)
    {
        current.DescriptionId = GetIntVal(context.FLOAT());
        base.EnterDescriptionId(context);
    }

    public override void EnterCombatMods(AttackSkillParser.CombatModsContext context)
    {
        current.CombatAccuracyMod = GetFloatVal(context.FLOAT(0));
        current.CombatDamageMultiplierMod = GetFloatVal(context.FLOAT(1));
        base.EnterCombatMods(context);
    }

    public override void EnterLevel(AttackSkillParser.LevelContext context)
    {
        current.Level = GetIntVal(context.FLOAT());
        base.EnterLevel(context);
    }

    public override void EnterJobPointCost(AttackSkillParser.JobPointCostContext context)
    {
        current.JobPointCost = GetIntVal(context.FLOAT());
        base.EnterJobPointCost(context);
    }

    public override void EnterCosts(AttackSkillParser.CostsContext context)
    {
        current.Costs1 = GetFloatVal(context.FLOAT(0));
        current.Costs2 = GetFloatVal(context.FLOAT(1));

        base.EnterCosts(context);
    }

    public override void EnterAffCost(AttackSkillParser.AffCostContext context)
    {
        current.AffinityCost = GetIntVal(context.FLOAT());
        base.EnterAffCost(context);
    }

    public override void EnterAffinity(AttackSkillParser.AffinityContext context)
    {
        current.Affinity = GetStringVal(context.STRING());
        base.EnterAffinity(context);
    }

    public override void EnterAnim(AttackSkillParser.AnimContext context)
    {
        current.Anim = GetStringVal(context.STRING()).ToLower();
        base.EnterAnim(context);
    }

    //public override void EnterAnimBlock(AttackSkillParser.AnimBlockContext context)
    //{
    //    current.AnimBlock = GetStringVal(context.STRING());
    //    base.EnterAnimBlock(context);
    //}

    public override void EnterAnimSpeed(AttackSkillParser.AnimSpeedContext context)
    {
        current.Anim = GetStringVal(context.STRING()).ToLower();
        current.AnimSpeed = GetFloatVal(context.FLOAT());
        base.EnterAnimSpeed(context);
    }

    public override void EnterAnimStartFrame(AttackSkillParser.AnimStartFrameContext context)
    {
        current.AnimTimeStart= (GetFloatVal(context.FLOAT()) / 30.0f);
        base.EnterAnimStartFrame(context);
    }

    public override void EnterAnimTime(AttackSkillParser.AnimTimeContext context)
    {
        current.Anim = GetStringVal(context.STRING()).ToLower();
        current.AnimSpeed = -(Math.Abs(GetFloatVal(context.FLOAT())));
        base.EnterAnimTime(context);
    }

    public override void EnterChargeAnim(AttackSkillParser.ChargeAnimContext context)
    {
        current.AnimCharge = GetStringVal(context.STRING()).ToLower();
        base.EnterChargeAnim(context);
    }

    public override void EnterComboButton(AttackSkillParser.ComboButtonContext context)
    {
        current.ComboButton = GetStringVal(context.STRING());
        base.EnterComboButton(context);
    }

    public override void EnterDefendAnim(AttackSkillParser.DefendAnimContext context)
    {
        current.AnimDefend = GetStringVal(context.STRING()).ToLower();
        base.EnterDefendAnim(context);
    }

    public override void EnterDistanceDelay(AttackSkillParser.DistanceDelayContext context)
    {
        current.DistanceDelay1 = GetFloatVal(context.FLOAT(0));
        current.DistanceDelay2 = GetFloatVal(context.FLOAT(1));

        base.EnterDistanceDelay(context);
    }

    public override void EnterEffect(AttackSkillParser.EffectContext context)
    {
        currentSkillEffect = new SkillEffect();
        currentSkillEffect.Effects = GetStringVal(context.STRING());
        currentSkillEffect.Chance = GetFloatVal(context.FLOAT(0));
        currentSkillEffect.Val2 = GetFloatVal(context.FLOAT(1));

        current.EffectList.Add(currentSkillEffect);

        base.EnterEffect(context);
    }

    public override void EnterEffectCondition(AttackSkillParser.EffectConditionContext context)
    {
        currentSkillEffect.ConditionList.Add(GetStringVal(context.STRING()));
        base.EnterEffectCondition(context);
    }

    public override void EnterEffectFX(AttackSkillParser.EffectFXContext context)
    {
        current.EffectFX = GetStringVal(context.STRING());
        base.EnterEffectFX(context);
    }

    public override void EnterEffectRange(AttackSkillParser.EffectRangeContext context)
    {
        current.EffectRange = GetIntVal(context.FLOAT());
        current.EffectRangeName = GetStringVal(context.STRING());
        base.EnterEffectRange(context);
    }

    public override void EnterEffectSkillCond(AttackSkillParser.EffectSkillCondContext context)
    {
        current.EffectSkillConditionList.Add(GetStringVal(context.STRING()));
        base.EnterEffectSkillCond(context);
    }

    public override void EnterEffectStatusCond(AttackSkillParser.EffectStatusCondContext context)
    {
        currentSkillEffect.StatusConditionList.Add(GetStringVal(context.STRING()));
        base.EnterEffectStatusCond(context);
    }

    public override void EnterExcludeRange(AttackSkillParser.ExcludeRangeContext context)
    {
        current.ExcludeRange = GetIntVal(context.FLOAT());
        current.ExcludeRangeName = GetStringVal(context.STRING());
        base.EnterExcludeRange(context);
    }

    public override void EnterFx(AttackSkillParser.FxContext context)
    {
        current.FX = GetStringVal(context.STRING()).ToLower();
        base.EnterFx(context);
    }

    public override void EnterFxCTAG(AttackSkillParser.FxCTAGContext context)
    {
        current.FXCTAG = GetStringVal(context.STRING());
        base.EnterFxCTAG(context);
    }

    public override void EnterFxMove(AttackSkillParser.FxMoveContext context)
    {
        current.FXMove = GetStringVal(context.STRING());
        base.EnterFxMove(context);
    }

    public override void EnterFxProjectile(AttackSkillParser.FxProjectileContext context)
    {
        current.FXProjectile = GetStringVal(context.STRING());
        base.EnterFxProjectile(context);
    }

    public override void EnterFxProjectileImpact(AttackSkillParser.FxProjectileImpactContext context)
    {
        current.FXProjectileImpact = GetStringVal(context.STRING());
        base.EnterFxProjectileImpact(context);
    }

    //public override void EnterFxProjectileSequence(AttackSkillParser.FxProjectileSequenceContext context)
    //{
    //    current.FXProjectileSequenceVal1 = GetStringVal(context.STRING(0));
    //    current.FXProjectileSequenceVal2 = GetStringVal(context.STRING(1));
    //    current.FXProjectileSequenceVal3 = GetStringVal(context.STRING(2));
    //    current.FXProjectileSequenceVal4 = GetIntVal(context.FLOAT());
    //    base.EnterFxProjectileSequence(context);
    //}

    public override void EnterLoopAnim(AttackSkillParser.LoopAnimContext context)
    {
        current.AnimLoop = GetStringVal(context.STRING()).ToLower();
        base.EnterLoopAnim(context);
    }

    public override void EnterLowAnim(AttackSkillParser.LowAnimContext context)
    {
        current.AnimLow = GetStringVal(context.STRING()).ToLower();
        base.EnterLowAnim(context);
    }

    public override void EnterFxSwing(AttackSkillParser.FxSwingContext context)
    {
        current.FXSwing = GetStringVal(context.STRING());
        base.EnterFxSwing(context);
    }

    public override void EnterMeter(AttackSkillParser.MeterContext context)
    {
        current.MeterName = GetStringVal(context.STRING());
        current.MeterVal1 = GetFloatVal(context.FLOAT(0));
        current.ComboLength = GetFloatVal(context.FLOAT(1));
        base.EnterMeter(context);
    }

    public override void EnterMoveAnim(AttackSkillParser.MoveAnimContext context)
    {
        current.AnimMove = GetStringVal(context.STRING()).ToLower();
        base.EnterMoveAnim(context);
    }

    public override void EnterMoveRange(AttackSkillParser.MoveRangeContext context)
    {
        current.MoveRange = GetIntVal(context.FLOAT());
        current.MoveRangePattern = GetStringVal(context.STRING());
        base.EnterMoveRange(context);
    }

    public override void EnterMoveRangeCondition(AttackSkillParser.MoveRangeConditionContext context)
    {
        current.MoveRangeConditionList.Add(GetStringVal(context.STRING()));
        base.EnterMoveRangeCondition(context);
    }

    public override void EnterMoveToAttackMod(AttackSkillParser.MoveToAttackModContext context)
    {
        current.MoveToAttackMod = GetIntVal(context.FLOAT());
        base.EnterMoveToAttackMod(context);
    }
    public override void EnterMultiHitData(AttackSkillParser.MultiHitDataContext context)
    {
        current.MultiHitData = GetStringVal(context.STRING());
        base.EnterMultiHitData(context);
    }

    public override void EnterPrereq(AttackSkillParser.PrereqContext context)
    {
        current.PreReqName = GetStringVal(context.STRING());
        base.EnterPrereq(context);
    }

    public override void EnterProjectileAttr(AttackSkillParser.ProjectileAttrContext context)
    {
        current.ProjectileAttributesList.Add(GetStringVal(context.STRING()));
        base.EnterProjectileAttr(context);
    }


    public override void EnterProjectile(AttackSkillParser.ProjectileContext context)
    {
        current.ProjectileWeapon = GetStringVal(context.STRING(0));
        current.ProjectileAxis = GetStringVal(context.STRING(1));
        current.ProjectileSpeed = GetFloatVal(context.FLOAT());
        base.EnterProjectile(context);
    }


    public override void EnterProjectileRotation(AttackSkillParser.ProjectileRotationContext context)
    {
        current.ProjectileRotationVal1 = GetIntVal(context.FLOAT(0));
        current.ProjectileRotationVal2 = GetIntVal(context.FLOAT(1));
        current.ProjectileRotationVal3 = GetIntVal(context.FLOAT(2));
        current.ProjectileRotationVal4 = GetIntVal(context.FLOAT(3));
        base.EnterProjectileRotation(context);
    }

    public override void EnterProjectileSequence(AttackSkillParser.ProjectileSequenceContext context)
    {
        current.ProjectileSequenceVal1 = GetStringVal(context.STRING(0));
        current.ProjectileSequenceVal2 = GetStringVal(context.STRING(1));
        current.ProjectileSequenceVal3 = GetStringVal(context.STRING(2));
        current.ProjectileSequenceVal4 = GetIntVal(context.FLOAT());
        base.EnterProjectileSequence(context);
    }

    public override void EnterProxy(AttackSkillParser.ProxyContext context)
    {
        current.ProxyVal1 = GetStringVal(context.STRING(0));
        current.ProxyVal2 = GetStringVal(context.STRING(1));
        base.EnterProxy(context);
    }

    public override void EnterRange(AttackSkillParser.RangeContext context)
    {
        current.Range = GetIntVal(context.FLOAT());
        current.RangeName = GetStringVal(context.STRING());
        base.EnterRange(context);
    }



    public override void EnterReplaces(AttackSkillParser.ReplacesContext context)
    {
        current.Replaces = GetStringVal(context.STRING());
        base.EnterReplaces(context);
    }

    public override void EnterShiftData(AttackSkillParser.ShiftDataContext context)
    {
        SkillShiftData shiftData = new SkillShiftData();

        shiftData.ShiftClass = GetStringVal(context.STRING(0));
        shiftData.Appearance= GetStringVal(context.STRING(1));

        shiftData.ConstitutionAdd = GetIntVal(context.FLOAT(0));
        shiftData.PowerAdd = GetIntVal(context.FLOAT(1));
        shiftData.AccuracyAdd = GetIntVal(context.FLOAT(2));
        shiftData.DefenseAdd = GetIntVal(context.FLOAT(3));
        shiftData.InitiativeAdd = GetIntVal(context.FLOAT(4));
        shiftData.MovementAdd = GetIntVal(context.FLOAT(5));

        current.SkillShiftData = shiftData;

        base.EnterShiftData(context);
    }

    public override void EnterSkillFree(AttackSkillParser.SkillFreeContext context)
    {
        current.SkilllFreeList.Add(GetStringVal(context.STRING()));
        base.EnterSkillFree(context);
    }

    public override void EnterSkillStatusSituationStatusCondition(AttackSkillParser.SkillStatusSituationStatusConditionContext context)
    {
        currentSkillStatus.SituationStatusConditionList.Add(GetStringVal(context.STRING()));
        base.EnterSkillStatusSituationStatusCondition(context);
    }

    public override void EnterSkillStatusSituationUnitCondition(AttackSkillParser.SkillStatusSituationUnitConditionContext context)
    {
        currentSkillStatus.SituationUnitConditionList.Add(GetStringVal(context.STRING()));
        base.EnterSkillStatusSituationUnitCondition(context);
    }

    public override void EnterSplitEffect(AttackSkillParser.SplitEffectContext context)
    {
        currentSplitEffect = new SkillSplitEffect();
        current.SplitEffectList.Add(currentSplitEffect);
        base.EnterSplitEffect(context);
    }


    public override void EnterSplitEffectCondition(AttackSkillParser.SplitEffectConditionContext context)
    {
        currentSplitEffect.ConditionList.Add(GetStringVal(context.STRING()));
        base.EnterSplitEffectCondition(context);
    }

    public override void EnterSound(AttackSkillParser.SoundContext context)
    {
        current.Sound = GetStringVal(context.STRING());
        base.EnterSound(context);
    }

    public override void EnterStatus(AttackSkillParser.StatusContext context)
    {
        currentSkillStatus = new AttackSkillStatus();
        currentSkillStatus.Owner = current;
        current.StatusList.Add(currentSkillStatus);
        currentSkillStatus.Description = GetStringVal(context.STRING());
        currentSkillStatus.Val1 = GetIntVal(context.FLOAT(0));
        currentSkillStatus.ModifierValue = GetFloatVal(context.FLOAT(1));
        base.EnterStatus(context);
    }

    public override void EnterStatusChance(AttackSkillParser.StatusChanceContext context)
    {
        currentSkillStatus.Chance = GetIntVal(context.FLOAT());
        base.EnterStatusChance(context);
    }

    public override void EnterStatusCondition(AttackSkillParser.StatusConditionContext context)
    {
        currentSkillStatus.ConditionList.Add(GetStringVal(context.STRING()));
        base.EnterStatusCondition(context);
    }

    public override void EnterStatusDuration(AttackSkillParser.StatusDurationContext context)
    {
        currentSkillStatus.DurationDescription = GetStringVal(context.STRING());
        currentSkillStatus.Duration = GetFloatVal(context.FLOAT());
        base.EnterStatusDuration(context);
    }

    public override void EnterStatusInterval(AttackSkillParser.StatusIntervalContext context)
    {
        currentSkillStatus.IntervalDescription = GetStringVal(context.STRING());
        currentSkillStatus.Interval = GetIntVal(context.FLOAT());
        base.EnterStatusInterval(context);
    }

    public override void EnterStatusSituationAffinityCondition(AttackSkillParser.StatusSituationAffinityConditionContext context)
    {
        currentSkillStatus.SituationAffinityConditionList.Add(GetStringVal(context.STRING()));
        base.EnterStatusSituationAffinityCondition(context);
    }

    public override void EnterStatusSituationSkillCondition(AttackSkillParser.StatusSituationSkillConditionContext context)
    {
        currentSkillStatus.SituationSkillConditionList.Add(GetStringVal(context.STRING()));
        base.EnterStatusSituationSkillCondition(context);
    }

    public override void EnterStatusTarget(AttackSkillParser.StatusTargetContext context)
    {
        currentSkillStatus.TargetType = GetStringVal(context.STRING(0));
        currentSkillStatus.TargetVal = GetIntVal(context.FLOAT());
        currentSkillStatus.TargetRestriction = GetStringVal(context.STRING(1));
        base.EnterStatusTarget(context);
    }

    public override void EnterStatusUseLimit(AttackSkillParser.StatusUseLimitContext context)
    {
        currentSkillStatus.UseLimit = GetIntVal(context.FLOAT());
        base.EnterStatusUseLimit(context);
    }

    public override void EnterSubSkill(AttackSkillParser.SubSkillContext context)
    {
        current.SubSkillName = GetStringVal(context.STRING());
        base.EnterSubSkill(context);
    }

    public override void EnterSummonData(AttackSkillParser.SummonDataContext context)
    {
        current.SummonDataDescription = GetStringVal(context.STRING(0));
        current.SummonDataType = GetStringVal(context.STRING(1));
        current.SummonDataVal = GetIntVal(context.FLOAT());
        base.EnterSummonData(context);
    }

    public override void EnterTargetCondition(AttackSkillParser.TargetConditionContext context)
    {
        current.TargetConditionsList.Add(GetStringVal(context.STRING()));
        base.EnterTargetCondition(context);
    }

    public override void EnterUsabilityCondition(AttackSkillParser.UsabilityConditionContext context)
    {
        current.UsabilityConditionList.Add(GetStringVal(context.STRING()));
        base.EnterUsabilityCondition(context);
    }

    public override void EnterWeaponReq(AttackSkillParser.WeaponReqContext context)
    {
        current.WeaponReq = GetStringVal(context.STRING());
        base.EnterWeaponReq(context);
    }

    public AttackSkill current;
    public SkillEffect currentSkillEffect;
    public AttackSkillStatus currentSkillStatus;
    public SkillSplitEffect currentSplitEffect;
    private Dictionary<String, AttackSkill> m_dictionary;

    
}


