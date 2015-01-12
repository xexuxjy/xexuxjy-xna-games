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
            InitCategories();
            LoadALLMODCoreStats();
        }

        //public static void SetActorStats(int level, ActorClass actorClass, CharacterData characterData)
        //{
        //    // quick and dirty way of generating characters.
        //    int accuracy = 10 * level;
        //    int defense = 12 * level;
        //    int power = 8 * level;
        //    int cons = 10 * level;

    
        //    //BaseActor baseActor = new BaseActor();
        //    characterData.ACC = accuracy;
        //    characterData.DEF = defense;
        //    characterData.PWR = power;
        //    characterData.CON = cons;
        //    characterData.ActorClass = actorClass;
        //}


        public static void InitCategories()
        {

            ClassDataMap[ActorClass.Amazon] = new ActorClassData("Amazon","amazon", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Support));
            ClassDataMap[ActorClass.Archer] = new ActorClassData("Archer", "archer",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Support));
            ClassDataMap[ActorClass.ArcherF] = new ActorClassData("Archer", "archerF",(ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Support));
            ClassDataMap[ActorClass.BanditA] = new ActorClassData("Bandit", "banditA",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.BanditAF] = new ActorClassData("Bandit","banditAF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.BanditB] = new ActorClassData("Bandit", "banditB",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.Barbarian] = new ActorClassData("Barbarian", "barbarian",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.BarbarianF] = new ActorClassData("Barbarian", "barbarianF",(ActorClassAttributes.Female| ActorClassAttributes.Human | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.Bear] = new ActorClassData("Bear", "bear",(ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.BearGreater] = new ActorClassData("Bear", "beargreater",(ActorClassAttributes.Beast | ActorClassAttributes.Heavy | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.Berserker] = new ActorClassData("Berserker", "berserker",(ActorClassAttributes.Male| ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.BerserkerF] = new ActorClassData("Berserker","berserkerf", (ActorClassAttributes.Female| ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Nordargh));
            ClassDataMap[ActorClass.Cat] = new ActorClassData("Cat", "cat",(ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
            ClassDataMap[ActorClass.CatGreater] = new ActorClassData("Cat","catgreater", (ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium ));
            ClassDataMap[ActorClass.Centurion] = new ActorClassData("Centurion", "centurion",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.CenturionF] = new ActorClassData("Centurion", "centurionF",(ActorClassAttributes.Female| ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.ChannelerImp] = new ActorClassData("Channeler", "channelerImp",(ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Arcane| ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.Cyclops] = new ActorClassData("Cyclops", "cyclops",(ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium ));
            ClassDataMap[ActorClass.CyclopsGreater] = new ActorClassData("Cyclops", "cyclopsgreater",(ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
            ClassDataMap[ActorClass.DarkLegionnaire] = new ActorClassData("DarkLegionnaire","legionnaireDark", (ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.Dervish] = new ActorClassData("Dervish", "dervish",(ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.DervishF] = new ActorClassData("Dervish", "dervishF",(ActorClassAttributes.Female | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Dummy] = new ActorClassData("Dummy", "prop_practicepost1",(ActorClassAttributes.Male | ActorClassAttributes.Light));


            ClassDataMap[ActorClass.Eiji] = new ActorClassData("Eiji", "eiji",(ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Steppes | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Galdr] = new ActorClassData("Galdr", "galdr",(ActorClassAttributes.Female | ActorClassAttributes.Arcane | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

            ClassDataMap[ActorClass.Galverg] = new ActorClassData("Galverg", "galverg",(ActorClassAttributes.Male | ActorClassAttributes.Heavy| ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

            ClassDataMap[ActorClass.Gungnir] = new ActorClassData("Gungir", "gungir",(ActorClassAttributes.Male | ActorClassAttributes.Support | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.GungnirF] = new ActorClassData("Gungir", "gungirF",(ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));

            ClassDataMap[ActorClass.Gwazi] = new ActorClassData("Gwazi", "gwazi",(ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Expanse | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Legionnaire] = new ActorClassData("Legionnarie", "legionnaire",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.LegionnaireF] = new ActorClassData("Legionnarie", "legionanaireF",(ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

            ClassDataMap[ActorClass.Ludo] = new ActorClassData("Ludo", "ludo",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Minotaur] = new ActorClassData("Minotaur", "minotaur",(ActorClassAttributes.Beast | ActorClassAttributes.Heavy));
            ClassDataMap[ActorClass.Mongrel] = new ActorClassData("Mongrel", "mongrel",(ActorClassAttributes.Male | ActorClassAttributes.Light | ActorClassAttributes.Nordargh | ActorClassAttributes.Beast));
            ClassDataMap[ActorClass.MongrelShaman] = new ActorClassData("MongrelShaman","mongrelshaman", (ActorClassAttributes.Male | ActorClassAttributes.Arcane| ActorClassAttributes.Nordargh | ActorClassAttributes.Beast));
            ClassDataMap[ActorClass.Murmillo] = new ActorClassData("Murmillo", "murmillo",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.MurmilloF] = new ActorClassData("Murmillo", "murmilloF",(ActorClassAttributes.Female| ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Mutuus] = new ActorClassData("Mutuus", "mutuus",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

            ClassDataMap[ActorClass.Ogre] = new ActorClassData("Ogre", "ogre",(ActorClassAttributes.Beast | ActorClassAttributes.Heavy));
            ClassDataMap[ActorClass.Peltast] = new ActorClassData("Peltast", "peltast",(ActorClassAttributes.Male | ActorClassAttributes.Support| ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.PeltastF] = new ActorClassData("Peltast", "peltastF",(ActorClassAttributes.Female | ActorClassAttributes.Support | ActorClassAttributes.Imperia | ActorClassAttributes.Human));

            ClassDataMap[ActorClass.SamniteExp] = new ActorClassData("Samnite", "samniteexp",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Expanse));
            ClassDataMap[ActorClass.SamniteExpF] = new ActorClassData("Samnite","samniteexpF", (ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Expanse));
            ClassDataMap[ActorClass.SamniteImp] = new ActorClassData("Samnite", "samniteimp",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.SamniteImpF] = new ActorClassData("Samnite", "samniteimpF",(ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.SamniteSte] = new ActorClassData("Samnite", "samniteste",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Steppes));
            ClassDataMap[ActorClass.SamniteSteF] = new ActorClassData("Samnite", "samnitesteF",(ActorClassAttributes.Female| ActorClassAttributes.Human | ActorClassAttributes.Heavy | ActorClassAttributes.Steppes));

            ClassDataMap[ActorClass.Satyr] = new ActorClassData("Satyr", "satyr",(ActorClassAttributes.Male| ActorClassAttributes.Beast | ActorClassAttributes.Arcane ));
            ClassDataMap[ActorClass.Scarab] = new ActorClassData("Scarab", "scarab",(ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Medium));
            ClassDataMap[ActorClass.Scorpion] = new ActorClassData("Scorpion", "scorpion",(ActorClassAttributes.Male| ActorClassAttributes.Beast | ActorClassAttributes.Medium ));
            ClassDataMap[ActorClass.SecutorImp] = new ActorClassData("Secutor", "secutorimp",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.SecutorImpF] = new ActorClassData("Secutor", "secutorimpF",(ActorClassAttributes.Female| ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.SecutorSte] = new ActorClassData("Secutor", "secutorste",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Steppes));
            ClassDataMap[ActorClass.SecutorSteF] = new ActorClassData("Secutor", "secutorsteF",(ActorClassAttributes.Female | ActorClassAttributes.Human | ActorClassAttributes.Light | ActorClassAttributes.Steppes));
            ClassDataMap[ActorClass.Summoner] = new ActorClassData("Summoner", "summoner",(ActorClassAttributes.Male | ActorClassAttributes.Human | ActorClassAttributes.Arcane));
            ClassDataMap[ActorClass.UndeadMeleeImpA] = new ActorClassData("UndeadLegionnaire", "skeletonimp2",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia));
            ClassDataMap[ActorClass.UndeadCasterA] = new ActorClassData("UndeadSummoner", "skeletonimp2", (ActorClassAttributes.Male | ActorClassAttributes.Arcane));
            ClassDataMap[ActorClass.Urlan] = new ActorClassData("Urlan", "urlan",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh| ActorClassAttributes.Human));
            ClassDataMap[ActorClass.UrsulaCostumeA] = new ActorClassData("Ursula", "ursula",(ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.UrsulaCostumeB] = new ActorClassData("Ursula", "ursula",(ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Ursula] = new ActorClassData("Ursula", "ursula",(ActorClassAttributes.Female | ActorClassAttributes.Medium | ActorClassAttributes.Nordargh | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.ValensCostumeA] = new ActorClassData("Valens", "valens",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.ValensCostumeB] = new ActorClassData("Valens", "valens",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Valens] = new ActorClassData("Valens", "valens",(ActorClassAttributes.Male | ActorClassAttributes.Medium | ActorClassAttributes.Imperia | ActorClassAttributes.Human));
            ClassDataMap[ActorClass.Wolf] = new ActorClassData("Wolf", "wolf",(ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Light));
            ClassDataMap[ActorClass.WolfGreater] = new ActorClassData("Wolf", "wolfgreater",(ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Light));
            ClassDataMap[ActorClass.Yeti] = new ActorClassData("Yeti", "yeti",(ActorClassAttributes.Male | ActorClassAttributes.Beast | ActorClassAttributes.Heavy| ActorClassAttributes.Nordargh));
        }


        public static void SetActorSkillStatsForLevel(BaseActor baseActor)
        {

        }

        public static bool CheckLevelUp(CharacterData characterData, int xpGained)
        {
            return false;
        }


        public static CharacterData GenerateRandomCharacterUNITDB(String[] tokens)
        {
            CharacterData characterData = new CharacterData();
            int counter =0;
            String name = tokens[counter++];
            int val1 = int.Parse(tokens[counter++]);
            String startSlot = tokens[counter++];
            
            int minLevel = int.Parse(tokens[counter++]);
            int maxLevel  = int.Parse(tokens[counter++]);
            int val4 = int.Parse(tokens[counter++]);

            int level = -1; // base of player level?u



            if (minLevel > 0 && maxLevel > 0)
            {
                
                level = GladiusGlobals.Random.Next(minLevel, maxLevel);
            }


            characterData.Level = level;
            characterData.Name = name;

     
            //characterData



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

            String className = characterData.ActorClassData.Name;
            ModCOREStat statData = StatForClassLevel(className, level);
            if (statData != null)
            {
                characterData.CON = statData.CON;
                characterData.PWR = statData.PWR;
                characterData.ACC= statData.ACC;
                characterData.DEF= statData.DEF;
                characterData.INI= statData.INI;
                characterData.MOV= statData.MOV;
            }


            return characterData;
        }


        public static void BuildClassesForMask(HashSet<String> classes)
        {

            // 0 = mongel? nah
            // 1 = light
            // 2 = mediumu
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


        }

        public static void LoadALLMODCoreStats()
        {
            
            TextAsset[] allFiles = Resources.LoadAll<TextAsset>("Resources/ExtractedData/ModCoreStatFiles");
            foreach(TextAsset file in allFiles)
            {
                String[] lines = file.text.Split('\n');
                LoadMODCOREStats(lines);
            }
        }


        public static void LoadMODCOREStats(String[] fileData)
        {
            if (fileData.Length > 0)
            {
                String className = "unk";
                if (fileData[0].StartsWith("// STATSET"))
                {
                    className = fileData[0].Substring(fileData[0].LastIndexOf(":") + 1);
                    className = className.Trim();

                    if (!ClassVariantStatData.ContainsKey(className))
                    {
                        ClassVariantStatData[className] = new Dictionary<String, List<ModCOREStat>>();
                    }


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

                            ModCOREStat modCOREStat = new ModCOREStat();
                            modCOREStat.className = className;
                            modCOREStat.variantName = variant;
                            modCOREStat.level = int.Parse(level);
                            modCOREStat.CON = int.Parse(con);
                            modCOREStat.PWR = int.Parse(pow);
                            modCOREStat.ACC = int.Parse(acc);
                            modCOREStat.DEF = int.Parse(def);
                            modCOREStat.INI = int.Parse(ini);
                            modCOREStat.MOV = float.Parse(mov);

                            List<ModCOREStat> statList;
                            if (!ClassVariantStatData[className].ContainsKey(variant))
                            {
                                ClassVariantStatData[className][variant] = new List<ModCOREStat>();
                            }

                            statList = ClassVariantStatData[className][variant];

                            statList.Add(modCOREStat);


                            int ibreak = 0;
                        }
                    }


                }
            }



        }

        public static ModCOREStat StatForClassLevel(string className,int level)
        {
            ModCOREStat stats = null;
            Dictionary<String,List<ModCOREStat>> row;

            if(ClassVariantStatData.TryGetValue(className,out row))
            {
                // find the first variant that contains the level?
                foreach(String key in row.Keys)
                {
                    List<ModCOREStat> statRow = row[key];
                    if(statRow.Count >= level)
                    {
                        stats = statRow[level];
                        break;
                    }
                }
            }

            return stats;
        }


        public static Dictionary<String, Dictionary<String, List<ModCOREStat>>> ClassVariantStatData = new Dictionary<String, Dictionary<String, List<ModCOREStat>>>();

        public static Dictionary<ActorClass, ActorClassData> ClassDataMap = new Dictionary<ActorClass, ActorClassData>();


        public static Dictionary<ActorClass, int[]> ActorXPLevels = new Dictionary<ActorClass, int[]>();
        public static ActorClass[] Heavy = new ActorClass[] {};
        public static ActorClass[] Medium = new ActorClass[] { };
        public static ActorClass[] Heavies = new ActorClass[] { };

    }

    public class ActorClassData
    {
        public String Name;
        public String MeshName;
        ActorClassAttributes m_mask;

        public ActorClassData(String name, String meshName,ActorClassAttributes mask)
        {
            Name = name;
            MeshName = meshName;
            m_mask = mask;
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

        public bool IsMale { get { return (m_mask & ActorClassAttributes.Male) > 0; } }
        public bool IsFemale { get { return (m_mask & ActorClassAttributes.Female) > 0; } }
        public bool IsLight { get { return (m_mask & ActorClassAttributes.Light) > 0; } }
        public bool IsMedium { get { return (m_mask & ActorClassAttributes.Medium) > 0; } }
        public bool IsHeavy { get { return (m_mask & ActorClassAttributes.Heavy) > 0; } }
        public bool IsArcane { get { return (m_mask & ActorClassAttributes.Arcane) > 0; } }
        public bool IsSupport { get { return (m_mask & ActorClassAttributes.Support) > 0; } }
        public bool IsBeast { get { return (m_mask & ActorClassAttributes.Beast) > 0; } }
        public bool IsNordargh { get { return (m_mask & ActorClassAttributes.Nordargh) > 0; } }
        public bool IsImperia { get { return (m_mask & ActorClassAttributes.Imperia) > 0; } }
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


    public class ModCOREStat
    {
        public String className;
        public String variantName;
        public int level;
        public int CON;
        public int PWR;
        public int ACC;
        public int DEF;
        public int INI;
        public float MOV;

    }


//}
