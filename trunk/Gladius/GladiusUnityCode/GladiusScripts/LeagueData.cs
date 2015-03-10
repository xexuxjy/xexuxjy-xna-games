using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius;
using UnityEngine;

public class ArenaData
{
    public string ArenaName
    { get; set; }

    public string LeagueFile
    { get; set; }

    public string OfficeName
    { get; set; }

    public string OfficeDesc
    { get; set; }

    public string TagLine1
    { get; set; }

    public string TagLine2
    { get; set; }

    public String BackgroundTextureName
    { get; set; }

    public Texture BackgroundTexture
    { get; set; }

    public String OwnerThumbnailName
    { get; set; }

    public Texture OwnerThumnnailTexture
    { get; set; }

    public int LeaguePointsNeeded
    {
        get;
        set;
    }

    public String Officer
    {
        get;
        set;
    }

    public Boolean DataLoaded
    { get; set; }


    public void LoadData()
    {
        if (!DataLoaded)
        {
            TextAsset textAsset = (TextAsset)Resources.Load(LeagueFile);
            if (textAsset != null)
            {
                Debug.Log("Loading ArenaData file : " + LeagueFile);
                String data = textAsset.text;
                ParseExtractedData(data);
            }
            DataLoaded = true;
        }
    }


    public void ParseExtractedData(String data)
    {
        String[] allLines = data.Split('\n');

        int counter = 0;
        char[] splitTokens = new char[] { ':', ',', '\t' };

        LeagueData leagueData = null;
        ArenaEncounter encounter = null;

        while (counter < allLines.Length)
        {
            String line = allLines[counter++];
            if (line.StartsWith("//"))
            {
                continue;
            }
            String[] tokens = GladiusGlobals.SplitAndTidyString(line, splitTokens);

            if (tokens.Length == 0)
            {
                continue;
            }

            if (tokens[0] == ("OFFICENAME"))
            {
                OfficeName = tokens[1];
            }
            else if (tokens[0] == ("OFFICEDESC"))
            {
                OfficeDesc = tokens[1];
            }
            else if (tokens[0] == ("TAGLINE1"))
            {
                TagLine1 = tokens[1];
            }
            else if (tokens[0] == ("TAGLINE2"))
            {
                TagLine2 = tokens[1];
            }
            else if (tokens[0] == ("LEAGUEPTSNEEDED"))
            {
                LeaguePointsNeeded = int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("OFFICER"))
            {
                Officer = tokens[1];
            }
            else if (tokens[0] == ("RECRUIT"))
            {
                ArenaRecruit r = ArenaRecruit.FromRowData(tokens);
                Recruits.Add(r);
            }
            else if (tokens[0] == ("SCHOOL"))
            {
                ArenaSchool s = ArenaSchool.FromRowData(tokens);
                Schools.Add(s);
            }
            else if (tokens[0] == ("LEAGUE"))
            {
                encounter = null;
                leagueData = new LeagueData();
                Leagues.Add(leagueData);
                leagueData.Name = tokens[1];
                leagueData.Id = tokens[2];
            }
            else if (tokens[0] == ("ENCOUNTER"))
            {
                encounter = new ArenaEncounter();
                encounter.Name = tokens[1];
                encounter.Id = tokens[2];
                leagueData.ArenaEncounters.Add(encounter);
            }
            else if (tokens[0] == ("LEAGUEDESC"))
            {
                leagueData.DescriptionId = tokens[1];
            }
            else if (tokens[0] == ("LEAGUEPTS"))
            {
                leagueData.LeaguePoints = int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("ENCPTSNEEDED"))
            {
                leagueData.EncounterPointsNeeded = int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("MINPOP"))
            {
                leagueData.MinimumPopularity = int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("TIER"))
            {
                leagueData.Tier = int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("ONHOVER"))
            {
                if (encounter != null)
                {
                    encounter.OnHover1 = int.Parse(tokens[1]);
                    encounter.OnHover2 = int.Parse(tokens[2]);
                }
                else
                {
                    leagueData.OnHover1 = int.Parse(tokens[1]);
                    leagueData.OnHover2 = int.Parse(tokens[2]);
                }
            }
            else if (tokens[0] == ("ONSELECT"))
            {
                if (encounter != null)
                {
                    encounter.OnSelect1 = int.Parse(tokens[1]);
                    encounter.OnSelect2 = int.Parse(tokens[2]);
                }
                else
                {
                    leagueData.OnSelect1 = int.Parse(tokens[1]);
                    leagueData.OnSelect2 = int.Parse(tokens[2]);
                }
            }
            else if (tokens[0] == ("ONWIN"))
            {
                if (encounter != null)
                {
                    encounter.OnWin = int.Parse(tokens[1]);
                }
                else
                {
                    leagueData.OnWin = int.Parse(tokens[1]);
                }
            }
            else if (tokens[0] == ("PRIZECOMPLETION"))
            {
                leagueData.PrizeCompletion = tokens[1];
            }
            else if (tokens[0] == ("PRIZEMASTERY"))
            {
                leagueData.PrizeMastery = tokens[1];
            }
            else if (tokens[0] == ("HERO"))
            {
                leagueData.Hero = tokens[1];
            }
            else if (tokens[0] == ("PRIZETIER"))
            {
                //int which = int.Parse(tokens[2]);
                //if (which == 0)
                //{
                //    encounter.PrizeTier1 = tokens[1];
                //}
                //else if (which == 1)
                //{
                //    encounter.PrizeTier2 = tokens[1];
                //}
                //else if (which == 2)
                //{
                //    encounter.PrizeTier2 = tokens[2];
                //}
                //else
                //{
                //    Debug.LogError("unexpected prize tier : " + tokens[1]);
                //}
            }
        }
    }


