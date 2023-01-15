using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using static Test19;

public class Test19 : BaseTest
{
    private List<BluePrint> m_blueprints = new List<BluePrint>();
    private List<SimulationResult> m_simulationResults = new List<SimulationResult>();

    public int m_totalTime = 24;
    public int m_currentTime = 0;

    public int[] m_currentLimits = new int[(int)RobotType.NumValues];

    public override void RunTest()
    {
        DateTime startTime = DateTime.Now;

        TestID = 19;
        IsTestInput = true;
        IsPart2 = false;
        ReadDataFile();


        string numberPattern = @"\.*[\+-]?\d+\.*";

        Regex r1 = new Regex(numberPattern);
        foreach (string line in m_dataFileContents)
        {
            Match m1 = r1.Match(line);
            string blueprintIdStr = m1.Value;
            m1 = m1.NextMatch();

            string oreRobotCost = m1.Value;
            m1 = m1.NextMatch();

            string clayRobotCost = m1.Value;
            m1 = m1.NextMatch();

            string obsidianRobotOreCost = m1.Value;
            m1 = m1.NextMatch();

            string obsidianRobotClayCost = m1.Value;
            m1 = m1.NextMatch();

            string geodeRobotOreCost = m1.Value;
            m1 = m1.NextMatch();

            string geodeRobotObsidianCost = m1.Value;


            BluePrint blueprint = new BluePrint(int.Parse(blueprintIdStr), int.Parse(oreRobotCost),
                int.Parse(clayRobotCost), int.Parse(obsidianRobotOreCost), int.Parse(obsidianRobotClayCost),
                int.Parse(geodeRobotOreCost), int.Parse(geodeRobotObsidianCost));

            m_blueprints.Add(blueprint);
        }

        int[] robots = new int[(int)RobotType.NumValues];
        robots[(int)RobotType.Ore] = 1;
        int[] inventory = new int[(int)RobotType.NumValues];

        int ibreak = 0;

        m_currentLimits[(int)RobotType.Ore] = 8;
        m_currentLimits[(int)RobotType.Clay] = 5;

        //foreach (BluePrint bp in m_blueprints)
        //{
        //    int[] limits = new int[] { 1, 8, 0, 0 };
        //    m_simulationResults.Add(RunSimulation(bp, limits, true));

        //    int timeForClay = bp.ClayRobotCost;
        //    int timeForObsidian = bp.ObsidianRobotClayCost * timeForClay;
        //    int timeForGeode = bp.GeodeRobotObsidianCost * timeForObsidian;

        //    m_debugInfo.Add(String.Format("Blueprint {0} clay {1} obsidian {2} geode {3}", bp.Id, timeForClay, timeForObsidian, timeForGeode));
        //    break;
        //}


        int totalScore = 0;
        foreach (BluePrint bp in m_blueprints)
        {
            DateTime bpTime = DateTime.Now;
            Result startResult = new Result();
            startResult.BluePrint = bp;
            startResult.Robots[(int)RobotType.Ore] = 1;

            var bestResult = DoStep(startResult);
            DebugOutput(SummariseBluePrint(bp));
            DebugOutput("Best result blueprint " + bp.Id + " : " + bestResult.Inventory[(int)RobotType.Geode] + "  :  " + string.Join(",",bestResult.Route));
            double bpElapsed = DateTime.Now.Subtract(bpTime).TotalMilliseconds;
            DebugOutput("Elapsed = " + bpElapsed + " ms");
            totalScore += (bestResult.Inventory[(int)RobotType.Geode] * bp.Id);
        }

        //RunSimulation(best.BluePrint,best.Limits,true);
        double elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        DebugOutput("\nElapsed = "+elapsed+" ms");
        DebugOutput("Total score : "+totalScore);

        WriteDebugInfo();
    }

    public void DebugOutput(string s)
    {
        m_debugInfo.Add(s);
        System.Console.WriteLine(s);
    }

