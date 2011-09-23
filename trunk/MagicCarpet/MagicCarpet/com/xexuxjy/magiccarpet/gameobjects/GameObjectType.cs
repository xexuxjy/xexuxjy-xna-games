using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    [Flags]
    public enum GameObjectType
    {
        None = 0,
        Magician = 1,
        ManaBall = 2,
        Castle = 4,
        Balloon = 8,
        Spell = 16,
        Player=32,
        Monster=64,
        Terrain=128,
        NumTypes
    }
}
