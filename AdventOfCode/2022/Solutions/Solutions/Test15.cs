using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

public class Test15 : BaseTest
{
    Dictionary<LongVector2, LongVector2> SensorBeaconMap = new Dictionary<LongVector2, LongVector2>();
    HashSet<LongVector2> CoverageMap = new HashSet<LongVector2>();
    List<LongBounds> AllLongBounds = new List<LongBounds>();

    long m_minx = long.MaxValue;
    long m_miny = long.MaxValue;
    long m_maxx = long.MinValue;
    long m_maxy = long.MinValue;

    long ChosenLine = 10;//2000000;

    public override void RunTest()
    {
        TestID = 15;
        IsTestInput = true;
        IsPart2 = true;

        if (IsTestInput)
        {
            ChosenLine = 10;//10;
        }
        else
        {
            ChosenLine = 2000000;
        }

        ReadDataFile();

        foreach (string line in m_dataFileContents)
        {
            string[] tokens = line.Split(':');
            LongVector2 sensor = GetLongVector2(tokens[0]);
            LongVector2 beacon = GetLongVector2(tokens[1]);
            if (SensorBeaconMap.Values.Contains(beacon))
            {
                long ibreak = 0;
            }
            SensorBeaconMap[sensor] = beacon;
        }

        BuildBounds();
        if(IsPart2)
        {
            DoPart2();
        }
        else
        {
            DoPart1();
        }

        int c = CountBeaconsInRow((int)ChosenLine);
        if (IsTestInput)
        {
            DrawDebug();
        }

        WriteDebugInfo();
    }

    public void DoPart1()
    {
        long count = 0;
        foreach (LongVector2 sensor in SensorBeaconMap.Keys)
        {
            System.Console.WriteLine("Checking sensor " + sensor + "  count = " + (count++));

            long distance = sensor.ManhattanDistance(SensorBeaconMap[sensor]);

            LongVector2 start = new LongVector2(sensor.X - distance, sensor.Y - distance);
            LongVector2 end = new LongVector2(sensor.X + distance, sensor.Y + distance);

            // check and see if it hits our coverage line somehow. should limit data
            if (IsTestInput)
            {
                for (long y = (long)start.Y; y <= end.Y; ++y)
                {
                    for (long x = (long)start.X; x <= end.X; ++x)
                    {
                        LongVector2 v = new LongVector2(x, y);
                        if (v.ManhattanDistance(sensor) <= distance)
                        {
                            CoverageMap.Add(v);
                        }
                    }
                }
            }
            else
            {

                for (long x = (long)start.X; x <= end.X; ++x)
                {
                    LongVector2 v = new LongVector2(x, ChosenLine);
                    if (v.ManhattanDistance(sensor) <= distance)
                    {
                        CoverageMap.Add(v);
                    }
                }
            }

        }


        m_debugInfo.Add(String.Format("Line {0} has {1} blank spaces", ChosenLine, CountRow(ChosenLine)));

    }
    public void DoPart2()
    {   
        int min = 0;
        int max = IsTestInput?20:4000000;

        int foundx = -1;
        int foundy = -1;

        for(int y=min;y<=max;++y)
        {
            for(int x=min;x<=max;++x)
            {
                if(x== 11 && y==14)
                {
                    int ibreak2 = 0;
                }

                bool included = false;
                foreach(var alb in AllLongBounds)
                {
                    if(alb.Contains(x,y))
                    {
                        included = true;
                        break;
                    }
                }
                if(included == false)
                {
                    foundx = x;
                    foundy= y;
                    int ibreak = 0;
                }
            }
        }

    }


    public void BuildBounds()
    {
        foreach (var v in SensorBeaconMap.Keys)
        {
            LongVector2 destination = SensorBeaconMap[v];
            long x;
            long y;
            v.ManhattanDistance(destination,out x,out y);
            long distance = x+y;

            // need to add / sub distance from position

            m_minx = (long)Math.Min(m_minx, v.X - distance);
            m_miny = (long)Math.Min(m_miny, v.Y - distance);

            m_maxx = (long)Math.Max(m_maxx, v.X + distance);
            m_maxy = (long)Math.Max(m_maxy, v.Y + distance);

            LongBounds lb = new LongBounds(v, x,y);
            AllLongBounds.Add(lb);

        }

    }

    public int CountBeaconsInRow(int row)
    {
        HashSet<LongVector2> hash = new HashSet<LongVector2>();
        foreach (LongVector2 v in SensorBeaconMap.Values)
        {
            hash.Add(v);
        }
        return hash.Count(x => x.Y == row);
    }

    public long CountRow(long row)
    {
        long empty = 0;
        LongVector2 start = new LongVector2(m_minx, row);
        LongVector2 end = new LongVector2(m_maxx, row);
        long diff = m_maxx - m_minx;

        for (long i = 0; i < diff; ++i)
        {
            LongVector2 v = start + new LongVector2(i, 0);
            if (CharAtLocation(v) == '#')
            {
                empty++;
            }
        }


        return empty;
    }



    public void DrawDebug()
    {

        if (IsTestInput)
        {
            m_miny = -2;
            m_minx = -4;
        }

        long xrange = m_maxx - m_minx;
        long yrange = m_maxy - m_miny;

        xrange += 1;
        yrange += 1;

        string[] columns = new string[xrange];
        for (long i = 0; i < xrange; i++)
        {
            columns[i] = "" + (m_minx + i);
        }

        long xInset = 0;
        long digits = Math.Max(("" + m_maxx).Length, ("" + m_minx).Length);
        for (long y = 0; y < digits; y++)
        {
            StringBuilder header = new StringBuilder();
            for (long i = 0; i < xInset; ++i)
            {
                header.Append(" ");
            }

            for (long x = 0; x < xrange; x++)
            {
                if (y < columns[x].Length)
                {
                    header.Append(columns[x][(int)y]);
                }
                else
                {
                    header.Append(" ");
                }
            }
            m_debugInfo.Add(header.ToString());
        }

        for (long y = 0; y < yrange; y++)
        {
            StringBuilder data = new StringBuilder();
            for (long x = 0; x < xrange; x++)
            {
                if (x == 0)
                {
                    string xval = ("" + (m_miny + y));
                    data.Append(xval);
                    long inset = 4 - xval.Length;
                    for (long i = 0; i < inset; ++i)
                    {
                        data.Append(' ');
                    }

                }
                LongVector2 v = new LongVector2(m_minx + x, m_miny + y);
                char c = CharAtLocation(v);
                data.Append(c);

            }
            m_debugInfo.Add(data.ToString());
        }


    }


    public LongVector2 GetLongVector2(string val)
    {
        string pattern = @"[\+-]?\d+";

        Regex r1 = new Regex(pattern);

        string[] split = val.Split(",");
        Match m1 = r1.Match(split[0]);
        string xstr = "";
        while (m1.Success)
        {
            xstr += m1.Value;
            m1 = m1.NextMatch();
        }


        Match m2 = r1.Match(split[1]);
        string ystr = "";
        while (m2.Success)
        {
            ystr += m2.Value;
            m2 = m2.NextMatch();
        }

        return new LongVector2(long.Parse(xstr), long.Parse(ystr));
    }


    public char CharAtLocation(LongVector2 v)
    {
        if (SensorBeaconMap.Keys.Contains(v))
        {
            return 'S';
        }
        if (SensorBeaconMap.Values.Contains(v))
        {
            return 'B';
        }
        if (CoverageMap.Contains(v))
        {
            return '#';
        }

        return '.';
    }
}