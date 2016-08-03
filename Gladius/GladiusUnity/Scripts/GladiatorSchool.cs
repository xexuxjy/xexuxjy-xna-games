using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Text;
using System.IO;
//namespace Gladius
//{
public class GladiatorSchool
{

    public void Hire(CharacterData gladiator)
    {
        System.Diagnostics.Debug.Assert(!m_recruits.Contains(gladiator));
    }

    public void Fire(CharacterData gladiator)
    {
        System.Diagnostics.Debug.Assert(m_recruits.Contains(gladiator));

    }

    // per battle value
    public bool AllDead
    {
        get; set;
    }


    public int Days
    { get; set; }

    public int Gold
    { get; set; }

    public string Name
    { get; set; }

    public string HeroName
    { get; set; }

    public SchoolRank SchoolRank
    {
        get;
        set;
    }

    //CharacterData m_currentCharacter;
    //public CharacterData CurrentCharacter
    //{
    //    get { return m_currentCharacter; }
    //    set { m_currentCharacter = value; }
    //}

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

    public List<CharacterData> Gladiators
    {
        get { return m_recruits; }
    }

    public CharacterData GetGladiator(string name)
    {
        return m_recruits.Find(x => x.Name == name);
    }

    public List<CharacterData> CurrentParty
    {
        get { return m_currentParty; }
    }

    public void SetCurrentParty(List<CharacterData> party)
    {
        m_currentParty.Clear();
        m_currentParty.AddRange(party);
    }


    public void Load(String name)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(GladiusGlobals.SchoolsPath + name);
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
                currentCharacterData = new CharacterData();
                currentCharacterData.Name = lineTokens[1];
                currentCharacterData.ActorClass = (ActorClass)Enum.Parse(typeof(ActorClass), lineTokens[2].ToUpper());
                classSet.Add(lineTokens[2]);

                //m_recruits[currentCharacterData.Name] = currentCharacterData;
                m_recruits.Add(currentCharacterData);

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
        sb.AppendLine(String.Format("NAME: \"{0}\"", Name));
        sb.AppendLine(String.Format("HERO: \"{0}\"", HeroName));
        sb.AppendLine(String.Format("GOLD: {0}", Gold));

        foreach (String item in m_schoolInventory)
        {
            sb.AppendLine(String.Format("INVENTORY: \"{0}\"", item));
        }

        foreach (CharacterData unit in m_recruits)
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

    public void SaveSchoolXml(StringBuilder sb)
    {
        sb.AppendFormat("<School name=\"{0}\"  hero=\"{1}\" gold=\"{3}\" days=\"{4}\">");
        sb.Append("<Inventory>");
        foreach (String item in m_schoolInventory)
        {
            sb.AppendFormat("<Item id=\"{0}\"/>", item);
        }
        sb.Append("</Inventory>");
        foreach (CharacterData unit in m_recruits)
        {
            unit.ToXml(sb);
        }

        sb.Append("<Badges>");
        foreach (string badge in m_completedBadges)
        {
            sb.AppendFormat("<Badge id=\"{0}\"/>", badge);
        }
        sb.Append("</Badges>");

        sb.Append("<Popularity>");
        foreach (String town in m_townPopularity.Keys)
        {
            sb.AppendFormat("<Town name=\"{0}\" popularity=\"{1}\">", town, m_townPopularity[town]);
        }
        sb.Append("</Popularity>");

        sb.Append("</School>");
    }

    public static GladiatorSchool LoadSchoolXml(XmlDocument doc)
    {
        GladiatorSchool school = new GladiatorSchool();
        school.Name = doc.SelectSingleNode("School/@name").Value;
        school.HeroName = doc.SelectSingleNode("School/@hero").Value;
        school.Gold = int.Parse(doc.SelectSingleNode("School/@gold").Value);
        school.Days = int.Parse(doc.SelectSingleNode("School/@days").Value);
        XmlNodeList items = doc.SelectNodes("//Item");
        foreach (XmlNode node in items)
        {
            school.AddToInventory(node.SelectSingleNode("@id").Value);
        }

        XmlNodeList recruits = doc.SelectNodes("//Unit");
        foreach (XmlNode recruit in recruits)
        {
            CharacterData cd = CharacterData.FromXml(recruit);
            school.m_recruits.Add(cd);
        }

        XmlNodeList badges = doc.SelectNodes("//Badge");
        foreach (XmlNode node in badges)
        {
            school.m_completedBadges.Add(node.SelectSingleNode("@id").Value);
        }

        XmlNodeList towns = doc.SelectNodes("//Town");
        foreach (XmlNode node in badges)
        {
            school.m_townPopularity[node.SelectSingleNode("@name").Value] = int.Parse(node.SelectSingleNode("@popularity").Value);
        }

        return school;

    }





    public void AddToInventory(String item)
    {
        m_schoolInventory.Add(item);

    }

    public void RemoveFromInventory(String item)
    {
        m_schoolInventory.Remove(item);
    }

    public Color SchoolColour
    {
        get
        {
            if (TeamName == GladiusGlobals.PlayerTeam)
            {
                return Color.blue;
            }
            else if (TeamName == GladiusGlobals.EnemyTeam1)
            {
                return Color.yellow;
            }
            else if (TeamName == GladiusGlobals.EnemyTeam2)
            {
                return Color.magenta;
            }
            else if (TeamName == GladiusGlobals.EnemyTeam3)
            {
                return Color.green;
            }
            return Color.black;
        }

    }


    public int TownPopularity(string townName)
    {
        int val = 0;
        m_townPopularity.TryGetValue(townName, out val);
        return val;
    }
    public String TeamName;


    SchoolRank m_currentRank = SchoolRank.Bronze;
    //Dictionary<String, CharacterData> m_recruits = new Dictionary<String, CharacterData>();
    List<CharacterData> m_recruits = new List<CharacterData>();
    Dictionary<String, int> m_townPopularity = new Dictionary<string, int>();
    List<CharacterData> m_currentParty = new List<CharacterData>();
    List<String> m_schoolInventory = new List<string>();
    List<String> m_completedBadges = new List<string>();

}

//}
