using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Xml;
using Gladius;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

public class TownData
{
    public static TownData ForName(TownSummaryInfo summaryInfo)
    {
        TownData td = new TownData();
        td.TownSummary = summaryInfo;

        String filename = GladiusGlobals.LeaguesPath + summaryInfo.ArenaName + "_leagues";
        td.ArenaOffice = LoadOffceData(filename);

        if (td.ArenaOffice != null)
        {
            td.ArenaOffice.TownData = td;
            String heroName = "Ursula";
            string townName = summaryInfo.InternalName;
            td.Name = townName;
            td.TownTextureName = String.Format("town_{0}", townName).ToLower();
            td.ArenaTextureName = String.Format("arena_{0}", td.TownSummary.ArenaName).ToLower();
            td.ArenaBirdseyeTextureName = String.Format("{0}", td.TownSummary.ArenaName).ToLower();
            td.LeagueOwnerTextureName = String.Format("leagueowner_{0}", townName).ToLower();
            td.LocaleTitleTextureName = String.Format("localetitle_{0}", townName).ToLower();
            td.LocaleTextureName = String.Format("locale_{0}", townName).ToLower();
            td.ShopkeeperTextureName = String.Format("shopkeeper_{0}", townName).ToLower();
            td.ArenaOffice.ownerThumbnailName = td.ShopkeeperTextureName;

            td.townHistoryJournalEntry = GladiusJournal.Get(String.Format("{0}_History_Town", townName));
            td.arenaHistoryJournalEntry = GladiusJournal.Get(String.Format("{0}_History_Arena", townName));



            TryLoadShop(String.Format("GladiusData/Shops/Shop{0}{1}", townName, "Ursula"), out td.UrsulaShop);
            TryLoadShop(String.Format("GladiusData/Shops/Shop{0}{1}", townName, "Valens"), out td.ValensShop);
            TryLoadShop(String.Format("GladiusData/Shops/Shop{0}{1}", townName, "Pro"), out td.ProShop);
            TryLoadShop(String.Format("GladiusData/Shops/Shop{0}{1}", townName, "Generic"), out td.GenericShop);
            td.BuildData();
        }
        else
        {
            Debug.LogWarning("Can;t load league file for " + summaryInfo.InternalName);
        }
        return td;
    }

    public static void TryLoadShop(string shopFile,out Shop shop)
    {
        string shopData = GladiusGlobals.ReadTextAsset(shopFile);
        if (!String.IsNullOrEmpty(shopData))
        {
            shop = new Shop();
            shop.ShopFile = shopFile;
            shop.LoadData();
        }
        else
        {
            shop = null;
        }
    }


    public static ArenaOffice LoadOffceData(String filename)
    {
        //Debug.Log("Parsing arena : " + filename);
        ArenaOffice result = null;
        //TextAsset textAsset = (TextAsset)Resources.Load(filename);
        String officeData = GladiusGlobals.ReadTextAsset(filename);

        if (officeData != null)
        {
            var lexer = new GladiusLeagueLexer(new Antlr4.Runtime.AntlrInputStream(officeData));

            lexer.RemoveErrorListeners();

            CommonTokenStream commonTokenStream = new CommonTokenStream(lexer);
            var parser = new GladiusLeagueParser(commonTokenStream);
            IParseTree parseTree = parser.root();
            ArenaLeageParser listener = new ArenaLeageParser();
            ParseTreeWalker.Default.Walk(listener, parseTree);
            result = listener.CurrentOffice;

        }
        return result;
    }

    public void BuildData()
    {
        if(ArenaOffice != null)
        {
            foreach(LeagueData ld in ArenaOffice.Leagues)
            {
                ld.LoadData();
            }
        }
    }


    public String Name = "A Town";
    public TownSummaryInfo TownSummary;

    public String TownTextureName;
    public String ArenaTextureName;
    public String ArenaBirdseyeTextureName;
    public String LeagueOwnerTextureName;
    public String LocaleTitleTextureName;
    public String LocaleTextureName;
    public String ShopkeeperTextureName;
    public JournalEntry townHistoryJournalEntry;
    public JournalEntry arenaHistoryJournalEntry;


    //public Shop Shop;

    
    public Shop Shop
    { get
        {
            //if (GameStateManager.Instance.GameState.PlayerSchool.HeroName == GladiatorSchool.HeroUrsula && UrsulaShop != null)
            //{
            //    return UrsulaShop;
            //}
            //if (GameStateManager.Instance.GameState.PlayerSchool.HeroName == GladiatorSchool.HeroValens && ValensShop != null)
            //{
            //    return ValensShop;
            //}
            if (ProShop != null)
            {
                return ProShop;
            }
            return GenericShop;
        }
    }

    public Shop UrsulaShop;
    public Shop ValensShop;
    public Shop ProShop;
    public Shop GenericShop;

    public ArenaOffice ArenaOffice;

}





