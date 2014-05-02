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
        JobPoints,
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
        Head = 0,
        LHand = 1,
        RHand = 2,
        Body = 3,
        Special = 4,
        NumItems = 5
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
        Bandit,
        Barbarian,
        Bear,
        Berserker,
        Centurion,
        Channeler,
        Cyclops,
        Dervish,
        Eiji,
        Gungir,
        Gwazi,
        Legionnaire,
        Ludo,
        Minotaur,
        Mongrel,
        MongrelShaman,
        Murmillo,
        Ogre,
        Peltast,
        PlainsCat,
        Samnite,
        Satyr,
        Scarab,
        Scorpion,
        Secutor,
        Summoner,
        UndeadLegionnaire,
        UndeadSummoner,
        Urlan,
        Ursula,
        Valens,
        Wolf,
        Yeti
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

    public enum AffinityType
    {
        Light,
        Dark,
        Air,
        Earth,
        Fire,
        Water
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

    public enum AttackSkillRange
    {
        Self,
        Plus,
        Plus2x2,
        Plus3x3,
        Square,
        Diamond
    }
}