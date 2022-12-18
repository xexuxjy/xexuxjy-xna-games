using System.Diagnostics;
using System.Numerics;
using System.Text;

public class Test11 : BaseTest
{
    public override void RunTest()
    {
        TestID = 11;
        ReadDataFile();

        Dictionary<int, Monkey> monkeyMap = new Dictionary<int, Monkey>();
        List<Monkey> monkeyList = new List<Monkey>();

        for (int i = 0; i < m_dataFileContents.Count; ++i)
        {
            if (m_dataFileContents[i].StartsWith("Monkey"))
            {
                string[] temp = new string[6];
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = m_dataFileContents[i + j];
                }

                Monkey monkey = Monkey.ReadData(temp);
                Debug.Assert(monkey != null);
                i += temp.Length;
                monkeyMap[monkey.ID] = monkey;
                monkeyList.Add(monkey);
            }
        }

        UInt128 groupModulo = 1;
        foreach(Monkey monkey in monkeyList)
        {
            groupModulo *= monkey.TestDivisor;
        }

        bool extraWorried = true;

        int numRounds = 10000;
        for (int round = 0; round < numRounds; ++round)
        {
            UInt128 worryLevel = 0;
            foreach (Monkey monkey in monkeyList)
            {
                while (monkey.HasItems)
                {
                    // initial worry level.
                    worryLevel = monkey.CurrentItemWorry;

                    //m_debugInfo.Add(string.Format("Monkey {0} worry {1}", monkey.ID, worryLevel));

                    // inspect current item and update worry level

                    worryLevel = monkey.Inspect(worryLevel);
                    //m_debugInfo.Add(string.Format("Monkey {0} updated worry {1}", monkey.ID, worryLevel));


                    if (!extraWorried)
                    {
                        // boredom
                        worryLevel /= 3;
                        //m_debugInfo.Add(string.Format("Monkey {0} boredom {1}", monkey.ID, worryLevel));
                    }
                    else
                    {
                        // going by the group modulo will preserve the order without the worry levels going insane.
                        worryLevel %= groupModulo;
                    }

                    // check and move items.
                    Monkey target = null;
                    monkey.Check(worryLevel, monkeyMap, out target);
                    //m_debugInfo.Add(string.Format("Monkey {0} thrown item of worry {1} to {2}", monkey.ID, worryLevel, target.ID));
                    //m_debugInfo.Add("\n");
                }


            }

            System.Console.WriteLine("After Round {0}", round + 1);
            m_debugInfo.Add(String.Format("After Round {0}", round + 1));
            foreach (Monkey m in monkeyList)
            {
                m_debugInfo.Add(m.DebugInfo);
            }

        }

        List<Monkey> sorted = monkeyList.OrderByDescending(x => x.InspectionCount).ToList();

        m_debugInfo.Add("Highest : " + sorted[0].InspectionCount);
        m_debugInfo.Add("Second : " + sorted[1].InspectionCount);

        UInt128 finalResult = (sorted[0].InspectionCount * sorted[1].InspectionCount);

        m_debugInfo.Add(String.Format("Final result = " + (sorted[0].InspectionCount * sorted[1].InspectionCount)));    

        WriteDebugInfo();
        int ibreak = 0;
    }
}


public class Monkey
{
    public static Monkey ReadData(String[] input)
    {
        int count = 0;
        string monkeyId = input[count++];
        Debug.Assert(monkeyId != null && monkeyId.StartsWith("Monkey"));
        string items = input[count++];
        string operation = input[count++];
        string test = input[count++];
        string pass = input[count++];
        string fail = input[count++];

        int id = int.Parse(monkeyId.Split(" ")[1].Replace(":", ""));

        string temp = items.Substring(items.IndexOf(":") + 1);
        string[] tokens = temp.Split(",");

        Monkey monkey = new Monkey(id, operation, test, pass, fail);
        foreach (string token in tokens)
        {
            monkey.AddItem((UInt128)int.Parse(token));
        }

        return monkey;

    }

    public Monkey(int id, string operation, string test, string testPass, string testFail)
    {
        m_id = id;
        BuildOperation(operation);
        m_passThrowTarget = BuildTarget(testPass);
        m_failThrowTarget = BuildTarget(testFail);
        BuildTest(test);

    }

    public bool HasItems
    { get { return m_items.Count > 0; } }

    public UInt128 CurrentItemWorry
    {
        get
        {
            if (m_items.Count > 0)
            {
                return m_items[0];
            }
            return 0;
        }
    }

    public UInt128 InspectionCount
    { get; set; }

    private void BuildOperation(string operation)
    {
        String key = "Operation: ";
        string temp = operation.Substring(operation.IndexOf(key) + key.Length);
        string[] tokens = temp.Split(" ");
        Debug.Assert(tokens[0] == "new");
        Debug.Assert(tokens[1] == "=");
        Debug.Assert(tokens[2] == "old");

        if (tokens[3] == "+")
        {
            m_operationFunction = Add;
        }
        else if (tokens[3] == "*")
        {
            m_operationFunction = Mul;
        }

        if (tokens[4] == "old")
        {
            m_operationAmount = null;
        }
        else
        {
            m_operationAmount = UInt128.Parse(tokens[4]);
        }

    }

    private void BuildTest(string test)
    {
        string key = "Test: divisible by ";
        m_testDivisor = UInt128.Parse(test.Substring(test.IndexOf(key) + key.Length));
    }

    private int BuildTarget(string target)
    {
        string key = "monkey";
        return int.Parse(target.Substring(target.IndexOf(key) + key.Length));
    }

    private static UInt128 Add(UInt128 a, UInt128 b)
    {
        return a + b;
    }

    private static UInt128 Mul(UInt128 a, UInt128 b)
    {
        return a * b;
    }

    public int ID
    {
        get { return m_id; }
    }

    public void AddItem(UInt128 item)
    {
        m_items.Add(item);
    }

    public void ThrowItem(Monkey target, UInt128 item)
    {
        target.AddItem(item);
    }

    public UInt128 Inspect(UInt128 worryLevel)
    {
        InspectionCount++;
        UInt128 amount = 0;
        if (m_operationAmount != null)
        {
            amount = m_operationAmount.Value;
        }
        else
        {
            amount = worryLevel;
        }

        return m_operationFunction(worryLevel, amount);

        //int target = Check() ? m_passThrowTarget : m_failThrowTarget;
        //ThrowItem(map[m_passThrowTarget], 0);

    }

    public void Check(UInt128 worryLevel, Dictionary<int, Monkey> map, out Monkey targetMonkey)
    {
        int k = 20;

        int target = (worryLevel % m_testDivisor) == 0 ? m_passThrowTarget : m_failThrowTarget;

        m_items.RemoveAt(0);

        targetMonkey = map[target];

        ThrowItem(targetMonkey, worryLevel);
    }

    public UInt128 TestDivisor
    { get { return m_testDivisor; } }


    public string DebugInfo
    {
        get
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Monkey " + ID + ": inspected items " + InspectionCount + " times");
            //foreach (UInt128 val in m_items)
            //{
            //    sb.Append(val);
            //    sb.Append(",");
            //}
            return sb.ToString();
        }
    }

    Func<UInt128, UInt128, UInt128> m_operationFunction;

    private UInt128? m_operationAmount;
    private int m_id;
    //private int m_worryLevel;

    private int m_passThrowTarget;
    private int m_failThrowTarget;

    private UInt128 m_testDivisor = 1;

    private List<UInt128> m_items = new List<UInt128>();
}