    public List<ArenaRecruit> Recruits = new List<ArenaRecruit>();
    public List<ArenaSchool> Schools = new List<ArenaSchool>();
    public List<LeagueData> Leagues = new List<LeagueData>();
}


public class ArenaRecruit
{
    public static ArenaRecruit FromRowData(String[] inputTokens)
    {
        ArenaRecruit recruit = new ArenaRecruit();
        int count = 0;
        string[] rowData = inputTokens[1].Split(' ');
        List<string> tidied = new List<string>();
        for (int i = 0; i < rowData.Length; ++i)
        {
            if (!String.IsNullOrEmpty(rowData[i]))
            {
                tidied.Add(rowData[i]);
            }
        }
        recruit.Name = rowData[count++];
        recruit.val1 = int.Parse(tidied[count++]);
        recruit.val2 = int.Parse(tidied[count++]);
        recruit.val3 = int.Parse(tidied[count++]);
        recruit.val4 = int.Parse(tidied[count++]);
        recruit.val5 = int.Parse(tidied[count++]);
        recruit.val6 = int.Parse(tidied[count++]);
        recruit.val7 = int.Parse(tidied[count++]);
        recruit.val8 = int.Parse(tidied[count++]);
        recruit.val9 = int.Parse(tidied[count++]);

        return recruit;
    }

    public String Name
    { get; set; }

    public int val1;
    public int val2;
    public int val3;
    public int val4;
    public int val5;
    public int val6;
    public int val7;
    public int val8;
    public int val9;


}

public class ArenaSchool
{
    public string Name
    {
        get;
        set;
    }

    public int Level
    {
        get;
        set;
    }

    public static ArenaSchool FromRowData(String[] tokens)
    {
        ArenaSchool leagueSchool = new ArenaSchool();
        leagueSchool.Name = tokens[1];
        leagueSchool.Level = int.Parse(tokens[2]);
        return leagueSchool;
    }

}


public class ArenaEncounter
{
    /*
     * ENCOUNTER	"TrikataOpen3", 2554
    ENCDESC		5917
    ENCFILE		"imperia\bloodyhalo\TrikataOpen3.enc"
    ENCPTS		1
    ENTRYFEE	500
    TYPE		"Open"
    TEAMS		1 3 3 0 0
    ONHOVER		0	0
    ONSELECT	0	0
    ONWIN		5938
    PRIZETIER	"SyrnaCash0" 0
    PRIZETIER	"SyrnaCash01" 1
    PRIZETIER	"SyrnaCash02" 2
    FREQUENCY	100
*/

