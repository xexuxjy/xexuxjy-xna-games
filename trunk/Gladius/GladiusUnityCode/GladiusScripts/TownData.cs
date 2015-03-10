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
        td.Shop = new Shop();
        td.ArenaData = new ArenaData();

        td.Name = node.SelectSingleNode("@name").Value;
        td.Region = node.SelectSingleNode("@region").Value;
        td.BackgroundTextureName = node.SelectSingleNode("@texture").Value;

        td.Shop.ShopFile = node.SelectSingleNode("Shop/@dataFile").Value;
        td.Shop.OwnerThumbnailName = node.SelectSingleNode("Shop/@owner").Value;
        if (String.IsNullOrEmpty(td.Shop.OwnerThumbnailName))
        {
            td.Shop.OwnerThumbnailName = "GladiusUI/ClassThumbnails/shopkeeper_" + td.Name.ToLower() + ".tga";
        }

        td.ArenaData.ArenaName = node.SelectSingleNode("Arena/@name").Value;
        td.ArenaData.LeagueFile = node.SelectSingleNode("Arena/@leagueFile").Value;
        td.ArenaData.BackgroundTextureName = node.SelectSingleNode("Arena/@texture").Value;
        td.ArenaData.OwnerThumbnailName = node.SelectSingleNode("Arena/@owner").Value;
        if (String.IsNullOrEmpty(td.ArenaData.OwnerThumbnailName))
        {
            td.ArenaData.OwnerThumbnailName = "GladiusUI/ClassThumbnails/leagueowner_" + td.Name.ToLower() + ".tga";
        }
        
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
        Shop.LoadData(); 
        ArenaData.LoadData();
    }


    public String Name = "A Town";
    public String Region = "Imperia";
    public String BackgroundTextureName;
    public Shop Shop;
    public ArenaData ArenaData;
    
}
