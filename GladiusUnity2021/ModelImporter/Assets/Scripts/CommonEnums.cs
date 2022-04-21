using System.Collections.Generic;
using System;
//namespace Gladius
//{
    public enum GameMode
    {
        None,
        Arena,
        OverlandNordagh,
        OverlandImperia,
        OverlandSteppes,
        OverlandExpanse,
        Town,
        Loading,
        Menu
    };

//public class GameObjectAttributeType
//{
//    public string Name { get; set; }
//    public string LongName { get; set; }

//    public GameObjectAttributeType(string name, string longName)
//    {
//        Name = name;
//        LongName = longName;
//    }
//    //public static string[] names = new string[] { "LVL", "XP", "Next", "HP", "DAM", "PWR", "ACC", "DEF", "INI", "CON", "MOV", "Arm", "Wpn" };
//    //public static string[] longNames = new string[] { "Level", "Experience Points", "Next Level Points", "Hitpoints", "damage", "power", "accuracy", "defense", "initiative", "CON", "MOV", "Arm", "Wpn" };


//    public readonly static GameObjectAttributeType Accuracy = new GameObjectAttributeType("ACC", "accuracy");
//    public readonly static GameObjectAttributeType Defense = new GameObjectAttributeType("DEF", "defense");
//    public readonly static GameObjectAttributeType Power = new GameObjectAttributeType("PWR", "power");
//    public readonly static GameObjectAttributeType Constitution = new GameObjectAttributeType("CON", "constitution");
//    public readonly static GameObjectAttributeType Health = new GameObjectAttributeType("HP", "health");
//    public readonly static GameObjectAttributeType Movement = new GameObjectAttributeType("MOV", "movement");
//    public readonly static GameObjectAttributeType CriticalChance = new GameObjectAttributeType("CRT", "critical chance");
//    public readonly static GameObjectAttributeType Affinity = new GameObjectAttributeType("AFF", "affinity");
//    public readonly static GameObjectAttributeType DefenseAffinityChargeRate = new GameObjectAttributeType("DAF", "defense affinity charge rate");
//    public readonly static GameObjectAttributeType OffenseAffinityChargeRate = new GameObjectAttributeType("OAF", "offense affinity charge rate");
//    public readonly static GameObjectAttributeType ShieldEffectiveness = new GameObjectAttributeType("SHI", "shield effectiveness");
//    public readonly static GameObjectAttributeType CharacterSkillPoints = new GameObjectAttributeType("SP", "skill points");
//    public readonly static GameObjectAttributeType ArenaSkllPoints = new GameObjectAttributeType("ASP", "arena skill points");
//    public readonly static GameObjectAttributeType Experience = new GameObjectAttributeType("EXP", "experience");
//    public readonly static GameObjectAttributeType Initiative = new GameObjectAttributeType("INI", "initiative");
//    public readonly static GameObjectAttributeType NumKills = new GameObjectAttributeType("NKI", "number of kills");
//    public readonly static GameObjectAttributeType TotalDamageDone = new GameObjectAttributeType("TDD", "total damage done");
//    public readonly static GameObjectAttributeType TotalDamageReceived = new GameObjectAttributeType("TDR", "total damage received");
//    public readonly static GameObjectAttributeType NumberOfHits = new GameObjectAttributeType("NH", "number of hits");
//    public readonly static GameObjectAttributeType NumberOfCriticalHits = new GameObjectAttributeType("NCH", "number of critical hits");
//    public readonly static GameObjectAttributeType Level = new GameObjectAttributeType("LVL", "level");
//    public readonly static GameObjectAttributeType Xp = new GameObjectAttributeType("XP", "experience");
//    public readonly static GameObjectAttributeType NextXp = new GameObjectAttributeType("Next", "next expereince");
//    public readonly static GameObjectAttributeType Damage = new GameObjectAttributeType("DAM", "damage");
//    public readonly static GameObjectAttributeType Armour = new GameObjectAttributeType("Arm", "armour");
//    public readonly static GameObjectAttributeType Weapon = new GameObjectAttributeType("Wpn", "weapon");

//}