    public string SummariseBluePrint(BluePrint bp)
    {
        int timeForClay = bp.ClayRobotCost;
        int timeForObsidian = bp.ObsidianRobotClayCost * timeForClay;
        int timeForGeode = bp.GeodeRobotObsidianCost * timeForObsidian;

        return string.Format("Blueprint {0} clay {1} obsidian {2} geode {3}",bp.Id,timeForClay,timeForObsidian,timeForGeode);
    }

    public SimulationResult RunSimulation(BluePrint bluePrint, int[] limits, bool debugInfo)
    {
        m_currentTime = 0;
        RobotType[] preferred = new RobotType[] { RobotType.Geode, RobotType.Obsidian, RobotType.Clay, RobotType.Ore };

        SimulationResult simulationResult = new SimulationResult(bluePrint, debugInfo ? m_debugInfo : null);

        for (int i = 0; i < limits.Length; ++i)
        {
            if (limits[i] > 0)
            {
                simulationResult.SetLimit((RobotType)i, limits[i]);
            }
        }

        if (debugInfo)
        {
            m_debugInfo.Add("===========================BluePrint " + bluePrint.Id + " : " + string.Join(", ", limits) + "===========================");
        }

        while (m_currentTime < m_totalTime)
        {
            if (debugInfo)
            {
                m_debugInfo.Add("\n== Minute " + (m_currentTime + 1) + " ==");
            }

            int[] projectedInventory = simulationResult.ProjectedInventory(2);

            foreach (RobotType preferredType in preferred)
            {
                if (!simulationResult.CanBuild(preferredType, simulationResult.Inventory) && simulationResult.CanBuild(preferredType, projectedInventory))
                {
                    break;
                }

                if (simulationResult.ShouldBuild(preferredType))
                {
                    if (simulationResult.StartBuildRobot(preferredType))
                    {
                        break;
                    }
                }
            }

            simulationResult.UpdateInventory();
            simulationResult.CompleteBuild();

            m_currentTime++;
        }
        return simulationResult;
    }



    public static List<Result> ScratchList = new List<Result>();

    private int m_highestResult = 100;

    public Result DoStep(Result input)
    {
        if (input.Time <= m_totalTime)
        {
            //System.Console.WriteLine("Doing action "+input.action+" at time "+input.time);
            Result maxResult = input;

            for (int i = 0; i < (int)RobotType.NumValues; i++)
            {
                if (m_currentLimits[i] != 0 && input.Robots[i] >= m_currentLimits[i])
                {
                    continue;
                }

                
                var subStep = SubStep(input,(RobotType)i);
                if (subStep != null )
                {
                    var result = DoStep(subStep);

                    if (result != null )     
                    {
                        if (maxResult.Inventory[(int)RobotType.Geode] < result.Inventory[(int)RobotType.Geode])
                        {
                            maxResult = result;
                            if(result.Time < m_highestResult)
                            {
                                m_highestResult = result.Time;
                                //System.Console.WriteLine("Have result for time "+maxResult.time+"  :  "+maxResult.inventory[(int)RobotType.Geode]+"  :  "+maxResult.route);
                            }
                        }
                    }
                }
                else
                {
                    int ibreak = 0;
                }

            }

            return maxResult;
        }
        else
        {
            return null ;
        }
    }



    public Result SubStep(Result input,RobotType action)
    {
        // 
        //if(input.route.StartsWith("WaitWaitWaitWaitWaitWaitWaitWaitWaitWait"))
        //{
        //    return null ;
        //}

        //if(action == RobotType.Ore && input.Time > 12)
        //{
        //    return null;
        //}
            
        //if(action == RobotType.Clay && input.Time > 18)
        //{
        //    return null;
        //}


        bool canBuild = false;
        int[] costs = input.BluePrint.Costs[action];

        if (action != RobotType.Wait)
        {
            bool valid = true;
            for (int i = 0; i < input.Inventory.Length; i++)
            {
                if (input.Inventory[i] < costs[i])
                {
                    valid = false;
                    break;
                }
            }
            canBuild = valid;
        }

        if(canBuild && (action == RobotType.Geode || action == RobotType.Obsidian ))
        {
            int ibreak =0 ;
        }

        if (canBuild || action == RobotType.Wait)
        {
            Result newResult = input.Copy(action);

            if (canBuild)
            {
                for (int i = 0; i < newResult.Inventory.Length; i++)
                {
                    newResult.Inventory[i] -= costs[i];
                }
            }

            for (int i = 0; i < newResult.Inventory.Length; i++)
            {
                newResult.Inventory[i] += newResult.Robots[i];
            }

            if (canBuild)
            {
                newResult.Robots[(int)action]++;
            }

            newResult.Time += 1;
            return newResult;

        }
        return null ;
    }


