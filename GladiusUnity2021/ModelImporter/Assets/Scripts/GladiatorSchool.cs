using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;

[Serializable]
public class GladiatorSchool
{
    public static Color[] SchoolColours = new Color[] { Color.green, Color.blue, Color.magenta, Color.red };
    public const string HeroUrsula = "Ursula";
    public const string HeroValens= "Valens";


    public GladiatorSchool()
    {
        Gladiators = new UnitList();
        GladiatorHires = new UnitList();
        CurrentParty = new UnitList();
        TownPopularity = new Dictionary<string, int>();
        LeagueEncounterWins = new Dictionary<int, int>();
        EncounterScores =  new Dictionary<string, int>();
        LeagueScores = new Dictionary<string, int>();
        CompletedBadges = new List<string>();
    }


    public string Name
    { get; set; }

    public string HeroName
    { get; set; }

    public SchoolRank SchoolRank
    {get;set;}

    public int SchoolColourId
    {get; set;}

    public UnitList Gladiators
    { get; set; }

    public UnitList GladiatorHires
    { get; set; }

    public UnitList CurrentParty
    { get; set; }

    private int m_days;
    public int Days
    {
        get
        {
            return m_days;
        }
        set
        {
            m_days = value;
            if (DaysChanged != null)
            {
                DaysChanged(m_days);
            }
        }

    }

    public float TimeOfDay
    { get; set; }


    private int m_gold;
    public int Gold
    {
        get
        {
            return m_gold;
        }
        set
        {
            m_gold = value;
            if (GoldChanged != null)
            {
                GoldChanged(m_gold);
            }
        }
    }

    public Dictionary<String, int> TownPopularity
    { get; set; }

    public Dictionary<int, int> LeagueEncounterWins
    { get; set; }

    public List<String> CompletedBadges
    { get; set; }



    public Dictionary<string, int> EncounterScores
    { get; set; }
    public Dictionary<string, int> LeagueScores
    { get; set; }


    // per battle value
    public bool AllDead
    {
        get; set;
    }

    
    public bool PlayerTeam
    { get; set; }


    ItemStore m_itemStore = new ItemStore();



    public void Recruit(CharacterData gladiator)
    {
        System.Diagnostics.Debug.Assert(!Gladiators.Contains(gladiator));
        Gladiators.Add(gladiator);
    }

    public void Hire(CharacterData gladiator)
    {
        System.Diagnostics.Debug.Assert(!Gladiators.Contains(gladiator));
        GladiatorHires.Add(gladiator);
    }

    public void Fire(CharacterData gladiator)
    {
        System.Diagnostics.Debug.Assert(Gladiators.Contains(gladiator));
        Gladiators.Remove(gladiator);
    }




    public event Action<int> DaysChanged;
    public event Action<int> GoldChanged;




    public int GetMaxSize()
    {
        switch (SchoolRank)
        {
            case SchoolRank.Bronze: return 8;
            case SchoolRank.Silver: return 16;
            case SchoolRank.Gold: return 24;
        }
        System.Diagnostics.Debug.Assert(false);
        return -1;
    }


    public CharacterData GetGladiator(string name)
    {
        return Gladiators.Find(x => x.Name == name);
    }

    public void SetCurrentParty(UnitList party)
    {
        CurrentParty.Clear();
        CurrentParty.AddRange(party);
    }


    public void Load(String name)
    {
        String data = GladiusGlobals.ReadTextAsset(GladiusGlobals.SchoolsPath + name);
        Parse(data);
    }

    public HashSet<String> classSet = new HashSet<String>();

