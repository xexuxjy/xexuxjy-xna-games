﻿using UnityEngine;
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




        public static Dictionary<ActorClass, ActorCategory> CategoryClass = new Dictionary<ActorClass, ActorCategory>();
        public static Dictionary<ActorClass, int[]> ActorXPLevels = new Dictionary<ActorClass, int[]>();
        public static ActorClass[] Heavy = new ActorClass[] { };
        public static ActorClass[] Medium = new ActorClass[] { };
        public static ActorClass[] Heavies = new ActorClass[] { };

    }
}