using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Misc;
using Gladius;
using UnityEngine;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime;

public class ArenaOffice
{
    public string officeNameString;
    public int officeNameId;
    public int historyId = 0;

    // these are extras added via xml
    public string backgroundTextureName;
    public string ownerThumbnailName;
    public string leagueNameFile;
    // end.

    public int officeDescId;

    public int tagLine1;
    public int tagLine2;

    public int leaguePointsNeeded;

    public string officer;

    public string modelName;

    public List<CharacterData> Recruits = new List<CharacterData>();
    public List<ArenaSchool> Schools = new List<ArenaSchool>();
    public List<LeagueData> Leagues = new List<LeagueData>();
    public TownData TownData;
}

public class ArenaSchool
{
    public string name;
    public int id;
}


public class LeagueData
{
    public void LoadData()
    {
        foreach(LeagueEncounterData led in Encounters)
        {
            led.LoadData();
        }

    }

    public ArenaOffice ArenaOffice;
    public string Name;
    public int Id;
    public int dDscId;
    public string DesignNotes;
    public int LeaguePoints;
    public int EncounterPointsNeeded;
    public int MinimumPopularity;
    public int Tier;

    public int OnHover;
    public int OnSelect;
    public int OnWin;

    public List<PrizeInfo> CompletionPrizes = new List<PrizeInfo>();
    public List<PrizeInfo> MasteryPrizes = new List<PrizeInfo>();
    public List<LeagueEncounterData> Encounters = new List<LeagueEncounterData>();
    public List<BadgeData> RequiredBadges = new List<BadgeData>();
}

public class LeagueEncounterData
{
    public void LoadData()
    {
        String path = GladiusGlobals.EncountersPath + EncounterFile;
        TextAsset ta = GladiusGlobals.LoadTextAsset(path);
        if (ta != null)
        {
            //Debug.Log("Loading encounter : " + path);
            var lexer = new GladiusEncounterLexer(new Antlr4.Runtime.AntlrInputStream(ta.text));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GladiusEncounterParser(commonTokenStream);
            IParseTree parseTree = parser.root();
            EncounterDataParser listener = new EncounterDataParser();
            ParseTreeWalker.Default.Walk(listener, parseTree);
            EncounterData = listener.CurrentEncounter;
        }
        else
        {
            //Debug.LogWarning("can't load encounter file : " + path);
        }

    }


    public string Name;
    public int Id;

    public int Desc;
    public string DesignNotes;
    public string EncounterFile;

    public int EncounterPoints;
    public int EntryFee;

    //public List<int> TeamSizes = new List<int>();
    public int MinHeroTeam;
    public int MaxHeroTeam;
    public int Team1Size;
    public int Team2Size;
    public int Team3Size;

    public int OnHover;
    public int OnSelect;
    public int OnWin;

    public int Frequency;

    public string Type;

    public LeagueData League;
    public EncounterData EncounterData;

}

public class EncounterData
{
    public string name;
    public int ursualsEase;
    public int valensEase;
    public List<PrizeInfo> prizes = new List<PrizeInfo>();
    public string scene;
    public string gridFileName;
    private GridFile gridFile;
    public string propsFile;
    public string music;
    public string battleScript;
    public List<CameraTrack> cameraTracks = new List<CameraTrack>();

    public int totalPopularity;
    public int crowdLevel;
    public List<string> cutScenes = new List<string>();

    public bool candie;


    public List<TeamData> teams = new List<TeamData>();

    public List<Prop> props = new List<Prop>();

    public String GridFileName
    {
        get { return gridFileName; }
    }

    public GridFile GridFile
    {
        get
        {
            if (gridFile == null)
            {
                gridFile = GridFileManager.GetGridFile(GridFileName);
            }
            return gridFile;
        }
    }


}

public class TeamData
{
    public int TeamIndex;
    public int val2;
    public string gridFileName;

    public string name;
    public GladiatorSchool School;

    public bool AllDead;
    private List<TeamDataSlot> m_teamSlots = new List<TeamDataSlot>();
    private GridFile m_gridFile;

    public void SetCharactersFromSchool()
    {
        if(School != null)
        { 
            int size = Math.Min(School.CurrentParty.Count, m_teamSlots.Count);
            for (int i = 0; i < size; ++i)
            {
                m_teamSlots[i].CharacterData = School.CurrentParty[i];
            }
        }
    }

    public void SetCharacters(List<CharacterData> characters)
    {
        m_teamSlots.Clear();
        m_teamSlots.Capacity = characters.Count;
        int size = Math.Min(characters.Count, m_teamSlots.Count);
        for(int i=0;i<size;++i)
        {
            m_teamSlots[i].CharacterData = characters[i];
        }

    }

