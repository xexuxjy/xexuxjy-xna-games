using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Antlr4.Runtime.Misc;
using System.Globalization;
using System.Text;




public static class ActorGenerator
{
    public static void BuildEncounterTeamData(int opponentTeamIndex, LeagueEncounterData leagueEncounterData, ArenaOffice arenaOffice)
    {
        if (opponentTeamIndex < leagueEncounterData.EncounterData.teams.Count)
        {
            TeamData teamData = leagueEncounterData.EncounterData.teams[opponentTeamIndex];

            string schoolName = null;
            if (string.IsNullOrEmpty(teamData.School.Name))
            {
                teamData.School.Name = "HOMETOWNSCHOOL";
            }

            if (teamData.School.Name == "HOMETOWNSCHOOL")
            {
                if (arenaOffice.Schools.Count > 0)
                {
                    schoolName = arenaOffice.Schools[0].name;
                }
            }
            else if (teamData.School.Name == "REGIONSCHOOL")
            {
                if (arenaOffice.Schools.Count > 0)
                {
                    schoolName = arenaOffice.Schools[0].name;
                }
            }
            else
            {
                schoolName = teamData.School.Name;
            }

            teamData.School.Name = schoolName;

            teamData.School.CurrentParty.Clear();

            GladiatorSchool school = null;
            //List<CharacterData> teamCharacters = new List<CharacterData>();
            if (ActorGenerator.AllSchools.TryGetValue(teamData.School.Name, out school))
            {
                teamData.School = school;
                SetupTeamData(teamData);
            }
            else
            {
                Debug.LogWarning("Unable to find school called : " + schoolName);
            }
        }
    }

    public static void SetupTeamData(TeamData teamData)
    {
        List<CharacterData> schoolCopy = new List<CharacterData>(teamData.School.Gladiators);
        List<CharacterData> chosenList = new List<CharacterData>();

        int numOpponents = teamData.Slots.Count;
        for (int i = 0; i < numOpponents; ++i)
        {
            int randomChoice = (int)(UnityEngine.Random.value * (float)schoolCopy.Count);
            if (randomChoice < schoolCopy.Count - 1)
            {
                CharacterData chosen = schoolCopy[randomChoice];
                teamData.School.CurrentParty.Add(chosen);
                schoolCopy.Remove(chosen);
            }
        }

        // put the information in the team sections
        teamData.SetCharactersFromSchool();
    }



    public static void ActorGeneratorInit()
    {
        if (ActorClassDefs.Count == 0)
        {
            InitClassCategories();
            LoadAllStatsFiles();
            LoadAllItemsFiles();
            LoadAllSkillsFiles();
            LoadAllGladiators();

            //Debug.LogFormat("Loaded [{0}] ClassDefs", ActorClassDefs.Count);
            //Debug.LogFormat("Loaded [{0}] Stats Files", ClassStatsMap.Count);
            //Debug.LogFormat("Loaded [{0}] Items Files", ClassItemsMap.Count);
            //Debug.LogFormat("Loaded [{0}] Skills Files", ClassSkillsMap.Count);
            //Debug.LogFormat("Loaded [{0}] Gladiators Schools", AllSchools.Count);
        }
    }

    public static void InitClassCategories()
    {
        ArenaOffice result = null;
        String data = GladiusGlobals.ReadTextAsset(GladiusGlobals.DataRoot + "ClassDefs");
        if (data != null)
        {
            var lexer = new GladiusClassDefLexer(new Antlr4.Runtime.AntlrInputStream(data));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GladiusClassDefParser(commonTokenStream);
            IParseTree parseTree = parser.root();
            MyGladiusClassDefParser listener = new MyGladiusClassDefParser();
            ParseTreeWalker.Default.Walk(listener, parseTree);
            ActorClassDefs.AddRange(listener.ClassDefs);
        }

    }
    
    // check for items being useable by a class.
    public static bool ItemValidForClass(Item item, ActorClassDef actorClassData)
    {
        bool isValid = false;
        List<ItemCategoryEntry> iceList = null;
        if (actorClassData.ItemCategoryMap.TryGetValue(item.ItemLocation, out iceList))
        {
            foreach (ItemCategoryEntry ice in iceList)
            {
                if (ice.itemLocation == item.ItemLocation)
                {
                    if (ice.filter1 == item.ItemSubType || ice.filter1 == "<Any>")
                    {
                        if (ice.filter2 == item.ClassInfo)
                        {
                            isValid = true;
                            break;
                        }
                    }
                }
            }   
        }
        return isValid;
    }

    private static List<string> Heros = new List<string>(new String[] { "eiji", "galverg", "ludo", "urlan", "ursula", "valens" });
    public static bool HeroCharacter(ActorClassDef acd)
    {
        return Heros.Contains(acd.unitSet);
    }

    private static List<string> BowUsers = new List<string>(new String[] { "archer", "archerf", "amazon", "eiji" });

    public static bool BowUser(ActorClassDef acd)
    {
        return BowUsers.Contains(acd.unitSet);
    }

    private static List<string> SpearThrowers = new List<string>(new String[] { "peltast", "peltastf", "gungnir", "gungnirf" });

    public static bool SpearThrower(ActorClassDef acd)
    {
        return SpearThrowers.Contains(acd.unitSet);
    }

    private static int RandomNameCount = 0;

    public static CharacterData CreateRandomCharacterForLevel(int level)
    {
        CharacterData result = null;
        while (true)
        {
            int randomCharacter = (int)((float)ActorClassDefs.Count * UnityEngine.Random.value);
            ActorClassDef def = ActorClassDefs[randomCharacter];
            
            if (!HeroCharacter(def))
            {
                result = new CharacterData();
                result.ClassDefName = def.name;

                result.Name = "Random" + (RandomNameCount++);
                return result;
            }
        }
        return null;
    }

