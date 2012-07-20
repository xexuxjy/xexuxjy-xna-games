using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace com.xexuxjy.magiccarpet.gameobjects
{
    [Flags]
    public enum GameObjectType
    {
        none = 0,
        magician = 1,
        manaball = 2,
        castle = 4,
        balloon = 8,
        spell = 16,
        player=32,
        monster=64,
        terrain=128,
        camera=256,
        skydome=512,
        gui=1024,
        manager = 2048,
        treemanager = 4096,
        NUMTYPES
    }
}
