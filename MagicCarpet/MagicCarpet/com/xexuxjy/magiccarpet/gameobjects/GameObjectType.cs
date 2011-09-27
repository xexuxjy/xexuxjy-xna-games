using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    [Flags]
    public enum GameObjectType
    {
        NONE = 0,
        MAGICIAN = 1,
        MANABALL = 2,
        CASTLE = 4,
        BALLOON = 8,
        SPELL = 16,
        PLAYER=32,
        MONSTER=64,
        TERRAIN=128,
        NUMTYPES
    }
}
