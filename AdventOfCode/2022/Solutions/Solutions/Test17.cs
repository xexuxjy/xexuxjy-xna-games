using System.Runtime.CompilerServices;
using System.Text;

public class Test17 : BaseTest
{
    private Board m_board;
    private Shape m_currentShape;
    private string m_moveList;
    private List<Shape> m_shapes = new List<Shape>();
    private int m_shapeIndex = 0;
    private long m_rockCount = 0;

    private Dictionary<char, LongVector2> m_moveMap = new Dictionary<char, LongVector2>();


    public override void RunTest()
    {
        TestID = 17;
        IsTestInput = false;
        IsPart2 = true;
        m_rockCount = IsPart2? (1000000000000-1) : (2022-1);


        ReadDataFile();
        m_shapes.Add(new Shape1());
        m_shapes.Add(new Shape2());
        m_shapes.Add(new Shape3());
        m_shapes.Add(new Shape4());
        m_shapes.Add(new Shape5());

        m_board = new Board();
        m_moveList = m_dataFileContents[0];
        m_moveMap[Shape.MoveLeft] = new LongVector2(-1, 0);
        m_moveMap[Shape.MoveRight] = new LongVector2(1, 0);
        m_moveMap[Shape.MoveDown] = new LongVector2(0, -1);


        Simulate();

        m_debugInfo.Add("Final Height : " + m_board.HighestPoint);

        WriteDebugInfo();

    }

    public void Simulate()
    {
        int moveIndex = 0;
        int moveCount = m_moveList.Count();
        bool keepGoing = true;
        while (m_rockCount >= 0)
        {
            m_currentShape = GetNextShape();

            while (m_currentShape.IsFalling)
            {
                char moveChar = m_moveList[moveIndex];
                //m_debugInfo.Add("Before move");
                //m_debugInfo.Add(m_board.DrawDebug());
                bool didMove = m_currentShape.ApplyMove(moveChar, m_moveMap[moveChar]);
                //m_debugInfo.Add("After move : "+moveChar);
                //m_debugInfo.Add(m_board.DrawDebug());
                m_currentShape.ApplyMove(Shape.MoveDown, m_moveMap[Shape.MoveDown]);
                //m_debugInfo.Add("After move down");
                //m_debugInfo.Add(m_board.DrawDebug());
                //m_debugInfo.Add("Move Index == "+moveIndex);

                moveIndex++;

                if (moveIndex >= moveCount)
                {
                    moveIndex = 0;
                }

            }
            m_currentShape.FillBoard(Board.CURRENT);
            DrawDebugBoard();
            m_currentShape.FillBoard(Board.RESTING);
            m_board.CheckTruncate();
            m_board.EnsureFull();
        }
    }

    public void DrawDebugBoard()
    {
        if(!IsPart2)
        {
            m_debugInfo.Add(m_board.DrawDebug());
        }
    }

    public Shape GetNextShape()
    {
        Shape shape = m_shapes[m_shapeIndex];//(Shape)Activator.CreateInstance(m_shapes[m_shapeIndex]);
        m_shapeIndex++;
        m_shapeIndex = m_shapeIndex % m_shapes.Count;
        m_rockCount--;

        shape.Initialise(m_board);
        shape.FillBoard(Board.CURRENT);

        return shape;
    }
}


public class Board
{
    public const int Width = 7;
    public const char EMPTY = '.';
    public const char RESTING = '#';
    public const char CURRENT = '@';

    public bool ShouldTruncate = true;

    private long m_highestFilledRow = 0;
    private long m_hightestPoint = 0;

    public long HighestPoint
    { get { return m_hightestPoint; } }

    private List<char> m_occupiedList = new List<char>();


    public Board()
    {
        EnsureFull();
        SetOccupied(Width, 20, EMPTY);
    }

    public char IsOccupied(long x, long y,bool adjust = true)
    {
        if (y < 0)
        {
            return RESTING;
        }

        int adjustedY = adjust ? AdjustY(y) : (int)y;

        //EnsureFull((int)x, adjustedY);
        return m_occupiedList[(Width * adjustedY) + (int)x];
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdjustY(long y)
    {
        return (int)(y - m_highestFilledRow);
    }


    public void SetOccupied(long x, long y, char value)
    {
        if (y < 0)
        {
            return;
        }

        int adjustedY = AdjustY(y);

        //EnsureFull((int)x, adjustedY);

        int index = (adjustedY * Width) + (int)x;
        m_occupiedList[index] = value;


        bool fullRow = true;
        for (int i = 0; i < Width; ++i)
        {
            if (m_occupiedList[(adjustedY * Width) + i] == Board.EMPTY)
            {
                fullRow = false;
                break;
            }
        }

    }

    public void CheckTruncate()
    {
        if (ShouldTruncate)
        {
            for (int i = 0; i < m_occupiedList.Count; i += Width)
            {
                bool allOccupied = true;
                for (int x = 0; x < Width; x++)
                {
                    if (m_occupiedList[i + x] == EMPTY)
                    {
                        allOccupied = false;
                        break;
                    }
                }
                if (allOccupied)
                {
                    int row = i / Width;
                    m_highestFilledRow += row;
                    m_occupiedList.RemoveRange(0, row * Width);

                }
            }
        }

    }


    public void EnsureFull()
    {
        int y = AdjustY(HighestPoint + 100);

        int count = m_occupiedList.Count;
        int extra = ((y*Width)+Width-count);

        for(int i=0;i<extra;++i)
        {
            m_occupiedList.Add(EMPTY);
        }
    }

    public void CalcHighestPoint()
    {
        int count = m_occupiedList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (m_occupiedList[i] != EMPTY)
            {
                int row = i / Width;
                m_hightestPoint = m_highestFilledRow + row + 1;
                break;
            }
        }
    }


