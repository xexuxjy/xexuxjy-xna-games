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
            m_positions.Add(position);
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
        // how do you identify somethign completely inside.
        // well 

        //work out bounds of cube.
        // slice and fill externals
        // rescan about how many lines hold both outside and gaps.

        //bool[,,] m_outlineMap = new bool[

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

        int maxVal = Math.Max(maxX, Math.Max(maxY, maxZ));
        maxVal++;

        int[,,] touches = new int[maxVal, maxVal, maxVal];


        // any cube with 6 neighbours is solid.
        // any cube with 0 neighbours is either floating in an enclosed gap or not part of scan.
        int enclosedCount = 0;

        List<IntVector3> gaps = new List<IntVector3>();


        for (int x = minX; x < maxX; ++x)
        {
            List<IntVector3> slice = m_positions.FindAll(a => a.X == x).OrderBy(a => a.Y).ThenBy(a => a.Z).ToList();

            for (int y = minY; y < maxY; ++y)
            {
                List<IntVector3> row = slice.FindAll(a => a.Y == y).OrderBy(b => b.Z).ToList();

                if (row.Count > 2)
                {
                    for (int i = 0; i < row.Count - 1; ++i)
                    {
                        int distance = row[i].ManhattanDistance(row[i + 1]) - 1;
                        for(int j=0;j<distance;++j)
                        {
                            gaps.Add(new IntVector3(x,y,row[i].Z+j));
                        }

                    }
                }

                for(int z = minZ;z<maxZ;++z)
                {
                    IntVector3 loc = new IntVector3(x, y, z);

                    // all the blocks on this level.

                }
            }
        }
        
        foreach(IntVector3 gap in gaps)
        {
            if(IsEnclosed(gap))
            {
                enclosedCount++;
            }
        }

        // new plan - work via a flood fill - figure out how sections join, flood filled sections form
        // a similar map to the original interms of touching and calcs



        int ibreak = 0;

        m_debugInfo.Add("Part 2 : "+(m_freeSides-(enclosedCount * 6)));

    }

    public IntVector3[] m_offsets = new IntVector3[]{
                                new IntVector3(1,0,0),new IntVector3(-1,0,0),
                                new IntVector3(0,1,0),new IntVector3(0,-1,0),
                                new IntVector3(0,0,1),new IntVector3(0,0,-1)};

    public bool IsEnclosed(IntVector3 cube)
    {
        if(!m_touchingMap.ContainsKey(cube))
        {
            foreach(IntVector3 offset in m_offsets)
            {
                if(m_touchingMap.ContainsKey((cube+offset)))
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsInside(IntVector3 cube)
    {
        if(m_touchingMap.ContainsKey(cube))
        {
            return m_touchingMap[cube].Count == 0;
        }
        return false;
    }


    public bool DoCubesTouch(IntVector3 cube1, IntVector3 cube2)
    {
        return cube1.ManhattanDistance(cube2) == 1;
    }




}