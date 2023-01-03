using DijkstraAlgorithm;
using System.Diagnostics;
using System.Text.RegularExpressions;

public class Test16 : BaseTest
{
    public List<Valve> AllValves = new List<Valve>();
    public Dictionary<string, Valve> ValveLookup = new Dictionary<string, Valve>();

    public Valve CurrentValve;

    public int MaxTime = 30;
    public int UsedTime = 0;
    public int TotalFlow = 0;
    public int HighScore = 0;


    Graph<string> StringGraph = new Graph<string>();
    List<Path<string>> AllPaths = new List<Path<string>>();

    public override void RunTest()
    {
        TestID = 16;
        IsTestInput = false;
        IsPart2 = true;

        ReadDataFile();
        //Valve AA has flow rate=0; tunnels lead to valves DD, II, BB
        foreach (string line in m_dataFileContents)
        {
            string[] tokens = line.Split(';');
            string[] lhsTokens = tokens[0].Split(new char[] { ' ', '=' });

            string valveId = lhsTokens[1];
            int flowRate = int.Parse(lhsTokens[5]);

            string rhs = tokens[1].Replace("tunnels lead to valves", "");
            rhs = rhs.Replace("tunnel leads to valve", "");

            string[] rhsTokens = rhs.Split(',');

            Valve v = new Valve() { Id = valveId, FlowRate = flowRate };
            foreach (string link in rhsTokens)
            {
                v.StringLinks.Add(link.Trim());

            }
            AllValves.Add(v);
            ValveLookup[valveId] = v;

        }
        int ibreak = 0;


        foreach (Valve v in AllValves)
        {
            StringGraph.AddVertex(v.Id);
            foreach (string link in v.StringLinks)
            {
                StringGraph.AddEdge(new Tuple<string, string>(v.Id, link));
                AllPaths.Add(new Path<string>() { Source = v.Id, Destination = link, Cost = 1 });

                v.ValveLinks.Add(ValveLookup[link]);

            }
        }

        //Valve[] valves = AllValves.FindAll(x=>x.FlowRate > 0).ToArray();
        //string[] strings = new string[valves.Length];
        //for(int i=0;i<valves.Length;i++)
        //{
        //    strings[i] = valves[i].Id;
        //}
        ////foreach(string[] combo in Combinations.CombinationsRosettaWoRecursion<string>(strings,3))
        //foreach(var combo in Combinations.Permute<string>(strings))
        //{
        //    m_debugInfo.Add(string.Join(", ", combo.ToArray()));
        //    int ibreak2 =0;
        //}


        MaxTime = IsPart2?26:30;

        Simulate2();
        WriteDebugInfo();

    }

    public void Simulate2()
    {
        Search(ValveLookup["AA"], AllValves.FindAll(x => x.FlowRate > 0), (MaxTime, 0),!IsPart2);
        m_debugInfo.Add("High score was : "+HighScore);
        int ibreak = 0;
    }


    public void Simulate()
    {
        // could brute force every option in terms of opening to find best...


        Random random = new Random();
        CurrentValve = ValveLookup["AA"];

        while (UsedTime < MaxTime)
        {
            Valve move = null;
            int bestFlow = 0;
            foreach (Valve v in AllValves)
            {
                if (v == CurrentValve)
                {
                    continue;
                }

                if (!v.Open)
                {
                    int distance = Engine.CalculateShortestPathBetween(CurrentValve.Id, v.Id, AllPaths).Count;

                    // don't go to valves that we can't reach in time.
                    if (!(UsedTime + distance + 1 > MaxTime))
                    {
                        int cost = (MaxTime - UsedTime - (distance + 1)) * v.FlowRate;
                        if (cost > bestFlow)
                        {
                            move = v;
                            bestFlow = cost;
                        }
                    }
                }

            }

            if (move != null)
            {
                var results = Engine.CalculateShortestPathBetween(CurrentValve.Id, move.Id, AllPaths);
                if (results != null)
                {
                    foreach (Path<string> path in results)
                    {
                        TravelToValve(ValveLookup[path.Destination]);
                    }
                    OpenCurrentValve();
                }
            }
            else
            {
                Valve destination = CurrentValve.ValveLinks[random.Next(CurrentValve.ValveLinks.Count)];
                TravelToValve(destination);
            }
        }
        m_debugInfo.Add("Total flow was " + TotalFlow);
    }