    public String DrawDebug()
    {

        List<string> lines = new List<string>();
        int count = 0;

        StringBuilder line = new StringBuilder();

        for (int i = 0; i < Width + 2; ++i)
        {
            line.Append('+');
        }
        lines.Add(line.ToString());


        int rows = m_occupiedList.Count / Width;

        for (int y = 0; y < rows ; ++y)
        {


            line = new StringBuilder();
            line.Append('+');
            for (int x = 0; x < Width; ++x)
            {
                line.Append(IsOccupied(x, y,false));
            }
            line.Append('+');
            lines.Add(line.ToString());
        }
        lines.Add("\n");
        lines.Reverse();
        line = new StringBuilder();
        foreach (string var in lines)
        {
            line.AppendLine(var);
        }

        return line.ToString();
    }

}


public abstract class Shape
{
    Board m_board;

    public const char MoveLeft = '<';
    public const char MoveRight = '>';
    public const char MoveDown = 'v';



    public void Initialise(Board board)
    {
        m_board = board;
        m_board.CalcHighestPoint();
        IsFalling = true;
        // position = bottom left
        ShapePosition = new LongVector2(2, board.HighestPoint + 3);

    }

    public bool IsFalling { get; set; }
    public LongVector2 ShapePosition { get; set; }

    public abstract int Width { get; }
    public abstract int Height { get; }



    public bool ApplyMove(char moveChar, LongVector2 moveV2)
    {
        FillBoard(Board.EMPTY);

        char fillChar = Board.CURRENT;

        LongVector2 resultant = ShapePosition + moveV2;

        if (moveChar == MoveLeft || moveChar == MoveRight)
        {
            if (resultant.X >= 0 && resultant.X + Width <= Board.Width)
            {
                bool canMove = true;

                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        if (IsOccupied(x, y))
                        {
                            if (m_board.IsOccupied(resultant.X + x, resultant.Y + y) != Board.EMPTY)
                            {
                                canMove = false;
                                break;
                            }
                        }
                    }
                }
                if (canMove)
                {
                    ShapePosition = resultant;
                }
                return canMove;
            }
        }
        else if (moveChar == MoveDown)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (IsOccupied(x, y))
                    {
                        if (m_board.IsOccupied(resultant.X + x, resultant.Y + y) != Board.EMPTY)
                        {
                            // can't move any lower?
                            IsFalling = false;
                            fillChar = Board.RESTING;
                            break;
                        }
                    }
                }
            }
            if (IsFalling)
            {
                ShapePosition = resultant;
            }
        }

        FillBoard(fillChar);
        return true;
    }


    public bool IsOccupied(int x, int y)
    {
        return GetDebug(y)[x] == '#';
    }

    public abstract string GetDebug(int line);

    public void FillBoard(char value)
    {
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                LongVector2 pos = new LongVector2(x, y) + ShapePosition;

                //need a 'leave as is value' ?
                if (IsOccupied(x, y))
                {
                    m_board.SetOccupied(pos.X, pos.Y, value);
                }
            }
        }
        m_board.CalcHighestPoint();
    }
}

public class Shape1 : Shape
{
    // ####
    public override int Width { get { return 4; } }
    public override int Height { get { return 1; } }

    public override string GetDebug(int line)
    {
        return "####";
    }

}



public class Shape2 : Shape
{
    // .#.
    // ###
    // .#.


    public override int Width { get { return 3; } }
    public override int Height { get { return 3; } }
    public override string GetDebug(int line)
    {
        if (line == 0 || line == 2)
        {
            return ".#.";
        }
        if (line == 1)
        {
            return "###";
        }
        return "";
    }
}


public class Shape3 : Shape
{
    public override int Width { get { return 3; } }
    public override int Height { get { return 3; } }
    public override string GetDebug(int line)
    {
        if (line == 1 || line == 2)
        {
            return "..# ";
        }
        if (line == 0)
        {
            return "###";
        }
        return "";
    }
}


public class Shape4 : Shape
{
    public override int Width { get { return 1; } }
    public override int Height { get { return 4; } }
    public override string GetDebug(int line)
    {
        return "#";
    }
}

public class Shape5 : Shape
{
    public override int Width { get { return 2; } }
    public override int Height { get { return 2; } }
    public override string GetDebug(int line)
    {
        return "##";
    }
}
