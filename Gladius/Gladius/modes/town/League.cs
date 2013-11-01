using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework;
using Gladius.util;
using Gladius.gamestatemanagement.screenmanager;

namespace Gladius.modes.town
{
    public class League
    {



        public static int Counter;
        public String Name;
        public int ArenaId;
        public String FlavourText;
        public List<Battle> Battles = new List<Battle>();

        public static League BuildRandom(int level)
        {
            League league = new League();
            league.Name = "Random League" + (++Counter);
            league.FlavourText = "It was a dark and stormy night...";
            return league;
        }
        
    }



    public class Battle
    {
        public String Name;
        public int Level;
        public int XPReward;
        public static int Counter;
        public List<ActorClass> RequiredClasses = new List<ActorClass>();
        public List<ActorClass> ProhibitedClasses = new List<ActorClass>();
        public List<BaseActor> Actors = new List<BaseActor>();

        public int MaxParticipants;
        public int NumTeams;
        
        public static Battle BuildRandom(League league,int level,int playerParticipants,int numTeams,int opponentsPerTeam,GameScreen gameScreen)
        {
            Battle battle = new Battle();
            battle.Name = league.Name + " Battle "+(++Counter);
            battle.XPReward = 50*level;
            for (int i = 0; i < numTeams; ++i)
            {
                for (int j = 0; j < opponentsPerTeam; ++j)
                {
                    int clevel = level;
                    double d = Globals.Random.NextDouble();
                    if (d < 0.2)
                    {
                        clevel--;
                    }
                    else if (d > 0.8)
                    {
                        clevel++;
                    }
                    clevel = MathHelper.Clamp(clevel, BaseActor.MinLevel, BaseActor.MaxLevel);
                    BaseActor actor = ActorGenerator.GenerateActor(clevel, ActorClass.Barbarian, gameScreen);
                    battle.Actors.Add(actor);
                }
            }
            return battle;
        }
    }


    public class ItemDrop
    {
        public int itemId;
        public int quantity;
    }

}