    public static CharacterData CreateRandomCharacter(UnitDBRestriction restriciton, int level)
    {
        List<ActorClassDef> validClasses = new List<ActorClassDef>();

        ActorClassDef actorClass = null;
        foreach (ActorClassDef actorClassData in ActorClassDefs)
        {
            if (restriciton != null && string.Equals(actorClassData.name, restriciton.Class1, StringComparison.InvariantCultureIgnoreCase))
            {
                validClasses.Add(actorClassData);
            }
            //bool valid = true;
            //if (actorClassData.name.Contains("DarkGod"))
            //{
            //    continue;
            //}

            //if (actorClassData.name.Contains("Costume"))
            //{
            //    continue;
            //}

            //if (actorClassData.name.Contains("Affinity") || actorClassData.name.Contains("affinity"))
            //{
            //    continue;
            //}

            //if (actorClassData.name.Contains("Titan"))
            //{
            //    continue;
            //}

            //if (actorClassData.name.Contains("Wolf"))
            //{
            //    continue;
            //}

            //if (actorClassData.name.Contains("arbarianF"))
            //if (actorClassData.name.Contains("Centurion"))
            //if(actorClassData.name.Contains("Archer"))
            //if(actorClassData.name.Contains("Peltast"))
            //if (actorClassData.name.Contains("MurmilloF"))
            //if (actorClassData.name == "Murmillo")
            //{
            //    validClasses.Add(actorClassData);
            //}
            //if (restriciton.RequiredMask == 0 || (
            //    (((int)actorClassData.Mask) & restriciton.RequiredMask) != 0))
            //{
            //    // ignore hero characters for now...
            //    if (!HeroCharacter(actorClassData))
            //    {
            //        validClasses.Add(actorClassData);
            //    }
            //}
        }
        // pick one at random?
        int randomChar = UnityEngine.Random.Range(0, validClasses.Count - 1);
        if (randomChar < 0 || randomChar >= validClasses.Count)
        {
            int ibreak = 0;
        }
        actorClass = validClasses[randomChar];
        CharacterData cd = CreateCharacterForLevel(actorClass.name , level);

        // give the character some appropriate equipment.
        ItemSet itemData = ItemsForClassLevel(cd.CurrentClassDef.name, null, cd.Level);
        if(itemData != null)
        {
            itemData.Apply(cd);
        }

        return cd;
    }

    public static bool CheckLevelUp(CharacterData characterData, int xpGained)
    {
        return false;
    }


    public static CharacterData CreateCharacterForLevel(string classDefName,int level,string statSetName=null,string skillSetName=null)
    {
        CharacterData characterData = null;
        if (classDefName != null)
        {
            characterData = new CharacterData();
            characterData.ClassDefName = classDefName;
            characterData.Level = level + 1;

            // character data can act as an empty slot saying whats allowed , as well as being an actual character slot?
            String className = characterData.CurrentClassDef != null ? characterData.CurrentClassDef.name : "Unassigned";

            StatsSet resolvedStatsSet = StatForClassLevel(classDefName, statSetName, level);
            characterData.CopyModCoreStat(resolvedStatsSet);

            List< SkillSetItem> skillItemList = SkillSetForClassLevel(classDefName, skillSetName, level);

            foreach (SkillSetItem skillSetItem in skillItemList)
            {
                if (skillSetItem.MinLevel <= level)
                {
                    characterData.LearnSkill(skillSetItem.SkillName);
                }
            }

        }
        return characterData;

    }

    //public static CharacterData FromRecruitInfo(String name, int[] stats)
    //{
    //    CharacterData cd = new CharacterData();
    //    cd.Name = name;

    //    //fixme - use the character name to lookup all gladiator stats.


    //    return cd;

    //}