public enum GameObjectAttributeType
{
    None,
    Accuracy,
    Defense,
    Power,
    Constitution,
    Health,
    Movement,
    MoveRate,
    CriticalChance,
    Affinity,
    DefenseAffinityChargeRate,
    OffenseAffinityChargeRate,
    ShieldEffectiveness,
    CharacterSkillPoints, // used for advancing character, buying skills etc
    ArenaSkillPoints,     // used for limiting skills in arena combat.
    Experience,
    Initiative,
    NumKills,
    TotalDamageDone,
    TotalDamageReceived,
    NumberOfHits,
    NumberOfCriticalHits,
    Level,
    Xp,
    NextXp,
    Damage,
    Armour,
    Weapon,
    CombatAttack,
    CombatDefend,
    Crowd,
    NumTypes
}


public enum CrowdState
{
    None,
    Idle,
    ExcitedA,
    ExcitedB,
    BooA,
    BooB,
    Throw
}



public enum Affinity
{
    AFF_ALL = -1,
    AFF_NONE = 0,
    AFF_EARTH,
    AFF_WATER,
    AFF_DARK,
    AFF_AIR,
    AFF_FIRE,
    AFF_LIGHT
}

public enum GameObjectAttributeModifierDurationType
    {
        InstantPermanent,
        InstantTemporary,
        OverTimePermanent,
        OverTimeTemporary
    }

    public enum GameObjectAttributeModifierType
    {
        Add,
        Multiply
    }

    public enum ItemLocation
    {
        None,
        Weapon,	
        Armor,	
        Shield,	
        Helmet,	
        Accessory,
        Projectile,
        NumItems
    }



    public enum DamageType
    {
        None,
        Physical,
        Ranged,
        Air,
        Earth,
        Fire,
        Water,
        Light,
        Dark
    };

    public enum TargetCondition
    {
        Default,
        Self,
        FriendOnlyNotSelf,
        AnySquare,
        EnemyOnly,
    }

    public enum AttackResultType
    {
        None,
        Blocked,
        Evaded,
        Hit,
        Critical,
        Weak
    }

    public enum SkillIconState
    {
        Available,
        Selected,
        Unavailable
    }

    public enum SkillIcon
    {
        Move=0,
        Attack=1,
        Combo=2,
        Special=3,
        Affinity=4
    }

    public enum AttackType
    {
        Attack,
        Block,
        SingleOrtho,
        Move,
        EndTurn
    }



public enum ControlState
    {
        None,
        ChoosingSkill,
        UsingGrid
    }


    public enum SchoolRank
    {
        Bronze=1,
        Silver=2,
        Gold=3
    }


    public enum GladiusCameraMode
    {
        None,
        Orbit,
        NormalFollow,
        Manual,
        Combat,
        Overland,
        Overhead,
        MovementCursor
    }

    public enum SkillEffectRangeType
    {
        Self,
        Plus3x3,
        Square,
        Cone,
        Linear,
        Square2x2,
        SrcLinear2x2,
        SrcLinear3x3,
        SourceSquare,
        SourceDiamond,
        SourceLinear,
        SourceCone
    }

 //   public static class StatusConditions
 //   {
 //       public static string[] Conditions = 
	//{
 //       "berserk",
 //       "bleeding",
 //       "blind",
 //       "charm",
 //       "confusion",
 //       "fear",
 //       "frozen",
 //       "petrification",
 //       "poison",
 //       "root",
 //       "stun"
 //   };
 //   }
    
    [Flags]
    public enum ClassAttributeFlags
    {
        Light = 1,
        Medium = 2,
        Heavy = 4,
        Arcane = 8,
        Support = 16,
        Beast = 32,
        Male = 64,
        Female = 128,
        Nordargh = 256,
        Imperia = 512,
        Expanse = 1024,
        Steppes = 2048,
        Human = 4096
    }


    // 0 = mongel? nah
    // 1 = light
    // 2 = legionnaire
    // 4 - centurion?  or heavy?
    // 8 = arcane
    // 16 = support required
    // 24 = prohibited arcane + support
    // 32 = beast
    // 56 = prohibited beast, required arcane + support
    // 64 = male
    // 128 = female
    // heavy = 56 (32+16+8)
    // 256 = Nordargh only?
    // 512 = Imperia only
    // 1024 = Southern Expanse?
    // 2048 = Windward Steppes
    // 1056 = 1024 + 32 required beast?
    // 3633 = 2048 + 1024 + imperia + beast + support + light?   == barbarian?  
    // 4096 = human
    // 4160 = human male        4096+64
    // 4224 = human female      4096+128



    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
//}
