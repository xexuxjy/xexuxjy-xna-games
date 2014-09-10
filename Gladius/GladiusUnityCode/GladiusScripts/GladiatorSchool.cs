using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Text;
using System.IO;
namespace Gladius
{
    public class GladiatorSchool
    {

        public void Hire(CharacterData gladiator)
        {
            System.Diagnostics.Debug.Assert(!m_recruits.ContainsKey(gladiator.Name));
        }

        public void Fire(CharacterData gladiator)
        {
            System.Diagnostics.Debug.Assert(m_recruits.ContainsKey(gladiator.Name));

        }

        public SchoolRank SchoolRank
        {
            get;
            set;
        }

        CharacterData m_currentCharacter;
        public CharacterData CurrentCharacter
        {
            get { return m_currentCharacter; }
            set { m_currentCharacter = value; }
        }

        public int CurrentSize
        {
            get;
            set;
        }

        public int GetMaxSize()
        {
            switch (m_currentRank)
            {
                case SchoolRank.Bronze: return 8;
                case SchoolRank.Silver: return 16;
                case SchoolRank.Gold: return 24;
            }
            System.Diagnostics.Debug.Assert(false);
            return -1;
        }

        public Dictionary<String,CharacterData> Gladiators
        {
            get { return m_recruits; }
        }

        public List<CharacterData> CurrentParty
        {
            get { return m_currentParty; }
        }

        public void Load(String name)
        {
            TextAsset textAsset = (TextAsset)Resources.Load(GladiusGlobals.SchoolsPath+name);
            String data = textAsset.text;
            Parse(data);
        }

        public HashSet<String> classSet = new HashSet<String>();

        public void Parse(String data)
        {
            String[] lines = data.Split('\n');
            CharacterData currentCharacterData = null;
            char[] tokens = new char[] { ',', ':' };
         
            for (int counter = 0; counter < lines.Length; counter++)
            {
                String line = lines[counter].Trim();
                if (line.StartsWith("//"))
                {
                    continue;
                }


                String[] lineTokens = GladiusGlobals.SplitAndTidyString(line,tokens);

                if (lineTokens[0] == "NAME")
                {
                    schoolName = lineTokens[1];
                }
                else if (lineTokens[0] == "HERO")
                {
                    heroName = lineTokens[1];
                }
                else if (lineTokens[0] == "GOLD")
                {
                    gold = int.Parse(lineTokens[1]);
                }
                else if (lineTokens[0] == "CREATEUNIT")
                {
                    currentCharacterData = new CharacterData();
                    currentCharacterData.Name = lineTokens[1];
                    currentCharacterData.ActorClass = (ActorClass)Enum.Parse(typeof(ActorClass), lineTokens[2]);
                    classSet.Add(lineTokens[2]);

                    m_recruits[currentCharacterData.Name] = currentCharacterData;

                }
                else if (lineTokens[0] == "LEVEL")
                {
                    currentCharacterData.Level = int.Parse(lineTokens[1]);
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
                    currentCharacterData.MOV = float.Parse(lineTokens[6]);
                }
                else if (lineTokens[0] == "ITEMSCOMP")
                {
                    //	weapon,	armor,	shield,	helmet,	accessory
                    currentCharacterData.AddItemByNameAndLoc(lineTokens[1], ItemLocation.Weapon);
                    currentCharacterData.AddItemByNameAndLoc(lineTokens[2], ItemLocation.Armor);
                    currentCharacterData.AddItemByNameAndLoc(lineTokens[3], ItemLocation.Shield);
                    currentCharacterData.AddItemByNameAndLoc(lineTokens[4], ItemLocation.Helmet);
                    currentCharacterData.AddItemByNameAndLoc(lineTokens[5], ItemLocation.Accessory);
                }
                else if (lineTokens[0] == "SKILL")
                {
                    currentCharacterData.AddSkill(lineTokens[1]);
                }
                else if (lineTokens[0] == "INVENTORY")
                {
                    AddToInventory(lineTokens[1]);
                }

            }
        }


        public String SaveSchool()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(String.Format("NAME: \"{0}\"", schoolName));
            sb.AppendLine(String.Format("HERO: \"{0}\"", heroName));
            sb.AppendLine(String.Format("GOLD: {0}", gold));

            foreach (String item in m_schoolInventory)
            {
                sb.AppendLine(String.Format("INVENTORY: \"{0}\"", item));
            }

            foreach (CharacterData unit in m_recruits.Values)
            {
                sb.AppendLine(String.Format("CREATEUNIT: \"{0}\",\"{1}\"", unit.Name, unit.ActorClass));
                sb.AppendLine(String.Format("LEVEL: {0}", unit.Level));
                sb.AppendLine(String.Format("EXPERIENCE: {0}", unit.Experience));
                sb.AppendLine(String.Format("JOBPOINTS: {0}", unit.JobPoints));
                sb.AppendLine("	//            	CON, PWR, ACC, DEF, INT, MOVE");
                sb.AppendLine(String.Format("CORESTATSCOMP2: {0},{1},{2},{3},{4},{5}", unit.CON, unit.PWR, unit.ACC, unit.DEF, unit.INI, unit.MOV));
                sb.AppendLine("//	weapon,	armor,	shield,	helmet,	accessory");
                sb.AppendLine(String.Format("ITEMSCOMP: \"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\"", unit.GetItemNameAtLoc(ItemLocation.Weapon), unit.GetItemNameAtLoc(ItemLocation.Armor), unit.GetItemNameAtLoc(ItemLocation.Shield), unit.GetItemNameAtLoc(ItemLocation.Helmet), unit.GetItemNameAtLoc(ItemLocation.Accessory)));

                foreach (String skill in unit.m_skillList)
                {
                    sb.AppendLine(String.Format("SKILL: \"{0}\"", skill));
                }
                sb.AppendLine();
                sb.AppendLine();
            }


            return sb.ToString();
        }

        public void AddToInventory(String item)
        {
            m_schoolInventory.Add(item);

        }

        public void RemoveFromInventory(String item)
        {
            m_schoolInventory.Remove(item);
        }


        String schoolName;
        String heroName;
        int gold;
        SchoolRank m_currentRank = SchoolRank.Bronze;
        Dictionary<String, CharacterData> m_recruits = new Dictionary<String, CharacterData>();
        List<CharacterData> m_currentParty = new List<CharacterData>();
        List<String> m_schoolInventory = new List<String>();
    }

}