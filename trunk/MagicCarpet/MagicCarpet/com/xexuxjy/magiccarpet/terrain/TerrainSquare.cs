using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.xexuxjy.magiccarpet.gameobjects;

namespace com.xexuxjy.magiccarpet.terrain
{
    public class TerrainSquare
    {
        public TerrainSquare()
        {
            Type = TerrainType.grass;
        }

        public TerrainType Type;
        public GameObject Occupier;
    }
}
