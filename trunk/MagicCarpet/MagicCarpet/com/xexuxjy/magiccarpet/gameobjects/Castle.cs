using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    public class Castle : GameObject
    {
        public Castle(Game game)
            : base(game, GameObjectType.Castle)
        {
        }

        public int Level
        {
            get { return m_level; }
            set { m_level = value; }
        }

        public float StoredMana
        {
            get { return m_storedMana; }
            set { m_storedMana = value; }
        }

        private int m_level;
        private float m_storedMana;
    }
}
