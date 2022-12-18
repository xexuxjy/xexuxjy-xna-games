using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Text;

public class Test12 : BaseTest, IMapData
{
    public int m_width;
    public int m_height;

    Vector2 m_startPoint = new Vector2();
    Vector2 m_endPoint = new Vector2();

    public Route m_route;

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
                if (m_dataFileContents[y][x] == 'S')
                {
                    m_startPoint = new Vector2(x, y);
                }
                if (m_dataFileContents[y][x] == 'E')
                {
                    m_endPoint = new Vector2(x, y);
                }

            }
        }

        m_aStar = new AStar();
        m_aStar.Initialize(this);


        m_route = new Route(m_startPoint, m_width, m_height);


        while (m_route.CurrentPosition != m_endPoint)
        {
            char currentHeight = HeightAtPoint(m_route.CurrentPosition);
            bool managedToMove = false;
            foreach (Vector2 direction in directions)
            {
                Vector2 possibleMove = m_route.CurrentPosition + direction;
                if (InBounds(possibleMove))
                {
                    // don't go somewhere we've already been.
                    if (!m_route.Visited(possibleMove))
                    {
                        char targetHeight = HeightAtPoint(possibleMove);
                        if (CanMove(currentHeight, targetHeight))
                        {
                            m_route.AddMove(possibleMove, direction);
                            managedToMove = true;
                            // found a move
                            break;
                        }
                    }
                }
            }
            if (!managedToMove)
            {
                // oops - backtrack?
                int ibreak = 0;
                m_route.BackTrack();
            }

        }
        // we win!
        int ibreak2 = 0;
        m_debugInfo.Add(DrawRoute());

        WriteDebugInfo();
    }



    public string DrawRoute()
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
                //else if (v == m_startPoint)
                //{
                //    c = 'S';
                //}
                else
                {
                    int index = m_route.Locations.IndexOf(v);
                    if (index != -1)
                    {
                        //Vector2 move = m_route.Moves[index-1];
                        Vector2 move = m_route.GetDirection(v);
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
            return 'Z';
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
        throw new NotImplementedException();
    }


}



public class Route
{
    public Vector2 CurrentPosition
    { get; private set; }

    private int m_width;
    private int m_height;

    bool[] m_visited;
    private List<Vector2> m_locations = new List<Vector2>();
    private List<Vector2> m_moves = new List<Vector2>();
    private Dictionary<Vector2, Vector2> m_moveDictionary = new Dictionary<Vector2, Vector2>();

    public List<Vector2> Moves
    { get { return m_moves; } }

    public List<Vector2> Locations
    { get { return m_locations; } }


    public Route(Vector2 startPoint, int width, int height)
    {
        CurrentPosition = startPoint;
        m_width = width;
        m_height = height;
        m_visited = new bool[m_height * m_width];
        SetVisited(startPoint, true);
        m_locations.Add(startPoint);
    }

    public Vector2 GetDirection(Vector2 position)
    {
        if (m_moveDictionary.ContainsKey(position))
        {
            return m_moveDictionary[position];
        }
        return Vector2.Zero;
    }

    public bool Visited(Vector2 v)
    {
        return m_visited[(int)((v.Y * m_width) + v.X)];
    }

    public void SetVisited(Vector2 v, bool val)
    {
        m_visited[(int)((v.Y * m_width) + v.X)] = val;
    }


    public void AddMove(Vector2 v, Vector2 direction)
    {
        m_moveDictionary[CurrentPosition] = direction;
        Debug.Assert(!m_locations.Contains(v));
        m_locations.Add(v);
        m_moves.Add(direction);

        SetVisited(v, true);
        CurrentPosition = v;
    }

    public void BackTrack()
    {
        m_locations.RemoveAt(m_locations.Count - 1);
        m_moves.RemoveAt(m_moves.Count - 1);
        CurrentPosition = m_locations.Last();
    }
}