    public static void LoadAllStatsFiles()
    {

        TextAsset[] allFiles = GladiusGlobals.LoadAllTextAsset(GladiusGlobals.DataRoot + "StatFiles");
        foreach (TextAsset file in allFiles)
        {

            try
            {
                String[] lines = file.text.Split('\n');
                StatsSetBlock setBlock = LoadStatsFile(lines);

                StatsSetBlock existingBlock = null;
                if (!ClassStatsMap.TryGetValue(setBlock.MainClass, out existingBlock))
                {
                    ClassStatsMap[setBlock.MainClass] = setBlock;
                }
                else
                {
                    existingBlock.Merge(setBlock);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Exception loading stat files for [{0}][{1}]", file.name, e);
            }
        }
        int ibreak = 0;
    }


    public static StatsSetBlock LoadStatsFile(String[] fileData)
    {
        StatsSetBlock setBlock = null;
        if (fileData.Length > 0)
        {
            String className = "unk";
            setBlock = new StatsSetBlock(fileData[0]);

            List<String> shortList = new List<String>();
            char[] splitTokens = new char[] { ' ', ',' };
            for (int i = 1; i < fileData.Length; ++i)
            {
                if (fileData[i].StartsWith("MODCORESTATSCOMP:"))
                {
                    String[] tokens = fileData[i].Split(splitTokens);
                    shortList.Clear();
                    foreach (String token in tokens)
                    {
                        if (!String.IsNullOrEmpty(token))
                        {
                            shortList.Add(token);
                        }
                    }
                    int counter = 0;

                    String pad = shortList[counter++];
                    String variant = shortList[counter++].ToLower() ;
                    String level = shortList[counter++];
                    String con = shortList[counter++];
                    String pow = shortList[counter++];
                    String acc = shortList[counter++];
                    String def = shortList[counter++];
                    String ini = shortList[counter++];
                    String mov = shortList[counter++];

                    StatsSet item = new StatsSet();
                    item.ClassName = className;
                    item.VariantName = variant;
                    item.Level = int.Parse(level);
                    item.CON = int.Parse(con);
                    item.PWR = int.Parse(pow);
                    item.ACC = int.Parse(acc);
                    item.DEF = int.Parse(def);
                    item.INI = int.Parse(ini);
                    item.MOV = float.Parse(mov);

                    setBlock.AddItem(item);

                    int ibreak = 0;
                }
            }
        }
        return setBlock;
    }

    public static int AdjustLevel(int level)
    {
        level = GladiusGlobals.Clamp(level, 1, 30);
        level--;
        return level;
    }

    public static String GetUseClass(string className)
    {
        string useClass = null;
        ActorClassDef acd = ActorClassDefs.Find(x => x.name.Equals(className,StringComparison.CurrentCultureIgnoreCase));
        if (acd != null)
        {
            useClass = acd.unitSet;
            useClass = useClass.ToLower();
        }
        else
        {
            int ibreak = 0;
        }
        return useClass;
    }


    public static StatsSet StatForClassLevel(string className, string setName, int level)
    {
        String useClass = null;
        StatsSet set = null;
        if (className == null)
        {
            Debug.LogWarningFormat("StatForClassLevel error [{0}] [{1}]", className, setName);
        }
        else
        {

            useClass = GetUseClass(className);
            level = AdjustLevel(level);
            if (useClass != null)
            {
                StatsSetBlock setBlock = null;
                if (ClassStatsMap.TryGetValue(useClass, out setBlock))
                {
                    List<StatsSet> statRow = null;
                    // don't know the name, so get the first...
                    if (String.IsNullOrEmpty(setName))
                    {
                        foreach (var key in setBlock.NamedStatsSets.Keys)
                        {
                            setName = key;
                            break;
                        }
                    }
                    setName = setName.ToLower();

                    if (setBlock.NamedStatsSets.TryGetValue(setName, out statRow))
                    {
                        if (level < statRow.Count)
                        {
                            set = statRow[level];
                        }
                    }
                }
            }
        }
        if (set == null)
        {
            //Debug.LogWarningFormat("can't find class set for [{0}] [{1}] [{2}]",className,useClass,setName);
        }
        return set;
    }

    public static ItemSet ItemsForClassLevel(string className, string setName, int level)
    {
        String useClass = null;

        ItemSet set = null;
        if (className == null)
        {
            Debug.LogWarningFormat("ItemsForClassLevel error [{0}] [{1}]", className, setName);
        }
        else
        {
            useClass = GetUseClass(className);
            if (useClass != null)
            {

                ItemSetBlock setBlock = null;
                level = AdjustLevel(level);

                if (ClassItemsMap.TryGetValue(useClass, out setBlock))
                {
                    List<ItemSet> statRow = null;
                    // don't know the name, so get the first...
                    if (String.IsNullOrEmpty(setName))
                    {
                        foreach (var key in setBlock.SubBlockMap.Keys)
                        {
                            setName = key;
                            break;
                        }
                    }
                    setName = setName.ToLower();

                    if (setBlock.SubBlockMap.TryGetValue(setName, out statRow))
                    {
                        level = GladiusGlobals.Clamp(level, 0, statRow.Count - 1);
                        set = statRow[level];
                    }
                }
            }
        }
        if (set == null)
        {
            //Debug.LogWarningFormat("can't find item set for [{0}] [{1}] [{2}]", className, useClass,setName);
        }

        return set;
    }

    public static List<SkillSetItem> SkillSetForClassLevel(string className, string setName, int level)
    {
        String useClass = null;
        List<SkillSetItem> setList = null;
        SkillSetBlock setBlock = null;
        if (className == null)
        {
            Debug.LogWarningFormat("SkillSetForClassLevel error [{0}] [{1}]", className, setName);
        }
        else
        {

            useClass = GetUseClass(className);
            if (useClass != null)
            {
                if (ClassSkillsMap.TryGetValue(useClass, out setBlock))
                {
                    if (String.IsNullOrEmpty(setName))
                    {
                        foreach (var key in setBlock.SubBlockMap.Keys)
                        {
                            setName = key;
                            break;
                        }
                    }
                    setName = setName.ToLower();

                    setBlock.SubBlockMap.TryGetValue(setName, out setList);
                }
            }
        }
        if (setList == null)
        {
            //Debug.LogWarningFormat("can't find skill set for [{0}] [{1}] [{2}]", className, useClass, setName);
        }


        return setList;
    }


    /*
//name, class, customize info, stats, items, skills, school
NUMUNITS: 2173
"","","","","","",""
"Aaden","BanditAF","4#School_RedYellow1","Statset0","ItemSetNE","skillsetaffE","Fliuch Falcons"
"Aapehty","BeastEarth","","StatSet0","ItemSetEE","skillsetSumEarth","The Fearsome Foursome"
"Abagai","Amazon","2#","StatSet3","ItemSetSU","SkillSetComb","WildernessSteppesOutlaw"
"Abbah","BanditB","3#","StatSet0","ItemSetEA","SkillSetAffA","WildernessExpanseOutlaw"
     */

    public static void LoadAllGladiators()
    {
        String data = GladiusGlobals.ReadTextAsset(GladiusGlobals.DataRoot + "GladiatorNames");

        string[] lines = data.Split('\n');
        // data starts on the 3rd line
        for (int counter = 3; counter < lines.Length; counter++)
        {
            string line = lines[counter];

            string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ',' },true,false);
            if (lineTokens.Length == 7)
            {
                GladiatorData gladiatorData = new GladiatorData();
                gladiatorData.name = lineTokens[0];
                gladiatorData.className = lineTokens[1];
                gladiatorData.customizeInfo = lineTokens[2];
                gladiatorData.statSetName = lineTokens[3].ToLower();
                gladiatorData.itemSetName = lineTokens[4].ToLower();
                gladiatorData.skillSetName = lineTokens[5].ToLower();
                gladiatorData.schoolName = lineTokens[6];

                GladiatorSchool school = null;
                if (!AllSchools.TryGetValue(gladiatorData.schoolName, out school))
                {
                    school = new GladiatorSchool();
                    school.Name = gladiatorData.schoolName;
                    AllSchools[school.Name] = school;
                }
                CharacterData cd = FromGladiatorData(gladiatorData);
                school.Gladiators.Add(cd);
                AllGladiators.Add(gladiatorData.name, cd);
            }
            else
            {
                // error
                int ibreak = 0;
            }
        }
    }


    public static CharacterData FromGladiatorData(GladiatorData gd)
    {
        // find the mod stat;
        StatsSet statData = StatForClassLevel(gd.className, gd.statSetName, gd.level);
        ItemSet itemData = ItemsForClassLevel(gd.className, gd.itemSetName, gd.level);
        List<SkillSetItem> skillSetList = SkillSetForClassLevel(gd.className, gd.skillSetName, gd.level);

        CharacterData cd = new CharacterData();
        cd.ClassDefName = gd.className;
        cd.Name = gd.name;
        
        // 
        //TeamData teamData = new TeamData();
        //teamData.name = gd.schoolName;
        //cd.TeamData = teamData;
        ////cd.SchoolName = gd.schoolName;
        
        if (statData != null)
        {
            statData.Apply(cd);
        }
        if (itemData != null)
        {
            itemData.Apply(cd);
        }
        if (skillSetList != null)
        {
            foreach (SkillSetItem skill in skillSetList)
            {
                cd.LearnSkill(skill.SkillName);
            }
        }
        return cd;
    }


    public static void LoadAllItemsFiles()
    {

        TextAsset[] allFiles = GladiusGlobals.LoadAllTextAsset(GladiusGlobals.DataRoot + "ItemFiles");
        foreach (TextAsset file in allFiles)
        {
            String data = file.text;

            var lexer = new GladiusItemSetLexer(new Antlr4.Runtime.AntlrInputStream(data));
            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);

            var parser = new GladiusItemSetParser(commonTokenStream);
            IParseTree parseTree = parser.root();
            MyGladiusItemSetParser listener = new MyGladiusItemSetParser();
            listener.ClassItemsMap = ClassItemsMap;

            ParseTreeWalker.Default.Walk(listener, parseTree);
        }
    }


    public static void LoadAllSkillsFiles()
    {
        TextAsset[] allFiles = GladiusGlobals.LoadAllTextAsset(GladiusGlobals.DataRoot + "SkillFiles");
        foreach (TextAsset file in allFiles)
        {
            try
            {
                String[] lines = file.text.Split('\n');
                SkillSetBlock setBlock = LoadSkillsFile(lines);
                if (setBlock != null && setBlock.MainClass != null)
                {
                    SkillSetBlock existingBlock = null;
                    if (!ClassSkillsMap.TryGetValue(setBlock.MainClass, out existingBlock))
                    {
                        ClassSkillsMap[setBlock.MainClass] = setBlock;
                    }
                    else
                    {
                        existingBlock.Merge(setBlock);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Exception loading skill files for [{0}][{1}]", file.name, e);
            }

        }
    }

    public static SkillSetBlock LoadSkillsFile(String[] lines)
    {
        ////// SKILLSET export Class: Scorpion Scorpion  Affinity: None
        //MODLEVELRANGESKILL:  skillsetcomb,  0, 30, "Scorpion Attack"                 // JP:   0/  0	AccMod -2
        //MODLEVELRANGESKILL:  skillsetcomb,  0, 30, "Scorpion Evade"                  // JP:   0/  0	AccMod 0

        String skillSetLine = null;
        foreach (String s in lines)
        {
            if (s.Contains("SKILLSET"))
            {
                skillSetLine = s;
                break;
            }
        }

        if (skillSetLine == null)
        {
            return null;
        }
        SkillSetBlock currentSetBlock = new SkillSetBlock(skillSetLine);

        foreach (String line in lines)
        {
            //if (line.StartsWith("////// SKILLSET"))
            //{
            //    skillSets.Add(currentSetBlock);
            //}

            if (line.StartsWith("MODLEVELRANGESKILL:"))
            {

                string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ',', ':' });
                if (lineTokens.Length == 5)
                {
                    SkillSetItem skill = new SkillSetItem(lineTokens);
                    currentSetBlock.AddItem(skill);
                }

            }
        }
        return currentSetBlock;
    }


    public static Dictionary<String, SkillSetBlock> ClassSkillsMap = new Dictionary<string, SkillSetBlock>();
    public static Dictionary<String, ItemSetBlock> ClassItemsMap = new Dictionary<string, ItemSetBlock>();
    public static Dictionary<String, StatsSetBlock> ClassStatsMap = new Dictionary<string, StatsSetBlock>();
    public static List<ActorClassDef> ActorClassDefs = new List<ActorClassDef>();


    public static Dictionary<String, GladiatorSchool> AllSchools = new Dictionary<string, GladiatorSchool>();
    public static Dictionary<String, CharacterData> AllGladiators = new Dictionary<string, CharacterData>();

}

