using System.Text;
using System.Xml.Linq;

public class Test24 : BaseTest
{

    List<Cave> m_caveSimulations = new List<Cave>();
    public IntVector2 Explorer = new IntVector2();

    public const int NO_ROUTE = int.MaxValue;
    public const int MAX_DEPTH = 2000;

    List<IntVector2> m_shortestRoute = new List<IntVector2>();
    Dictionary<IntVector3, bool> m_exploredRoutes = new Dictionary<IntVector3, bool>();


    public IntVector2 StartPosition{get;set; }
    public IntVector2 EndPosition{get;set; }



    public override void RunTest()
    {
        DateTime startTime = DateTime.Now;

        TestID = 24;
        IsTestInput = false;
        IsPart2 = true;
        ReadDataFile();

        Cave firstCave = new Cave(new IntVector2(m_dataFileContents[0].Length, m_dataFileContents.Count)); 
        ResetCaves(firstCave);

        Wind.CharacterMap[IntVector2.Right] = '>';
        Wind.CharacterMap[IntVector2.Left] = '<';
        Wind.CharacterMap[IntVector2.Down] = 'v';
        Wind.CharacterMap[IntVector2.Up] = '^';


        for (int y = 0; y < m_dataFileContents.Count; y++)
        {
            string line = m_dataFileContents[y];

            for (int x = 0; x < line.Length; x++)
            {
                if (y == 0 && line[x] == '.')
                {
                    StartPosition = new IntVector2(x, y);
                }
                if (y == m_dataFileContents.Count - 1 && line[x] == '.')
                {
                    EndPosition = new IntVector2(x, y);
                }

                if (line[x] == '#')
                {
                    firstCave.AddWall(new IntVector2(x, y));
                }
                else if (line[x] == '>')
                {
                    firstCave.AddWind(new Wind() { Position = new IntVector2(x, y), Direction = IntVector2.Right });

                }
                else if (line[x] == '<')
                {
                    firstCave.AddWind(new Wind() { Position = new IntVector2(x, y), Direction = IntVector2.Left });

                }
                else if (line[x] == '^')
                {
                    firstCave.AddWind(new Wind() { Position = new IntVector2(x, y), Direction = IntVector2.Up });

                }
                else if (line[x] == 'v')
                {
                    firstCave.AddWind(new Wind() { Position = new IntVector2(x, y), Direction = IntVector2.Down });

                }

            }
        }

        List<IntVector2> moveList = new List<IntVector2>();
        TestRoute(StartPosition, StartPosition,EndPosition,0, moveList);
        DebugOutput("Found shortest as : " + m_shortestRoute.Count);

        if(IsPart2)
        {
            int there = m_shortestRoute.Count;

            m_shortestRoute.Clear();
            m_exploredRoutes.Clear();
            ResetCaves(m_caveSimulations[there]);

            TestRoute(EndPosition,EndPosition,StartPosition,0, moveList);
            int andBackAgain = m_shortestRoute.Count;
            DebugOutput($"There and back again....  : {there}  , {andBackAgain} = {there+andBackAgain}");

            m_shortestRoute.Clear();
            m_exploredRoutes.Clear();
            ResetCaves(m_caveSimulations[andBackAgain]);


            TestRoute(StartPosition,StartPosition,EndPosition,0, moveList);
            int andFinally = m_shortestRoute.Count;

            DebugOutput($"AndFinally....  : {there}  , {andBackAgain}, {andFinally} = {there + andBackAgain + andFinally}");


        }

        //for(int i=0;i<20;++i)
        //{
        //    m_debugInfo.Add(GetCave(i).GenerateDebug(new IntVector2()));
        //}


        if (IsTestInput)
        {
            if (m_shortestRoute.Count > 0)
            {
                for (int i = 0; i < m_shortestRoute.Count; i++)
                {
                    m_debugInfo.Add(GetCave(i).GenerateDebug(m_shortestRoute[i],StartPosition,EndPosition));
                }
            }
        }
        double bpElapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        DebugOutput("Elapsed = " + bpElapsed + " ms");


        WriteDebugInfo();
    }

    public void ResetCaves(Cave start)
    {
        m_caveSimulations.Clear();
        m_caveSimulations.Add(start);
    }

    public Cave GetCave(int depth)
    {
        while (m_caveSimulations.Count - 1 < depth)
        {
            m_caveSimulations.Add(m_caveSimulations.Last().SimulateNext());
        }

        return m_caveSimulations[depth];

    }

