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
        IsTestInput = false;
        IsPart2 = false;
        ReadDataFile();

        BuildBluePrints();

        int totalScore = IsPart2?1:0;
        m_totalTime = IsPart2?32:24;

        Task<int>[] allTasks = new Task<int>[m_blueprints.Count];
        int count = 0;
        foreach (BluePrint bp in m_blueprints)
        {
            allTasks[count] = new Task<int>(() =>
            {
                Console.WriteLine("Task on thread {0} started.", Thread.CurrentThread.ManagedThreadId);

                DateTime bpTime = DateTime.Now;
                Result startResult = new Result();
                startResult.BluePrint = bp;
                startResult.Robots[(int)RobotType.Ore] = 1;
                startResult.Cache = new Dictionary<Result, Result>();

                int javaScore = GetMaxGeodes(bp, m_totalTime);
                var bestResult = DoStep(startResult);
                int score = 0;
                if (bestResult != null)
                {
                    score = bestResult.Inventory[(int)RobotType.Geode];
                }


                DebugOutput(SummariseBluePrint(bp));
                DebugOutput("Best result blueprint " + bp.Id + " : " + score);
                double bpElapsed = DateTime.Now.Subtract(bpTime).TotalMilliseconds;
                DebugOutput("Elapsed = " + bpElapsed + " ms");
                Console.WriteLine("Task on thread {0} completed.", Thread.CurrentThread.ManagedThreadId);
                if(IsPart2)
                {
                    return javaScore;
                }
                else
                {
                    return (javaScore * bp.Id);
                }
            });


            allTasks[count].Start();
            count++;

        }
        Task.WaitAll(allTasks);
        foreach (Task<int> t in allTasks)
        {
            if(IsPart2)
            {
                totalScore *= t.Result;
            }
            else
            {
                totalScore += t.Result;
            }
        }

        //foreach (BluePrint bp in m_blueprints)
        //{
        //    int score = GetMaxGeodes(bp, 24);
        //    int ibreak2 = 0;
        //    int scoreF
        //}


        //RunSimulation(best.BluePrint,best.Limits,true);
        double elapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        DebugOutput("\nElapsed = " + elapsed + " ms");
        DebugOutput("Total score : " + totalScore);

        WriteDebugInfo();
    }


    public void BuildBluePrints()
    {
        int lineCount = IsPart2?3:m_dataFileContents.Count;
        
        string numberPattern = @"\.*[\+-]?\d+\.*";

        Regex r1 = new Regex(numberPattern);
        foreach (string line in m_dataFileContents)
        {
            if (line.StartsWith("#"))
            {
                continue;
            }
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
            lineCount--;
            if(lineCount == 0)
            {
                break;
            }

        }
    }

    public string SummariseBluePrint(BluePrint bp)
    {
        int timeForClay = bp.ClayRobotCost;
        int timeForObsidian = bp.ObsidianRobotClayCost * timeForClay;
        int timeForGeode = bp.GeodeRobotObsidianCost * timeForObsidian;

        return string.Format("Blueprint {0} clay {1} obsidian {2} geode {3}", bp.Id, timeForClay, timeForObsidian, timeForGeode);
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
        if (input == null)
        {
            return null;
        }

        if (input.Cache.ContainsKey(input))
        {
            return input.Cache[input];
        }

        Result maxResult = input;

        if (input.Time <= m_totalTime)
        {
            for (int i = 0; i < (int)RobotType.NumValues; i++)
            {
                //if (m_currentLimits[i] != 0 && input.Robots[i] >= m_currentLimits[i])
                //{
                //    continue;
                //}


                var subStep = SubStep(input, (RobotType)i, false);
                if (subStep != null)
                {
                    var subStepResult = DoStep(subStep);

                    if (subStepResult != null)
                    {
                        if (maxResult.Score < subStepResult.Score)
                        {
                            maxResult = subStepResult;
                        }
                    }
                }
            }
            var waitSubStep = SubStep(input, RobotType.Ore, true);
            var waitSubStepResult = DoStep(waitSubStep);

            if (waitSubStepResult != null)
            {
                if (maxResult.Score < waitSubStepResult.Score)
                {
                    maxResult = waitSubStepResult;
                }
            }
        }
        else
        {
            maxResult = null;
        }

        //input.Cache[input] = maxResult;

        return maxResult;
    }



    public Result SubStep(Result input, RobotType action, bool wait)
    {
        if (wait)
        {
            Result waitResult = input.Copy(action);
            waitResult.Time += 1;

            for (int i = 0; i < waitResult.Inventory.Length; i++)
            {
                waitResult.Inventory[i] += waitResult.Robots[i];
            }
            return waitResult;
        }

        int[] costs = input.BluePrint.Costs[action];


        for (int i = 0; i < input.Inventory.Length; i++)
        {
            if (input.Inventory[i] < costs[i])
            {
                return null;
            }
        }

        int maxOre = input.BluePrint.MaxPerTurnCosts[RobotType.Ore];
        int maxClay = input.BluePrint.MaxPerTurnCosts[RobotType.Clay];
        int maxObisidian = input.BluePrint.MaxPerTurnCosts[RobotType.Obsidian];

        int ore = input.Inventory[(int)RobotType.Ore];
        int clay = input.Inventory[(int)RobotType.Clay];
        int obsidian = input.Inventory[(int)RobotType.Obsidian];

        //if ((action == RobotType.Ore && ore >= maxOre) ||
        //    (action == RobotType.Clay && clay >= maxClay) ||
        //    (action == RobotType.Obsidian && (obsidian >= maxObisidian || (clay == 0))) ||
        //    (action == RobotType.Geode && obsidian == 0))
        //{
        //    return null;
        //}

        if ((action == RobotType.Ore && ore >= maxOre))
        {
            return null;
        }
        if(action == RobotType.Clay && clay >= maxClay)
        {
            return null;
        }
        if(action == RobotType.Obsidian && (obsidian >= maxObisidian || (clay < maxClay)))
        {
            return null;
        }
        if((action == RobotType.Geode && obsidian == 0))
        {
            return null;
        }

        Result newResult = input.Copy(action);
        newResult.Time += 1;

        for (int i = 0; i < newResult.Inventory.Length; i++)
        {
            newResult.Inventory[i] -= costs[i];
        }

        for (int i = 0; i < newResult.Inventory.Length; i++)
        {
            newResult.Inventory[i] += newResult.Robots[i];
        }

        newResult.Robots[(int)action]++;

        return newResult;

    }

    private int GetMaxGeodes(BluePrint bp, int nrMinutes)
	{
		int result = 0;
		for (int i = 0; i < 4; i++)
		{
			result = Math.Max(result, getMaxGeodesForType(bp, nrMinutes, (RobotType)i, 0, 0, 0, 0, 1, 0, 0, 0));
		}
		return result;
	}


    private int getMaxGeodesForType(BluePrint bp, int minutesLeft, RobotType goal, int nrOre, int nrClay, int nrOb,
			int nrGeo, int nrOreRobots, int nrClayRobots, int nrObRobots, int nrGeoRobots)
	{
		if (minutesLeft == 0)
		{
			return nrGeo;
		}
		// Stop building a robot if we have more of the resource it builds than we need.
		// final int maxOre = Math.Max(b.oreCost(), Math.Max(b.clayOreCost(),
		// Math.Max(b.obOreCost(), b.geoObCost())));
		int maxOre = bp.MaxPerTurnCosts[(int)RobotType.Ore];

//		if (goal == ORE_ROBOT && nrOre >= maxOre || goal == CLAY_ROBOT && nrClay >= b.obClayCost()
//				|| goal == OB_ROBOT && (nrOb >= b.geoObCost() || nrClay == 0) || goal == GEO_ROBOT && nrOb == 0)
//		{
//			return 0;
//		}

		if (goal == RobotType.Ore && nrOre >= maxOre)
		{
			return 0;
		}

		if (goal == RobotType.Clay && nrClay >= bp.Costs[RobotType.Obsidian][(int)RobotType.Clay])
		{
			return 0;
		}
		if (goal == RobotType.Obsidian && (nrOb >= bp.Costs[RobotType.Geode][(int)RobotType.Obsidian] || nrClay == 0))
		{
			return 0;
		}
		if (goal == RobotType.Geode && nrOb == 0)
		{
			return 0;
		}

        State state = new State(nrOre, nrClay, nrOb, nrGeo, nrOreRobots, nrClayRobots, nrObRobots, nrGeoRobots,minutesLeft, goal);

		//if (cache.containsKey(state))
		//{
		//	return cache.get(state);
		//}
		int max = 0;

		while (minutesLeft > 0)
		{
			if (goal == RobotType.Ore && nrOre >= bp.Costs[RobotType.Ore][(int)RobotType.Ore])
			{ // Build ore robot
				int tmpMax = 0;
				for (int newGoal = 0; newGoal < 4; newGoal++)
				{
					tmpMax = Math.Max(tmpMax,
							getMaxGeodesForType(bp, minutesLeft - 1, (RobotType)newGoal, nrOre - bp.Costs[RobotType.Ore][(int)RobotType.Ore] + nrOreRobots,
									nrClay + nrClayRobots, nrOb + nrObRobots, nrGeo + nrGeoRobots, nrOreRobots + 1,
									nrClayRobots, nrObRobots, nrGeoRobots));
				}
				max = Math.Max(max, tmpMax);
				//cache.put(state, max);
				return max;
			} else if (goal == RobotType.Clay  && nrOre >= bp.Costs[RobotType.Clay][(int)RobotType.Ore])
			{ // Build clay robot
				int tmpMax = 0;
				for (int newGoal = 0; newGoal < 4; newGoal++)
				{
					tmpMax = Math.Max(tmpMax,
							getMaxGeodesForType(bp, minutesLeft - 1, (RobotType)newGoal, nrOre - bp.Costs[RobotType.Clay][(int)RobotType.Ore] + nrOreRobots,
									nrClay + nrClayRobots, nrOb + nrObRobots, nrGeo + nrGeoRobots, nrOreRobots,
									nrClayRobots + 1, nrObRobots, nrGeoRobots));
				}
				max = Math.Max(max, tmpMax);
				//cache.put(state, max);
				return max;
			} else if (goal == RobotType.Obsidian  && nrOre >= bp.Costs[RobotType.Obsidian][(int)RobotType.Ore] && nrClay >= bp.Costs[RobotType.Obsidian][(int)RobotType.Clay])
			{ // Build ob robot
				int tmpMax = 0;
				for (int newGoal = 0; newGoal < 4; newGoal++)
				{
					tmpMax = Math.Max(tmpMax,
							getMaxGeodesForType(bp, minutesLeft - 1, (RobotType)newGoal, nrOre - bp.Costs[RobotType.Obsidian][(int)RobotType.Ore] + nrOreRobots,
									nrClay - bp.Costs[RobotType.Obsidian][(int)RobotType.Clay] + nrClayRobots, nrOb + nrObRobots, nrGeo + nrGeoRobots,
									nrOreRobots, nrClayRobots, nrObRobots + 1, nrGeoRobots));
				}
				max = Math.Max(max, tmpMax);
				//cache.put(state, max);
				return max;
			} else if (goal == RobotType.Geode && nrOre >= bp.Costs[RobotType.Geode][(int)RobotType.Ore] && nrOb >= bp.Costs[RobotType.Geode][(int)RobotType.Obsidian])
			{ // Build geo robot
				int tmpMax = 0;
				for (int newGoal = 0; newGoal < 4; newGoal++)
				{
					tmpMax = Math.Max(tmpMax,
							getMaxGeodesForType(bp, minutesLeft - 1, (RobotType)newGoal, nrOre - bp.Costs[RobotType.Geode][(int)RobotType.Ore] + nrOreRobots,
									nrClay + nrClayRobots, nrOb - bp.Costs[RobotType.Geode][(int)RobotType.Obsidian] + nrObRobots, nrGeo + nrGeoRobots,
									nrOreRobots, nrClayRobots, nrObRobots, nrGeoRobots + 1));
				}
				max = Math.Max(max, tmpMax);
				//cache.put(state, max);
				return max;
			}
			// Can not build a robot, so continue gathering resources.
			minutesLeft--;
			nrOre += nrOreRobots;
			nrClay += nrClayRobots;
			nrOb += nrObRobots;
			nrGeo += nrGeoRobots;
			max = Math.Max(max, nrGeo);
		}
		//cache.put(state, max);
		return max;
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
        public int[] Inventory = new int[(int)RobotType.NumValues];
        public int[] Robots = new int[(int)RobotType.NumValues];
        public int Time;
        //public List<RobotType> Route = new List<RobotType>();
        public Dictionary<Result, Result> Cache;
        public RobotType Action;

        public Result Copy(RobotType action)
        {
            Result result = new Result();
            result.BluePrint = BluePrint;
            Array.Copy(Inventory, result.Inventory, Inventory.Length);
            Array.Copy(Robots, result.Robots, Robots.Length);
            result.Time = Time;
            //result.Route.AddRange(Route);
            //result.Route.Add(action);
            result.Action = action;
            result.Cache = Cache;
            return result;
        }

        public int Score
        { get { return Inventory[(int)RobotType.Geode]; } }


        public override bool Equals(object obj)
        {
            Result r = obj as Result;
            if (r.Time != Time)
            {
                return false;
            }
            if (r.Action != Action)
            {
                return false;
            }
            for (int i = 0; i < Inventory.Length; ++i)
            {
                if (r.Inventory[i] != Inventory[i])
                {
                    return false;
                }
            }
            for (int i = 0; i < Robots.Length; ++i)
            {
                if (r.Robots[i] != Robots[i])
                {
                    return false;
                }
            }
            return true;
            //return obj is Result result &&
            //       EqualityComparer<int[]>.Default.Equals(Inventory, result.Inventory) &&
            //       EqualityComparer<int[]>.Default.Equals(Robots, result.Robots) &&
            //       Time == result.Time &&
            //       Action == result.Action;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Inventory, Robots, Time, Action);
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

        public Dictionary<RobotType, int[]> Costs = new Dictionary<RobotType, int[]>();
        public Dictionary<RobotType, int> MaxPerTurnCosts = new Dictionary<RobotType, int>();

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

            int maxOre = Math.Max(oreRobotCost, Math.Max(clayRobotCost, Math.Max(obsidianRobotOreCost, geodeRobotOreCost)));
            int maxClay = obsidianRobotClayCost;
            int maxObsidian = geodeRobotObsidianCost;

            MaxPerTurnCosts[RobotType.Ore] = maxOre;
            MaxPerTurnCosts[RobotType.Clay] = maxClay;
            MaxPerTurnCosts[RobotType.Obsidian] = maxObsidian;

        }


        public void GetCost2(RobotType type, int[] values)
        {
            Array.Fill(values, 0);

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



public record State(int nrOre, int nrClay, int nrOb, int nrGeo, int nrOreRobot, 
                    int nrClayRobot, int nrObRobot,int nrGeoRobot, int minutesLeft, RobotType goal)
{
}

public enum RobotType
{
    Ore,
    Clay,
    Obsidian,
    Geode,
    NumValues
};