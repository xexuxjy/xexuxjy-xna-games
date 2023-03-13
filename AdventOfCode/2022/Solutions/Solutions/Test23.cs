using System.Linq;
using System.Text;

public class Test23 : BaseTest
{
    protected List<Elf> m_elfList = new List<Elf>();

    public static Dictionary<Direction,IntVector2> DirectionMap = new Dictionary<Direction,IntVector2>();
    public static Dictionary<Direction,Direction[]> RelativeDirectionMap = new Dictionary<Direction,Direction[]>();



    public override void RunTest()
    {
        DateTime startTime = DateTime.Now;

        TestID = 23;
        IsTestInput = false;
        
        DirectionMap[Direction.North] = new IntVector2(0,-1);
        DirectionMap[Direction.South] = new IntVector2(0,1);
        DirectionMap[Direction.East] = new IntVector2(1,0);
        DirectionMap[Direction.West] = new IntVector2(-1,0);
        DirectionMap[Direction.NorthEast] = new IntVector2(1,-1);
        DirectionMap[Direction.SouthEast] = new IntVector2(1,1);
        DirectionMap[Direction.NorthWest] = new IntVector2(-1,-1);
        DirectionMap[Direction.SouthWest] = new IntVector2(-1,1);

        RelativeDirectionMap[Direction.North] = new Direction[]{Direction.NorthWest,Direction.North,Direction.NorthEast };
        RelativeDirectionMap[Direction.South] = new Direction[]{Direction.SouthWest,Direction.South,Direction.SouthEast };
        RelativeDirectionMap[Direction.East] = new Direction[]{Direction.NorthEast,Direction.East,Direction.SouthEast };
        RelativeDirectionMap[Direction.West] = new Direction[]{Direction.NorthWest,Direction.West,Direction.SouthWest};


        ReadDataFile();
         
        for(int y=0;y<m_dataFileContents.Count;y++)
        {
            string line = m_dataFileContents[y];
            for(int x=0;x<line.Length;++x)
            {
                if(line[x] == '#')
                {
                    Elf elf = new Elf(){Id = ""+m_elfList.Count,Position = new IntVector2(x,y) };
                    m_elfList.Add(elf);
                }
            }
        }

        //DrawBoard();


        int numRounds = 0;

        Dictionary<IntVector2,List<Elf>> moveCounts = new Dictionary<IntVector2,List<Elf>>();
        List<Direction> directionChoices = new List<Direction>();
        directionChoices.Add(Direction.North);  
        directionChoices.Add(Direction.South);  
        directionChoices.Add(Direction.West);  
        directionChoices.Add(Direction.East);  

        Dictionary<Elf,IntVector2> proposedMap = new Dictionary<Elf,IntVector2>();


        HashSet<IntVector2> occupiedPositions = new HashSet<IntVector2>();

        bool keepGoing = true;

        while(keepGoing)
        {
            occupiedPositions.Clear();
            moveCounts.Clear();

            foreach(Elf elf in m_elfList)
            {
                occupiedPositions.Add(elf.Position);
            }
            
            foreach(Elf elf in m_elfList)
            {
                elf.Consider(directionChoices,occupiedPositions);
            }

            // all happy stop
            int happyElves = m_elfList.FindAll(x=>x.HappyWhereIs).Count;
            
            if( happyElves != m_elfList.Count)
            {
                foreach(Elf elf in m_elfList)
                {
                    List<Elf> elfList;
                    if(!moveCounts.TryGetValue(elf.ProposedPosition,out elfList))
                    {
                        elfList = new List<Elf>();
                        moveCounts[elf.ProposedPosition] = elfList;
                    }
                    elfList.Add(elf);
                }
                int succesfulMoves = 0;
                foreach(IntVector2 pos in moveCounts.Keys)
                {
                    if(moveCounts[pos].Count == 1)
                    {
                        Elf elf = moveCounts[pos][0];
                        if(elf.ShouldMove && elf.ProposedPosition == pos)
                        {
                            //elf.Move(m_debugInfo);
                            elf.Move();
                            succesfulMoves++;
                        }
                    }
                }

                // rotate directions around.
                directionChoices.Add(directionChoices[0]);
                directionChoices.RemoveAt(0);
                numRounds++;
                
                // debug.
                m_debugInfo.Add("Round  : "+numRounds);
                DrawBoard();

                //if (numRounds > 5)
                //{
                //    keepGoing = false;
                //}
                if(numRounds == 10)
                {
                    int score = CalculateScore();
                    int ibreak = 0;
                }
            }
            else
            {
                keepGoing = false;
                DebugOutput($"Noone moved at round {numRounds}");
            }
        
            
        }
        double bpElapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        DebugOutput("Elapsed = " + bpElapsed + " ms");


        WriteDebugInfo();
    }

