using System.Collections.Generic;
using System;
//namespace Gladius
//{
    public enum GameState
    {
        None,
        Arena,
        Overland,
        GameOverWin,
        GameOverLose,
        Town,
        Loading,
        Menu
    };

    public enum OverlandZone
    {
        None,
        Nordagh,
        Imperia,
        Steppes,
        Expanse
    };

    public enum GameObjectAttributeType
    {
        Accuracy,
        Defense,
        Power,
        Constitution,
        Health,
        Movement,
        CriticalChance,
        EarthAffinity,
        AirAffinity,
        FireAffinity,
        WaterAffinity,
        LightAffinity,
        DarkAffinity,
        CharacterSkillPoints, // used for advancing character, buying skills etc
        ArenaSkillPoints,     // used for limiting skills in arena combat.
        Affinity,
        Experience,
        Initiative,
        NumKills,
        TotalDamageDone,
        TotalDamageReceived,
        NumberOfHits,
        NumberOfCriticalHits,
        NumTypes
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
        Weapon,	
        Armor,	
        Shield,	
        Helmet,	
        Accessory,
        NumItems
    }

    public enum ActorCategory
    {
        Light,
        Medium,
        Heavy,
        Support,
        Arcane
    }

    public enum SkillClass
    {
    NONE,
    AFFINITYBEAST,
    AFFINITYBEASTGREATER,
    AMAZON,
    ARCHER,
    BANDIT,
    BARBARIAN,
    BEAR,
    BEARGREATER,
    BERSERKER,
    BOSS,
    CAT,
    CATGREATER,
    CENTURION,
    CHANNELER,
    CITIZEN,
    CYCLOPS,
    CYCLOPSGREATER,
    DARKCAT,
    DARKGOD,
    DARKLEGIONNAIRE,
    DARKWOLF,
    DERVISH,
    DUMMY,
    EIJI,
    GALDR,
    GALVERG,
    GUNGNIR,
    GUNGNIRF,
    GWAZI,
    INVALID,
    LEGIONNAIRE,
    LUDO,
    MINOTAUR,
    MONGREL,
    MONGRELSHAMAN,
    MURMILLO,
    MUTUUS,
    OGRE,
    OTHER,
    PELTAST,
    PROP,
    SAMNITE,
    SATYR,
    URLAN,
    URSULA,
    VALENS,
    SCARAB,
    SCORPION,
    SECUTOR,
    SUMMONER,
    UNDEADCASTER,
    UNDEADMELEE,
    WOLF,
    WOLFGREATER,
    YETI

}


public enum ActorClass
    {
        AMAZON,
        ARCHER,
        ARCHERF,
        BANDITA,
        BANDITAF,
        BANDITB,
        BARBARIAN,
        BARBARIANF,
        BEAR,
        BEARGREATER,
        BERSERKER,
        BERSERKERF,
        CAT,
        CATGREATER,
        CENTURION,
        CENTURIONF,
        CHANNELERIMP,
        CYCLOPS,
        CYCLOPSGREATER,
        DARKLEGIONNAIRE,
        DERVISH,
        DERVISHF,
        DUMMY,
        EIJI,
        GALDR,
        GALVERG,
        GUNGNIR,
        GUNGNIRF,
        GWAZI,
        LEGIONNAIRE,
        LEGIONNAIREF,
        LUDO,
        MINOTAUR,
        MONGREL,
        MONGRELSHAMAN,
        MURMILLO,
        MURMILLOF,
        MUTUUS,
        OGRE,
        PELTAST,
        PELTASTF,
        SAMNITEEXP,
        SAMNITEEXPF,
        SAMNITEIMP,
        SAMNITEIMPF,
        SAMNITESTE,
        SAMNITESTEF,
        SATYR,
        URLAN,
        URSULA,
        VALENS,
        SCARAB,
        SCORPION,
        SECUTORIMP,
        SECUTORIMPF,
        SECUTORSTE,
        SECUTORSTEF,
        SUMMONER,
        UNDEADCASTERA,
        UNDEADMELEEIMPA,
        URSULACOSTUMEA,
        URSULACOSTUMEB,
        VALENSCOSTUMEA,
        VALENSCOSTUMEB,
        WOLF,
        WOLFGREATER,
        YETI
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
        Miss,
        Hit,
        Critical,
        Blocked,
        Avoided
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


    public enum AnimationEnum
    {
        None,
        Idle,
        IdleEngaged,
        IdleWounded,
        IdlePassive,
        IdleDeath,
        IdleKnockDown,
        Death,
        ReactVictory,
        ReactLoss,
        ReactHitWeakF,
        ReactHitWeakB,
        ReactHitWeakL,
        ReactHitWeakR,
        ReactHitStrongF,
        ReactHitStrongB,
        ReactHitStrongL,
        ReactHitStrongR,
        ReactKnockback,
        ReactDowned,
        ReactKnockdownB,
        MoveWalk,
        MoveRun,
        MoveClimbHalf,
        MoveJumpDownHalf,
        MoveGetup,
        Attack1,
        Attack2,
        Attack3,
        Block,
        SandToss
    }

//public enum SquareType
//{
//    Empty,
//    Level1,
//    Level2,
//    Level3,
//    Unaccesible,
//    Wall,
//    Crowd,
//    Pillar
//    //Mobile
//}

public enum ControlState
    {
        None,
        ChoosingSkill,
        UsingGrid
    }

    public enum ActionButton
    {
        Move1Up,
        Move1Down,
        Move1Left,
        Move1Right,
        Move2Up,
        Move2Down,
        Move2Left,
        Move2Right,
        ActionButton1,
        ActionButton2,
        ActionButton3,
        ActionButton4
    }

    public enum SchoolRank
    {
        Bronze,
        Silver,
        Gold
    }


    public enum GladiusCameraMode
    {
        None,
        Normal,
        Manual,
        Combat,
        Overland,
        BirdsEye,
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

    public static class StatusConditions
    {
        public static string[] Conditions = 
	{
        "statuscond_bleeding",
        "statuscond_blind",
        "statuscond_charmed",
        "statuscond_confused",
        "statuscond_doom",
        "statuscond_feared",
        "statuscond_frozen",
        "statuscond_petrified",
        "statuscond_root",
        "statuscond_stunned"
	};
    }
    
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