    public UnitDBRestriction GetRestrictionForSlot(int slotIndex)
    {
        if (slotIndex < m_teamSlots.Count)
        {
            if (m_teamSlots[slotIndex] != null)
            {
                return m_teamSlots[slotIndex].Restriction;
            }
        }
        return null;
    }

    public List<TeamDataSlot> Slots
    { get { return m_teamSlots; } }

    public String GridFileName
    {
        get { return gridFileName; }
    }

    public GridFile GridFile
    {
        get
        {
            if(m_gridFile == null)
            {
                m_gridFile = GridFileManager.GetGridFile(GridFileName);
            }
            return m_gridFile;
        }
    }
}

public class TeamDataSlot
{
    public TeamDataSlot(UnitDBRestriction restriction)
    {
        Restriction = restriction;
    }

    public UnitDBRestriction Restriction;
    public CharacterData CharacterData;
}




//public class CharacterAndRestriction
//{
//    public CharacterAndRestriction(CharacterData cd,UnitDB restriction)
//    {
//        CharacterData = cd;
//        Restriction = restriction;
//    }

//    public CharacterData CharacterData
//    { get; set; }

//    public UnitDB Restriction
//    { get; set; }
//}


public class CameraTrack
{
    public string val1;
    public string val2;
}


public class PrizeInfo
{
    public string name;
    public int id;

}

public class ArenaLeageParser : GladiusLeagueBaseListener
{


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


    public override void EnterOfficeName([NotNull] GladiusLeagueParser.OfficeNameContext context)
    {
        base.EnterOfficeName(context);
        CurrentOffice = new ArenaOffice();
        CurrentOffice.officeNameString = GetStringVal(context.STRING());
        CurrentOffice.officeNameId = GetIntVal(context.INT());
    }

    public override void EnterOfficeDesc([NotNull] GladiusLeagueParser.OfficeDescContext context)
    {
        base.EnterOfficeDesc(context);
        CurrentOffice.officeDescId = GetIntVal(context.INT());
    }

    public override void EnterTagLine1([NotNull] GladiusLeagueParser.TagLine1Context context)
    {
        base.EnterTagLine1(context);
        CurrentOffice.tagLine1 = GetIntVal(context.INT());
    }

    public override void EnterTagLine2([NotNull] GladiusLeagueParser.TagLine2Context context)
    {
        base.EnterTagLine2(context);
        CurrentOffice.tagLine2 = GetIntVal(context.INT());
    }

    public override void EnterLeaguePtsNeeded([NotNull] GladiusLeagueParser.LeaguePtsNeededContext context)
    {
        base.EnterLeaguePtsNeeded(context);
        CurrentOffice.leaguePointsNeeded = GetIntVal(context.INT());
    }

    public override void EnterOfficer([NotNull] GladiusLeagueParser.OfficerContext context)
    {
        base.EnterOfficer(context);
        CurrentOffice.officer = GetStringVal(context.STRING());
        //Debug.Log("Entering officer" + CurrentOffice.officer);

    }

    public override void EnterRecruit([NotNull] GladiusLeagueParser.RecruitContext context)
    {
        //Debug.Log("Entering recruit ");

        base.EnterRecruit(context);

        string name = GetStringVal(context.STRING());


        int[] stats = new int[9];
        for (int i = 0; i < stats.Length; ++i)
        {
            stats[i] = GetIntVal(context.INT(i));
        }


        CharacterData characterData;
        if (!ActorGenerator.AllGladiators.TryGetValue(name, out characterData))
        {
            Debug.LogError("Can't find gladiator called : " + name);
        }
        if (characterData != null)
        {
            characterData.Level = stats[0];
            characterData.Cost = stats[1];
            characterData.HireCost = stats[2];

            CurrentOffice.Recruits.Add(characterData);
        }
        //Debug.Log("leaving recruit");
    }

    public override void EnterSchool([NotNull] GladiusLeagueParser.SchoolContext context)
    {
        base.EnterSchool(context);
        ArenaSchool school = new ArenaSchool();

        school.name = GetStringVal(context.STRING());
        school.id = GetIntVal(context.INT());

        CurrentOffice.Schools.Add(school);
    }

    public override void EnterLeague([NotNull] GladiusLeagueParser.LeagueContext context)
    {
        //Debug.Log("Entering league");

        base.EnterLeague(context);
        CurrentEncounterData = null; // reset this here so we add to the right place...
        CurrentLeagueData = new LeagueData();
        CurrentLeagueData.ArenaOffice = CurrentOffice;
        CurrentOffice.Leagues.Add(CurrentLeagueData);
        CurrentLeagueData.Name = GetStringVal(context.STRING());
        CurrentLeagueData.Id = GetIntVal(context.INT());

    }

