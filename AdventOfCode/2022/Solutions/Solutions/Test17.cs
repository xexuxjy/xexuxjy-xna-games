using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Test17 : BaseTest
{
    private Board m_board;
    private Shape m_currentShape;
    private string m_moveList;
    private List<Shape> m_shapes = new List<Shape>();
    private int m_shapeIndex = 0;
    private int m_moveIndex = 0;

    private long m_totalRocks = 0;
    private long m_rockCount = 0;

    private bool m_foundCycle = false;
    private long m_cyclePeriod = 0;
    private long m_cycleHeight = 0;
    private long m_extraHeight = 0;
    private Tuple<int,int> m_cycleTuple = new Tuple<int, int>(-1,-1);

    private Dictionary<char, LongVector2> m_moveMap = new Dictionary<char, LongVector2>();
    private Dictionary<Tuple<int,int>,List<long>> m_cycleMap = new Dictionary<Tuple<int, int>, List<long>>();
    private Dictionary<Tuple<int,int>,List<long>> m_cycleRocksMap = new Dictionary<Tuple<int, int>, List<long>>();
    private List<long> m_boardHeights = new List<long>();


    public override void RunTest()
    {
        TestID = 17;
        IsTestInput = false;
        IsPart2 = true;
        //m_totalRocks = IsPart2 ? (1000000000000 - 1) : (2022 - 1);
        m_totalRocks = IsPart2 ? (1000000000000) : (2022);

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
        m_debugInfo.Add("Final Height : " + (m_board.HighestPoint+m_extraHeight));

        WriteDebugInfo();

    }

    public void Simulate()
    {
        int moveCount = m_moveList.Count();
        DateTime lastTime = DateTime.Now;

        while(m_rockCount < m_totalRocks)
        {
            m_currentShape = m_shapes[m_shapeIndex];//(Shape)Activator.CreateInstance(m_shapes[m_shapeIndex]);
            
            CheckCycle();
            if(m_foundCycle)
            {
                //Found period 35 / 53 SimulationState[round = 74, y = 119] & SimulationState[round = 39, y = 66] |
                //    IdxState[jetIdx = 21, rockIdx = 0] | 28571428568 | 999999999989
                //cur height 184 + 28571428568 * 53
                //part 2: 1514285714288
                //35ms

                //Found period 1710 / 2620 SimulationState[round = 3424, y = 5327] & SimulationState[round = 1714, y = 2707] | IdxState[jetIdx = 9976, rockIdx = 0] | 584795318 | 999999998914
                //cur height 9598 + 584795318 * 2620
                //part 2: 1532163742758
                //3146ms
                          



                long numPeriods = (m_totalRocks-m_rockCount) / m_cyclePeriod;
                //long remainingSim = m_totalRocks % m_cyclePeriod;
                
                
                long boardHeightForFirst = m_cycleMap[m_cycleTuple][0];
                long boardHeightForSecond = m_cycleMap[m_cycleTuple][1];

                long rockNumberforFirst = m_cycleRocksMap[m_cycleTuple][0];



                //m_extraHeight = boardHeightForFirst + (m_cycleHeight*numPeriods);
                m_extraHeight = m_cycleHeight*(numPeriods);

                //m_rockCount = (m_cyclePeriod * numPeriods)+rockNumberforFirst;
                
                m_rockCount +=  m_cyclePeriod * numPeriods;//m_totalRocks - remainingSim;

                //m_board.Clear();

                m_cycleMap.Clear();
                m_cycleRocksMap.Clear();

                m_foundCycle = false;
            }

            m_currentShape.Initialise(m_board);

            while (m_currentShape.IsFalling)
            {
                char moveChar = m_moveList[m_moveIndex];
                bool didMove = m_currentShape.ApplyMove(moveChar, m_moveMap[moveChar]);
                m_currentShape.ApplyMove(Shape.MoveDown, m_moveMap[Shape.MoveDown]);
                m_moveIndex++;

                if (m_moveIndex >= moveCount)
                {
                    m_moveIndex = 0;
                }

            }

            m_boardHeights.Add(m_board.HighestPoint);

            m_rockCount++;
            m_shapeIndex++;
            m_shapeIndex = m_shapeIndex % m_shapes.Count;


            DrawDebugBoard();
            m_currentShape.FillBoard(true);
            m_board.CheckTruncate();
            m_board.EnsureFull();

            int countCheck = 100000;
            if (m_rockCount % countCheck == 0)
            {
                DateTime now = DateTime.Now;
                TimeSpan elapsed = now.Subtract(lastTime);
                lastTime = now;
                double remainingSeconds = (m_rockCount / countCheck) * elapsed.TotalSeconds;
                double remainingHours = remainingSeconds / 3600;
                //System.Console.WriteLine("RockCount = " + m_totalRocks + " time " + elapsed.TotalSeconds + " predicted remaining = " + remainingSeconds + "  - " + remainingHours);

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


    public void CheckCycle()
    {
        if(m_shapeIndex != 0)
        {
            return;
        }

        Tuple<int,int> key = new Tuple<int, int>(m_shapeIndex,m_moveIndex);
        List<long> heights; 
        List<long> rocks=null;
        if(!m_cycleMap.TryGetValue(key,out heights))
        {
            heights = new List<long>();
            rocks = new List<long>();
            m_cycleMap[key] = heights;
            m_cycleRocksMap[key] = rocks;
        }
        rocks = m_cycleRocksMap[key];

        //heights.Add(m_board.HighestPoint);
        heights.Add(m_currentShape.ShapePosition.Y);
        rocks.Add(m_rockCount);

        Tuple<int,int> zeroKey = new Tuple<int, int>(0,0);


        // compare the values in the grid at different heights




        if(heights.Count > 1 )
        {
            List<long> diffs = new List<long>();
            for(int i = 0;i<heights.Count-1;++i)
            {
                diffs.Add(heights[i+1]-heights[i]);
            }

            List<long> rockDiffs = new List<long>();
            for(int i = 0;i<rocks.Count-1;++i)
            {
                rockDiffs.Add(rocks[i+1]-rocks[i]);
            }

            long roundsDiff = rockDiffs[0];

            int line1 = m_board.GetLine(heights[0]);
            int line2 = m_board.GetLine(heights[1]);

            long height1 = heights[0];
            long height2 = heights[1];


            for(int i=0;i<roundsDiff;++i)
            {
                if(m_board.GetLine(height1) != m_board.GetLine(height2))
                {
                    return;
                }
                height1--;
                height2--;
            }


            m_cyclePeriod = rockDiffs[0];
            m_cycleHeight = diffs[0];

            //System.Console.WriteLine(string.Format("Found matching cycle at ({0},{1}) : diff {2} : rockDiff {3}",key.Item1,key.Item2,string.Join(",",diffs),string.Join(",",rockDiffs)));
            m_foundCycle = true;
            m_cycleTuple = key;
        }


    }
}



public class Board
{
    public const int Width = 7;
    public const char EMPTY = '.';
    public const char RESTING = '#';
    public const char CURRENT = '@';

    public bool ShouldTruncate = false;

    private long m_highestFilledRow = 0;
    private long m_hightestPoint = 0;

    public long HighestPoint
    { get { return m_hightestPoint; } }

    //private List<char> m_occupiedList = new List<char>();

    private List<int> m_occupiedList = new List<int>();

    public Board()
    {
        EnsureFull();
        //SetOccupied(Width, 20, false);
    }

    public void Clear()
    {
        m_highestFilledRow = 0;
        m_hightestPoint = 0;
        m_occupiedList.Clear();
        EnsureFull();

    }

    public int GetLine(long y,bool adjust=true)
    {
        if (y < 0)
        {
            return 0;
        }

        int adjustedY = adjust ? AdjustY(y) : (int)y;
        return m_occupiedList[adjustedY];
    }

    public bool IsOccupied(long x, long y, bool adjust = true)
    {
        if (y < 0)
        {
            return true;
        }

        int adjustedY = adjust ? AdjustY(y) : (int)y;
        int rowVal = m_occupiedList[adjustedY];
        int masked = (rowVal & (1 << (int)x));
        return masked != 0;

        //EnsureFull((int)x, adjustedY);
        //return m_occupiedList[(Width * adjustedY) + (int)x];
    }

    public bool IsOccupiedMask(int maskVal,long y,bool adjust = true)
    {
        if (y < 0)
        {
            return true;
        }

        int adjustedY = adjust ? AdjustY(y) : (int)y;
        int rowVal = m_occupiedList[adjustedY];

        return (rowVal | maskVal) != (rowVal ^ maskVal);

    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int AdjustY(long y)
    {
        return (int)(y - m_highestFilledRow);
    }

    public void MaskRow(long row, int mask)
    {
        // need to make sure we don't overwrite existing values? not sure how though...

        int adjustedY = AdjustY(row);
        int rowValue = m_occupiedList[adjustedY];
        if (mask == 0)
        {
            rowValue = 0;
        }
        else
        {
            rowValue |= mask;
        }

        m_occupiedList[adjustedY] = rowValue;
    }

    public void SetOccupied(long x, long y, bool value)
    {
        if (y < 0)
        {
            return;
        }

        int adjustedY = AdjustY(y);

        int rowValue = m_occupiedList[adjustedY];
        if (value)
        {
            rowValue |= 1 << (int)x;
        }
        else
        {
            rowValue &= ~(1 << (int)x);
        }

        m_occupiedList[adjustedY] = rowValue;

    }

    public const int FullRow = 1 | 2 | 4 | 8 | 16 | 32 | 64;
    public void CheckTruncate()
    {
        if (ShouldTruncate)
        {
            int count = m_occupiedList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                if (m_occupiedList[i] == FullRow)
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
        int y = AdjustY(HighestPoint + 10);

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

        int rows = m_occupiedList.Count;

        for (int y = 0; y < rows; ++y)
        {
            line = new StringBuilder();
            line.Append('+');
            for (int x = 0; x < Width; ++x)
            {
                line.Append(IsOccupied(x, y, false) ? "#" : ".");
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
        //FillBoard(true);
    }

    public bool IsFalling { get; set; }
    public LongVector2 ShapePosition { get; set; }

    public abstract int Width { get; }
    public abstract int Height { get; }

    public abstract int MaskForLine(int row);

    public bool ApplyMove(char moveChar, LongVector2 moveV2)
    {
        //FillBoard(false);

        LongVector2 resultant = ShapePosition + moveV2;

        if (moveChar == MoveLeft || moveChar == MoveRight)
        {
            if (resultant.X >= 0 && resultant.X + Width <= Board.Width)
            {
                bool canMove = true;

                for (int y = 0; y < Height; ++y)
                {
                    int maskVal = MaskForLine(y) << (int)resultant.X;
                    if(m_board.IsOccupiedMask(maskVal,resultant.Y+y))
                    {
                        canMove = false;
                        break;
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
                int maskVal = MaskForLine(y) << (int)resultant.X;
                if(m_board.IsOccupiedMask(maskVal,resultant.Y+y))
                {
                    IsFalling = false;
                    break;
                }

                //for (int x = 0; x < Width; ++x)
                //{
                //    if (IsOccupied(x, y))
                //    {
                //        if (m_board.IsOccupied(resultant.X + x, resultant.Y + y))
                //        {
                //            // can't move any lower?
                //            IsFalling = false;
                //            break;
                //        }
                //    }
                //}
            }
            if (IsFalling)
            {
                ShapePosition = resultant;
            }
        }

        //FillBoard(true);
        return true;
    }


    public abstract bool IsOccupied(int x, int y);
    public abstract string GetDebug(int line);

    public void FillBoard(bool value)
    {
        if (false)
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (IsOccupied(x, y))
                    {
                        LongVector2 pos = new LongVector2(x, y) + ShapePosition;
                        m_board.SetOccupied(ShapePosition.X + x, ShapePosition.Y + y, value);
                    }
                }
            }
        }
        else
        {
            for (int y = 0; y < Height; ++y)
            {
                int maskValue = value ? MaskForLine(y) : 0;

                //int val = maskValue << ((7 - Width) - (int)ShapePosition.X);
                int val = maskValue << (int)ShapePosition.X;
                m_board.MaskRow(ShapePosition.Y + y, val);
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
        return 8 | 4 | 2 | 1;
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
        if (row == 1)
        {
            return 1 | 2 | 4;
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
            return 4;
        }
        return 1 | 2 | 4;
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