    public string Name
    { get; set; }
    public string Id
    { get; set; }
    public string EncounterFile
    { get; set; }
    public int EncounterPoints
    { get; set; }
    public int EntryFee
    { get; set; }

    public string Type
    { get; set; }

    public int[] Teams = new int[5];


    public int OnHover1
    {
        get;
        set;
    }
    public int OnHover2
    {
        get;
        set;
    }
    public int OnSelect1
    { get; set; }

    public int OnSelect2
    { get; set; }

    public int OnWin
    { get; set; }

    public string PrizeTier1
    { get; set; }
    public string PrizeTier2
    { get; set; }
    public string PrizeTier3
    { get; set; }

    public int Frequency
    {
        get;
        set;
    }

    }


    public class LeagueData
    {
        /*
    LEAGUE		"Judgement of Trikata League", 2191
    LEAGUEDESC	3756
    LEAGUEPTS	2
    ENCPTSNEEDED	6
    MINPOP		150
    TIER		0
    ONHOVER		5740	100
    ONSELECT	5744	100
    ONWIN		5745
    PRIZECOMPLETION	"TrikataTribunalPrize" 0
    PRIZECOMPLETION	"TrikataTribunalPrize" 1
    PRIZECOMPLETION	"TrikataTribunalPrize" 2
    PRIZEMASTERY	"TrikataCash0" 0
    PRIZEMASTERY	"TrikataCash01" 1
    PRIZEMASTERY	"TrikataCash02" 2
        */
    public String Name
    { get; set; }

    public String Id
    { get; set; }

    public String DescriptionId
    { get; set; }

    public int LeaguePoints
    { get; set; }

    public int EncounterPointsNeeded
    { get; set; }

    public int MinimumPopularity
    { get; set; }

    public int Tier
    { get; set; }

    public String Hero
    { get; set; }

    public String Badge1
    { get; set; }

    public String Badge2
    { get; set; }

    public int OnHover1
    {
        get;
        set;
    }
    public int OnHover2
    {
        get;
        set;
    }
    public int OnSelect1
    { get; set; }

    public int OnSelect2
    { get; set; }

    public int OnWin
    { get; set; }

    public String PrizeCompletion
    { get; set; }

    public String PrizeMastery
    { get; set; }

    //public List<BattleData> Battles = new List<BattleData>();
    public List<ArenaEncounter> ArenaEncounters = new List<ArenaEncounter>();
}