    public override void EnterLeagueDesc([NotNull] GladiusLeagueParser.LeagueDescContext context)
    {
        base.EnterLeagueDesc(context);
        CurrentLeagueData.dDscId = GetIntVal(context.INT());
    }

    public override void EnterDesignNotes([NotNull] GladiusLeagueParser.DesignNotesContext context)
    {
        base.EnterDesignNotes(context);
        if (CurrentEncounterData != null)
        {
            CurrentEncounterData.DesignNotes = GetStringVal(context.STRING());
        }
        else
        {
            CurrentLeagueData.DesignNotes = GetStringVal(context.STRING());
        }
        }

    public override void EnterLeaguePts([NotNull] GladiusLeagueParser.LeaguePtsContext context)
    {
        base.EnterLeaguePts(context);
        CurrentLeagueData.LeaguePoints = GetIntVal(context.INT());
    }

    public override void EnterEncptsNeeded([NotNull] GladiusLeagueParser.EncptsNeededContext context)
    {
        base.EnterEncptsNeeded(context);
        CurrentLeagueData.EncounterPointsNeeded = GetIntVal(context.INT());
    }

    public override void EnterMinPop([NotNull] GladiusLeagueParser.MinPopContext context)
    {
        base.EnterMinPop(context);
        CurrentLeagueData.MinimumPopularity = GetIntVal(context.INT());
    }

    public override void EnterTier([NotNull] GladiusLeagueParser.TierContext context)
    {
        base.EnterTier(context);
        CurrentLeagueData.Tier = GetIntVal(context.INT());
    }

    public override void EnterBadge([NotNull] GladiusLeagueParser.BadgeContext context)
    {
        base.EnterBadge(context);
        BadgeData badge = new BadgeData();
        badge.BadgeNameId = GetIntVal(context.INT());
        badge.Name = GetStringVal(context.STRING());
        CurrentLeagueData.RequiredBadges.Add(badge);
    }

    public override void EnterOnHover([NotNull] GladiusLeagueParser.OnHoverContext context)
    {
        base.EnterOnHover(context);
        if (CurrentEncounterData != null)
        {
            CurrentEncounterData.OnHover = GetIntVal(context.INT(0));
        }
        else
        {
            CurrentLeagueData.OnHover = GetIntVal(context.INT(0));
        }
    }

    public override void EnterOnSelect([NotNull] GladiusLeagueParser.OnSelectContext context)
    {
        base.EnterOnSelect(context);
        if (CurrentEncounterData != null)
        {
            CurrentEncounterData.OnSelect = GetIntVal(context.INT(0));
        }
        else
        {
            CurrentLeagueData.OnSelect = GetIntVal(context.INT(0));
        }
    }

    public override void EnterOnWin([NotNull] GladiusLeagueParser.OnWinContext context)
    {
        base.EnterOnWin(context);
        CurrentLeagueData.OnWin = GetIntVal(context.INT());
    }

    public override void EnterFrequency([NotNull] GladiusLeagueParser.FrequencyContext context)
    {
        base.EnterFrequency(context);
        CurrentEncounterData.Frequency = GetIntVal(context.INT());
    }

    public override void EnterPrizeCompletion([NotNull] GladiusLeagueParser.PrizeCompletionContext context)
    {
        base.EnterPrizeCompletion(context);
        PrizeInfo prize = new PrizeInfo();
        prize.id = GetIntVal(context.INT());
        prize.name = GetStringVal(context.STRING());
        CurrentLeagueData.CompletionPrizes.Add(prize);
    }

    public override void EnterPrizeMastery([NotNull] GladiusLeagueParser.PrizeMasteryContext context)
    {
        base.EnterPrizeMastery(context);
        PrizeInfo prize = new PrizeInfo();
        prize.id = GetIntVal(context.INT());
        prize.name = GetStringVal(context.STRING());
        CurrentLeagueData.MasteryPrizes.Add(prize);
    }

    public override void EnterEncounter([NotNull] GladiusLeagueParser.EncounterContext context)
    {
        //Debug.Log("Entering encounter");
        base.EnterEncounter(context);
        CurrentEncounterData = new LeagueEncounterData();
        CurrentEncounterData.Id = GetIntVal(context.INT());
        CurrentEncounterData.Name = GetStringVal(context.STRING());
        CurrentLeagueData.Encounters.Add(CurrentEncounterData);
        CurrentEncounterData.League = CurrentLeagueData;
    }

