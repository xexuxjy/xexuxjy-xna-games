using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Text;

public class Test12 : BaseTest, IMapData
{
    public int m_width;
    public int m_height;

    List<Vector2> m_startPoints = new List<Vector2>();
    Vector2 m_endPoint = new Vector2();

    public AStar m_aStar;

    static Vector2[] Directions = new Vector2[] { new Vector2(-1, 0), new Vector2(1, 0), new Vector2(0, -1), new Vector2(0, 1) };


    public override void RunTest()
    {
        TestID = 12;
        ReadDataFile();


        m_height = m_dataFileContents.Count;
        m_width = m_dataFileContents[0].Length;


        for (int y = 0; y < m_dataFileContents.Count; y++)
        {
            for (int x = 0; x < m_dataFileContents[y].Length; x++)
            {
                if (m_dataFileContents[y][x] == 'S' || m_dataFileContents[y][x] == 'a')
                {
                    m_startPoints.Add(new Vector2(x, y));
                }
                if (m_dataFileContents[y][x] == 'E')
                {
                    m_endPoint = new Vector2(x, y);
                }

            }
        }

        int shortestPath = int.MaxValue;
        List<Vector2> shortestResults = null;

        foreach (Vector2 startPoint in m_startPoints)
        {
            m_aStar = new AStar();
            m_aStar.Initialize(this);
            List<Vector2> results = new List<Vector2>();
            if (m_aStar.FindPath(startPoint, m_endPoint, results))
            {

                if (results.Count < shortestPath)
                {
                    shortestPath = results.Count;
                    shortestResults = results;
                }
            }
        }

        // we win!
        int ibreak2 = 0;
        m_debugInfo.Add(DrawRoute(shortestResults));

        WriteDebugInfo();
    }

    public string DrawRoute(List<Vector2> points)
    {
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < m_height; ++y)
        {
            for (int x = 0; x < m_width; ++x)
            {
                Vector2 v = new Vector2(x, y);
                char c = '.';
                if (v == m_endPoint)
                {
                    c = 'E';
                }
                else
                {
                    int index = points.IndexOf(v);
                    if (index != -1 && index > 0)
                    {
                        Vector2 move = v - points[index - 1];
                        if (move == new Vector2(-1, 0))
                        {
                            c = '<';
                        }
                        else if (move == new Vector2(1, 0))
                        {
                            c = '>';
                        }
                        else if (move == new Vector2(0, -1))
                        {
                            c = '^';
                        }
                        else if (move == new Vector2(0, 1))
                        {
                            c = 'v';
                        }
                    }
                }
                sb.Append(c);
            }
            sb.Append("\n");
        }
        return sb.ToString();
    }

    public bool InBounds(Vector2 v)
    {
        return v.X >= 0 && v.X < m_width && v.Y >= 0 && v.Y < m_height;

    }
    public bool InBounds(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0 && y < m_height);
    }

    //can move any number down, but at most one up
    public bool CanMove(char from, char to)
    {
        int diff = ((int)to - (int)from);
        return (diff <= 1);
    }

    public char HeightAtPoint(Vector2 v)
    {
        return HeightAtPoint((int)v.X, (int)v.Y);
    }
    public char HeightAtPoint(int x, int y)
    {
        char c = m_dataFileContents[y][x];
        if (c == 'S')
        {
            return 'a';
        }
        if (c == 'E')
        {
            return 'z';
        }
        return c;
    }

    public bool CanMove(Vector2 from, Vector2 to)
    {
        if (InBounds(to))
        {
            char currentHeight = HeightAtPoint(from);
            char targetHeight = HeightAtPoint(to);
            return CanMove(currentHeight, targetHeight);
        }
        return false;

    }

    public Vector2[] GetDirections()
    {
        return Directions;
    }

    public Vector2 GetTargetPosition()
    {
        return m_endPoint;
    }

    public float DistanceToTarget(Vector2 v)
    {
        float distanceX = Math.Abs(v.X - GetTargetPosition().X);
        float distanceY = Math.Abs(v.Y - GetTargetPosition().Y);

        return distanceX + distanceY;
    }


}