public static class LeagueManager
{
    public static ArenaData Load(String path)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(path);
        if (textAsset != null)
        {
            Debug.Log("Loading ArenaData file : "+path);
            String data = textAsset.text;
            return ParseExtractedData(data);
        }
        return null;
    }


    public static ArenaData ParseExtractedData(String data)
    {
        String[] allLines = data.Split('\n');

        int counter = 0;
        char[] splitTokens = new char[] { ':', ',', '\t' };

        ArenaData arenaData = new ArenaData();
        LeagueData leagueData = null;
        ArenaEncounter encounter = null;
        //LeagueData leagueData = new LeagueData();
        //GladiusGlobals.CurrentShop = shop;

        while (counter < allLines.Length)
        {
            String line = allLines[counter++];
            if (line.StartsWith("//"))
            {
                continue;
            }
            String[] tokens = GladiusGlobals.SplitAndTidyString(line, splitTokens);

            if (tokens.Length == 0)
            {
                continue;
            }

            if (tokens[0] == ("OFFICENAME"))
            {
                arenaData.OfficeName = tokens[1];
            }
            else if (tokens[0] == ("OFFICEDESC"))
            {
                arenaData.OfficeDesc = tokens[1];
            }
            else if (tokens[0] == ("TAGLINE1"))
            {
                arenaData.TagLine1 = tokens[1];
            }
            else if (tokens[0] == ("TAGLINE2"))
            {
                arenaData.TagLine2 = tokens[1];
            }
            else if (tokens[0] == ("LEAGUEPTSNEEDED"))
            {
                arenaData.LeaguePointsNeeded = int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("OFFICER"))
            {
                arenaData.Officer = tokens[1];
            }
            else if (tokens[0] == ("RECRUIT"))
            {
                ArenaRecruit r = ArenaRecruit.FromRowData(tokens);
                arenaData.Recruits.Add(r);
            }
            else if (tokens[0] == ("SCHOOL"))
            {
                ArenaSchool s = ArenaSchool.FromRowData(tokens);
                arenaData.Schools.Add(s);
            }
            else if (tokens[0] == ("LEAGUE"))
            {
                encounter = null;
                leagueData = new LeagueData();
                arenaData.Leagues.Add(leagueData);
                leagueData.Name = tokens[1];
                leagueData.Id = tokens[2];
            }
            else if (tokens[0] == ("ENCOUNTER"))
            {
                encounter = new ArenaEncounter();
                encounter.Name = tokens[1];
                encounter.Id = tokens[2];
                leagueData.ArenaEncounters.Add(encounter);
            }
            else if (tokens[0] == ("LEAGUEDESC"))
            {
                leagueData.DescriptionId = tokens[1];

            }
            else if (tokens[0] == ("LEAGUEPTS"))
            {
                leagueData.LeaguePoints = int.Parse(tokens[1]); 
            }
            else if (tokens[0] == ("ENCPTSNEEDED"))
            {
                leagueData.EncounterPointsNeeded= int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("MINPOP"))
            {
                leagueData.MinimumPopularity = int.Parse(tokens[1]); 
            }
            else if (tokens[0] == ("TIER"))
            {
                leagueData.Tier = int.Parse(tokens[1]);
            }
            else if (tokens[0] == ("ONHOVER"))
            {
                if (encounter != null)
                {
                    encounter.OnHover1 = int.Parse(tokens[1]);
                    encounter.OnHover2 = int.Parse(tokens[2]);
                }
                else
                {
                    leagueData.OnHover1 = int.Parse(tokens[1]);
                    leagueData.OnHover2 = int.Parse(tokens[2]);
                }
            }
            else if (tokens[0] == ("ONSELECT"))
            {
                if (encounter != null)
                {
                    encounter.OnSelect1 = int.Parse(tokens[1]);
                    encounter.OnSelect2 = int.Parse(tokens[2]);
                }
                else
                {
                    leagueData.OnSelect1 = int.Parse(tokens[1]);
                    leagueData.OnSelect2 = int.Parse(tokens[2]);
                }
            }
            else if (tokens[0] == ("ONWIN"))
            {
                if (encounter != null)
                {
                    encounter.OnWin = int.Parse(tokens[1]);
                }
                else
                {
                    leagueData.OnWin = int.Parse(tokens[1]);
                }
            }
            else if (tokens[0] == ("PRIZECOMPLETION"))
            {
                leagueData.PrizeCompletion = tokens[1];
            }
            else if (tokens[0] == ("PRIZEMASTERY"))
            {
                leagueData.PrizeMastery = tokens[1];
            }
            else if (tokens[0] == ("HERO"))
            {
                leagueData.Hero = tokens[1];
            }
            else if (tokens[0] == ("PRIZETIER"))
            {
                //int which = int.Parse(tokens[2]);
                //if (which == 0)
                //{
                //    encounter.PrizeTier1 = tokens[1];
                //}
                //else if (which == 1)
                //{
                //    encounter.PrizeTier2 = tokens[1];
                //}
                //else if (which == 2)
                //{
                //    encounter.PrizeTier2 = tokens[2];
                //}
                //else
                //{
                //    Debug.LogError("unexpected prize tier : " + tokens[1]);
                //}
            }
        }

        return arenaData;
    }



}


public class BattleData
{
    public int ID;
    public String Name;
    public int MinLevel;
    public int MinRenown;
    public SchoolRank MinRank;
    public List<ActorCategory> PermittedCategories = new List<ActorCategory>();
    public List<ActorCategory> ProhitiedCategories = new List<ActorCategory>();
    public int EntryCost;
    public int GoldReward;
    public int XPReward;
    public int TreasureDropRank;
    public int NumPoints;



    public static BattleData GenerateDummyBattleData()
    {
        BattleData battleData = new BattleData();
        battleData.Name = "Dummy Battle";
        battleData.GoldReward = 1500;
        battleData.XPReward = 1000;
        battleData.TreasureDropRank = 1;
        battleData.NumPoints = 1;
        return battleData;
    }


}