    public void Parse(String data)
    {
        String[] lines = data.Split('\n');

        CharacterData currentCharacterData = null;
        String currentCharacterName = "";
        String currentCharacterClass = "";
        int currentCharacterLevel = 1;

        char[] tokens = new char[] { ',', ':' };

        for (int counter = 0; counter < lines.Length; counter++)
        {
            try
            {

                String line = lines[counter].Trim();
                if (line.StartsWith("//"))
                {
                    continue;
                }


                String[] lineTokens = GladiusGlobals.SplitAndTidyString(line, tokens);
                if (lineTokens.Length == 0)
                {
                    continue;
                }


                if (lineTokens[0] == "NAME")
                {
                    Name = lineTokens[1];
                }
                else if (lineTokens[0] == "HERO")
                {
                    HeroName = lineTokens[1];
                }
                else if (lineTokens[0] == "GOLD")
                {
                    Gold = int.Parse(lineTokens[1]);
                }
                else if (lineTokens[0] == "CREATEUNIT")
                {
                    currentCharacterName = lineTokens[1];
                    currentCharacterClass = lineTokens[2];


                }
                else if (lineTokens[0] == "LEVEL")
                {
                    int level = int.Parse(lineTokens[1]);
                    currentCharacterData = ActorGenerator.CreateCharacterForLevel(currentCharacterClass, level);
                    currentCharacterData.Name = currentCharacterName;
                    Recruit(currentCharacterData);

                }
                else if (lineTokens[0] == "EXPERIENCE")
                {
                    currentCharacterData.Experience = int.Parse(lineTokens[1]);
                }
                else if (lineTokens[0] == "JOBPOINTS")
                {
                    currentCharacterData.JobPoints = int.Parse(lineTokens[1]);
                }
                else if (lineTokens[0] == "CORESTATSCOMP2")
                {
                    //            	CON, PWR, ACC, DEF, INT, MOVE
                    currentCharacterData.CON = int.Parse(lineTokens[1]);
                    currentCharacterData.PWR = int.Parse(lineTokens[2]);
                    currentCharacterData.ACC = int.Parse(lineTokens[3]);
                    currentCharacterData.DEF = int.Parse(lineTokens[4]);
                    currentCharacterData.INI = int.Parse(lineTokens[5]);
                    currentCharacterData.MOVEMENTRATE = float.Parse(lineTokens[6]);
                }
                else if (lineTokens[0] == "ITEMSCOMP")
                {
                    //	weapon,	armor,	shield,	helmet,	accessory
                    if (lineTokens.Length > 1)
                    {
                        currentCharacterData.AddItemByNameAndLoc(lineTokens[1], ItemLocation.Weapon);
                    }
                    if (lineTokens.Length > 2)
                    {
                        currentCharacterData.AddItemByNameAndLoc(lineTokens[2], ItemLocation.Armor);
                    }
                    if (lineTokens.Length > 3)
                    {
                        currentCharacterData.AddItemByNameAndLoc(lineTokens[3], ItemLocation.Shield);
                    }
                    if (lineTokens.Length > 4)
                    {
                        currentCharacterData.AddItemByNameAndLoc(lineTokens[4], ItemLocation.Helmet);
                    }
                    if (lineTokens.Length > 5)
                    {
                        currentCharacterData.AddItemByNameAndLoc(lineTokens[5], ItemLocation.Accessory);
                    }
                }
                else if (lineTokens[0] == "SKILL")
                {
                    currentCharacterData.LearnSkill(lineTokens[1]);
                }
                else if (lineTokens[0] == "INVENTORY")
                {
                    AddItem(lineTokens[1]);
                }
            }
            catch(Exception e)
            {
                int ibreak = 0;
            }
        }
    }





    public void AddItem(String item)
    {
        m_itemStore.AddItem(item);
    }

    public void RemoveItem(String item)
    {
        m_itemStore.RemoveItem(item);
    }

    public int GetItemCount(string item)
    {
        return m_itemStore.GetItemCount(item);
    }


    public Color GetSchoolColour()
    {
        return SchoolColours[SchoolColourId];
    }

    
    public int GetTownPopularity(string name)
    {
        int val = 0;
        TownPopularity.TryGetValue(name, out val);
        return val;
    }

    public void SetTownPopularity(string name, int val)
    {
        TownPopularity[name] = val;
    }



    public bool EncounterValid(GladiatorSchool school, LeagueEncounterData encounterData)
    {
        return true;
        //encounterData.
    }

    public bool LeagueValid(GladiatorSchool school, LeagueData leagueData)
    {
        bool leagueValid = false;
        bool hasAllBadges = true;
        foreach(BadgeData badgeData in leagueData.RequiredBadges)
        {
            if (!CompletedBadges.Contains(badgeData.Name))
            {
                hasAllBadges = false;
                break;
            }
        }

        int townPopularity = TownPopularity[leagueData.ArenaOffice.TownData.Name];
        bool popValid = townPopularity >= leagueData.MinimumPopularity;

        bool tierValid = leagueData.Tier <= (int)SchoolRank;

        leagueValid = hasAllBadges && popValid && tierValid;

        return leagueValid;
    }

    public int GetScoreForEncounter(string name)
    {
        int score = 0;
        if (!EncounterScores.TryGetValue(name, out score))
        {
            EncounterScores[name] = score;
        }
        return score;
    }

    public void SetScoreForEncounter(string name,int score)
    {
        EncounterScores[name] = score;
    }

    public void SetScoreForLeague(string name, int score)
    {
        LeagueScores[name] = score;
    }

    public int GetScoreForLeague(string name)
    {
        int score = 0;
        if (!LeagueScores.TryGetValue(name, out score))
        {
            LeagueScores[name] = score;
        }
        return score;
    }



    public int GetPointsForLeague(string leagueName)
    {
        int totalScore = 0;
        LeagueData leagueData = TownManager.LeagueDataInfo[leagueName];
        foreach (LeagueEncounterData leagueEncounterData in leagueData.Encounters)
        {
            totalScore += GetScoreForEncounter(leagueEncounterData.Name);
        }
        return totalScore;
    }


    public void SetLeagueMaxScores()
    {
        SchoolRank = SchoolRank.Gold;

        foreach (TownData townData in TownManager.TownDictionary.Values)
        {
            SetTownPopularity(townData.Name,100);
            if (townData.ArenaOffice != null)
            {
                foreach (LeagueData leagueData in townData.ArenaOffice.Leagues)
                {
                    SetScoreForLeague(leagueData.Name, leagueData.LeaguePoints);
                    foreach (BadgeData badgeData in leagueData.RequiredBadges)
                    {
                        CompletedBadges.Add(badgeData.Name);
                    }
                    foreach (LeagueEncounterData leagueEncounterData in leagueData.Encounters)
                    {
                        SetScoreForEncounter(leagueEncounterData.Name, leagueEncounterData.EncounterPoints);
                    }
                }
            }
        }
        
    }


    



}

//}