public class GladiatorData
{
    public string name;
    public string className;
    public string customizeInfo;
    public string statSetName;
    public string itemSetName;
    public string skillSetName;
    public string schoolName;
    public int level = 1;
}



public class StatsSetBlock
{
    public StatsSetBlock(String line)
    {
        string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 4)
        {
            MainClass = lineTokens[3];
            MainClass = MainClass.ToLower();
        }
        else
        {
            int ibreak = 0;
        }

    }

    public void AddItem(StatsSet set)
    {
        List<StatsSet> setList = null;
        if (!NamedStatsSets.TryGetValue(set.VariantName, out setList))
        {
            setList = new List<StatsSet>();
            NamedStatsSets[set.VariantName] = setList;
        }

        setList.Add(set);

    }

    public void Merge(StatsSetBlock sb)
    {
        if (sb.MainClass == MainClass)
        {
            foreach (String key in sb.NamedStatsSets.Keys)
            {
                List<StatsSet> currentVals;
                if (!NamedStatsSets.TryGetValue(key, out currentVals))
                {
                    currentVals = new List<StatsSet>();
                    NamedStatsSets[key] = currentVals;
                }
                currentVals.AddRange(sb.NamedStatsSets[key]);
            }

        }
    }
    public string MainClass;
    public Dictionary<String, List<StatsSet>> NamedStatsSets = new Dictionary<string, List<StatsSet>>();

}


