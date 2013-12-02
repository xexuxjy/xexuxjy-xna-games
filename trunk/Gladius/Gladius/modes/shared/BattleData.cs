using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.actors;
using Microsoft.Xna.Framework;

namespace Gladius.modes.shared
{
    public class BattleData
    {
        public string Name
        {
            get;
            set;
        }




        List<Point> m_availableStartPoints = new List<Point>();
        List<CharacterData> m_opponentList = new List<CharacterData>();
        List<ActorClass> PermittedClasses = new List<ActorClass>();
        List<ActorClass> ProhibitedClasses = new List<ActorClass>();
    }
}
