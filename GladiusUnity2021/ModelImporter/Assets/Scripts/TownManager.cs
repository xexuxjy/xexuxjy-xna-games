using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class TownManager
{
    public static void Load()
    {
        TownSummaryInfo["belfort"] = new TownSummaryInfo("Belfort", "belfortarena", "belfort", "imperia", new Vector3(-31.2136f, 7.8316f, 32.9755f),1985);
        TownSummaryInfo["caltha"] = new TownSummaryInfo("Caltha", "calthaarena", "caltha", "imperia", new Vector3(-10.6484f, 7.3303f, -13.5266f ),2018);
        TownSummaryInfo["crobeska"] = new TownSummaryInfo("Crobeska", "mongrelsmaw", "Crobeska", "imperia", new Vector3(7.1141f, 0.1138f,-57.4479f),6405);
        TownSummaryInfo["orus"] = new TownSummaryInfo("Orus", "exuroseye", "Orus", "imperia", new Vector3(49.3804f, 2.9405f, -38.6729f ),6166);
        TownSummaryInfo["pirgos"] = new TownSummaryInfo("Pirgos", "pirgosarena", "pirgos", "imperia", new Vector3(30.0000f, 10.8910f, 24.0000f ),7178);
        TownSummaryInfo["syrna"] = new TownSummaryInfo("Syrna", "theatreantiquitis", "Syrna", "imperia", new Vector3(-83.2408f, 3.6612f, -59.7933f),7189);
        TownSummaryInfo["trikata"] = new TownSummaryInfo("Trikata", "bloodyhalo", "Trikata", "imperia", new Vector3(-43.3f,3.387f,-93.125f), 5442);
        
        TownSummaryInfo["fliuch"] = new TownSummaryInfo("Fliuch", "thefen", "Fliuch", "nordagh", new Vector3(-48.4072f, 15.3256f,131.9354f ),7191);
        TownSummaryInfo["mordare"] = new TownSummaryInfo("Mordare", "mordaresden", "Mordare", "nordagh", new Vector3(-93.0644f, 19.4063f, 138.8981f ),4345);
        TownSummaryInfo["orin"] = new TownSummaryInfo("Orin", "orinskeep", "Orin", "nordagh", new Vector3(-68.9460f, 6.0010f, 73.3006f ),7202);
        TownSummaryInfo["roanor"] = new TownSummaryInfo("Roanor", "thepit", "Roanor", "nordagh", new Vector3(-20.4919f, 7.9677f, 58.2320f ),7193);
        TownSummaryInfo["sloan"] = new TownSummaryInfo("Sloan", "stadiumdreas", "Sloan", "nordagh", new Vector3(-147.3809f, 7.5105f,59.4674f),7197);
        TownSummaryInfo["vargen"] = new TownSummaryInfo("Vargen", "fjordfallen", "Vargen", "nordagh", new Vector3(-128.9776f, 14.1990f, 100.3942f ),7200);

        TownSummaryInfo["altahrun"] = new TownSummaryInfo("Altahrun", "altahrunruins", "Altahrun", "steppes", new Vector3(158.7390f, 13.8765f, -27.8496f ),5464);
        TownSummaryInfo["ononhaar"] = new TownSummaryInfo("Ononhaar", "Ononhaar", "Ononhaar", "steppes", new Vector3(131.4961f, 16.2239f, 70.1544f ),5395);
        TownSummaryInfo["wastes"] = new TownSummaryInfo("Wastes", "wanderingsoul", "Wastes", "steppes", new Vector3(220.6919f, 14.3027f, 18.8528f ),5469);
        TownSummaryInfo["yuset"] = new TownSummaryInfo("Yuset", "arenasuren", "Yuset", "steppes", new Vector3(112.9689f, 16.8994f, 5.3426f ),5450);

        TownSummaryInfo["akaran"] = new TownSummaryInfo("Akaran", "palaceibliis", "Akaran", "expanse", new Vector3(152.6300f, 18.7200f, 89.0500f ),5360);
        TownSummaryInfo["qaarah"] = new TownSummaryInfo("Qaarah", "scorchedoasis", "Qaarah", "expanse", new Vector3(65.4978f, 21.3488f, 23.0380f ),8219);
        TownSummaryInfo["saraaizel"] = new TownSummaryInfo("Saraaizel", "offeringplate", "Saraaizel", "expanse", new Vector3(169.6658f, 19.8927f, 13.1236f ),5422);


        foreach (string townName in TownSummaryInfo.Keys)
        {
            TownData td = TownData.ForName(TownSummaryInfo[townName]);
            TownDictionary[townName] = td;
            if (td.ArenaOffice != null)
            {
                foreach (LeagueData leagueData in TownDictionary[townName].ArenaOffice.Leagues)
                {
                    LeagueDataInfo[leagueData.Name] = leagueData;
                }
            }
        }
    }

    public static TownData GetTownData(string name)
    {
        TownData result = null;
        TownDictionary.TryGetValue(name.ToLower(),out result);
        return result;
    }


    public static Dictionary<string, TownData> TownDictionary = new Dictionary<string,TownData>();
    public static Dictionary<string, TownSummaryInfo> TownSummaryInfo = new Dictionary<string, TownSummaryInfo>();
    public static Dictionary<string, LeagueData> LeagueDataInfo = new Dictionary<string, LeagueData>();
}

public class TownSummaryInfo
{
    public string InternalName;
    public string ArenaName;
    public string Region;
    public string BackgroundName;
    public Vector3 Position;
    public int TownHistory;

    public TownSummaryInfo(string internalName, string arenaName, string backgroundName, string region,Vector3 worldPosition,int history)
    {
        InternalName = internalName;
        ArenaName = arenaName;
        Region = region;
        BackgroundName = backgroundName;
        Position = worldPosition;
        TownHistory = history;
    }

}


