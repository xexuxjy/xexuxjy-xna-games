using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using static Test19;

public class Test19 : BaseTest
{
    private List<BluePrint> m_blueprints = new List<BluePrint>();
    private List<SimulationResult> m_simulationResults= new List<SimulationResult>();

    public int m_totalTime = 24;
    public int m_currentTime = 0;

    public override void RunTest()
    {
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


        int ibreak = 0;

        foreach(BluePrint bluePrint in m_blueprints)
        {
            int[] limits = new int[]{4,4,0,0 };
            m_simulationResults.Add(RunSimulation(bluePrint,limits,true));
        }

        SimulationResult best = m_simulationResults.OrderByDescending(x=>x.Score).FirstOrDefault();

        //RunSimulation(best.BluePrint,best.Limits,true);

        WriteDebugInfo();
    }

    public SimulationResult RunSimulation(BluePrint bluePrint,int[] limits,bool debugInfo)
    {
        m_currentTime = 0;
        RobotType[] preferred = new RobotType[] { RobotType.Geode, RobotType.Obsidian, RobotType.Clay, RobotType.Ore };

        SimulationResult simulationResult = new SimulationResult(bluePrint, debugInfo?m_debugInfo:null);
    
        for(int i=0;i<limits.Length;++i)
        {
            if(limits[i] > 0)
            {
                simulationResult.SetLimit((RobotType)i,limits[i]);
            }
        }

        if(debugInfo)
        {
            m_debugInfo.Add("===========================BluePrint "+bluePrint.Id+" : "+string.Join(", ", limits)+"===========================");
        }

        while (m_currentTime < m_totalTime)
        {
            if(debugInfo)
            {
                m_debugInfo.Add("\n== Minute "+(m_currentTime+1)+" ==");
            }

            int[] projectedInventory = simulationResult.ProjectedInventory(2);

            foreach (RobotType preferredType in preferred)
            {
                if(!simulationResult.CanBuild(preferredType,simulationResult.Inventory) && simulationResult.CanBuild(preferredType,projectedInventory))
                {
                    break;
                }

                if(simulationResult.ShouldBuild(preferredType))
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
        {get{return m_inventory[(int)RobotType.Geode]; } }

        public int[] Limits
        {get{return m_robotLimits; } }

        public void UpdateInventory()
        {
            for (int i = 0; i < m_robots.Length; ++i)
            {
                m_inventory[i] += m_robots[i];
            }
            if(m_debugInfo != null)
            {
                m_debugInfo.Add(DebugSummary);
            }
        }

        public void SetLimit(RobotType type,int limit)
        {
            m_robotLimits[(int)type] = limit;
        }

        public bool ShouldBuild(RobotType type)
        {
            bool shouldBuild = m_robotLimits[(int)type]==0;
            if(m_robotLimits[(int)type] != 0)
            {
                shouldBuild =  m_robots[(int)type] < m_robotLimits[(int)type];
            }
            return shouldBuild;
        }

        public int[] Inventory{get {return m_inventory;} }

        public int[] ProjectedInventory(int rounds)
        {
            int[] projected = new int[m_inventory.Length];
            for(int i=0;i<projected.Length;++i)
            {
                projected[i] = m_inventory[i] + (m_robots[i] * rounds);
            }
            return projected;
        }


        public bool CanBuild(RobotType type,int[] inventory)
        {
            int[] values = new int[m_robots.Length];
            BluePrint.GetCost(type,values);
            bool success = true;
            for(int i=0;i<values.Length;++i)
            {
                if (values[i] > 0 && inventory[i] < values[i])
                {
                    success = false;
                    break;
                }
            }
            return success;
        }

        public bool StartBuildRobot(RobotType type)
        {
            bool canBuild = CanBuild(type,m_inventory);
            if (!canBuild)
            {
                return false;
            }


            m_building[(int)type] = true;

            int[] values = new int[m_building.Length];
            BluePrint.GetCost(type,values);

            if(m_debugInfo != null)
            {
                string robotCost = "";
                for(int i=0;i<values.Length;++i)
                {
                    if(values[i] > 0)
                    {
                        robotCost += values[i] + " "+((RobotType)i)+" ";
                    }
                } 

                m_debugInfo.Add(string.Format("Spend {0} to start building a {1}-collecting robot.", robotCost, type));
            }


            for(int i=0;i<values.Length;i++)
            {
                m_inventory[i] -= values[i];
            }

            return true;
        }

        public void CompleteBuild()
        {
            for(int i=0;i<m_building.Length;++i)
            {
                if(m_building[i])
                {
                    m_robots[i]++;
                    if(m_debugInfo != null)
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


    public record BluePrint
    {
        public int Id;
        public int OreRobotCost;
        public int ClayRobotCost;
        public int ObsidianRobotOreCost;
        public int ObsidianRobotClayCost;
        public int GeodeRobotOreCost;
        public int GeodeRobotObsidianCost;

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
        }

        public void GetCost(RobotType type,int[] values)
        {
            if(type == RobotType.Ore)
            {
                values[(int)RobotType.Ore] = OreRobotCost;
            }
            if(type == RobotType.Clay)
            {
                values[(int)RobotType.Ore] = ClayRobotCost;
            }
            if(type == RobotType.Obsidian)
            {
                values[(int)RobotType.Ore] = ObsidianRobotOreCost;
                values[(int)RobotType.Clay] = ObsidianRobotClayCost;
            }
            if(type == RobotType.Geode)
            {
                values[(int)RobotType.Ore] = GeodeRobotOreCost;
                values[(int)RobotType.Obsidian] = GeodeRobotObsidianCost;
            }
        }


    }


}


public enum RobotType
{
    Ore,
    Clay,
    Obsidian,
    Geode,
    NumValues
};