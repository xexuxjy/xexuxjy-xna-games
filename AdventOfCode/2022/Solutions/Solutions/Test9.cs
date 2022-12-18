using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Text;

public class Test9 : BaseTest
{
    private List<RopeLink> m_ropeLinks = new List<RopeLink>();
    private StringBuilder m_debugStringBuilder = new StringBuilder();

    public override void RunTest()
    {
        TestID = 9;
        ReadDataFile();


        int numLinks = 10;
        for (int i = 0; i < numLinks; ++i)
        {
            RopeLink ropeLink = new RopeLink();
            if (i == 0)
            {
                ropeLink.DebugSymbol = "H";
            }
            else
            {
                ropeLink.DebugSymbol = "" + i;
            }
            m_ropeLinks.Add(ropeLink);
            ropeLink.Position = ropeLink.Position;
        }

        foreach (string move in m_dataFileContents)
        {
            string[] tokens = move.Split(' ');
            Debug.Assert(tokens.Length == 2);
            string direction = tokens[0];
            int count = int.Parse(tokens[1]);

            PerformMove(direction, count);
        }
        WriteDebug();
        int ibreak = 0;
    }

    public void WriteDebug()
    {
        using (StreamWriter sw = new StreamWriter(new FileStream(InputPath + "puzzle-" + TestID + "-debug.txt", FileMode.Create)))
        {
            sw.WriteLine(m_debugStringBuilder.ToString());
        }
    }
    public void DrawGrid(List<RopeLink> links, Vector2 start, StringBuilder sb)
    {
        int width = 20;
        int height = 20;

        for (int y = height - 1; y >= 0; --y)
        {
            for (int x = 0; x < width; ++x)
            {
                string debug = "";
                Vector2 v = new Vector2(x, y);
                for (int i = 0; i < links.Count; ++i)
                {
                    if (v == links[i].Position)
                    {
                        debug = links[i].DebugSymbol;
                        break;
                    }
                }

                if (debug == "")
                {
                    if (v == start)
                    {
                        debug = "s";
                    }
                    else
                    {
                        debug = ".";
                    }
                }
                sb.Append(debug);
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        sb.AppendLine();
    }

    public void PerformMove(string direction, int count)
    {
        Vector2 move = new Vector2();
        switch (direction)
        {
            case "U":
                move = new Vector2(0, 1);
                break;
            case "D":
                move = new Vector2(0, -1);
                break;
            case "L":
                move = new Vector2(-1, 0);
                break;
            case "R":
                move = new Vector2(1, 0);
                break;
        }

        for (int i = 0; i < count; ++i)
        {
            m_ropeLinks[0].Position += move;
            // move all the links in the chain.
            for (int j = 1; j < m_ropeLinks.Count; ++j)
            {
                UpdateLink(m_ropeLinks[j - 1], m_ropeLinks[j]);
            }
        }
        DrawGrid(m_ropeLinks,StartPoint, m_debugStringBuilder);
    }

    public void UpdateLink(RopeLink head, RopeLink tail)
    {
        int xdiff = (int)(head.Position.X - tail.Position.X);
        int ydiff = (int)(head.Position.Y - tail.Position.Y);

        int xlen = Math.Abs(xdiff);
        int ylen = Math.Abs(ydiff);

        Vector2 move = new Vector2(0, 0);

        int xchange = 0;
        int ychange = 0;

        // move one space closer
        if (xlen == 2 && ylen == 0)
        {
            xchange = xdiff > 0 ? 1 : -1;
        }
        else if (ylen == 2 && xlen == 0)
        {
            ychange = ydiff > 0 ? 1 : -1;
        }
        // not already touching
        else if (!(xlen <= 1 && ylen <= 1))
        {
            xchange = xdiff > 0 ? 1 : -1;
            ychange = ydiff > 0 ? 1 : -1;
        }

        move = new Vector2(xchange, ychange);
        if (move.LengthSquared() != 0)
        {
            tail.Position += move;
        }

    }

    private Vector2 StartPoint= Vector2.Zero;
}


public class RopeLink
{
    private Vector2 m_position;
    public Vector2 Position
    {
        get { return m_position; }
        set
        {
            m_position = value;
            m_movePoints.Add(m_position);
        }
    }
    public String DebugSymbol="";
    public HashSet<Vector2> m_movePoints = new HashSet<Vector2>();

}