    public override void EnterEncDesc([NotNull] GladiusLeagueParser.EncDescContext context)
    {
        base.EnterEncDesc(context);
        CurrentEncounterData.Desc = GetIntVal(context.INT());
    }

    public override void EnterEncFile([NotNull] GladiusLeagueParser.EncFileContext context)
    {
        base.EnterEncFile(context);
        //CurrentEncounterData.encounterFile = GetStringVal(context.STRING()).Replace(".enc","");
        CurrentEncounterData.EncounterFile = GetStringVal(context.STRING());
        //string ending = ".enc";
        //if (CurrentEncounterData.EncounterFile.EndsWith(ending))
        //{
        //    CurrentEncounterData.EncounterFile = CurrentEncounterData.EncounterFile.Remove(CurrentEncounterData.EncounterFile.Length - ending.Length);
        //}
    }

    public override void EnterType([NotNull] GladiusLeagueParser.TypeContext context)
    {
        base.EnterType(context);
        CurrentEncounterData.Type = GetStringVal(context.STRING());
    }

    public override void EnterEncpts([NotNull] GladiusLeagueParser.EncptsContext context)
    {
        base.EnterEncpts(context);
        CurrentEncounterData.EncounterPoints = GetIntVal(context.INT());
    }

    public override void EnterEntryFee([NotNull] GladiusLeagueParser.EntryFeeContext context)
    {
        base.EnterEntryFee(context);
        CurrentEncounterData.EntryFee = GetIntVal(context.INT());
    }

    public override void EnterTeams([NotNull] GladiusLeagueParser.TeamsContext context)
    {
        base.EnterTeams(context);
        CurrentEncounterData.MinHeroTeam = GetIntVal(context.INT(0));
        CurrentEncounterData.MaxHeroTeam = GetIntVal(context.INT(1));
        CurrentEncounterData.Team1Size = GetIntVal(context.INT(2));
        CurrentEncounterData.Team2Size = GetIntVal(context.INT(3));
        CurrentEncounterData.Team3Size = GetIntVal(context.INT(4));
    }

    //public override void EnterModelName([NotNull] GladiusLeagueParser.ModelNameContext context)
    //{
    //    base.EnterModelName(context);
    //    CurrentOffice.modelName = GetStringVal(context.STRING());
    //}

    public ArenaOffice CurrentOffice;
    LeagueData CurrentLeagueData;
    LeagueEncounterData CurrentEncounterData;

}


public class EncounterDataParser : GladiusEncounterBaseListener
{
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


    public override void EnterEncounterName([NotNull] GladiusEncounterParser.EncounterNameContext context)
    {
        base.EnterEncounterName(context);
        CurrentEncounter = new EncounterData();
        CurrentEncounter.name = GetStringVal(context.STRING());

    }

    public override void EnterUrsulaEase([NotNull] GladiusEncounterParser.UrsulaEaseContext context)
    {
        base.EnterUrsulaEase(context);
        CurrentEncounter.ursualsEase = GetIntVal(context.INT());
    }

    public override void EnterValensEase([NotNull] GladiusEncounterParser.ValensEaseContext context)
    {
        base.EnterValensEase(context);
        CurrentEncounter.valensEase = GetIntVal(context.INT());
    }

    public override void EnterPrizeTier([NotNull] GladiusEncounterParser.PrizeTierContext context)
    {
        base.EnterPrizeTier(context);
        PrizeInfo prize = new PrizeInfo();
        prize.id = GetIntVal(context.INT());
        prize.name = GetStringVal(context.STRING());
        CurrentEncounter.prizes.Add(prize);

    }

    public override void EnterScene([NotNull] GladiusEncounterParser.SceneContext context)
    {
        base.EnterScene(context);
        CurrentEncounter.scene = GetStringVal(context.STRING());
    }

    public override void EnterGridFile([NotNull] GladiusEncounterParser.GridFileContext context)
    {
        base.EnterGridFile(context);
        if (CurrentTeam != null)
        {
            CurrentTeam.gridFileName = GetStringVal(context.STRING());
        }
        else
        {
            CurrentEncounter.gridFileName = GetStringVal(context.STRING());
        }
    }

    public override void EnterPropsFile([NotNull] GladiusEncounterParser.PropsFileContext context)
    {
        base.EnterPropsFile(context);
        CurrentEncounter.propsFile = GetStringVal(context.STRING());
    }

    public override void EnterMusic([NotNull] GladiusEncounterParser.MusicContext context)
    {
        base.EnterMusic(context);
        CurrentEncounter.music = GetStringVal(context.STRING());
    }

