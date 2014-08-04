using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;


namespace Gladius
{
    public static class ActorGenerator
    {
        public static void SetActorStats(int level, ActorClass actorClass,CharacterData characterData)
        {
            // quick and dirty way of generating characters.
            int accuracy = 10 * level;
            int defense = 12 * level;
            int power = 8 * level;
            int cons = 10 * level;

    
            //BaseActor baseActor = new BaseActor();
            characterData.ACC = accuracy;
            characterData.DEF = defense;
            characterData.PWR = power;
            characterData.CON = cons;
            characterData.ActorClass = actorClass;
        }


        //public static void LoadActors(String filename, List<BaseActor> results)
        //{
        //    using (StreamReader sr = new StreamReader(filename))
        //    {
        //        String result = sr.ReadToEnd();
        //        XmlDocument doc = new XmlDocument();
        //        doc.LoadXml(result);
        //        XmlNodeList nodes = doc.SelectNodes("//Character");
        //        foreach (XmlNode node in nodes)
        //        {
        //            CharacterData characterData = new CharacterData();
        //            characterData.SetupCharacterData(node as XmlElement);
        //            BaseActor baseActor = new BaseActor();
        //            baseActor.SetupCharacterData(characterData);
        //            results.Add(baseActor);
        //        }
        //    }
        //}


        public static void InitXPLevel(String filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                String result = sr.ReadToEnd();
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(result);
                XmlNodeList nodes = doc.SelectNodes("//Class");
                foreach (XmlNode node in nodes)
                {
                    XmlElement element = node as XmlElement;
                    ActorClass actorClass = (ActorClass)Enum.Parse(typeof(ActorClass), element.Attributes["name"].Value);
                    String value = element.Attributes["value"].Value;
                    String[] values = value.Split(',');
                    int[] xpSteps = new int[GladiusGlobals.MaxLevel];
                    for (int i = 0; i < values.Length; ++i)
                    {
                        xpSteps[i] = int.Parse(values[i]);
                    }
                    ActorXPLevels[actorClass] = xpSteps;
                }
            }
        }

        public static void InitCategories()
        {        

            CategoryClass[ActorClass.Amazon] = ActorCategory.Support;
            CategoryClass[ActorClass.Archer] = ActorCategory.Support;
            CategoryClass[ActorClass.ArcherF] = ActorCategory.Support;
            CategoryClass[ActorClass.BanditA] = ActorCategory.Light;
            CategoryClass[ActorClass.BanditAF] = ActorCategory.Light;
            CategoryClass[ActorClass.BanditB] = ActorCategory.Light;
            CategoryClass[ActorClass.Barbarian] = ActorCategory.Medium;
            CategoryClass[ActorClass.BarbarianF] = ActorCategory.Medium;
            CategoryClass[ActorClass.Bear] = ActorCategory.Heavy;
            CategoryClass[ActorClass.Berserker] = ActorCategory.Medium;
            CategoryClass[ActorClass.BerserkerF] = ActorCategory.Medium;
            CategoryClass[ActorClass.Cat] = ActorCategory.Medium;
            CategoryClass[ActorClass.CatGreater] = ActorCategory.Medium;
            CategoryClass[ActorClass.Centurion] = ActorCategory.Heavy;
            CategoryClass[ActorClass.CenturionF] = ActorCategory.Heavy;
            CategoryClass[ActorClass.ChannelerImp] = ActorCategory.Arcane;
            CategoryClass[ActorClass.Cyclops] = ActorCategory.Medium;
            CategoryClass[ActorClass.CyclopsGreater] = ActorCategory.Medium;
            CategoryClass[ActorClass.DarkLegionnaire] = ActorCategory.Medium;
            CategoryClass[ActorClass.Dervish] = ActorCategory.Light;
            CategoryClass[ActorClass.DervishF] = ActorCategory.Light;

            CategoryClass[ActorClass.Eiji] = ActorCategory.Support;
            CategoryClass[ActorClass.Galdr] = ActorCategory.Support;

            CategoryClass[ActorClass.Gungnir] = ActorCategory.Support;
            CategoryClass[ActorClass.GungnirF] = ActorCategory.Support;

            CategoryClass[ActorClass.Gwazi] = ActorCategory.Light;
            CategoryClass[ActorClass.Legionnaire] = ActorCategory.Medium;
            CategoryClass[ActorClass.LegionnaireF] = ActorCategory.Medium;

            CategoryClass[ActorClass.Ludo] = ActorCategory.Medium;
            CategoryClass[ActorClass.Minotaur] = ActorCategory.Heavy;
            CategoryClass[ActorClass.Mongrel] = ActorCategory.Medium;
            CategoryClass[ActorClass.MongrelShaman] = ActorCategory.Arcane;
            CategoryClass[ActorClass.Murmillo] = ActorCategory.Medium;
            CategoryClass[ActorClass.Ogre] = ActorCategory.Heavy;
            CategoryClass[ActorClass.Peltast] = ActorCategory.Support;
            CategoryClass[ActorClass.PeltastF] = ActorCategory.Support;

            CategoryClass[ActorClass.SamniteExp] = ActorCategory.Heavy;
            CategoryClass[ActorClass.SamniteExpF] = ActorCategory.Heavy;
            CategoryClass[ActorClass.SamniteImp] = ActorCategory.Heavy;
            CategoryClass[ActorClass.SamniteImpF] = ActorCategory.Heavy;
            CategoryClass[ActorClass.SamniteSte] = ActorCategory.Heavy;
            CategoryClass[ActorClass.SamniteSteF] = ActorCategory.Heavy;

            CategoryClass[ActorClass.Satyr] = ActorCategory.Medium;
            CategoryClass[ActorClass.Scarab] = ActorCategory.Medium;
            CategoryClass[ActorClass.Scorpion] = ActorCategory.Medium;
            CategoryClass[ActorClass.SecutorImp] = ActorCategory.Medium;
            CategoryClass[ActorClass.SecutorImpF] = ActorCategory.Medium;
            CategoryClass[ActorClass.SecutorSte] = ActorCategory.Medium;
            CategoryClass[ActorClass.SecutorSteF] = ActorCategory.Medium;
            CategoryClass[ActorClass.Summoner] = ActorCategory.Arcane;
            CategoryClass[ActorClass.UndeadMeleeImpA] = ActorCategory.Medium;
            CategoryClass[ActorClass.UndeadCasterA] = ActorCategory.Support;
            CategoryClass[ActorClass.Urlan] = ActorCategory.Medium;
            CategoryClass[ActorClass.UrsulaCostumeA] = ActorCategory.Medium;
            CategoryClass[ActorClass.UrsulaCostumeB] = ActorCategory.Medium;
            CategoryClass[ActorClass.ValensCostumeA] = ActorCategory.Medium;
            CategoryClass[ActorClass.ValensCostumeB] = ActorCategory.Medium;

            CategoryClass[ActorClass.Valens] = ActorCategory.Medium;
            CategoryClass[ActorClass.Wolf] = ActorCategory.Light;
            CategoryClass[ActorClass.Yeti] = ActorCategory.Heavy;
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

            int level = -1; // base of player level?



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

            String className = characterData.ActorClass.ToString();
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

        public static Dictionary<ActorClass, ActorCategory> CategoryClass = new Dictionary<ActorClass, ActorCategory>();
        public static Dictionary<ActorClass, int[]> ActorXPLevels = new Dictionary<ActorClass, int[]>();
        public static ActorClass[] Heavy = new ActorClass[] { };
        public static ActorClass[] Medium = new ActorClass[] { };
        public static ActorClass[] Heavies = new ActorClass[] { };

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


}
