using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Xml;
using Gladius;

public class TownData
{

    public void ToXml(StringBuilder sb)
    {
        sb.AppendFormat("<Town name=\"{0}\" region=\"{1}\" popularity=\"{2}\"/>", Name, Region, Popularity);
    }

    public static TownData FromXml(XmlNode node)
    {
        TownData td = new TownData();
        td.Name = node.SelectSingleNode("@name").Value;
        td.Region = node.SelectSingleNode("@region").Value;
        td.ShopFile = node.SelectSingleNode("@shopFile").Value;
        td.LeagueFile = node.SelectSingleNode("@leagueFile").Value;
        td.BackgroundTextureName = node.SelectSingleNode("@texture").Value;
        td.Arena = node.SelectSingleNode("@arena").Value;
        return td;
    }

    public int Popularity
    {
        get
        {
            if (GladiusGlobals.GameStateManager.OverlandStateCommon.GladiatorSchool != null)
            {
                return GladiusGlobals.GameStateManager.OverlandStateCommon.GladiatorSchool.TownPopularity(Name);
            }
            return 0;
        }
    }

    public void BuildData()
    {
        if (Shop == null && LeagueData == null)
        {
            Shop = ShopManager.Load(ShopFile);
            LeagueData = LeagueManager.Load(LeagueFile);
            BackgroundTexture = Resources.Load<Texture2D>(GladiusGlobals.TownsPath + BackgroundTextureName);

        }
    }


    public String Name = "A Town";
    public String Region = "Imperia";
    public String Arena = "";
    public String ShopFile;
    public String LeagueFile;
    public String BackgroundTextureName;
    public Texture2D BackgroundTexture;
    public Shop Shop;
    public LeagueData LeagueData;
    
}
