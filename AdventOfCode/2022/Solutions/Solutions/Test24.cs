using System.Text;
using System.Xml.Linq;

public class Test24 : BaseTest
{

    Cave m_cave = new Cave();
    List<Cave> m_caveSimulations = new List<Cave>();

    public IntVector2 Explorer = new IntVector2();

    public const int NO_ROUTE = int.MaxValue;
    public const int MAX_DEPTH = 50;

    int m_shortestRouteLength = NO_ROUTE;
    List<IntVector2> m_shortestRoute = new List<IntVector2>();
    public override void RunTest()
    {
        TestID = 24;
        IsTestInput = true;
        ReadDataFile();


        m_cave.Size = new IntVector2(m_dataFileContents[0].Length,m_dataFileContents.Count);

        Wind.CharacterMap[IntVector2.Right] = '>';
        Wind.CharacterMap[IntVector2.Left] = '<';
        Wind.CharacterMap[IntVector2.Down] = '^';
        Wind.CharacterMap[IntVector2.Up] = 'v';


        for(int y=0;y<m_dataFileContents.Count;y++)
        {
            string line = m_dataFileContents[y];

            for(int x=0;x<line.Length;x++)
            {
                if(y == 0 && line[x] == '.')
                {
                    m_cave.StartPosition = new IntVector2(x,y);
                }
                if(y == m_dataFileContents.Count-1 && line[x] == '.')
                {
                    m_cave.EndPosition = new IntVector2(x,y);
                }



                if(line[x] == '#')
                {
                    m_cave.AddWall(new IntVector2(x,y));
                }
                else if(line[x] == '>')
                {
                    m_cave.AddWind(new Wind(){Position = new IntVector2(x,y),Direction=IntVector2.Right});

                }
                else if(line[x] == '<')
                {
                    m_cave.AddWind(new Wind(){Position = new IntVector2(x,y),Direction=IntVector2.Left});

                }
                else if(line[x] == '^')
                {
                    m_cave.AddWind(new Wind(){Position = new IntVector2(x,y),Direction=IntVector2.Down});

                }
                else if(line[x] == 'v')
                {
                    m_cave.AddWind(new Wind(){Position = new IntVector2(x,y),Direction=IntVector2.Up});

                }
                
            }
        }

        m_debugInfo.Add(m_cave.GenerateDebug());
    
        Explorer = m_cave.StartPosition;

        // need a move, being a direction and time,
        List<IntVector2> moveList = new List<IntVector2>();
        int foundRouteDepth = TestRoute(GetCave(0).StartPosition,0,moveList);
        DebugOutput("Found shortest as : "+m_shortestRoute.Count);

        WriteDebugInfo();
    }

    public Cave GetCave(int depth)
    {
        if(m_caveSimulations.Count == 0)
        {
            m_caveSimulations.Add(m_cave);
        }

        while(m_caveSimulations.Count-1 < depth)
        {
            m_caveSimulations.Add(m_caveSimulations.Last().SimulateNext());
        }
        
        return m_caveSimulations[depth];

    }

    public static IntVector2[] MoveOptions = new IntVector2[]{new IntVector2(),IntVector2.Up,IntVector2.Down,IntVector2.Left,IntVector2.Right};


    public int TestRoute(IntVector2 position,int depth,List<IntVector2> moveList)
    {
        if(position == GetCave(0).EndPosition)
        {
            if(depth < m_shortestRouteLength)
            {
                m_shortestRouteLength = depth;
                m_shortestRoute.Clear();
                m_shortestRoute.AddRange(moveList);
                return NO_ROUTE;
            }
            return NO_ROUTE;
        }

        // stop overflow of continually staying in one place
        if(depth > MAX_DEPTH)
        {
            return NO_ROUTE;
        }

        if(m_shortestRouteLength != NO_ROUTE && depth >= m_shortestRouteLength)
        {
            return NO_ROUTE;
        }

        if(!GetCave(depth).IsEmpty(position))
        {
            return NO_ROUTE;
        }

        int localRoute = NO_ROUTE;
        foreach(IntVector2 option in MoveOptions)
        {
            moveList.Add(option);
            if(moveList.Count != depth+1)
            {
                int ibreak = 0;
            }

            int foundRoute = TestRoute(position+option,depth+1,moveList);
            if(foundRoute == NO_ROUTE)
            {
                moveList.Remove(moveList.Last());   
            }
            else
            {
                localRoute = foundRoute;
            }
        }

        
        return localRoute;

    }

}




public class Cave
{
    public int Time;

    public IntVector2 StartPosition;
    public IntVector2 EndPosition;

    public IntVector2 Size { get; set; }
    public void AddWall(IntVector2 wall)
    {
        m_walls.Add(wall);
    }

    public void AddWind(Wind wind)
    {
        m_winds.Add(wind);
    }

    public bool InBounds(IntVector2 pos)
    {
        return pos.X > 0 && pos.X < Size.X-1 && pos.Y >0 && pos.Y < Size.Y-1;
    }

    public bool IsEmpty(IntVector2 pos)
    {
        if(pos == StartPosition || pos == EndPosition)
        {
            return true;
        }
        if(!InBounds(pos)) return false;
        if(m_winds.Find(x=>x.Position == pos) != null)
        {
            return false;
        }
        return true;
    }

    public void Simulate()
    {
        foreach(Wind w in m_winds)
        {
            w.Position += w.Direction;

            if(w.Position.X <= 0)
            {
                w.Position.X += Size.X;
            }
            else if (w.Position.X == Size.X-1)
            {
                w.Position.X =1;
            }
            else if(w.Position.Y <= 0)
            {
                w.Position.Y += Size.Y;
            }
            else if (w.Position.Y == Size.Y-1)
            {
                w.Position.Y =1;
            }
        }
    }

    public Cave SimulateNext()
    {
        Cave nextCave = new Cave();
        nextCave.Time = Time+1;
        nextCave.m_walls.AddRange(m_walls);
        nextCave.Size = Size;
        nextCave.StartPosition = StartPosition;
        nextCave.EndPosition = EndPosition;


        foreach (Wind w in m_winds)
        {
            nextCave.AddWind(new Wind() { Position = w.Position, Direction = w.Direction });
        }
        nextCave.Simulate();
        return nextCave;
    }


    public const char NO_CHAR = 'x';

    public string GenerateDebug()
    {
        StringBuilder sb= new StringBuilder();
        for(int y=0;y<Size.Y;++y)
        {
            for(int x=0;x<Size.X;++x)
            {
                char addChar = NO_CHAR;

                IntVector2 v = new IntVector2(x,y);
                if(StartPosition == v || EndPosition == v)
                {
                    addChar = '.';
                }
                else if(m_walls.Contains(v))
                {
                    addChar = '#';
                }
                
                List<Wind> wl = m_winds.FindAll(x=>x.Position == v);
                if(wl != null && wl.Count > 0)
                {
                    if(wl.Count == 1)
                    {
                        addChar = Wind.CharacterMap[wl[0].Direction];
                    }
                    else
                    {
                        addChar = (char)(((int)'0')+wl.Count);
                    }
                }
                if (addChar == NO_CHAR)
                {
                    addChar = '.';
                }
                sb.Append(addChar);
            }
            sb.AppendLine("");
        }
        return sb.ToString();
    }



    private List<IntVector2> m_walls = new List<IntVector2>();
    private List<Wind> m_winds = new List<Wind>();
}



public class Wind
{
    public IntVector2 Position;
    public IntVector2 Direction;

    public static Dictionary<IntVector2,char> CharacterMap = new Dictionary<IntVector2,char>();

}