    public override void EnterCameraTrack([NotNull] GladiusEncounterParser.CameraTrackContext context)
    {
        base.EnterCameraTrack(context);
        CameraTrack cameraTrack = new CameraTrack();
        cameraTrack.val1 = GetStringVal(context.STRING(0));
        cameraTrack.val2 = GetStringVal(context.STRING(1));
        CurrentEncounter.cameraTracks.Add(cameraTrack);
    }

    public override void EnterBattleScript([NotNull] GladiusEncounterParser.BattleScriptContext context)
    {
        base.EnterBattleScript(context);
        CurrentEncounter.battleScript = GetStringVal(context.STRING());
    }

    public override void EnterTotalPop([NotNull] GladiusEncounterParser.TotalPopContext context)
    {
        base.EnterTotalPop(context);
        CurrentEncounter.totalPopularity = GetIntVal(context.INT());
    }

    public override void EnterCrowdLevel([NotNull] GladiusEncounterParser.CrowdLevelContext context)
    {
        base.EnterCrowdLevel(context);
        CurrentEncounter.crowdLevel = GetIntVal(context.INT());
    }

    public override void EnterCutscene([NotNull] GladiusEncounterParser.CutsceneContext context)
    {
        base.EnterCutscene(context);
        
    }

    public override void EnterCandie([NotNull] GladiusEncounterParser.CandieContext context)
    {
        base.EnterCandie(context);
    }

    public override void EnterTeam([NotNull] GladiusEncounterParser.TeamContext context)
    {
        base.EnterTeam(context);
        CurrentTeam = new TeamData();
        CurrentTeam.TeamIndex = GetIntVal(context.INT(0));
        CurrentTeam.val2 = GetIntVal(context.INT(1));
        CurrentEncounter.teams.Add(CurrentTeam);
    }

    public override void EnterSchool([NotNull] GladiusEncounterParser.SchoolContext context)
    {
        base.EnterSchool(context);
        CurrentTeam.School = new GladiatorSchool();
        CurrentTeam.School.Name = GetStringVal(context.STRING());
    }


    public override void EnterUnitDB([NotNull] GladiusEncounterParser.UnitDBContext context)
    {
        base.EnterUnitDB(context);
        //: 'UNITDB:' STRING COMMA INT COMMA STRING COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA STRING COMMA STRING COMMA STRING COMMA STRING COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT COMMA INT;
        int strCount = 0;
        int intCount = 0;
        string name = GetStringVal(context.STRING(strCount++));
        int val2 = GetIntVal(context.INT(intCount++)); 
        string startSlot = GetStringVal(context.STRING(strCount++));
        int minLevel = GetIntVal(context.INT(intCount++));
        int maxLevel = GetIntVal(context.INT(intCount++));
        int val6 = GetIntVal(context.INT(intCount++));
        int requiredMask = GetIntVal(context.INT(intCount++));
        int affinityRequirement = GetIntVal(context.INT(intCount++));
        string class1 = GetStringVal(context.STRING(strCount++));
        string class2 = GetStringVal(context.STRING(strCount++));
        string class3 = GetStringVal(context.STRING(strCount++));
        string class4 = GetStringVal(context.STRING(strCount++));
        int stats1 = GetIntVal(context.INT(intCount++));
        int stats2 = GetIntVal(context.INT(intCount++));
        int stats3 = GetIntVal(context.INT(intCount++));
        int stats4 = GetIntVal(context.INT(intCount++));
        int stats5 = GetIntVal(context.INT(intCount++));
        int stats6 = GetIntVal(context.INT(intCount++));
        int stats7 = GetIntVal(context.INT(intCount++));
        int stats8 = GetIntVal(context.INT(intCount++));
        int stats9 = GetIntVal(context.INT(intCount++));


        UnitDBRestriction unitDB = new UnitDBRestriction(name, val2, startSlot, minLevel, maxLevel, val6, requiredMask, affinityRequirement, class1, class2, class3, class4, stats1, stats2, stats3, stats4, stats5, stats6, stats7, stats8, stats9);
        //CharacterData randomCD = ActorGenerator.CreateRandomCharacter(unitDB,minLevel);

        //CurrentTeam.characters.Add(randomCD);
        CurrentTeam.Slots.Add(new TeamDataSlot(unitDB));

    }

    public EncounterData CurrentEncounter;
    public TeamData CurrentTeam;
}


public class BadgeData
{
    public string Name;
    public int BadgeNameId;
    public int BadgeDescId;
    public int BadgeTownId;
    public int LeagueId;
}