    public static IntVector2[] MoveOptions = new IntVector2[] { new IntVector2(), IntVector2.Up, IntVector2.Down, IntVector2.Left, IntVector2.Right };


    public bool TestRoute(IntVector2 position, IntVector2 start,IntVector2 end,int depth, List<IntVector2> moveList)
    {
        bool existingRoute;
        IntVector3 searchKey = new IntVector3(position.X, position.Y, depth);
        if (m_exploredRoutes.TryGetValue(searchKey, out existingRoute))
        {
            return existingRoute;
        }

        if (position == end)
        {
            if (m_shortestRoute.Count == 0 || (m_shortestRoute.Count > 0 && moveList.Count < m_shortestRoute.Count))
            {
                m_shortestRoute.Clear();
                m_shortestRoute.AddRange(moveList);
            }
            return true;
        }

        // stop overflow of continually staying in one place
        if (depth > MAX_DEPTH)
        {
            return false;
        }

        if (m_shortestRoute.Count > 0 && depth >= m_shortestRoute.Count)
        {
            return false;
        }

        if (!GetCave(depth).IsEmpty(position,start,end))
        {
            return false;
        }

        List<IntVector2> moveChoices = new List<IntVector2>();
        foreach (IntVector2 option in MoveOptions)
        {
            moveChoices.Add(position + option);
        }


        bool hasRoute = false;
        foreach (IntVector2 option in moveChoices.OrderBy(x => x.ManhattanDistance(EndPosition)))
        {
            moveList.Add(position);
            hasRoute |= TestRoute(option, start,end,depth + 1, moveList);
            moveList.RemoveAt(moveList.Count - 1);
        }

        m_exploredRoutes[searchKey] = hasRoute;


        return hasRoute;
    }


    public class Cave
{
    public int Time;

    private bool[] m_occupied;

    public Cave(IntVector2 size)
    {
        m_occupied = new bool[size.X * size.Y];
        Size = size;
    }

    public IntVector2 Size { get; private set; }
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
        return pos.X > 0 && pos.X < Size.X - 1 && pos.Y > 0 && pos.Y < Size.Y - 1;
    }

    public bool IsEmpty(IntVector2 pos,IntVector2 start,IntVector2 end)
    {
        if (pos == start || pos == end)
        {
            return true;
        }
        if (!InBounds(pos)) return false;

        return !m_occupied[(pos.Y * Size.X) + pos.X];
    }

    public void Simulate()
    {
        // 0 is wall left,top
        // x-1 is wall right
        // y-1 is wall bototm

        int wallWidth = 1;
        foreach (Wind w in m_winds)
        {
            w.Position += w.Direction;

            if (w.Position.X <= 0)
            {
                w.Position.X = Size.X - 2;
            }
            else if (w.Position.X == Size.X - 1)
            {
                w.Position.X = 1;
            }
            else if (w.Position.Y <= 0)
            {
                w.Position.Y = Size.Y - 2;
            }
            else if (w.Position.Y == Size.Y - 1)
            {
                w.Position.Y = 1;
            }
            m_occupied[(w.Position.Y * Size.X) + w.Position.X] = true;
        }
    }

    public Cave SimulateNext()
    {
        Cave nextCave = new Cave(Size);
        nextCave.Time = Time + 1;
        nextCave.m_walls.AddRange(m_walls);
        nextCave.Size = Size;

        foreach (Wind w in m_winds)
        {
            nextCave.AddWind(new Wind() { Position = w.Position, Direction = w.Direction });
        }
        nextCave.Simulate();
        return nextCave;
    }


    public const char NO_CHAR = 'x';

    public string GenerateDebug(IntVector2 explorerPosition,IntVector2 start,IntVector2 end)
    {
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < Size.Y; ++y)
        {
            for (int x = 0; x < Size.X; ++x)
            {
                char addChar = NO_CHAR;

                IntVector2 v = new IntVector2(x, y);

                if (v == explorerPosition)
                {
                    addChar = 'E';
                }
                else if (start == v || end == v)
                {
                    addChar = '.';
                }
                else if (m_walls.Contains(v))
                {
                    addChar = '#';
                }

                if (addChar == NO_CHAR)
                {
                    List<Wind> wl = m_winds.FindAll(x => x.Position == v);
                    if (wl != null && wl.Count > 0)
                    {
                        if (wl.Count == 1)
                        {
                            addChar = Wind.CharacterMap[wl[0].Direction];
                        }
                        else
                        {
                            addChar = (char)(((int)'0') + wl.Count);
                        }
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

    public static Dictionary<IntVector2, char> CharacterMap = new Dictionary<IntVector2, char>();

}

}