    public class SimulationResult
    {
        public BluePrint BluePrint;


        private int[] m_robots = new int[(int)RobotType.NumValues];
        private int[] m_robotLimits = new int[(int)RobotType.NumValues];
        private int[] m_inventory = new int[(int)RobotType.NumValues];
        private bool[] m_building = new bool[(int)RobotType.NumValues];


        private List<string> m_debugInfo;

        public SimulationResult(BluePrint blueprint, List<string> debugInfo)
        {
            BluePrint = blueprint;
            m_robots[(int)RobotType.Ore] = 1;
            m_debugInfo = debugInfo;
        }

        public int Score
        { get { return m_inventory[(int)RobotType.Geode]; } }

        public int[] Limits
        { get { return m_robotLimits; } }

        public void UpdateInventory()
        {
            for (int i = 0; i < m_robots.Length; ++i)
            {
                m_inventory[i] += m_robots[i];
            }
            if (m_debugInfo != null)
            {
                m_debugInfo.Add(DebugSummary);
            }
        }

        public void SetLimit(RobotType type, int limit)
        {
            m_robotLimits[(int)type] = limit;
        }

        public bool ShouldBuild(RobotType type)
        {
            bool shouldBuild = m_robotLimits[(int)type] == 0;
            if (m_robotLimits[(int)type] != 0)
            {
                shouldBuild = m_robots[(int)type] < m_robotLimits[(int)type];
            }
            return shouldBuild;
        }

        public int[] Inventory { get { return m_inventory; } }

        public int[] ProjectedInventory(int rounds)
        {
            int[] projected = new int[m_inventory.Length];
            for (int i = 0; i < projected.Length; ++i)
            {
                projected[i] = m_inventory[i] + (m_robots[i] * rounds);
            }
            return projected;
        }


        public bool CanBuild(RobotType type, int[] inventory)
        {
            int[] costs = BluePrint.Costs[type];
            bool success = true;
            for (int i = 0; i < costs.Length; ++i)
            {
                if (costs[i] > 0 && inventory[i] < costs[i])
                {
                    success = false;
                    break;
                }
            }
            return success;
        }

        public bool StartBuildRobot(RobotType type)
        {
            bool canBuild = CanBuild(type, m_inventory);
            if (!canBuild)
            {
                return false;
            }


            m_building[(int)type] = true;

            int[] costs = BluePrint.Costs[type];


            if (m_debugInfo != null)
            {
                string robotCost = "";
                for (int i = 0; i < costs.Length; ++i)
                {
                    if (costs[i] > 0)
                    {
                        robotCost += costs[i] + " " + ((RobotType)i) + " ";
                    }
                }

                m_debugInfo.Add(string.Format("Spend {0} to start building a {1}-collecting robot.", robotCost, type));
            }


            for (int i = 0; i < costs.Length; i++)
            {
                m_inventory[i] -= costs[i];
            }

            return true;
        }

        public void CompleteBuild()
        {
            for (int i = 0; i < m_building.Length; ++i)
            {
                if (m_building[i])
                {
                    m_robots[i]++;
                    if (m_debugInfo != null)
                    {
                        m_debugInfo.Add(string.Format("The new {0}-collecting robot is ready. You now have {1} of them.", (RobotType)i, m_robots[i]));
                    }
                }
                m_building[i] = false;
            }
        }


