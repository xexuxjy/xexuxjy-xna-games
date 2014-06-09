using UnityEngine;
using System.Collections;
using Gladius;
using System.Collections.Generic;
using System;
using System.Xml;
using System.Text;

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

    public SchoolRank SchoolRank
    {
        get;
        set;
    }

    public void Load(String filename)
    {
        TextAsset textAsset = (TextAsset)Resources.Load(filename);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(textAsset.text);

        XmlNodeList nodes = doc.SelectNodes("//Character");
        foreach (XmlNode node in nodes)
        {
            CharacterData characterData = new CharacterData();
            characterData.Load(node as XmlElement);
            m_recruits.Add(characterData);
        }
    }

    //public void Save(StreamWriter streamWriter)
    //{
    //    streamWriter.Write("<School>");
    //    foreach (CharacterData character in m_recruits)
    //    {
    //        character.Save(streamWriter);
    //    }
    //    streamWriter.Write("</School>");

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

    public List<CharacterData> CurrentParty
    {
        get { return m_currentParty; }
    }

    public void LoadExtractedData(String path)
    {
        TextAsset textAsset = (TextAsset)Resources.Load("UberSchool");
        String data = textAsset.text;
        ParseExtractedData(data);
    }

    public HashSet<String> classSet = new HashSet<String>();

    public void ParseExtractedData(String data)
    {
        String[] lines = data.Split('\n');
        CharacterData currentCharacterData = null;
        for (int counter = 0; counter < lines.Length; counter++)
        {
            String line = lines[counter];
            if (line.StartsWith("//"))
            {
                continue;
            }

            String[] lineTokens = line.Split(new char[] { ',', ':' });
            for (int i = 0; i < lineTokens.Length; ++i)
            {
                lineTokens[i] = TidyString(lineTokens[i]);
            }

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
                currentCharacterData.INT = int.Parse(lineTokens[5]);
                currentCharacterData.MOVE = int.Parse(lineTokens[6]);
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
                m_schoolInventory.Add(lineTokens[1]);
            }

        }


    }

    char[] trimChars = new char[] { '"', '\r', '\t',' ' };
    public String TidyString(String input)
    {
        String temp = input.Replace("\"", "").Replace("\t", "").Trim(trimChars);
        int commentIndex = temp.IndexOf("//");
        if (commentIndex > 0)
        {
            temp = temp.Substring(0, commentIndex);
        }
        return temp;

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

        foreach (CharacterData unit in m_recruits)
        {
            sb.AppendLine(String.Format("CREATEUNIT: \"{0}\",\"{1}\"", unit.Name, unit.ActorClass));
            sb.AppendLine(String.Format("LEVEL: {0}", unit.Level));
            sb.AppendLine(String.Format("EXPERIENCE: {0}", unit.Experience));
            sb.AppendLine(String.Format("JOBPOINTS: {0}", unit.JobPoints));
            sb.AppendLine("	//            	CON, PWR, ACC, DEF, INT, MOVE");
            sb.AppendLine(String.Format("CORESTATSCOMP2: {0},{1},{2},{3},{4},{5}", unit.CON, unit.PWR, unit.ACC, unit.DEF, unit.INT, unit.MOVE));
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




    String schoolName;
    String heroName;
    int gold;
    SchoolRank m_currentRank = SchoolRank.Bronze;
    List<CharacterData> m_recruits = new List<CharacterData>();
    List<CharacterData> m_currentParty = new List<CharacterData>();
    List<String> m_schoolInventory = new List<String>();
}

