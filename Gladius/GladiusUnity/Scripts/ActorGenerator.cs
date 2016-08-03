using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;


//namespace Gladius
//{
public static class ActorGenerator
{
    static ActorGenerator()
    {
        InitClassCategories();
        LoadAllStatsFiles();
        LoadAllItemsFiles();
        LoadAllSkillsFiles();
        LoadAllGladiators();

        Debug.LogFormat("Loaded [{0}] Stats Files", ClassStatsMap.Count);
        Debug.LogFormat("Loaded [{0}] Items Files", ClassItemsMap.Count);
        Debug.LogFormat("Loaded [{0}] Skills Files", ClassSkillsMap.Count);
        Debug.LogFormat("Loaded [{0}] Gladiators Schools", SchoolsAndGladiators.Count);
        int ibreak = 0;
    }

    public static void Test()
    {

    }


    public static void InitClassCategories()
    {

        ClassDataMap[ActorClass.AMAZON] = new ActorClassData(ActorClass.AMAZON, "Amazon", "amazon", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Support));
        ClassDataMap[ActorClass.ARCHER] = new ActorClassData(ActorClass.ARCHER, "Archer", "archer", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Support));
        ClassDataMap[ActorClass.ARCHERF] = new ActorClassData(ActorClass.ARCHERF, "Archer", "archerF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Support));
        ClassDataMap[ActorClass.BANDITA] = new ActorClassData(ActorClass.BANDITA, "Bandit", "banditA", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BANDITAF] = new ActorClassData(ActorClass.BANDITAF, "Bandit", "banditAF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BANDITB] = new ActorClassData(ActorClass.BANDITB, "Bandit", "banditB", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.BARBARIAN] = new ActorClassData(ActorClass.BARBARIAN, "Barbarian", "barbarian", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BARBARIANF] = new ActorClassData(ActorClass.BARBARIANF, "Barbarian", "barbarianF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BEAR] = new ActorClassData(ActorClass.BEAR, "Bear", "bear", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BEARGREATER] = new ActorClassData(ActorClass.BEARGREATER, "Bear", "beargreater", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BERSERKER] = new ActorClassData(ActorClass.BERSERKER, "Berserker", "berserker", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.BERSERKERF] = new ActorClassData(ActorClass.BERSERKERF, "Berserker", "berserkerf", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
        ClassDataMap[ActorClass.CAT] = new ActorClassData(ActorClass.CAT, "Cat", "cat", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.CATGREATER] = new ActorClassData(ActorClass.CATGREATER, "Cat", "catgreater", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.CENTURION] = new ActorClassData(ActorClass.CENTURION, "Centurion", "centurion", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.CENTURIONF] = new ActorClassData(ActorClass.CENTURIONF, "Centurion", "centurionF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.CHANNELERIMP] = new ActorClassData(ActorClass.CHANNELERIMP, "Channeler", "channelerImp", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Arcane | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.CYCLOPS] = new ActorClassData(ActorClass.CYCLOPS, "Cyclops", "cyclops", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.CYCLOPSGREATER] = new ActorClassData(ActorClass.CYCLOPSGREATER, "Cyclops", "cyclopsgreater", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.DARKLEGIONNAIRE] = new ActorClassData(ActorClass.DARKLEGIONNAIRE, "DarkLegionnaire", "legionnaireDark", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.DERVISH] = new ActorClassData(ActorClass.DERVISH, "Dervish", "dervish", (ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.DERVISHF] = new ActorClassData(ActorClass.DERVISHF, "Dervish", "dervishF", (ActorClassAttributes.Female | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.DUMMY] = new ActorClassData(ActorClass.DUMMY, "Dummy", "PropPracticePost", (ActorClassAttributes.Male | ActorClassAttributes.Light));


        ClassDataMap[ActorClass.EIJI] = new ActorClassData(ActorClass.EIJI, "Eiji", "eiji", (ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Steppes | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.GALDR] = new ActorClassData(ActorClass.GALDR, "Galdr", "galdr", (ActorClassAttributes.Female | ActorClassAttributes.Arcane | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.GALVERG] = new ActorClassData(ActorClass.GALVERG, "Galverg", "galverg", (ActorClassAttributes.Male | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.GUNGNIR] = new ActorClassData(ActorClass.GUNGNIR, "Gungir", "gungir", (ActorClassAttributes.Male | ActorClassAttributes.Support | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.GUNGNIRF] = new ActorClassData(ActorClass.GUNGNIRF, "Gungir", "gungirF", (ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.GWAZI] = new ActorClassData(ActorClass.GWAZI, "Gwazi", "gwazi", (ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.LEGIONNAIRE] = new ActorClassData(ActorClass.LEGIONNAIRE, "Legionnarie", "legionnaire", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.LEGIONNAIREF] = new ActorClassData(ActorClass.LEGIONNAIREF, "Legionnarie", "legionanaireF", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.LUDO] = new ActorClassData(ActorClass.LUDO, "Ludo", "ludo", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.MINOTAUR] = new ActorClassData(ActorClass.MINOTAUR, "Minotaur", "minotaur", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy));
        ClassDataMap[ActorClass.MONGREL] = new ActorClassData(ActorClass.MONGREL, "Mongrel", "mongrel", (ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Nordargh | ActorClassAttributes.Beast));
        ClassDataMap[ActorClass.MONGRELSHAMAN] = new ActorClassData(ActorClass.MONGRELSHAMAN, "MongrelShaman", "mongrelshaman", (ActorClassAttributes.Male | ActorClassAttributes.Arcane | ActorClassAttributes.Nordargh | ActorClassAttributes.Beast));
        ClassDataMap[ActorClass.MURMILLO] = new ActorClassData(ActorClass.MURMILLO, "Murmillo", "murmillo", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.MURMILLOF] = new ActorClassData(ActorClass.MURMILLOF, "Murmillo", "murmilloF", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.MUTUUS] = new ActorClassData(ActorClass.MUTUUS, "Mutuus", "mutuus", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.OGRE] = new ActorClassData(ActorClass.OGRE, "Ogre", "ogre", (ActorClassAttributes.Beast | ActorClassAttributes.Heavy));
        ClassDataMap[ActorClass.PELTAST] = new ActorClassData(ActorClass.PELTAST, "Peltast", "peltast", (ActorClassAttributes.Male | ActorClassAttributes.Support | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.PELTASTF] = new ActorClassData(ActorClass.PELTASTF, "Peltast", "peltastF", (ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

        ClassDataMap[ActorClass.SAMNITEEXP] = new ActorClassData(ActorClass.SAMNITEEXP, "Samnite", "samniteexp", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Expanse));
        ClassDataMap[ActorClass.SAMNITEEXPF] = new ActorClassData(ActorClass.SAMNITEEXPF, "Samnite", "samniteexpF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Expanse));
        ClassDataMap[ActorClass.SAMNITEIMP] = new ActorClassData(ActorClass.SAMNITEIMP, "Samnite", "samniteimp", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SAMNITEIMPF] = new ActorClassData(ActorClass.SAMNITEIMPF, "Samnite", "samniteimpF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SAMNITESTE] = new ActorClassData(ActorClass.SAMNITESTE, "Samnite", "samniteste", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Steppes));
        ClassDataMap[ActorClass.SAMNITESTEF] = new ActorClassData(ActorClass.SAMNITESTEF, "Samnite", "samnitesteF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Steppes));

        ClassDataMap[ActorClass.SATYR] = new ActorClassData(ActorClass.SATYR, "Satyr", "satyr", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Arcane));
        ClassDataMap[ActorClass.SCARAB] = new ActorClassData(ActorClass.SCARAB, "Scarab", "scarab", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.SCORPION] = new ActorClassData(ActorClass.SCORPION, "Scorpion", "scorpion", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
        ClassDataMap[ActorClass.SECUTORIMP] = new ActorClassData(ActorClass.SECUTORIMP, "Secutor", "secutorimp", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SECUTORIMPF] = new ActorClassData(ActorClass.SECUTORIMPF, "Secutor", "secutorimpF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.SECUTORSTE] = new ActorClassData(ActorClass.SECUTORSTE, "Secutor", "secutorste", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Steppes));
        ClassDataMap[ActorClass.SECUTORSTEF] = new ActorClassData(ActorClass.SECUTORSTEF, "Secutor", "secutorsteF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Steppes));
        ClassDataMap[ActorClass.SUMMONER] = new ActorClassData(ActorClass.SUMMONER, "Summoner", "summoner", (ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Arcane));
        ClassDataMap[ActorClass.UNDEADMELEEIMPA] = new ActorClassData(ActorClass.UNDEADMELEEIMPA, "UndeadLegionnaire", "skeletonimp2", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia));
        ClassDataMap[ActorClass.UNDEADCASTERA] = new ActorClassData(ActorClass.UNDEADCASTERA, "UndeadSummoner", "skeletonimp2", (ActorClassAttributes.Male | ActorClassAttributes.Arcane));
        ClassDataMap[ActorClass.URLAN] = new ActorClassData(ActorClass.URLAN, "Urlan", "urlan", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.URSULACOSTUMEA] = new ActorClassData(ActorClass.URSULACOSTUMEA, "Ursula", "ursula", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.URSULACOSTUMEB] = new ActorClassData(ActorClass.URSULACOSTUMEB, "Ursula", "ursula", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.URSULA] = new ActorClassData(ActorClass.URSULA, "Ursula", "ursula", (ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.VALENSCOSTUMEA] = new ActorClassData(ActorClass.VALENSCOSTUMEA, "Valens", "valens", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.VALENSCOSTUMEB] = new ActorClassData(ActorClass.VALENSCOSTUMEB, "Valens", "valens", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.VALENS] = new ActorClassData(ActorClass.VALENS, "Valens", "valens", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
        ClassDataMap[ActorClass.WOLF] = new ActorClassData(ActorClass.WOLF, "Wolf", "wolf", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Light));
        ClassDataMap[ActorClass.WOLFGREATER] = new ActorClassData(ActorClass.WOLFGREATER, "Wolf", "wolfgreater", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Light));
        ClassDataMap[ActorClass.YETI] = new ActorClassData(ActorClass.YETI, "Yeti", "yeti", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
    }

    // check for items being useable by a class.
    public static bool ItemValidForClass(Item item, ActorClassData actorClassData)
    {
        String itemClass = item.ClassInfo;
        if (Enum.IsDefined(typeof(ActorClass), itemClass))
        {
            ActorClass ac = (ActorClass)Enum.Parse(typeof(ActorClass), itemClass);
            return actorClassData.ActorClass == ac;
        }
        else
        if (Enum.IsDefined(typeof(ActorClassAttributes), itemClass))
        {
            ActorClassAttributes aca = (ActorClassAttributes)Enum.Parse(typeof(ActorClassAttributes), itemClass);
            return (actorClassData.Mask & aca) > 0;
        }
        return false;
    }


    public static bool HeroCharacter(ActorClassData acd)
    {
        switch (acd.ActorClass)
        {
            case ActorClass.EIJI:
            case ActorClass.GALVERG:
            case ActorClass.LUDO:
            case ActorClass.URLAN:
            case ActorClass.URSULA:
            case ActorClass.URSULACOSTUMEA:
            case ActorClass.URSULACOSTUMEB:
            case ActorClass.VALENS:
            case ActorClass.VALENSCOSTUMEA:
            case ActorClass.VALENSCOSTUMEB:
                return true;
            default:
                return false;
        }
    }

    private static int RandomNameCount = 0;
    public static CharacterData CreateRandomCharacter(int level)
    {
        CharacterData result = null;
        int randomCharacter = (int)((float)ClassDataMap.Keys.Count * UnityEngine.Random.value);
        ActorClass key = ClassDataMap.Keys.ToDynList()[randomCharacter];
        ActorClassData randomClass = ClassDataMap[key];
        result = new CharacterData();
        result.ActorClass = key;
        result.Name = "Random" + (RandomNameCount++);
        return result;

    }

    public static CharacterData CreateRandomCharacter(ActorClass actorClass, int level)
    {
        CharacterData result = null;
        ActorClassData randomClass = ClassDataMap[actorClass];
        result = new CharacterData();
        result.ActorClass = actorClass;
        result.Name = "Random" + (RandomNameCount++);
        return result;
    }

    public static bool CheckLevelUp(CharacterData characterData, int xpGained)
    {
        return false;
    }

    public static CharacterData CreateRandomCharacterForLevel(CharacterData characterData)
    {
        // choose a class based on mask.
        List<ActorClassData> validClasses = new List<ActorClassData>();
        foreach (ActorClassData actorClassData in ClassDataMap.Values)
        {
            if (characterData.RequiredMask == 0 || (
                (((int)actorClassData.Mask) & characterData.RequiredMask) != 0))
            {
                // ignore hero characters for now...
                if (!HeroCharacter(actorClassData))
                {
                    validClasses.Add(actorClassData);
                }
            }
        }
        // pick one at random?
        int randomChar = UnityEngine.Random.Range(0, validClasses.Count - 1);
        if (randomChar < 0 || randomChar >= validClasses.Count)
        {
            int ibreak = 0;
        }

        ActorClassData classData = validClasses[randomChar];
        characterData.ActorClass = classData.ActorClass;

        // character data can act as an empty slot saying whats allowed , as well as being an actual character slot?
        String className = characterData.ActorClassData != null ? characterData.ActorClassData.Name : "Unassigned";
        string statsSetName = characterData.StatsSetName;
        if (String.IsNullOrEmpty(statsSetName))
        {
            StatsSetBlock setBlock = null;
            if (ClassStatsMap.TryGetValue(characterData.ActorClassData.Name, out setBlock))
            {
                int random = UnityEngine.Random.Range(0, setBlock.SubBlockMap.Count - 1);
                //statsSetName = setBlock.SubBlockMap.Keys.ToDynList()[random];
                //statsSetName = setBlock.SubBlockMap.Keys.[random];
            }
        }

        if (String.IsNullOrEmpty(statsSetName))
        {
            Debug.LogErrorFormat("Unable to find stats for [{0}]", characterData.ActorClassData.Name);
        }
        else
        {
            characterData.StatsSetName = statsSetName;

            StatsSet statData = StatForClassLevel(className, statsSetName, characterData.Level);
            if (statData == null)
            {
                int ibreak = 0;
            }
            characterData.CopyModCoreStat(statData);
        }

        String skillSetName = characterData.SkillSetName;
        if (String.IsNullOrEmpty(skillSetName))
        {
            SkillSetBlock setBlock = null;
            if (ClassSkillsMap.TryGetValue(characterData.ActorClassData.Name, out setBlock))
            {
                int random = UnityEngine.Random.Range(0, setBlock.SubBlockMap.Count - 1);
                //skillSetName = setBlock.SubBlockMap.Keys.ToDynList()[random];
            }
        }

        if (String.IsNullOrEmpty(skillSetName))
        {
            Debug.LogErrorFormat("Unable to find skill for [{0}]", characterData.ActorClassData.Name);
        }
        else
        {
            characterData.SkillSetName = skillSetName;
        }
        return characterData;

    }


    public static CharacterData ParseUNITDB(String[] tokens)
    {
        CharacterData characterData = new CharacterData();
        // skip first token (unitdb)
        int counter = 0;
        String name = tokens[counter++];
        int val1 = int.Parse(tokens[counter++]);
        String startSlot = tokens[counter++];

        characterData.MinLevel = int.Parse(tokens[counter++]);
        characterData.MaxLevel = int.Parse(tokens[counter++]);

        int val4 = int.Parse(tokens[counter++]);

        int level = 1; // base of player level?u

        if (characterData.MinLevel > 0 && characterData.MaxLevel > 0)
        {
            level = GladiusGlobals.Random.Next(characterData.MinLevel, characterData.MaxLevel);
        }


        characterData.Level = level;
        characterData.Name = name;

        // seems to be a required mask
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
        // 4096 = human
        // 4160 = human male        4096+64
        // 4224 = human female      4096+128

        int mask1 = int.Parse(tokens[counter++]);
        characterData.RequiredMask = mask1;
        // affinity requirement
        // 5,2,1,4  ?? 
        // 1  = earth
        // 2 = water
        // 3 = light
        // 4 = air
        // 5 = fire
        // 6 = dark


        DamageType damageType = DamageType.Physical;
        int affinityRequirement = int.Parse(tokens[counter++]);
        if (affinityRequirement != -1)
        {
            switch (affinityRequirement)
            {
                case 1:
                    damageType = DamageType.Earth;
                    break;
                case 2:
                    damageType = DamageType.Water;
                    break;
                case 3:
                    damageType = DamageType.Air;
                    break;
                case 4:
                    damageType = DamageType.Fire;
                    break;
                default:
                    damageType = DamageType.Physical;
                    break;
            }
        }


        String class1 = tokens[counter++];
        String class2 = tokens[counter++];
        String class3 = tokens[counter++];
        String class4 = tokens[counter++];
        int val7 = int.Parse(tokens[counter++]);
        int requiredProhbited = int.Parse(tokens[counter++]);

        HashSet<String> allowedClasses = new HashSet<string>();
        allowedClasses.Add(class1);
        allowedClasses.Add(class2);
        allowedClasses.Add(class3);
        allowedClasses.Add(class4);

        int val9 = int.Parse(tokens[counter++]);
        int val10 = int.Parse(tokens[counter++]);
        int val11 = int.Parse(tokens[counter++]);
        int val12 = int.Parse(tokens[counter++]);

        return characterData;
    }

    public static void LoadAllStatsFiles()
    {

        TextAsset[] allFiles = Resources.LoadAll<TextAsset>(GladiusGlobals.DataRoot + "StatFiles");
        foreach (TextAsset file in allFiles)
        {
            try
            {
                String[] lines = file.text.Split('\n');
                StatsSetBlock setBlock = LoadStatsFile(lines);
                ClassStatsMap[setBlock.MainClass] = setBlock;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Exception loading stat files for [{0}][{1}]", file.name, e);
            }
        }
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
                    String variant = shortList[counter++];
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

    public static StatsSet StatForClassLevel(string className, string statsSetName, int level)
    {
        StatsSet set = null;
        StatsSetBlock setBlock = null;
        if (ClassStatsMap.TryGetValue(className, out setBlock))
        {
            List<StatsSet> statRow = null;
            if (setBlock.SubBlockMap.TryGetValue(statsSetName, out statRow))
            {
                if (level < statRow.Count)
                {
                    set = statRow[level - 1];
                }
            }

        }
        return set;
    }

    public static ItemSet ItemsForClassLevel(string className, string skillsetName, int level)
    {
        ItemSet set = null;
        ItemSetBlock setBlock = null;
        if (ClassItemsMap.TryGetValue(className, out setBlock))
        {
            List<ItemSet> statRow = null;
            if (setBlock.SubBlockMap.TryGetValue(skillsetName, out statRow))
            {
                if (level < statRow.Count)
                {
                    set = statRow[level - 1];
                }
            }

        }
        return set;
    }

    public static List<SkillSetItem> SkillSetForClassLevel(string className, string skillsetName, int level)
    {
        List<SkillSetItem> setList = null;
        SkillSetBlock setBlock = null;
        if (ClassSkillsMap.TryGetValue(className, out setBlock))
        {
            if (setBlock.SubBlockMap.TryGetValue(skillsetName, out setList))
            {
            }
            else
            {
            }

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
        TextAsset data = Resources.Load<TextAsset>(GladiusGlobals.DataRoot + "GladiatorNames");
        string[] lines = data.text.Split('\n');
        // data starts on the 3rd line
        for (int counter = 3; counter < lines.Length; counter++)
        {
            string line = lines[counter];

            string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ',' });
            if (lineTokens.Length == 7)
            {
                GladiatorData gladiatorData = new GladiatorData();
                gladiatorData.name = lineTokens[0];
                gladiatorData.className = lineTokens[1];
                gladiatorData.customizeInfo = lineTokens[2];
                gladiatorData.statSetName = lineTokens[3];
                gladiatorData.itemSetName = lineTokens[4];
                gladiatorData.skillSetName = lineTokens[5];
                gladiatorData.schoolName = lineTokens[6];

                List<GladiatorData> gladiators = null;
                if (!SchoolsAndGladiators.TryGetValue(gladiatorData.schoolName, out gladiators))
                {
                    gladiators = new List<GladiatorData>();
                    SchoolsAndGladiators[gladiatorData.schoolName] = gladiators;
                }
                gladiators.Add(gladiatorData);

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
        ItemSet itemData = ItemsForClassLevel(gd.className, gd.statSetName, gd.level);
        List<SkillSetItem> skillSetList = SkillSetForClassLevel(gd.className, gd.statSetName, gd.level);

        CharacterData cd = new CharacterData();
        cd.Name = gd.name;
        cd.TeamName = gd.schoolName;
        statData.Apply(cd);
        itemData.Apply(cd);

        foreach (SkillSetItem skill in skillSetList)
        {
            cd.AddSkill(skill.SkillName);
        }
        return cd;
    }


    public static void LoadAllItemsFiles()
    {

        TextAsset[] allFiles = Resources.LoadAll<TextAsset>(GladiusGlobals.DataRoot + "ItemFiles");
        foreach (TextAsset file in allFiles)
        {
            try
            {
                String[] lines = file.text.Split('\n');
                ItemSetBlock setBlock = LoadItemsFile(lines);
                ClassItemsMap[setBlock.MainClass] = setBlock;
            }
            catch (Exception e)
            {

            }

        }
    }

    public static ItemSetBlock LoadItemsFile(String[] lines)
    {
        ////// ITEMSET export Class: Galverg Galverg  Region: Imperia Affinity: Earth
        //          :      variant, Lv,Lv,                          weapon,                           armor,                          shield,                          helmet             accessory
        //MODITEMSCOMP:    itemsetIE,  1, 1,                              "",                              "",                              "",                              "",                              "" 	//, Cost, 000000,000000,Acc,Def,0000,0000
        //MODITEMSCOMP:    itemsetIE,  2, 2,                              "",                              "",                              "",                              "",                 "Earth Berkana" 	//, Cost, 001188,001188,Acc,Def,0000,0000


        ItemSetBlock currentItemSetBlock = new ItemSetBlock(lines[0]);
        foreach (String line in lines)
        {
            if (line.StartsWith("MODITEMSCOMP:"))
            {

                string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ',', ':' });
                if (lineTokens.Length == 9)
                {
                    ItemSet modItemStat = new ItemSet(lineTokens);
                    currentItemSetBlock.AddItem(modItemStat);
                }

            }
        }
        return currentItemSetBlock;
    }


    public static void LoadAllSkillsFiles()
    {
        TextAsset[] allFiles = Resources.LoadAll<TextAsset>(GladiusGlobals.DataRoot + "SkillFiles");
        foreach (TextAsset file in allFiles)
        {
            try
            {
                String[] lines = file.text.Split('\n');
                SkillSetBlock setBlock = LoadSkillsFile(lines);
                if (setBlock != null && setBlock.MainClass != null)
                {
                    ClassSkillsMap[setBlock.MainClass] = setBlock;
                }
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("Exception loading skill files for [{0}][{1}]", file.name, e);
            }

        }
    }

    public static SkillSetBlock LoadSkillsFile(String[] lines)
    {
        ////// SKILLSET export Class: Scorpion Scorpion  Affinity: None
        //MODLEVELRANGESKILL:  skillsetcomb,  0, 30, "Scorpion Attack"                 // JP:   0/  0	AccMod -2
        //MODLEVELRANGESKILL:  skillsetcomb,  0, 30, "Scorpion Evade"                  // JP:   0/  0	AccMod 0

        SkillSetBlock currentSetBlock = new SkillSetBlock(lines[0]);

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


    public static Dictionary<String, List<GladiatorData>> SchoolsAndGladiators = new Dictionary<string, List<GladiatorData>>();
    public static Dictionary<ActorClass, ActorClassData> ClassDataMap = new Dictionary<ActorClass, ActorClassData>();
    public static Dictionary<ActorClass, int[]> ActorXPLevels = new Dictionary<ActorClass, int[]>();

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

public class ActorClassData
{
    public String Name;
    public String MeshName;
    public ActorClass ActorClass;
    public ActorClassAttributes Mask;

    public ActorClassData(ActorClass actorClass, String name, String meshName, ActorClassAttributes mask)
    {
        ActorClass = actorClass;
        Name = name;
        MeshName = meshName;
        Mask = mask;
    }

    // 0 = mongel? nah
    // 1 = light
    // 2 = medium
    // 4 = heavy
    // 8 = arcane
    // 16 = support 
    // 24 = prohibited arcane + support
    // 32 = beast
    // 56 = prohibited beast, required arcane + support
    // 64 = male
    // 128 = female
    // heavy = 56 (32+16+8)
    // 256 = Nordargh only?
    // 512 = Imperia only
    // 4096 = human
    // 4160 = human male        4096+64
    // 4224 = human female      4096+128

    public bool IsMale { get { return (Mask & ActorClassAttributes.Male) > 0; } }
    public bool IsFemale { get { return (Mask & ActorClassAttributes.Female) > 0; } }
    public bool IsLight { get { return (Mask & ActorClassAttributes.Light) > 0; } }
    public bool IsMedium { get { return (Mask & ActorClassAttributes.Medium) > 0; } }
    public bool IsHeavy { get { return (Mask & ActorClassAttributes.Heavy) > 0; } }
    public bool IsArcane { get { return (Mask & ActorClassAttributes.Arcane) > 0; } }
    public bool IsSupport { get { return (Mask & ActorClassAttributes.Support) > 0; } }
    public bool IsBeast { get { return (Mask & ActorClassAttributes.Beast) > 0; } }
    public bool IsNordargh { get { return (Mask & ActorClassAttributes.Nordargh) > 0; } }
    public bool IsImperia { get { return (Mask & ActorClassAttributes.Imperia) > 0; } }
    public bool IsTwoHanded
    {
        get
        {
            return ActorClass == ActorClass.BARBARIAN || ActorClass == ActorClass.BARBARIANF || ActorClass == ActorClass.GALVERG || 
                ActorClass == ActorClass.MINOTAUR || ActorClass == ActorClass.OGRE || ActorClass == ActorClass.SAMNITEEXP || 
                ActorClass == ActorClass.SAMNITEEXPF || ActorClass == ActorClass.SAMNITEIMP || ActorClass == ActorClass.SAMNITEIMPF || 
                ActorClass == ActorClass.SAMNITESTE || ActorClass == ActorClass.SAMNITESTEF || ActorClass == ActorClass.URLAN;
        }
    }
    public bool IsOneHanded
    {
        get
        {
            return ActorClass == ActorClass.BANDITA || ActorClass == ActorClass.BANDITAF || ActorClass == ActorClass.BANDITB || 
                ActorClass == ActorClass.BERSERKER || ActorClass == ActorClass.BERSERKERF || ActorClass == ActorClass.CENTURION || 
                ActorClass == ActorClass.CENTURIONF || ActorClass == ActorClass.CYCLOPS || ActorClass == ActorClass.DARKLEGIONNAIRE || 
                ActorClass == ActorClass.DERVISH || ActorClass == ActorClass.DERVISHF || ActorClass == ActorClass.GWAZI || 
                ActorClass == ActorClass.LEGIONNAIRE || ActorClass == ActorClass.LEGIONNAIREF || ActorClass == ActorClass.LUDO || 
                ActorClass == ActorClass.MONGREL || ActorClass == ActorClass.MURMILLO || ActorClass == ActorClass.MURMILLOF || 
                ActorClass == ActorClass.SATYR || ActorClass == ActorClass.SECUTORIMP || ActorClass == ActorClass.SECUTORIMPF || 
                ActorClass == ActorClass.SECUTORSTE || ActorClass == ActorClass.SECUTORSTEF || ActorClass == ActorClass.URSULA || 
                ActorClass == ActorClass.VALENS;
        }
    }

    public bool IsCaster
    {
        get
        {
            return ActorClass == ActorClass.CHANNELERIMP || ActorClass == ActorClass.MONGRELSHAMAN || ActorClass == ActorClass.SUMMONER;
        }
    }

    public bool IsArcher
    {
        get
        {
            return ActorClass == ActorClass.AMAZON || ActorClass == ActorClass.ARCHER || ActorClass == ActorClass.ARCHERF || ActorClass == ActorClass.EIJI;
        }
    }

    public bool IsPeltast
    {
        get
        {
            return ActorClass == ActorClass.PELTAST || ActorClass == ActorClass.PELTASTF || ActorClass == ActorClass.GUNGNIR || ActorClass == ActorClass.GUNGNIRF;
        }
    }
}
[Flags]
public enum ActorClassAttributes
{
    Light = 1 << 0,
    Medium = 1 << 1,
    Heavy = 1 << 2,
    Arcane = 1 << 3,
    Support = 1 << 4,
    Beast = 1 << 5,
    Male = 1 << 6,
    Female = 1 << 7,
    Nordargh = 1 << 8,
    Imperia = 1 << 9,
    Steppes = 1 << 10,
    Expanse = 1 << 11,
    Human = 1 << 12
}


public class StatsSetBlock
{
    public StatsSetBlock(String line)
    {
        if (line.Contains("Samnite"))
        {
            int ibreak = 0;
        }
        string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 4)
        {
            MainClass = lineTokens[3];
        }
        else
        {
            int ibreak = 0;
        }

    }

    public void AddItem(StatsSet set)
    {
        List<StatsSet> setList = null;
        if (!SubBlockMap.TryGetValue(set.VariantName, out setList))
        {
            setList = new List<StatsSet>();
            SubBlockMap[set.VariantName] = setList;
        }

        setList.Add(set);

    }

    public string MainClass;
    public Dictionary<String, List<StatsSet>> SubBlockMap = new Dictionary<string, List<StatsSet>>();

}


public class StatsSet
{
    public String ClassName;
    public String VariantName;
    public int Level;
    public int CON;
    public int PWR;
    public int ACC;
    public int DEF;
    public int INI;
    public float MOV;


    public void Apply(CharacterData characterData)
    {
        characterData.Level = Level;
        characterData.CON = CON;
        characterData.PWR = PWR;
        characterData.ACC = ACC;
        characterData.DEF = DEF;
        characterData.INI = INI;
        characterData.MOV = MOV;
    }


}

public class ItemSet
{
    //public ModITEMStat(ItemSetSubBlock ownerBlock, String[] fields)
    public ItemSet(String[] fields)
    {
        if (fields.Length == 9)
        {
            //OwnerBlock = ownerBlock;
            int index = 1;
            VariantName = fields[index++];
            MinLevel = int.Parse(fields[index++]);
            MaxLevel = int.Parse(fields[index++]);
            Weapon = fields[index++];
            Armor = fields[index++];
            Shield = fields[index++];
            Helmet = fields[index++];
            Accessory = fields[index++];
        }
        else
        {
            int ibreak = 0;
        }
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
    public ItemSetBlock(String line)
    {
        string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 9)
        {

            MainClass = lineTokens[4];
            SubClass = lineTokens[5];
            Region = lineTokens[7];
            //Affinity = lineTokens[8];
        }
        else
        {
            int ibreak = 0;
        }
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
    //string Affinity;
    public Dictionary<String, List<ItemSet>> SubBlockMap = new Dictionary<string, List<ItemSet>>();

}


public class SkillSetBlock
{
    public SkillSetBlock(String line)
    {
        if (line.Contains("Samnite"))
        {
            int ibreak = 0;
        }

        string[] lineTokens = GladiusGlobals.SplitAndTidyString(line, new char[] { ' ' }, false);
        if (lineTokens.Length == 7)
        {
            MainClass = lineTokens[4];
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
        MinLevel = int.Parse(tokens[index++]);
        MaxLevel = int.Parse(tokens[index++]);
        SkillName = tokens[index++];
    }

    public string VariantName;
    public int MinLevel;
    public int MaxLevel;
    public String SkillName;

}

//}archer_idle.pan.archer_idleengaged.pan.archer_idlewounded.pan.archer_idlepassive.pan.archer_idledeath.pan.archer_death.pan.
//archer_reactvictory.pan.archer_reactloss.pan.archer_reacthitweakf.pan.archer_reacthitstrongl.pan.archer_reacthitstrongb.pan.
//archer_reacthitstrongr.pan.archer_reacthitstrongf.pan.archer_moverun.pan.archer_moveclimbhalf.pan.archer_movejumpdownhalf.pan
//.archer_reactdowned.pan.archer_reactknockdownb.pan.archer_movegetup.pan.archer_idleknockdown.pan.archer_reactknockback.pan

public static class CharacterAnimations
{
    public static string AnimationForCharacter(AnimationEnum animEnum, ActorClassData actorClassData)
    {
        switch (animEnum)
        {
            case AnimationEnum.Idle:
                return "idle";
            case AnimationEnum.IdleEngaged:
                return "idleengaged";
            case AnimationEnum.IdleWounded:
                return "idlewounded";
            case AnimationEnum.IdlePassive:
                return "idlepassive";
            case AnimationEnum.IdleDeath:
                return "idledeath";
            case AnimationEnum.Death:
                return "death";
            case AnimationEnum.ReactVictory:
                return "reactvictory";
            case AnimationEnum.ReactLoss:
                return "reactloss";
            case AnimationEnum.ReactHitWeakF:
                return "reacthitweakf";
            case AnimationEnum.ReactHitWeakB:
                return "reacthitweakb";
            case AnimationEnum.ReactHitWeakR:
                return "reacthitweakr";
            case AnimationEnum.ReactHitWeakL:
                return "reacthitweakl";
            case AnimationEnum.ReactHitStrongF:
                return "reacthitstrongf";
            case AnimationEnum.ReactHitStrongB:
                return "reacthitstrongb";
            case AnimationEnum.ReactHitStrongR:
                return "reacthitstrongr";
            case AnimationEnum.ReactHitStrongL:
                return "reacthitstrongl";
            case AnimationEnum.MoveRun:
                return "moverun";
            case AnimationEnum.MoveClimbHalf:
                return "moveclimbhalf";
            case AnimationEnum.MoveJumpDownHalf:
                return "movejumpdownhalf";
            case AnimationEnum.ReactDowned:
                return "reactdowned";
            case AnimationEnum.ReactKnockdownB:
                return "reactknockdownb";
            case AnimationEnum.MoveGetup:
                return "getup";
            case AnimationEnum.IdleKnockDown:
                return "idleknockdown";
            case AnimationEnum.ReactKnockback:
                return "reactknockback";
            case AnimationEnum.Attack1:
                if (actorClassData.IsArcher)
                {
                    return "bow_shot";
                }
                if (actorClassData.IsPeltast)
                {
                    return "spear_throw";
                }
                if (actorClassData.IsTwoHanded)
                {
                    return "2HStraightHit1";
                }
                if (actorClassData.IsOneHanded)
                {
                    return "Slice";
                }
                if (actorClassData.IsCaster)
                {
                    return "Casting";
                }
                return "";
            default:
                return "";
        }
    }

}