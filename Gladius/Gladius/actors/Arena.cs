using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gladius.actors
{
    public class Arena
    {

        public Arena(int width,int breadth)
        {
            m_arenaGrid = new SquareType[width, breadth];
            m_width = width;
            m_breadth = breadth;
            BuildDefaultArena();
        }

        public void BuildDefaultArena()
        {
            for (int i = 0; i < Width; ++i)
            {
                for (int j = 0; j < Breadth; ++j)
                {
                    if(i == 0 || j == 0 || i == Width-1 || j == Breadth -1)
                    {
                        m_arenaGrid[i, j] = SquareType.Wall;
                    }
                    else
                    {
                        m_arenaGrid[i, j] = SquareType.Empty;
                    }
                }
            }
        }


        public int Width
        {
            get
            {
                return m_width;
            }
        }
        public int Breadth
        {
            get
            {
                return m_breadth;
            }
        }


        public SquareType SquareTypeAtLocation(int x, int y)
        {
            return m_arenaGrid[x, y];
        }


        private SquareType[,] m_arenaGrid;
        private int m_width;
        private int m_breadth;

    }

    public enum SquareType
    {
        Empty,
        Level1,
        Level2,
        Level3,
        Unaccesible,
        Wall,
        Crowd,
    }





}
