using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TownManager
{
    public void Load()
    {
        TextAsset allTownData = (TextAsset)Resources.Load(GladiusGlobals.TownsPath + "AllTowns");
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(allTownData.text);
        XmlNodeList towns = doc.SelectNodes("//Town");
        foreach (XmlNode town in towns)
        {
            TownData townData = TownData.FromXml(town);
            m_townDictionary[townData.Name] = townData;
        }
    }

    public TownData Find(string name)
    {
        TownData result = null;
        m_townDictionary.TryGetValue(name, out result);
        return result;

    }




    private Dictionary<string, TownData> m_townDictionary = new Dictionary<string,TownData>();
}