public class StatsSet
{
    public String ClassName;
    public String VariantName;
    public int Level;
    public float CON;
    public float PWR;
    public float ACC;
    public float DEF;
    public float INI;
    public float MOV;


    public void Apply(CharacterData characterData)
    {
        characterData.Level = Level;
        characterData.CON = CON;
        characterData.PWR = PWR;
        characterData.ACC = ACC;
        characterData.DEF = DEF;
        characterData.INI = INI;
        characterData.MOVEMENTRATE = MOV;
    }


}

public class ItemSet
{
    //public ModITEMStat(ItemSetSubBlock ownerBlock, String[] fields)
    public ItemSet()
    {
    }

    public void Apply(CharacterData characterData)
    {
        if (!String.IsNullOrEmpty(Weapon))
        {
            characterData.AddItemByNameAndLoc(Weapon, ItemLocation.Weapon);
        }
        if (!String.IsNullOrEmpty(Armor))
        {
            characterData.AddItemByNameAndLoc(Armor, ItemLocation.Armor);
        }
        if (!String.IsNullOrEmpty(Shield))
        {
            characterData.AddItemByNameAndLoc(Shield, ItemLocation.Shield);
        }
        if (!String.IsNullOrEmpty(Helmet))
        {
            characterData.AddItemByNameAndLoc(Helmet, ItemLocation.Helmet);
        }
        if (!String.IsNullOrEmpty(Accessory))
        {
            characterData.AddItemByNameAndLoc(Accessory, ItemLocation.Accessory);
        }
    }

    public string VariantName;
    //ItemSetSubBlock OwnerBlock;
    public int MinLevel;
    public int MaxLevel;
    public string Weapon;
    public string Armor;
    public string Shield;
    public string Helmet;
    public string Accessory;

}

public class ItemSetBlock
{
    public ItemSetBlock()
    {
    }

    public void AddItem(ItemSet itemStat)
    {
        List<ItemSet> statList = null;
        if (!SubBlockMap.TryGetValue(itemStat.VariantName, out statList))
        {
            statList = new List<ItemSet>();
            SubBlockMap[itemStat.VariantName] = statList;
        }

        statList.Add(itemStat);

    }


    public string MainClass;
    public string SubClass;
    public string Region;
    string Affinity;
    public Dictionary<String, List<ItemSet>> SubBlockMap = new Dictionary<string, List<ItemSet>>();

}


public class SkillSetBlock
{
    public SkillSetBlock(String line)
    {
        string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 7)
        {
            MainClass = lineTokens[4];
            MainClass = MainClass.ToLower();
            SubClass = lineTokens[3];
            //      Affinity = lineTokens[7];
        }
        else
        {
            int ibreak = 0;
        }
    }

    public void AddItem(SkillSetItem skillStat)
    {
        List<SkillSetItem> statList = null;
        if (!SubBlockMap.TryGetValue(skillStat.VariantName, out statList))
        {
            statList = new List<SkillSetItem>();
            SubBlockMap[skillStat.VariantName] = statList;
        }

        statList.Add(skillStat);

    }

    public void Merge(SkillSetBlock sb)
    {
        if (sb.MainClass == MainClass)
        {
            foreach (String key in sb.SubBlockMap.Keys)
            {
                List<SkillSetItem> currentVals;
                if (!SubBlockMap.TryGetValue(key, out currentVals))
                {
                    currentVals = new List<SkillSetItem>();
                    SubBlockMap[key] = currentVals;
                }
                currentVals.AddRange(sb.SubBlockMap[key]);
            }

        }
    }


    public string MainClass;
    public string SubClass;
    //string Affinity;
    public Dictionary<String, List<SkillSetItem>> SubBlockMap = new Dictionary<string, List<SkillSetItem>>();

}

//MODLEVELRANGESKILL:  skillsetaffE,  0, 30, "Scorpion Attack"                 // JP:   0/  0	AccMod -2
public class SkillSetItem
{
    public SkillSetItem(String[] tokens)
    {
        int index = 1;
        VariantName = tokens[index++];
        VariantName = VariantName.ToLower();
        MinLevel = int.Parse(tokens[index++]);
        MaxLevel = int.Parse(tokens[index++]);
        SkillName = tokens[index++];
    }

    public string VariantName;
    public int MinLevel;
    public int MaxLevel;
    public String SkillName;

}

