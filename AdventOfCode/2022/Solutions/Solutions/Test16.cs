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


    Graph<string> StringGraph= new Graph<string>();
    List<Path<string>> AllPaths = new List<Path<string>>();

    public override void RunTest()
    {
        TestID = 16;
        IsTestInput = true;
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
                v.Links.Add(link.Trim());

            }
            AllValves.Add(v);
            ValveLookup[valveId] = v;

        }
        int ibreak = 0;


        foreach(Valve v in AllValves)
        {
            StringGraph.AddVertex(v.Id);
            foreach(string link in v.Links)
            {
                StringGraph.AddEdge(new Tuple<string,string>(v.Id, link));
                AllPaths.Add(new Path<string>(){Source = v.Id, Destination=link,Cost=1 });
            }
        }
        

        
        Simulate();
        WriteDebugInfo();

    }


    public void Simulate()
    {
        Random random = new Random();

        //var Results = Engine.CalculateShortestPathBetween("AA","JJ",AllPaths);


        //ShortestPath("AA","DD");

        CurrentValve = ValveLookup["AA"];
        int total = 0;
        while(UsedTime < MaxTime) 
        {
            Valve bestMove = GetBestClosedValve();

            Valve localMove = null;
            int bestFlow = 0;
            
            GetBestClosedValveLocal(CurrentValve,2,ref localMove,ref bestFlow);

            
            Valve move = null;
            if(bestMove != null && localMove != null)
            {
                int bestDistance = Engine.CalculateShortestPathBetween(CurrentValve.Id,bestMove.Id,AllPaths).Count;
                int localDistance = Engine.CalculateShortestPathBetween(CurrentValve.Id,localMove.Id,AllPaths).Count;

                //move = localMove.FlowRate+distance > bestMove.FlowRate? localMove: bestMove;
                //move = localMove.FlowRate> bestMove.FlowRate? localMove: bestMove;
                int localCost = (MaxTime-UsedTime-localDistance) * localMove.FlowRate;
                int bestCost = (MaxTime-UsedTime-bestDistance) * bestMove.FlowRate;

                move = localCost > bestCost ? localMove : bestMove;
            }

            if(move != null)
            {
                var results = Engine.CalculateShortestPathBetween(CurrentValve.Id,move.Id,AllPaths);
                if(results != null)
                {
                    foreach(Path<string> path in results)
                    {
                        TravelToValve(ValveLookup[path.Destination]);
                        if(!CurrentValve.Open && CurrentValve.FlowRate > 0)
                        {
                            OpenCurrentValve();
                        }
                    }
                }
            }
            else
            {
                Valve destination = ValveLookup[CurrentValve.Links[random.Next(CurrentValve.Links.Count)]];
                TravelToValve(destination);
            }
        }
        m_debugInfo.Add("Total flow was "+TotalFlow);
    }

    public void TravelToValve(Valve move)
    {
        m_debugInfo.Add("Moving from "+CurrentValve.Id+" to "+move.Id);
        CurrentValve = move;
        AdvanceTime();

    }

    public void AdvanceTime()
    {
        UsedTime++;
        TotalFlow += GetFlow();
    }

    public void OpenCurrentValve()
    {
        m_debugInfo.Add("Opening valve "+CurrentValve.Id);
        Debug.Assert(CurrentValve != null && CurrentValve.Open == false);
        CurrentValve.Open = true;
        AdvanceTime();
     
    }

    // need to get cleverer to search multiple links to find highest unopened with shortest route.
    public void GetBestClosedValveLocal(Valve current ,int depth,ref Valve bestValve,ref int bestFlow)
    {
        depth--;
        if(!current.Open && current.FlowRate > bestFlow)
        {
            bestValve= current;
            bestFlow= current.FlowRate;
        }

        if(depth > 0)
        {
            foreach(string link in current.Links)
            {
                Valve v = ValveLookup[link];
                GetBestClosedValveLocal(ValveLookup[link],depth,ref bestValve,ref bestFlow);
            }
        }
    }



    public Valve GetBestClosedValve()
    {
        var test = AllValves.FindAll(x=> !x.Open && x.FlowRate > 0).OrderByDescending(x=>x.FlowRate);
        return test.FirstOrDefault();
    }


    public int GetFlow()
    {
        int total = 0;
        foreach(Valve v in AllValves)
        {
            if(v.Open)
            {
                //total += (v.FlowRate * (MaxTime-UsedTime));
                total += v.FlowRate;
            }
        }
        return total;
    }

    public List<string> ShortestPath(string id,string target)
    {
        List<Valve> visited =  new List<Valve>();
        List<string> results = new List<string>();
        Stack<string> path = new Stack<string>();

        Valve v = ValveLookup[id];
        path.Push(id);
        Follow(v,visited,path,target);
        return results;
    }

    public void Follow(Valve v,List<Valve> visited,Stack<string> path,string target)
    {
        path.Push(v.Id);

        if(v.Id == target)
        {
            return ;
        }

        foreach(string linkid in v.Links)
        {
            Valve linkValve = ValveLookup[linkid];
            if(!visited.Contains(linkValve))
            {
                visited.Add(linkValve);
                Follow(linkValve,visited,path,target);
            }
        }
    }
}

public class Valve
{
    public string Id;
    public int FlowRate;
    public List<string> Links = new List<string>();
    public bool Open = false;
}



