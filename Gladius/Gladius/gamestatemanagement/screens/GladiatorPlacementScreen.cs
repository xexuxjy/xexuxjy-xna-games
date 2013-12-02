using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gladius.gamestatemanagement.screenmanager;
using Gladius.modes.shared;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Gladius.gamestatemanagement.screens
{
    public class GladiatorPlacementScreen : GameScreen
    {



        public void FillPosition(Point p,CharacterData character)
        {
            Debug.Assert(m_availablePositions.Contains(p) && character.StartPosition.HasValue == false);
            if(m_availablePositions.Contains(p) && character.StartPosition.HasValue == false)
            {
                m_characterMap[p] = character;
                m_availablePositions.Remove(p);
                m_filledPositions.Add(p);
                character.StartPosition = p;
            }
        }

        public void EmptyPosition(Point p)
        {
            Debug.Assert(m_filledPositions.Contains(p));
            if(m_filledPositions.Contains(p))
            {
                CharacterData character;
                if(m_characterMap.TryGetValue(p,out character))
                {
                    character.StartPosition = null;
                }
                m_filledPositions.Remove(p);
                m_availablePositions.Add(p);
            }
        }

        public Dictionary<Point, CharacterData> PlacementMap
        {
            get
            {
                return m_characterMap;
            }
        }

        public Dictionary<Point,CharacterData> m_characterMap = new Dictionary<Point,CharacterData>();
        public List<CharacterData> m_characters = new List<CharacterData>();
        public List<Point> m_availablePositions = new List<Point>();
        public List<Point> m_filledPositions = new List<Point>();
    }
}
