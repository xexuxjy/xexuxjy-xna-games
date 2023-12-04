using System.Numerics;

public class Test3 : BaseTest
{
    public override void RunTest()
    {
        TestID = 3;
        IsTestInput = false;
        IsPart2 = true;

        ReadDataFile();


        List<IntVector2> symbolPositions = new List<IntVector2>();
        List<IntVector2> gearPositions = new List<IntVector2>();
        List<IntVector3> numberPositions = new List<IntVector3>();

        Dictionary<IntVector2, List<IntVector3>> gearNumbersDictionary = new Dictionary<IntVector2, List<IntVector3>>();

        for (int y = 0; y < m_dataFileContents.Count; ++y)
        {
            string currentNumber = "";
            for (int x = 0; x < m_dataFileContents[y].Length; ++x)
            {
                char c = m_dataFileContents[y][x];
                if (Char.IsDigit(c))
                {
                    currentNumber += c;
                }
                else
                {
                    if (currentNumber != "")
                    {
                        IntVector3 number = new IntVector3(x - currentNumber.Length, y, int.Parse(currentNumber));
                        numberPositions.Add(number);
                        currentNumber = "";
                    }

                    if (c != '.')
                    {
                        symbolPositions.Add((new IntVector2(x, y)));
                        if (c == '*')
                        {
                            gearPositions.Add(new IntVector2(x,y));
                        }
                    }
                }
            }

            // deal with numbers at end of the line
            if (currentNumber != "")
            {
                IntVector3 number = new IntVector3(m_dataFileContents[y].Length - currentNumber.Length, y,
                    int.Parse(currentNumber));
                numberPositions.Add(number);
                currentNumber = "";
            }
        }

        List<IntVector3> validNumberPositions = new List<IntVector3>();
        foreach (IntVector3 numberPosition in numberPositions)
        {
            foreach (IntVector2 symbolPosition in symbolPositions)
            {
                //ypos can only work if within + or - 1 of symbol pos.
                if (InRange(symbolPosition, numberPosition))
                {
                    validNumberPositions.Add((numberPosition));
                    break;
                }
            }
        }

        int total = 0;
        foreach (IntVector3 validNumberPosition in validNumberPositions)
        {
            total += validNumberPosition.Z;
        }


        
        int gearTotal = 0;
        if (IsPart2)
        {
            foreach (IntVector2 gearPosition in gearPositions)
            {
                List<IntVector3> l = new List<IntVector3>();
                gearNumbersDictionary[gearPosition] = l;
                foreach (IntVector3 numberPosition in validNumberPositions)
                {
                    if (InRange(gearPosition, numberPosition))
                    {
                        l.Add((numberPosition));
                    }
                }
            }

            foreach (IntVector2 gearPosition in gearNumbersDictionary.Keys)
            {
                List<IntVector3> l = gearNumbersDictionary[gearPosition];
                if (l.Count == 2)
                {
                    gearTotal += (l[0].Z * l[1].Z);
                }
            }

        }

        DebugOutput("Total is : " + total);
        DebugOutput("Gear Total is "+gearTotal);
    }

    private bool InRange(IntVector2 symbolPosition, IntVector3 numberPosition)
    {
        return (Math.Abs(symbolPosition.Y - numberPosition.Y) <= 1) && (symbolPosition.X >= numberPosition.X - 1 &&
                                                                        symbolPosition.X <=
                                                                        (numberPosition.X + numberPosition.Z.ToString()
                                                                            .Length));
    }

    public bool IsSymbol(char c)
    {
        return (c == '#' || c == '$' || c == '+' || c == '*');
    }
}