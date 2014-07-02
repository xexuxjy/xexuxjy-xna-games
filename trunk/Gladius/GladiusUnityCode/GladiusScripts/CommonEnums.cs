using System.Collections.Generic;
using System;
namespace Gladius
{

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

    public enum ActorClass
    {
        Amazon,
        Archer,
        ArcherF,
        BanditA,
        BanditAF,
        BanditB,
        Barbarian,
        BarbarianF,
        Bear,
        BearGreater,
        Berserker,
        BerserkerF,
        Cat,
        CatGreater,
        Centurion,
        CenturionF,
        ChannelerImp,
        Cyclops,
        CyclopsGreater,
        DarkLegionnaire,
        Dervish,
        DervishF,
        Eiji,
        Galdr,
        Galverg,
        Gungnir,
        GungnirF,
        Gwazi,
        Legionnaire,
        LegionnaireF,
        Ludo,
        Minotaur,
        Mongrel,
        MongrelShaman,
        Murmillo,
        MurmilloF,
        Mutuus,
        Ogre,
        Peltast,
        PeltastF,
        SamniteExp,
        SamniteExpF,
        SamniteImp,
        SamniteImpF,
        SamniteSte,
        SamniteSteF,
        Satyr,
        Urlan,
        Ursula,
        Valens,
        Scarab,
        Scorpion,
        SecutorImp,
        SecutorImpF,
        SecutorSte,
        SecutorSteF,
        Summoner,
        UndeadCasterA,
        UndeadMeleeImpA,
        UrsulaCostumeA,
        UrsulaCostumeB,
        ValensCostumeA,
        ValensCostumeB,
        Wolf,
        WolfGreater,
        Yeti
    }


    public enum DamageType
    {
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
        Walk,
        Run,
        Climb,
        Attack1,
        Attack2,
        Attack3,
        BowShot,
        Miss,
        Block,
        Stagger,
        Die,
        Cast,
        Cheer
    }

    public enum SquareType
    {
        Empty,
        Level1,
        Level2,
        Level3,
        Unaccesible,
        Wall,
        Crowd,
        Pillar
        //Mobile
    }

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

    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}