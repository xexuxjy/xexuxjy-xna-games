using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius;

namespace GladiusCommon.Scripts
{
	public class LeagueData
	{

        public List<BattleData> Battles = new List<BattleData>();

    }


    public class BattleData
    {
        public int ID;
        public String Name;
        public int MinLevel;
        public int MinRenown;
        public SchoolRank MinRank;
        public List<ActorCategory> PermittedCategories = new List<ActorCategory>();
        public List<ActorCategory> ProhitiedCategories = new List<ActorCategory>();
        public int EntryCost;
        public int GoldReward;
        public int XPReward;
        public int TreasureDropRank;
        public int NumPoints;



        public static BattleData GenerateDummyBattleData()
        {
            BattleData battleData = new BattleData();
            battleData.Name = "Dummy Battle";
            battleData.GoldReward = 1500;
            battleData.XPReward = 1000;
            battleData.TreasureDropRank = 1;
            battleData.NumPoints = 1;
            return battleData;
        }

    
    }
}
