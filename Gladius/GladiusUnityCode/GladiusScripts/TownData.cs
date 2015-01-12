using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Xml;
using Gladius;

public class TownData : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToXml(StringBuilder sb)
    {
        sb.AppendFormat("<Town name=\"{0}\" region=\"{1}\" popularity=\"{2}\"/>", TownName, TownRegion, Popularity);
    }

    public static TownData FromXml(XmlNode node)
    {
        TownData td = new TownData();
        td.TownName = node.SelectSingleNode("@name").Value;
        td.TownRegion = node.SelectSingleNode("@region").Value;
        //td.Popularity = int.Parse(node.SelectSingleNode("@popularity").Value);
        return td;
    }

    public int Popularity
    {
        get
        {
            if (GladiusGlobals.GameStateManager.OverlandStateCommon.GladiatorSchool != null)
            {
                return GladiusGlobals.GameStateManager.OverlandStateCommon.GladiatorSchool.TownPopularity(TownName);
            }
            return 0;
        }
    }


    public String TownName = "A Town";
    public String TownRegion = "Imperia";
    public String TownDataPath;
    private int m_popularity;
}