        public String DebugSummary
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < m_inventory.Length; ++i)
                {
                    if (m_inventory[i] > 0)
                    {
                        string line = string.Format("{0} {1}-collecting robot collects {0} {1} . You now have {2} {1}", m_robots[i], "" + Enum.GetName(typeof(RobotType), i), m_inventory[i]);
                        sb.AppendLine(line);
                    }
                }
                return sb.ToString();
            }
        }

    }


    public class Result
    {
        public BluePrint BluePrint;
        public int[] Inventory =  new int[(int)RobotType.NumValues];
        public int[] Robots =  new int[(int)RobotType.NumValues];
        public int Time;
        public List<RobotType> Route = new List<RobotType>();

        public Result Copy(RobotType action)
        {
            Result result = new Result();
            result.BluePrint = BluePrint;
            Array.Copy(Inventory,result.Inventory,Inventory.Length);
            Array.Copy(Robots,result.Robots,Robots.Length);
            result.Time = Time;
            result.Route.AddRange(Route);
            result.Route.Add(action);
            return result;
        }

    }


    public record BluePrint
    {
        public int Id;
        public int OreRobotCost;
        public int ClayRobotCost;
        public int ObsidianRobotOreCost;
        public int ObsidianRobotClayCost;
        public int GeodeRobotOreCost;
        public int GeodeRobotObsidianCost;

        public Dictionary<RobotType,int[]> Costs = new Dictionary<RobotType, int[]>();

        public BluePrint(int id, int oreRobotCost, int clayRobotCost, int obsidianRobotOreCost,
                        int obsidianRobotClayCost, int geodeRobotOreCost, int geodeRobotObsidianCost)
        {
            Id = id;
            OreRobotCost = oreRobotCost;
            ClayRobotCost = clayRobotCost;
            ObsidianRobotOreCost = obsidianRobotOreCost;
            ObsidianRobotClayCost = obsidianRobotClayCost;
            GeodeRobotOreCost = geodeRobotOreCost;
            GeodeRobotObsidianCost = geodeRobotObsidianCost;

            Costs[RobotType.Wait] = new int[(int)RobotType.NumValues];
            Costs[RobotType.Ore] = new int[(int)RobotType.NumValues];
            Costs[RobotType.Clay] = new int[(int)RobotType.NumValues];
            Costs[RobotType.Obsidian] = new int[(int)RobotType.NumValues];
            Costs[RobotType.Geode] = new int[(int)RobotType.NumValues];

            Costs[RobotType.Ore][(int)RobotType.Ore] = oreRobotCost;

            Costs[RobotType.Clay][(int)RobotType.Ore] = clayRobotCost;
            
            Costs[RobotType.Obsidian][(int)RobotType.Ore] = obsidianRobotOreCost;
            Costs[RobotType.Obsidian][(int)RobotType.Clay] = obsidianRobotClayCost;

            Costs[RobotType.Geode][(int)RobotType.Ore] = geodeRobotOreCost;
            Costs[RobotType.Geode][(int)RobotType.Obsidian] = geodeRobotObsidianCost;



        }

        public void GetCost2(RobotType type, int[] values)
        {
            Array.Fill(values,0);

            if (type == RobotType.Ore)
            {
                values[(int)RobotType.Ore] = OreRobotCost;
            }
            if (type == RobotType.Clay)
            {
                values[(int)RobotType.Ore] = ClayRobotCost;
            }
            if (type == RobotType.Obsidian)
            {
                values[(int)RobotType.Ore] = ObsidianRobotOreCost;
                values[(int)RobotType.Clay] = ObsidianRobotClayCost;
            }
            if (type == RobotType.Geode)
            {
                values[(int)RobotType.Ore] = GeodeRobotOreCost;
                values[(int)RobotType.Obsidian] = GeodeRobotObsidianCost;
            }
        }


    }


}



public enum RobotType
{
    Wait,
    Ore,
    Clay,
    Obsidian,
    Geode,
    NumValues
};