    public int CalculateScore()
    {
        HashSet<IntVector2> occupiedPositions = new HashSet<IntVector2>();
        foreach(Elf elf in m_elfList)
        {
            occupiedPositions.Add(elf.Position);
        }



        IntVector2 min = new IntVector2(int.MaxValue, int.MaxValue);    
        IntVector2 max = new IntVector2(int.MinValue,   int.MinValue);

        foreach(IntVector2 pos in occupiedPositions)
        {
            min.X = Math.Min(min.X,pos.X);
            min.Y = Math.Min(min.Y,pos.Y);

            max.X = Math.Max(max.X,pos.X);
            max.Y = Math.Max(max.Y,pos.Y);
        }

        int emptySpace = 0;

        for(int y = min.Y;y<=max.Y;y++)
        {
            for(int x = min.X;x<=max.X;x++)
            {
                if(!occupiedPositions.Contains(new IntVector2(x,y)))
                {
                    emptySpace++;
                }
            }
        }


        return emptySpace;
    }

    public void DrawBoard()
    {
        HashSet<IntVector2> occupiedPositions = new HashSet<IntVector2>();
        foreach(Elf elf in m_elfList)
        {
            occupiedPositions.Add(elf.Position);
        }

        m_debugInfo.Add(GenerateDebug(occupiedPositions));

    }

    public string GenerateDebug(HashSet<IntVector2> occupiedPositions)
    {
        IntVector2 min = new IntVector2(int.MaxValue, int.MaxValue);    
        IntVector2 max = new IntVector2(int.MinValue,   int.MinValue);

        foreach(IntVector2 pos in occupiedPositions)
        {
            min.X = Math.Min(min.X,pos.X);
            min.Y = Math.Min(min.Y,pos.Y);

            max.X = Math.Max(max.X,pos.X);
            max.Y = Math.Max(max.Y,pos.Y);
        }

        if(min.X > 0)
        {
            min.X = 0;
        }
        if(min.Y > 0)
        {
            min.Y = 0;
        }

        max.X = max.X + 2;
        max.Y = max.Y + 2;

        StringBuilder sb= new StringBuilder();

        for(int y = min.Y;y<max.Y;y++)
        {
            for(int x = min.X;x<max.X;x++)
            {
                sb.Append(occupiedPositions.Contains(new IntVector2(x,y))?"#":".");
            }
            sb.AppendLine("");
        }

        return sb.ToString();
    }

    public class Elf
    {
        public String Id;
        public IntVector2 Position{get;set; }
        public IntVector2 ProposedPosition{get;set; }

        public bool HappyWhereIs{get;set; }

        public void Consider(List<Direction> directionChoices,HashSet<IntVector2> occupiedPositions)
        {
            HappyWhereIs = true;
            ProposedPosition = Position;

            foreach(IntVector2 offset in DirectionMap.Values)
            {
                IntVector2 adjusted = Position+offset;
                if(occupiedPositions.Contains(adjusted))
                {
                    HappyWhereIs = false;
                    break;
                }
            }

            if(!HappyWhereIs)
            {
                foreach(Direction d in directionChoices)
                {
                    bool positionFree = true;
                    foreach(Direction r in RelativeDirectionMap[d])
                    {
                        IntVector2 adjustedPosition = Position + DirectionMap[r];
                        if(occupiedPositions.Contains(adjustedPosition))
                        {
                            // can't go here?
                            positionFree = false;
                            break;
                        }

                    }
                    if(positionFree)
                    {
                        ProposedPosition = Position+DirectionMap[d];
                        break;
                    }
                }
            }
        }

        public bool ShouldMove
        {
            get{return !HappyWhereIs; }
        }

        public void Move(List<string> debug = null)
        {
            if(debug != null)
            {
                debug.Add(String.Format($"Elf {Id} moving from {Position} to {ProposedPosition}"));
            }
            Position = ProposedPosition;
        }

    }


}

public enum Direction
{
    North,
    South,
    East,
    West,
    NorthEast,
    SouthEast,
    NorthWest,
    SouthWest
}
