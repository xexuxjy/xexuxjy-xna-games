using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

public class Test22 : BaseTest
{
    private Map m_map;
    private Player m_player;

    const int BlockSize = 50;


    public override void RunTest()
    {

        TestID = 22;
        IsTestInput = false;
        IsPart2 = true;
        ReadDataFile();

        m_map = new Map(m_dataFileContents);
        m_player = new Player(m_map, new IntVector2(m_dataFileContents[0].IndexOf('.'), 0));

        string movements = m_dataFileContents.Last();
        List<string> commands = BuildCommands(movements);


        //foreach (string command in commands)
        for (int i = 0; i < commands.Count; i++)
        {
            string command = commands[i];

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
                    if (m_player.LocalFacePosition.Y < 0 || m_player.LocalFacePosition.Y > 49)
                    {
                        int face = FaceForCoordinate(m_player.Position);
                        IntVector2 min = new IntVector2();
                        IntVector2 max = new IntVector2();
                        BoundsForFace(face, ref min, ref max);

                    }
                    Walk(number,i);
                }
            }
            
            //if(i > 1000)
            //{
            //    break;
            //}

        }
        //m_debugInfo.Add(m_player.GetRoute());
        int score = (1000 * (m_player.Position.Y + 1) + (4 * (m_player.Position.X + 1)) + m_player.Degrees / 90);
        DebugOutput($"Result is {m_player.Position.Y + 1}  {m_player.Position.X + 1} {m_player.Degrees / 90}  Score {score}");

        WriteDebugInfo();

    }

    public void Walk(int steps,int instructionNumber)
    {
        for (int i = 0; i < steps; ++i)
        {
            IntVector2 startPos = m_player.Position;
            if(IsPart2)
            {
                m_player.Position = m_map.CubeWrapMove(m_player.Position, m_player.Forward,instructionNumber);
            }
            else
            {
                IntVector2 newPosition = m_player.Position + m_player.Forward;
                m_player.Position = m_map.WrapMove(m_player.Position, newPosition);
            }

   
            Test22.FaceForCoordinate(m_player.Position);
            m_debugInfo.Add($"Instuction {instructionNumber} position is ({m_player.Position.X}+{m_player.Position.Y}j) face is {FaceForCoordinate(m_player.Position)}");
            
                     // can't go any futher
            if(m_player.Position == startPos)
            {
                break;
            }

            
            //debugInfo.Add(GetRoute());
        }
    }



    public static IntVector2 FaceLocalDirection(int face, IntVector2 direction)
    {
        if (face == 1)
        {
            return direction;
        }
        if (face == 2)
        {
            return direction;
        }
        if (face == 3)
        {
            return direction;
        }
        if (face == 4)
        {
            return direction;
        }
        if (face == 5)
        {
            //return new IntVector2(-direction.X,direction.Y);
            return direction;
        }
        if (face == 6)
        {
            //return new IntVector2(direction.Y,direction.X);
            return direction;
        }
        Debug.Assert(false);
        return direction;
    }


    public static void BoundsForFace(int face, ref IntVector2 min, ref IntVector2 max)
    {
        switch (face)
        {
            case 1:
                min = new IntVector2(BlockSize, 0);
                max = new IntVector2((BlockSize * 2) - 1, BlockSize - 1);
                break;
            case 2:
                min = new IntVector2(BlockSize * 2, 0);
                max = new IntVector2((BlockSize * 3) - 1, BlockSize - 1);
                break;
            case 3:
                min = new IntVector2(BlockSize, BlockSize);
                max = new IntVector2((BlockSize * 2) - 1, (BlockSize * 2) - 1);
                break;
            case 4:
                min = new IntVector2(0, BlockSize * 2);
                max = new IntVector2((BlockSize) - 1, (BlockSize * 3) - 1);
                break;
            case 5:
                min = new IntVector2(BlockSize, (BlockSize * 2));
                max = new IntVector2((BlockSize * 2) - 1, (BlockSize * 3) - 1);
                break;
            case 6:
                min = new IntVector2(0, (BlockSize * 3));
                max = new IntVector2((BlockSize) - 1, (BlockSize * 4) - 1);
                break;

        }
    }

    public static int LinkedFace(int startFace, IntVector2 direction)
    {
        if (startFace == 1 && direction == new IntVector2(1, 0)) return 2;
        if (startFace == 1 && direction == new IntVector2(-1, 0)) return 4;
        if (startFace == 1 && direction == new IntVector2(0, 1)) return 3;
        if (startFace == 1 && direction == new IntVector2(0, -1)) return 6;

        if (startFace == 2 && direction == new IntVector2(1, 0)) return 5;
        if (startFace == 2 && direction == new IntVector2(-1, 0)) return 1;
        if (startFace == 2 && direction == new IntVector2(0, 1)) return 3;
        if (startFace == 2 && direction == new IntVector2(0, -1)) return 6;

        if (startFace == 3 && direction == new IntVector2(1, 0)) return 2;
        if (startFace == 3 && direction == new IntVector2(-1, 0)) return 4;
        if (startFace == 3 && direction == new IntVector2(0, 1)) return 5;
        if (startFace == 3 && direction == new IntVector2(0, -1)) return 1;

        if (startFace == 4 && direction == new IntVector2(1, 0)) return 5;
        if (startFace == 4 && direction == new IntVector2(-1, 0)) return 1;
        if (startFace == 4 && direction == new IntVector2(0, 1)) return 6;
        if (startFace == 4 && direction == new IntVector2(0, -1)) return 3;

        if (startFace == 5 && direction == new IntVector2(1, 0)) return 2;
        if (startFace == 5 && direction == new IntVector2(-1, 0)) return 4;
        if (startFace == 5 && direction == new IntVector2(0, 1)) return 6;
        if (startFace == 5 && direction == new IntVector2(0, -1)) return 3;

        if (startFace == 6 && direction == new IntVector2(1, 0)) return 5;
        if (startFace == 6 && direction == new IntVector2(-1, 0)) return 1;
        if (startFace == 6 && direction == new IntVector2(0, 1)) return 2;
        if (startFace == 6 && direction == new IntVector2(0, -1)) return 4;

        return -1;
    }
    public static int FaceForCoordinate(IntVector2 c)
    {

        IntVector2 min = new IntVector2();
        IntVector2 max = new IntVector2();
        for (int i = 1; i <= 6; ++i)
        {
            BoundsForFace(i, ref min, ref max);
            if (c.X >= min.X && c.Y >= min.Y && c.X <= max.X && c.Y <= max.Y)
            {
                return i;
            }
        }
        Debug.Assert(false);
        return -1;

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
        if (currentNumber != "")
        {
            commands.Add(currentNumber);
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
            set
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

        public IntVector2 LocalFacePosition
        {
            get
            {
                int face = FaceForCoordinate(Position);
                IntVector2 min = new IntVector2();
                IntVector2 max = new IntVector2(); ;
                BoundsForFace(face, ref min, ref max);
                return Position - min;
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
            int maxWidth = 0;
            foreach (string line in map.FindAll(x => x.Contains(".")))
            {
                if (line.Length > maxWidth)
                {
                    maxWidth = line.Length;
                }
            }

            foreach (string line in map.FindAll(x => x.Contains(".")))
            {
                string copy = line;
                if (copy.Length < maxWidth)
                {
                    copy = copy.PadRight(maxWidth, ' ');
                }
                m_map.Add(copy);
            }
            int ibreak = 0;
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

        public bool IsBlocked(IntVector2 v)
        {
            return IsBlocked(v.X, v.Y);
        }

        public bool IsBlocked(int x, int y)
        {
            return (m_map[y][x] == '#');
        }



        public IntVector2 WrapMove(IntVector2 start, IntVector2 end)
        {
            int startFace = FaceForCoordinate(start);

            IntVector2 pos = end;
            bool xMoveLeft = (end.X - start.X == -1);
            bool xMoveRight = (end.X - start.X == 1);

            bool yMoveUp = (end.Y - start.Y == 1);
            bool yMoveDown = (end.Y - start.Y == -1);

            int startX = 0;
            int endX = 0;
            int startY = 0;
            int endY = 0;

            if (xMoveLeft || xMoveRight)
            {
                StartEndRow(pos.Y, ref startX, ref endX);
                if (xMoveLeft && pos.X < startX)
                {
                    pos.X = endX;
                }
                else if (xMoveRight && pos.X > endX)
                {
                    pos.X = startX;
                }
            }
            if (yMoveUp || yMoveDown)
            {
                StartEndColumn(pos.X, ref startY, ref endY);
                if (yMoveDown && pos.Y < startY)
                {
                    pos.Y = endY;
                }
                else if (yMoveUp && pos.Y > endY)
                {
                    pos.Y = startY;
                }
            }

            int endFace = FaceForCoordinate(pos);


            if (IsBlocked(pos))
            {
                return start;
            }


            return pos;
        }

        //public IntVector2 RemapCoordinate(IntVector2 start, char side,int startFace,int endFace)
        //{
        //    1R -> 2L



        //}


        public IntVector2 CubeWrapMove(IntVector2 start, IntVector2 direction,int instructionNumber)
        {
            IntVector2 startLocalMin = new IntVector2();
            IntVector2 startLocalMax = new IntVector2();

            IntVector2 endLocalMin = new IntVector2();
            IntVector2 endLocalMax = new IntVector2();


            int startFace = FaceForCoordinate(start);
            int endFace = startFace;

            BoundsForFace(startFace, ref startLocalMin, ref startLocalMax);
            //IntVector2 faceLocalDirection = FaceLocalDirection(startFace, direction);


            IntVector2 localStart = start - startLocalMin;
            IntVector2 localEnd = localStart + faceLocalDirection;

            IntVector2 pos = start;

            IntVector2 localEndCopy = localEnd;

            if (localEnd.X < 0 || localEnd.X > BlockSize - 1 || localEnd.Y < 0 || localEnd.Y > BlockSize - 1)
            {
                if(instructionNumber == 384)
                {
                    int ibreak =0 ;
                }
                // changed face.
                endFace = LinkedFace(startFace, direction);
                BoundsForFace(endFace, ref endLocalMin, ref endLocalMax);
                localEnd = ApplyFaceChangeRule(startFace,endFace,localEnd);
                pos = localEnd + endLocalMin;


            }
            else
            {
                // same face .
                pos = startLocalMin + localEnd;
            }






            if (IsBlocked(pos))
            {
                return start;
            }
            Test22.FaceForCoordinate(pos);
            return pos;

        }
        
        public IntVector2 ApplyFaceChangeRule(int startFace,int endFace,IntVector2 localEnd)
        {
            if(startFace == 1 && endFace == 6)
            {
                return new IntVector2(0,localEnd.X);
            }
            if(startFace == 1 && endFace == 4)
            {
                return new IntVector2(0,(BlockSize-1)-localEnd.Y);
            }
            if(startFace == 2 && endFace == 3)
            {
                return new IntVector2(localEnd.Y,localEnd.X);
            }
            if(startFace == 2 && endFace == 5)
            {
                return new IntVector2(BlockSize-1,localEnd.Y);
            }
            if(startFace == 3 && endFace == 2)
            {
                return new IntVector2((BlockSize-1)-localEnd.Y,0);
            }
            if(startFace == 3 && endFace == 4)
            {
                return new IntVector2(localEnd.Y,0);
            }
            if(startFace == 4 && endFace == 3)
            {
                return new IntVector2(0,localEnd.X);
            }
            if(startFace == 4 && endFace == 1)
            {
                return new IntVector2(0,(BlockSize-1)-localEnd.Y);
            }
            if(startFace == 5 && endFace == 2)
            {
                return new IntVector2(0,(BlockSize-1)-localEnd.Y);
            }
            if(startFace == 5 && endFace == 6)
            {
                return new IntVector2(BlockSize-1,localEnd.X);
            }
            if(startFace == 6 && endFace == 1)
            {
                return new IntVector2(localEnd.Y,0);
            }
            if(startFace == 6 && endFace == 5)
            {
                return new IntVector2(localEnd.Y,BlockSize-1);
            }


            // adjust for new position in on face 
            if (localEnd.X < 0)
            {
                localEnd.X = BlockSize - 1;
            }
            if (localEnd.Y < 0)
            {
                localEnd.Y = BlockSize - 1;
            }
            if (localEnd.X > BlockSize - 1)
            {
                localEnd.X = 0;
            }
            if (localEnd.Y > BlockSize - 1)
            {
                localEnd.Y = 0;
            }
            return localEnd;

        }
        
        
        public void StartEndRow(int row, ref int startX, ref int endX)
        {
            int s1 = m_map[row].IndexOf(".");
            int s2 = m_map[row].IndexOf("#");

            if (s2 == -1)
            {
                s2 = s1;
            }
            startX = Math.Min(s1, s2);

            int e1 = m_map[row].LastIndexOf(".");
            int e2 = m_map[row].LastIndexOf("#");

            if (e2 == -1)
            {
                e2 = e1;
            }
            endX = Math.Max(e1, e2);
        }

        public void StartEndColumn(int column, ref int startY, ref int endY)
        {
            for (int i = 0; i < Height; ++i)
            {
                if (m_map[i][column] == '.' || m_map[i][column] == '#')
                {
                    startY = i;
                    break;
                }
            }

            for (int i = Height - 1; i > startY; --i)
            {
                if (m_map[i][column] == '.' || m_map[i][column] == '#')
                {
                    endY = i;
                    break;
                }
            }
        }

    }

}