public class UnitDBRestriction
{
    //public static CharacterData FromUnitDB(string name, int val2, string startSlot, int minLevel, int maxLevel, int val6, int requiredMask, int affinityRequirement, string class1, string class2, string class3, string class4, int stats1, int stats2, int stats3, int stats4, int stats5, int stats6, int stats7, int stats8, int stats9)
    public UnitDBRestriction(String class1)
    {
        Class1 = class1;
        // for testing.
    }
    public UnitDBRestriction(string name, int chance, string startSlot, int minLevel, int maxLevel, int relativeLevel, int requiredMask, int affinityRequirement, string class1, string class2, string class3, string class4, int excludeOrigin, int excludeClasses, int excludeCategories, int excludeAffinity, int stats5, int stats6, int stats7, int stats8, int stats9)
    {
        UnitName = name;
        Chance = chance;
        StartSlot = startSlot;
        MinLevel = minLevel;
        MaxLevel = maxLevel;
        RelativeLevel = relativeLevel;
        Attributes = requiredMask;
        AffinityRequirement = affinityRequirement;

        Class1 = class1;
        Class2 = class2;
        Class3 = class3;
        Class4 = class4;

        ExcludeOrigin = excludeOrigin;
        ExcludeClasses = excludeClasses;
        ExcludeCategories = excludeCategories;
        ExcludeAffinty = excludeAffinity;
        
        Facing = stats5;
        Mercenary = stats6;
        DropItemChance = stats7;
        Inactive = stats8;
        Untargettable = stats9;
    }

    public static string GetOrigin(int origin)
    {
        if (origin != 0)
        {
            if ((origin & (int)ClassAttributeFlags.Nordargh) != 0)
            {
                return ("from Nordargh\n");
            }
            if ((origin & (int)ClassAttributeFlags.Imperia) != 0)
            {
                return ("from Imperia\n");
            }
            if ((origin & (int)ClassAttributeFlags.Expanse) != 0)
            {
                return ("from the Southern Expanse\n");
            }
            if ((origin & (int)ClassAttributeFlags.Steppes) != 0)
            {
                return ("from Windward Steppes\n");
            }
        }
        return "";

    }

    static ClassAttributeFlags[] s_categories = new ClassAttributeFlags[] { ClassAttributeFlags.Beast, ClassAttributeFlags.Light, ClassAttributeFlags.Medium, 
        ClassAttributeFlags.Heavy, ClassAttributeFlags.Arcane, ClassAttributeFlags.Support, 
        ClassAttributeFlags.Human };

    static string[] s_categoryNames = new string[] { "Beast", "Light", "Medium", "Heavy", "Arcane", "Support", "Human" };

    public static string GetCategories(int category)
    {
        if(category != 0)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0;i<s_categories.Length;++i)
            {
                if((category & (int)s_categories[i]) != 0)
                {
                    if(sb.Length != 0)
                    {
                        sb.Append(", ");
                    }

                    sb.Append(s_categoryNames[i]);
                }
            }
            return sb.ToString();
        }
        return "";
    }

    public static string GetAffinity(int affinity)
    {
        if (affinity == (int)Affinity.AFF_DARK)
            return "Dark Affinity";
        if (affinity == (int)Affinity.AFF_EARTH)
            return "Earth Affinity";
        if (affinity == (int)Affinity.AFF_FIRE)
            return "Fire Affinity";
        if (affinity == (int)Affinity.AFF_LIGHT)
            return "Light Affinity";
        if (affinity == (int)Affinity.AFF_WATER)
            return "Water Affinity";
        if (affinity == (int)Affinity.AFF_AIR)
            return "Air Affinity";
        if (affinity == (int)Affinity.AFF_NONE)
            return "No Affinity";
        return "";
    }

    public static String GetClasses(int classMask)
    {
        if(classMask != 0)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0;i< ActorGenerator.ActorClassDefs.Count;++i)
            {
                int bitShift = 1;
                if ((classMask & (bitShift << i)) != 0)
                {
                    ActorClassDef classDef = ActorGenerator.ActorClassDefs[i];
                    if(sb.Length != 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append(classDef.name);
                }
            }
       }
        return "";
    }

    public string GetExcludeString()
    {
        StringBuilder sb = new StringBuilder();
        if (ExcludeCategories != 0)
        {
            sb.Append(GetCategories(ExcludeCategories));
            sb.Append("\n");
        }

        if (ExcludeClasses != 0)
        {
            sb.Append(GetClasses(ExcludeClasses));
            sb.Append("\n");
        }

        if (ExcludeAffinty != 0)
        {
            sb.Append(GetAffinity(ExcludeAffinty));
            sb.Append("\n");
        }

        if (ExcludeOrigin != 0)
        {
            sb.Append(GetOrigin(ExcludeOrigin));
            sb.Append("\n");
        }

        return sb.ToString();
    }


    public void GetRequiredProhibitedInfo(out String required, out String prohibited)
    {
        StringBuilder requiredSB = new StringBuilder();
        StringBuilder prohibitedSB = new StringBuilder();

        String classCategories = GetCategories(Attributes);
        if (classCategories.Length > 0)
        {
            ((ExcludeCategories == 0) ? requiredSB : prohibitedSB).Append(classCategories).Append("\n");
        }

        //GetClasses(BaseClassSet)

        String affinity = GetAffinity(AffinityRequirement);
        if (affinity.Length > 0)
        {
            ((ExcludeAffinty == 0) ? requiredSB : prohibitedSB).Append(affinity).Append("\n");
        }

        String origin = GetOrigin(Attributes);
        if(origin.Length > 0)
        {
            ((ExcludeOrigin == 0) ? requiredSB : prohibitedSB).Append(origin).Append("\n");
        }

        if (MinLevel > 0 && MaxLevel > 0)
        {
            requiredSB.Append(String.Format($"Between {MinLevel} and {MaxLevel}\n"));
        }
        else if (MinLevel > 0)
        {
            requiredSB.Append(String.Format($"Greater than {MinLevel}\n"));
        }
        else if (MaxLevel > 0)
        {
            requiredSB.Append(String.Format($"Less than {MaxLevel}\n"));
        }


        required = requiredSB.ToString();
        prohibited = prohibitedSB.ToString();
    }


    //UNITDB: 	"HERO", 100, "Start1", 0, 0, 0, 0, -1, "", "", "", "", 0, 0, 0, 0, 2, 0, 0, 0, 0
    public String UnitName;
    public int Chance;
    public String StartSlot;
    public int MinLevel;
    public int MaxLevel;
    public int RelativeLevel;

    public int Attributes;
    public int AffinityRequirement;

    public String Class1;
    public String Class2;
    public String Class3;
    public String Class4;

    public int ExcludeOrigin;
    public int ExcludeClasses;
    public int ExcludeCategories;
    public int ExcludeAffinty;

    public int Facing;
    public int Mercenary;
    public int DropItemChance;
    public int Inactive;
    public int Untargettable;
}



