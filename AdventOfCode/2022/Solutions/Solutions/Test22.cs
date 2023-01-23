using System.Text;
using System.Text.RegularExpressions;

public class Test22 : BaseTest
{
    private Map m_map;
    private Player m_player;

    public override void RunTest()
    {
        TestID = 22;
        IsTestInput = true;
        ReadDataFile();

        m_map = new Map(m_dataFileContents);
        m_player = new Player(m_map, new IntVector2(m_dataFileContents[0].IndexOf('.'), 0));

        string movements = m_dataFileContents.Last();
        List<string> commands = BuildCommands(movements);

        foreach (string command in commands)
        {
            if (command == "L")
            {
                m_player.TurnLeft();
            }
            else if (command == "R")
            {
                m_player.TurnRight();
            }
            else
            {
                int number;
                if (int.TryParse(command, out number))
                {
                    m_player.Walk(number,m_debugInfo);
                }
            }
            m_debugInfo.Add(m_player.GetRoute());
        }
        int score = (1000 * m_player.Position.X) + (4 * m_player.Position.Y) + m_player.Degrees;
        DebugOutput($"Result is {m_player.Position.X}  {m_player.Position.Y} {m_player.Degrees}  Score {score}");

        WriteDebugInfo();

    }

    private List<string> BuildCommands(string movements)
    {
        List<string> commands = new List<string>();
        string currentNumber = "";
        for (int i = 0; i < movements.Length; ++i)
        {
            if (movements[i] >= '0' && movements[i] <= '9')
            {
                currentNumber += movements[i];
            }
            else
            {
                if (currentNumber.Length > 0)
                {
                    commands.Add(currentNumber);
                    currentNumber = "";
                }

                commands.Add("" + movements[i]);
            }
        }
        return commands;
    }


    public class Player
    {
        private Map m_map;
        private char[,] m_route;

        public Player(Map map, IntVector2 start)
        {
            m_map = map;
            m_route = new char[map.Height, map.Width];
            int y = 0;
            foreach (string line in map.MapData)
            {
                int x = 0;
                foreach (char c in line)
                {
                    m_route[y, x] = c;
                    x++;
                }
                y++;
            }

            Position = start;
            Forward = new IntVector2(1, 0);
        }

        private IntVector2 m_position;
        public IntVector2 Position
        {
            get { return m_position; }
            private set
            {
                m_position = value;
                char c = '>';
                switch (Degrees)
                {
                    case 0:
                        c = '>';
                        break;
                    case 90:
                        c = 'v';
                        break;
                    case 180:
                        c = '<';
                        break;
                    case 270:
                        c = '^';
                        break;
                }

                m_route[m_position.Y, m_position.X] = c;
            }
        }

        public IntVector2 Forward { get; private set; }

        public int Degrees { get; private set; }

        public void TurnLeft()
        {
            Degrees -= 90;
            if (Degrees < 0)
            {
                Degrees += 360;
            }
            UpdateForward();
        }

        public void TurnRight()
        {
            Degrees += 90;
            if (Degrees >= 360)
            {
                Degrees = 0;
            }
            UpdateForward();
        }
        private void UpdateForward()
        {
            switch (Degrees)
            {
                case 0:
                    Forward = new IntVector2(1, 0);
                    break;
                case 90:
                    Forward = new IntVector2(0, 1);
                    break;
                case 180:
                    Forward = new IntVector2(-1, 0);
                    break;
                case 270:
                    Forward = new IntVector2(0, -1);
                    break;
                default:
                    int ibreak = 0;
                    break;
            }
        }


        public void Walk(int steps,List<string> debugInfo)
        {
            for (int i = 0; i < steps; ++i)
            {
                IntVector2 newPosition = Position + Forward;
                newPosition = m_map.WrapMove(newPosition);
                if (!m_map.IsBlocked(newPosition.X, newPosition.Y))
                {
                    Position = newPosition;
                    //debugInfo.Add(GetRoute());
                }
            }
        }


        public String GetRoute()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < m_route.GetLength(0); y++)
            {
                for (int x = 0; x < m_route.GetLength(1); x++)
                {
                    sb.Append(m_route[y, x]);
                }
                sb.Append("\n");
            }
            return sb.ToString();
        }

    }
    public class Map
    {
        private List<string> m_map = new List<string>();
        public Map(List<string> map)
        {
            m_map.AddRange(map.FindAll(x => x.Contains(".")));
        }

        public IEnumerable<string> MapData
        {
            get { return m_map; }
        }

        public int Width
        {
            get
            {
                return m_map[0].Length;
            }
        }
        public int Height
        {
            get { return m_map.Count; }
        }


        public bool IsBlocked(int x, int y)
        {
            return (m_map[y][x] != '.');
        }


        public IntVector2 WrapMove(IntVector2 start)
        {
            IntVector2 pos = start;
            if (pos.X == -1)
            {
                pos.X = Width - 1;
            }
            if (pos.X == Width)
            {
                pos.X = 0;
            }
            if (pos.Y == -1)
            {
                pos.Y = Height - 1;
            }
            if (pos.Y == Height)
            {
                pos.Y = 0;
            }


            int startX = 0;
            int endX = 0;
            int startY = 0;
            int endY = 0;

            StartEndRow(pos.Y, ref startX, ref endX);
            StartEndColumn(pos.X, ref startY, ref endY);

            if (pos.X < startX)
            {
                pos.X = endX;
            }
            if (pos.X > endX)
            {
                pos.X = startX;
            }
            if (pos.Y < startY)
            {
                pos.Y = endY;
            }
            if (pos.Y > endY)
            {
                pos.Y = startY;
            }
            return pos;
        }

        public void StartEndRow(int row, ref int startX, ref int endX)
        {
            startX = m_map[row].IndexOf(".");
            endX = m_map[row].LastIndexOf(".");
        }

        public void StartEndColumn(int column, ref int startY, ref int endY)
        {
            for (int i = 0; i < Height; ++i)
            {
                if (m_map[i][column] == '.')
                {
                    startY = i;
                    break;
                }
            }

            for (int i = Height-1; i > startY; --i)
            {
                if (m_map[i][column] == '.')
                {
                    endY = i;
                    break;
                }
            }
        }

    }

}