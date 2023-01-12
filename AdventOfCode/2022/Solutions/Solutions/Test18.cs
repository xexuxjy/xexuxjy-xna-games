public class Test18 : BaseTest
{
    public List<IntVector3> m_positions = new List<IntVector3>();
    public Dictionary<IntVector3, List<IntVector3>> m_touchingMap = new Dictionary<IntVector3, List<IntVector3>>();
    int m_freeSides = 0;
    int m_gaps = 0;

    public override void RunTest()
    {
        TestID = 18;
        IsTestInput = false;
        IsPart2 = true;
        ReadDataFile();


        foreach (string line in m_dataFileContents)
        {
            string[] tokens = line.Split(',');
            IntVector3 position = new IntVector3(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
            m_positions.Add(position+new IntVector3(1,1,1));
        }
        // order by xyz...
        m_positions = m_positions.OrderBy(a => a.X).ThenBy(a => a.Y).ThenBy(a => a.Z).ToList();

        for (int i = 0; i < m_positions.Count; ++i)
        {
            IntVector3 pos = m_positions[i];
            m_touchingMap[pos] = new List<IntVector3>();

            for (int j = i + 1; j < m_positions.Count; j++)
            {
                if (DoCubesTouch(pos, m_positions[j]))
                {
                    m_touchingMap[pos].Add(m_positions[j]);
                }
            }
        }

        Part1();
        Part2();

        WriteDebugInfo();
    }

    public void Part1()
    {
        m_freeSides = 0;
        foreach (IntVector3 pos in m_positions)
        {
            m_debugInfo.Add("Cube : " + pos + " - " + string.Join(' ', m_touchingMap[pos]));
            m_freeSides += (6 - (m_touchingMap[pos].Count * 2));

        }
        m_debugInfo.Add("Total area : " + m_freeSides);

    }

    public void Part2()
    {
        int minX = int.MaxValue;
        int maxX = int.MinValue;

        int minY = int.MaxValue;
        int maxY = int.MinValue;

        int minZ = int.MaxValue;
        int maxZ = int.MinValue;


        foreach (IntVector3 pos in m_positions)
        {
            minX = Math.Min(minX, pos.X);
            maxX = Math.Max(maxX, pos.X);

            minY = Math.Min(minY, pos.Y);
            maxY = Math.Max(maxY, pos.Y);

            minZ = Math.Min(minZ, pos.Z);
            maxZ = Math.Max(maxZ, pos.Z);


        }

        maxX+=2;
        maxY+=2;
        maxZ+=2;

        IntVector3 bounds = new IntVector3(maxX,maxY,maxZ);

        int[,,] layout = new int[maxX,maxY,maxZ];
        foreach(IntVector3 pos in m_positions)
        {
            layout[pos.X,pos.Y,pos.Z] = 255;
        }

        int intial = CountMatching(m_positions,layout,bounds,0);



        FloodFill2(new IntVector3(),bounds,layout,1);

        int final = CountMatching(m_positions,layout,bounds,1);

        int ibreak =0 ;

        // filled in all the positions.

        // check and see which are empty.


        //m_debugInfo.Add("Part 2 : FreeSides = " + m_freeSides + "  Enclosed = " + enclosedCount + "  Total = " + (m_freeSides - enclosedCount));
        //m_debugInfo.Add("Part 2b : FreeSides = " + m_freeSides + "  Enclosed = " + altEnclosedCount + "  Total = " + (m_freeSides - altEnclosedCount));

    }

    public int CountMatching(List<IntVector3> positions,int[,,] layout,IntVector3 bounds,int testVal)
    {
        int count = 0;
        foreach(IntVector3 pos in positions)
        {
            foreach(IntVector3 offset in m_offsets)
            {
                IntVector3 adjusted = pos+offset;
                if(adjusted < IntVector3.Zero || adjusted >= bounds)
                {
                    continue;
                }
                if(layout[adjusted.X,adjusted.Y,adjusted.Z] == testVal)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public IntVector3[] m_offsets = new IntVector3[]{
                                new IntVector3(1,0,0),new IntVector3(-1,0,0),
                                new IntVector3(0,1,0),new IntVector3(0,-1,0),
                                new IntVector3(0,0,1),new IntVector3(0,0,-1)};

    public bool IsEnclosed(IntVector3 cube)
    {
        if (!m_touchingMap.ContainsKey(cube))
        {
            foreach (IntVector3 offset in m_offsets)
            {
                if (m_touchingMap.ContainsKey((cube + offset)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsInside(IntVector3 cube)
    {
        if (m_touchingMap.ContainsKey(cube))
        {
            return m_touchingMap[cube].Count == 0;
        }
        return false;
    }


    public bool DoCubesTouch(IntVector3 cube1, IntVector3 cube2)
    {
        return cube1.ManhattanDistance(cube2) == 1;
    }

    public void FloodFill2(IntVector3 start,IntVector3 bounds,int[,,] layout,int fillValue)
    {
        Stack<IntVector3> workingQueue = new Stack<IntVector3>();

        workingQueue.Push(start);
        while (workingQueue.Count > 0)
        {
            IntVector3 current = workingQueue.Pop();
            layout[current.X,current.Y,current.Z] = fillValue;

            foreach (IntVector3 offset in m_offsets)
            {
                IntVector3 adjusted = current + offset;
                if(adjusted< IntVector3.Zero || adjusted >= bounds)
                {
                    continue;
                }

                if(layout[adjusted.X,adjusted.Y,adjusted.Z] == 0)
                {
                    workingQueue.Push(adjusted);
                }
            }
        }
    }


    public bool FloodFill(IntVector3 start, Dictionary<IntVector3, List<IntVector3>> results)
    {
        results.Clear();

        Queue<IntVector3> workingQueue = new Queue<IntVector3>();

        int breakLimit = 10;

        workingQueue.Enqueue(start);
        while (workingQueue.Count > 0)
        {
            IntVector3 current = workingQueue.Dequeue();

            List<IntVector3> touching = new List<IntVector3>();
            results[current] = touching;

            foreach (IntVector3 offset in m_offsets)
            {
                IntVector3 adjusted = current + offset;
                if (!m_touchingMap.ContainsKey(adjusted))
                {
                    if (!workingQueue.Contains(adjusted) && !results.ContainsKey(adjusted))
                    {
                        touching.Add(adjusted);
                        workingQueue.Enqueue(adjusted);
                    }
                }
            }
            // got too big, probably an outside square
            if (results.Count > breakLimit)
            {
                return false;
            }
        }

        return true;
    }



}