public class ActorClassDef
{
    public string name;
    public int displayNameId;
    public int descriptionId;
    public string unitSet;
    public string skillUse;
    public string voiceLinePrefix;
    public string soundTableName;
    public string mesh;
    public string MeshPath
    {
        get { return GladiusGlobals.ModelsRoot + "characters/" + mesh+"/"+mesh;}

    }



    public string classIcon;
    public string headIcon;

    public int gridSize;
    public int xpAward1;
    public float xpAward2;

    public float itemSize1 = 1.0f;
    public float itemSize2 = 1.0f;
    public float itemSize3 = 1.0f;

    public string affinty;

    public List<string> Attributes = new List<string>();

    public StatsSet LevelZeroStats;

    public Vector3 ScaleForItemSlot(ItemLocation location)
    {
        return new Vector3(itemSize1, itemSize2, itemSize3);
    }


    public Dictionary<ItemLocation, List<ItemCategoryEntry>> ItemCategoryMap = new Dictionary<ItemLocation, List<ItemCategoryEntry>>();


    public bool IsMale { get { return Attributes.Contains("Male"); } }
    public bool IsFemale { get { return Attributes.Contains("Female"); } }
    public bool IsLight { get { return Attributes.Contains("Light"); } }
    public bool IsMedium { get { return Attributes.Contains("Medium"); } }
    public bool IsHeavy { get { return Attributes.Contains("Heavy"); } }
    public bool IsArcane { get { return Attributes.Contains("Arcane"); } }
    public bool IsSupport { get { return Attributes.Contains("Support"); } }
    public bool IsBeast { get { return Attributes.Contains("Beast"); } }
    public bool IsNordagh { get { return Attributes.Contains("Nordagh"); } }
    public bool IsImperia { get { return Attributes.Contains("Imperia"); } }
    public bool IsSteppes{ get { return Attributes.Contains("Steppes"); } }
    public bool IsExpanse { get { return Attributes.Contains("Expanse"); } }
    public bool IsNoKnockBack { get { return Attributes.Contains("NoKnockback"); } }
    public bool IsNoKnockDown { get { return Attributes.Contains("NoKnockdown"); } }
    public bool IsNoFaceOnAttack { get { return Attributes.Contains("doesntfaceonattack"); } }
    public bool IsNoFaceOnDefend { get { return Attributes.Contains("doesntfaceondefend"); } }
    public bool IsNoMove { get { return Attributes.Contains("doesntmove"); } }
    public bool IsNoIdles { get { return Attributes.Contains("noidles"); } }
    public bool IsDeathRemainOnField { get { return Attributes.Contains("deathremainonfield"); } }
    public bool IsSimpleAnims { get { return Attributes.Contains("simpleanims"); } }



}

public class MyGladiusItemSetParser : GladiusItemSetBaseListener
{

    public override void EnterItemSetClassLine([NotNull] GladiusItemSetParser.ItemSetClassLineContext context)
    {
        base.EnterItemSetClassLine(context);
        String className = context.STRING()[1].GetStringVal().ToLower();
        
        if (!ClassItemsMap.TryGetValue(className, out Current))
        {
            Current = new ItemSetBlock();
            Current.MainClass = className;
            Current.SubClass = className;
            ClassItemsMap[className] = Current;
        }
    }

    public override void EnterModItemsComp([NotNull] GladiusItemSetParser.ModItemsCompContext context)
    {
        //MODITEMSCOMP:    itemsetIE,  1, 1,                              "",                              "",                              "",                              "",                              "" 	//, Cost, 000000,000000,Acc,Def,0000,0000
        base.EnterModItemsComp(context);

        ItemSet itemSet = new ItemSet();
        itemSet.VariantName = context.STRING().GetStringVal().ToLower();
        itemSet.MinLevel = context.INT()[0].GetIntVal();
        itemSet.MaxLevel = context.INT()[1].GetIntVal();
        itemSet.Weapon = context.ITEMNAME()[0].GetStringVal();
        itemSet.Armor = context.ITEMNAME()[1].GetStringVal();
        itemSet.Shield = context.ITEMNAME()[2].GetStringVal();
        itemSet.Helmet = context.ITEMNAME()[3].GetStringVal();
        itemSet.Accessory = context.ITEMNAME()[4].GetStringVal();

        if (Current != null)
        {
            Current.AddItem(itemSet);
        }

    }
    ItemSetBlock Current;

    public Dictionary<String, ItemSetBlock> ClassItemsMap;
}