    public (int Time, int Score) Move(Valve from, Valve to, (int Time, int Score) previous)
    {
        int distance = GetDistance(from,to);
        int endTime = previous.Time - distance - 1;
        return (endTime, previous.Score + (endTime * to.FlowRate));
    }

    private Dictionary<Tuple<Valve,Valve>,int> DistanceCache = new Dictionary<Tuple<Valve, Valve>, int>();
    public int GetDistance(Valve from, Valve to)
    {
        Tuple<Valve,Valve> t = new Tuple<Valve, Valve>(from,to);
        int distance = 0;
        if(!DistanceCache.TryGetValue(t,out distance))
        {
            distance = Engine.CalculateShortestPathBetween(from.Id, to.Id, AllPaths).Count;
            DistanceCache[t] = distance;
        }
        return distance;
    }

    public void Search(Valve from, List<Valve> to, (int Time, int Score) previous,bool elephant)
    {
        foreach (Valve dest in to)
        {
            var result = Move(from, dest, previous);
            if (result.Time >= 0)
            {
                if (result.Score > HighScore)
                {
                    HighScore = result.Score;
                }
                
         
                if(to.Count > 1)
                {
                    Search(dest, to.FindAll(x => x.FlowRate > 0 && x != dest), result,elephant);
                }
            }
            else if(!elephant && previous.Score >= HighScore / 2)
            {
                Search(ValveLookup["AA"],to,(26,previous.Score),true);
            }
        }
    }


    //public (int Time,int Score) Test(Valve valve,(int Time,int Score) previous)
    //{
    //    int score = 0;

    //    foreach(Valve v in AllValves.FindAll(x=>x.FlowRate > 0 && x.Id != valve.Id))
    //    {
    //        int distance = Engine.CalculateShortestPathBetween(valve.Id, v.Id, AllPaths).Count;

    //        calcScore;
    //        if(score > maxScore)
    //        {
    //            preserve state;
    //        }
    //        Test(v);
    //    }
    //}







    public void TravelToValve(Valve move)
    {
        m_debugInfo.Add("Moving from " + CurrentValve.Id + " to " + move.Id);
        CurrentValve = move;
        AdvanceTime();

    }

    public void AdvanceTime()
    {
        UsedTime++;
        if (UsedTime < MaxTime)
        {
            TotalFlow += GetFlow();
        }
    }

    public void OpenCurrentValve()
    {
        m_debugInfo.Add("Opening valve " + CurrentValve.Id);
        Debug.Assert(CurrentValve != null && CurrentValve.Open == false);
        CurrentValve.Open = true;
        AdvanceTime();

    }

    // need to get cleverer to search multiple links to find highest unopened with shortest route.
    public void GetBestClosedValveLocal(Valve current, int depth, ref Valve bestValve, ref int bestFlow)
    {
        depth--;
        if (!current.Open && current.FlowRate > bestFlow)
        {
            bestValve = current;
            bestFlow = current.FlowRate;
        }

        if (depth > 0)
        {
            foreach (Valve v in current.ValveLinks)
            {
                GetBestClosedValveLocal(v, depth, ref bestValve, ref bestFlow);
            }
        }
    }



    public Valve GetBestClosedValve()
    {
        var test = AllValves.FindAll(x => !x.Open && x.FlowRate > 0).OrderByDescending(x => x.FlowRate);
        return test.FirstOrDefault();
    }


    public int GetFlow()
    {
        int total = 0;
        foreach (Valve v in AllValves)
        {
            if (v.Open)
            {
                total += v.FlowRate;
            }
        }
        return total;
    }

    public List<string> ShortestPath(string id, string target)
    {
        List<Valve> visited = new List<Valve>();
        List<string> results = new List<string>();
        Stack<string> path = new Stack<string>();

        Valve v = ValveLookup[id];
        path.Push(id);
        Follow(v, visited, path, target);
        return results;
    }

    public void Follow(Valve v, List<Valve> visited, Stack<string> path, string target)
    {
        path.Push(v.Id);

        if (v.Id == target)
        {
            return;
        }

        foreach (Valve linkValve in v.ValveLinks)
        {
            if (!visited.Contains(linkValve))
            {
                visited.Add(linkValve);
                Follow(linkValve, visited, path, target);
            }
        }
    }
}

public class Valve
{
    public string Id;
    public int FlowRate;
    public List<string> StringLinks = new List<string>();
    public List<Valve> ValveLinks = new List<Valve>();
    public bool Open = false;
}



