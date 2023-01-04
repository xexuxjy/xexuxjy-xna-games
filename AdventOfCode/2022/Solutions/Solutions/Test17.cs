using System.Runtime.CompilerServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        m_rockCount = IsPart2 ? (1000000000000 - 1) : (2022 - 1);

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

        m_board.CalcHighestPoint();
        m_debugInfo.Add("Final Height : " + m_board.HighestPoint);

        WriteDebugInfo();

    }

    public void Simulate()
    {
        int moveIndex = 0;
        int moveCount = m_moveList.Count();
        DateTime lastTime = DateTime.Now;

        while (m_rockCount >= 0)
        {
            m_currentShape = GetNextShape();

            while (m_currentShape.IsFalling)
            {
                char moveChar = m_moveList[moveIndex];
                bool didMove = m_currentShape.ApplyMove(moveChar, m_moveMap[moveChar]);
                m_currentShape.ApplyMove(Shape.MoveDown, m_moveMap[Shape.MoveDown]);

                moveIndex++;

                if (moveIndex >= moveCount)
                {
                    moveIndex = 0;
                }

            }
            DrawDebugBoard();
            m_currentShape.FillBoard(true);
            m_board.CheckTruncate();
            m_board.EnsureFull();

            int countCheck =1000000;
            if (m_rockCount %  countCheck == 0)
            {
                DateTime now = DateTime.Now;
                TimeSpan elapsed = now.Subtract(lastTime);
                lastTime = now;
                long remainingSeconds = (m_rockCount / countCheck) * (long)elapsed.TotalSeconds;
                long remainingHours = remainingSeconds / 3600;
                System.Console.WriteLine("RockCount = " + m_rockCount+ " time "+ elapsed.TotalSeconds+" predicted remaining = "+remainingSeconds+ "  - "+remainingHours);

            }
        }

    }

    public void DrawDebugBoard()
    {
        if (!IsPart2)
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
        shape.FillBoard(true);

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

    //private List<char> m_occupiedList = new List<char>();
    
    private List<int> m_occupiedList = new List<int>();

    public Board()
    {
        EnsureFull();
        SetOccupied(Width, 20, false);
    }

    public bool IsOccupied(long x, long y, bool adjust = true)
    {
        if (y < 0)
        {
            return true;
        }

        int adjustedY = adjust ? AdjustY(y) : (int)y;
        int rowVal= m_occupiedList[adjustedY] ;
        int masked = (rowVal & (1<<(int)x));
        return  masked != 0;

        //EnsureFull((int)x, adjustedY);
        //return m_occupiedList[(Width * adjustedY) + (int)x];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdjustY(long y)
    {
        return (int)(y - m_highestFilledRow);
    }


    public void SetOccupied(long x, long y, bool value)
    {
        if (y < 0)
        {
            return;
        }

        int adjustedY = AdjustY(y);

        int rowValue = m_occupiedList[adjustedY];
        if(value)
        {
            rowValue |= 1 << (int)x;
        }
        else
        {
            rowValue &= ~(1 << (int)x);
        }

        m_occupiedList[adjustedY] = rowValue;

    }

    public const int FullRow = 1 |2 | 4 | 8 | 16 | 32 | 64;
    public void CheckTruncate()
    {
        if (ShouldTruncate)
        {
            int count = m_occupiedList.Count;
            for (int i = count - 1; i >= 0; i --)
            {
                if(m_occupiedList[i] == FullRow)
                {
                    m_highestFilledRow += i;
                    m_occupiedList.RemoveRange(0, i);
                    break;
                }
            }
        }
    }


    public void EnsureFull()
    {
        int y = AdjustY(HighestPoint + 100);

        int count = m_occupiedList.Count;
        int extra = (y + 1 - count);

        for (int i = 0; i < extra; ++i)
        {
            m_occupiedList.Add(0);
        }
    }

    public void CalcHighestPoint()
    {
        int count = m_occupiedList.Count;
        for (int i = count - 1; i >= 0; i--)
        {
            if (m_occupiedList[i] != 0)
            {
                int row = i;
                m_hightestPoint = m_highestFilledRow + row + 1;
                break;
            }
        }
    }


    public string DrawDebug()
    {
        List<string> lines = new List<string>();

        StringBuilder line = new StringBuilder();

        for (int i = 0; i < Width + 2; ++i)
        {
            line.Append('+');
        }
        lines.Add(line.ToString());


        int rows = m_occupiedList.Count / Width;

        for (int y = 0; y < rows; ++y)
        {


            line = new StringBuilder();
            line.Append('+');
            for (int x = 0; x < Width; ++x)
            {
                line.Append(IsOccupied(x, y, false)?"#":".");
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

    public abstract int MaskForLine(int row);

    public bool ApplyMove(char moveChar, LongVector2 moveV2)
    {
        FillBoard(false);

        bool fillValue = true;

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
                            if (m_board.IsOccupied(resultant.X + x, resultant.Y + y))
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
                        if (m_board.IsOccupied(resultant.X + x, resultant.Y + y))
                        {
                            // can't move any lower?
                            IsFalling = false;
                            fillValue = true;
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

        FillBoard(fillValue);
        return true;
    }


    public abstract bool IsOccupied(int x, int y);
    public abstract string GetDebug(int line);

    public void FillBoard(bool value)
    {
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                if (IsOccupied(x, y))
                {
                    //LongVector2 pos = new LongVector2(x, y) + ShapePosition;
                    m_board.SetOccupied(ShapePosition.X + x, ShapePosition.Y + y, value);
                }
            }
        }
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsOccupied(int x, int y)
    {
        return true;
    }
    public override int MaskForLine(int row)
    {
        return 1 | 2 | 4 |8;
    }

}



public class Shape2 : Shape
{
    // .#.
    // ###
    // .#.


    public override int Width { get { return 3; } }
    public override int Height { get { return 3; } }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsOccupied(int x, int y)
    {
        if (y == 1)
        {
            return true;
        }
        return x == 1;
    }

    public override int MaskForLine(int row)
    {
        if(row == 1)
        {
            return 1 | 2| 4;
        }
        return 2;
    }


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
    public override int MaskForLine(int row)
    {
        if (row == 1 || row == 2)
        {
            return 1;
        }
        return 1 |2| 4;
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsOccupied(int x, int y)
    {
        if (y == 0)
        { return true; }
        return x == 2;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsOccupied(int x, int y)
    {
        return true;
    }

    public override int MaskForLine(int row)
    {
        return 1;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override bool IsOccupied(int x, int y)
    {
        return true;
    }
    public override int MaskForLine(int row)
    {
        return 1 | 2;
    }

}