public class MyGladiusClassDefParser : GladiusClassDefBaseListener
{
    public List<ActorClassDef> ClassDefs = new List<ActorClassDef>();
    private ActorClassDef currentActor = null;
    public override void EnterCreateClass(GladiusClassDefParser.CreateClassContext context)
    {
        base.EnterCreateClass(context);
        currentActor = new ActorClassDef();
        ClassDefs.Add(currentActor);
        currentActor.name = context.STRING().GetStringVal();
        int ibreak = 0;
    }

    public override void EnterDisplayName(GladiusClassDefParser.DisplayNameContext context)
    {
        base.EnterDisplayName(context);
        currentActor.displayNameId = context.INT().GetIntVal();
    }

    public override void EnterDescriptionId(GladiusClassDefParser.DescriptionIdContext context)
    {
        base.EnterDescriptionId(context);
        currentActor.descriptionId = context.INT().GetIntVal();
    }

    public override void EnterUnitSet(GladiusClassDefParser.UnitSetContext context)
    {
        base.EnterUnitSet(context);
        currentActor.unitSet = context.STRING().GetStringVal();
        if (currentActor.unitSet != null)
        {
            currentActor.unitSet = currentActor.unitSet.ToLower();
        }
    }


    public override void EnterSkillUse(GladiusClassDefParser.SkillUseContext context)
    {
        base.EnterSkillUse(context);
        currentActor.skillUse = context.STRING().GetStringVal();
        if (currentActor.skillUse != null)
        {
            currentActor.skillUse = currentActor.skillUse.ToLower();
        }
    }

    public override void EnterVoiceLinePrefix(GladiusClassDefParser.VoiceLinePrefixContext context)
    {
        base.EnterVoiceLinePrefix(context);
        currentActor.voiceLinePrefix = context.STRING().GetStringVal();
    }

    public override void EnterSoundTable(GladiusClassDefParser.SoundTableContext context)
    {
        base.EnterSoundTable(context);
        currentActor.soundTableName = context.STRING().GetStringVal();
    }

    public override void EnterGridSize(GladiusClassDefParser.GridSizeContext context)
    {
        base.EnterGridSize(context);
        currentActor.gridSize = context.INT().GetIntVal();
    }

    public override void EnterMesh(GladiusClassDefParser.MeshContext context)
    {
        base.EnterMesh(context);
        currentActor.mesh = context.STRING().GetStringVal();
    }

    public override void EnterClassIcon(GladiusClassDefParser.ClassIconContext context)
    {
        base.EnterClassIcon(context);
        currentActor.classIcon = context.STRING().GetStringVal().ToLower();
    }

    public override void EnterHeadIcon(GladiusClassDefParser.HeadIconContext context)
    {
        base.EnterHeadIcon(context);
        currentActor.headIcon = context.STRING().GetStringVal().ToLower();
    }

    public override void EnterAffinity(GladiusClassDefParser.AffinityContext context)
    {
        base.EnterAffinity(context);
        currentActor.affinty = context.STRING().GetStringVal();
    }

    public override void EnterAttribute(GladiusClassDefParser.AttributeContext context)
    {
        base.EnterAttribute(context);
        String attribute = context.STRING().GetStringVal();
        currentActor.Attributes.Add(attribute);
    }

    public override void EnterItemSizes(GladiusClassDefParser.ItemSizesContext context)
    {
        base.EnterItemSizes(context);
        currentActor.itemSize1 = context.itemSubSize(0).INT() != null ? context.itemSubSize(0).INT().GetIntVal() : context.itemSubSize(0).FLOAT().GetFloatVal();
        currentActor.itemSize2 = context.itemSubSize(1).INT() != null ? context.itemSubSize(1).INT().GetIntVal() : context.itemSubSize(1).FLOAT().GetFloatVal();
        currentActor.itemSize3 = context.itemSubSize(2).INT() != null ? context.itemSubSize(2).INT().GetIntVal() : context.itemSubSize(2).FLOAT().GetFloatVal();
    }

    public override void EnterLevelZeroStats([NotNull] GladiusClassDefParser.LevelZeroStatsContext context)
    {
        base.EnterLevelZeroStats(context);
        currentActor.LevelZeroStats = new StatsSet();
        currentActor.LevelZeroStats.CON = context.INT(0).GetFloatVal();
        currentActor.LevelZeroStats.PWR = context.INT(1).GetFloatVal();
        currentActor.LevelZeroStats.ACC = context.INT(2).GetFloatVal();
        currentActor.LevelZeroStats.DEF = context.INT(3).GetFloatVal();
        currentActor.LevelZeroStats.INI = context.INT(4).GetFloatVal();
        currentActor.LevelZeroStats.MOV = context.INT(5).GetFloatVal();
    }

    public static TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;


    public override void EnterItemCat([NotNull] GladiusClassDefParser.ItemCatContext context)
    {
        base.EnterItemCat(context);

        String locationStr = context.STRING()[0].GetStringVal();
        String filter1 = context.STRING()[1].GetStringVal();
        String filter2 = context.STRING()[2].GetStringVal();

        ItemCategoryEntry ice = new ItemCategoryEntry();
        ice.itemLocation = (ItemLocation)Enum.Parse(typeof(ItemLocation),textInfo.ToTitleCase(locationStr));
        ice.filter1 = filter1;
        ice.filter2 = filter2;

        List<ItemCategoryEntry> iceList = null;
        if (!currentActor.ItemCategoryMap.TryGetValue(ice.itemLocation, out iceList))
        {
            iceList = new List<ItemCategoryEntry>();
            currentActor.ItemCategoryMap[ice.itemLocation] = iceList;
        }
        iceList.Add(ice);

    }

}





public class ItemCategoryEntry
{
    public ItemLocation itemLocation;
    public String filter1;
    public String filter2;
}



