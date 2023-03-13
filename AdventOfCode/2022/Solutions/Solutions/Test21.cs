public class Test21 : BaseTest
{
    Dictionary<string, MonkeyNode> m_map = new Dictionary<string, MonkeyNode>();
    public Int128 HumanValue = 0;

    public override void RunTest()
    {
        TestID = 21;
        IsTestInput = false;
        IsPart2 = true;
        ReadDataFile();
        foreach (string line in m_dataFileContents)
        {
            MonkeyNode.Parse(line, m_map,this);
        }

        if (IsPart2)
        {
            MonkeyNode lhs = m_map[m_map[MonkeyNode.ROOT].LHS];
            MonkeyNode rhs = m_map[m_map[MonkeyNode.ROOT].RHS];

            bool lhsHuman = lhs.ContainsID(MonkeyNode.HUMAN);
            bool rhsHuman = rhs.ContainsID(MonkeyNode.HUMAN);

            DebugOutput((lhsHuman?"LHS":"RHS")+" has the human  in it");

            Int128 matchedResult = 0;
            List<MonkeyNode> path = new List<MonkeyNode>();
            if (lhsHuman)
            {
                matchedResult = lhs.Find(rhs.Result);
            }
            else
            {
                matchedResult = rhs.Find(lhs.Result);
            }


            if (lhs.Result == rhs.Result)
            {
                DebugOutput("Root matches with : " + lhs.Result);
            }
            else
            {
                DebugOutput("Root mismatch : LHS " + lhs.Result + "  RHS " + rhs.Result);
            }
        }
        else
        {
            DebugOutput("Final result " + m_map[MonkeyNode.ROOT].Result);
        }

        WriteDebugInfo();
    }


    public bool GetPath(MonkeyNode node, string id, List<MonkeyNode> path)
    {
        if (node.Id == id)
        {
            path.Add(node);
            return true;
        }

        if (node.LHS != null && node.RHS != null)
        {
            MonkeyNode lhs = m_map[node.LHS];
            MonkeyNode rhs = m_map[node.RHS];

            path.Add(lhs);
            bool lhsContains = GetPath(lhs, id, path);

            if (!lhsContains)
            {
                path.Remove(lhs);
                path.Add(rhs);
                bool rhsContains = GetPath(rhs, id, path);
                if (!rhsContains)
                {
                    path.Remove(rhs);
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        return false;
    }

}

public class MonkeyNode
{
    public const string ROOT = "root";
    public const string HUMAN = "humn";

    public static MonkeyNode Parse(string line, Dictionary<string, MonkeyNode> map,Test21 test)
    {
        string[] tokens = line.Split(' ');
        string id = tokens[0].Replace(":", "");
        MonkeyNode newNode = new MonkeyNode();
        newNode.Id = id;
        newNode.m_map = map;
        newNode.Test = test;
        
        if (tokens.Length == 2)
        {
            newNode.Value = Int128.Parse(tokens[1]);
        }
        else
        {
            newNode.LHS = tokens[1];
            newNode.MathOperator = tokens[2];
            newNode.RHS = tokens[3];
        }

        map[id] = newNode;
        return newNode;
    }


    public Dictionary<string, MonkeyNode> m_map;
    public string Id;
    public string MathOperator = "";
    public string LHS;
    public string RHS;
    public Int128 Value;
    public Test21 Test;

    public Int128 Result
    {
        get
        {
            if(Test.IsPart2 && Id  == HUMAN)
            {
                return Test.HumanValue;
            }

            switch (MathOperator)
            {
                case "":
                    return Value;
                case "+":
                    return m_map[LHS].Result + m_map[RHS].Result;
                case "-":
                    return m_map[LHS].Result - m_map[RHS].Result;
                case "*":
                    return m_map[LHS].Result * m_map[RHS].Result;
                case "/":
                    return m_map[LHS].Result / m_map[RHS].Result;
            }
            return 0;
        }
    }

    public bool ContainsID(string id)
    {
        if (Id == id)
        {
            return true;
        }

        if (LHS != null && RHS != null)
        {
            bool lhsContains = m_map[LHS].ContainsID(id);
            bool rhsContains = m_map[RHS].ContainsID(id);
            return lhsContains || rhsContains;
        }
        return false;
    }


    public Int128 Find(Int128 valToMatch)
    {
        //Test.DebugOutput($"Checking node {Id} OP {MathOperator}");
        if(Id == HUMAN)
        {
            Test.DebugOutput("Found human node value : "+valToMatch);
            Test.HumanValue = valToMatch;
            return valToMatch;
        }

        if (LHS != null && RHS != null)
        {
            bool leftContains = m_map[LHS].ContainsID(HUMAN);
            bool rightContains = m_map[RHS].ContainsID(HUMAN);

            MonkeyNode known = leftContains?m_map[RHS] : m_map[LHS];
            MonkeyNode unknown = leftContains?m_map[LHS] : m_map[RHS];
            MonkeyNode lhsNode = m_map[LHS];
            MonkeyNode rhsNode = m_map[RHS];



            switch (MathOperator)
            {
                case "":
                    return Value;
                case "+":
                    Int128 newValAdd = valToMatch - known.Result;
                    return unknown.Find(newValAdd);
                case "-":
                    Int128 newValSub = 0;
                    if(unknown == rhsNode)
                    {
                        newValSub= known.Result - valToMatch;
                    }
                    else
                    {
                        newValSub = valToMatch + known.Result;
                    }
                    return unknown.Find(newValSub);
                case "*":
                    Int128 newValMul = valToMatch / known.Result;

                    return unknown.Find(newValMul);
                case "/":
                    if(unknown == lhsNode)
                    {
                        Int128 newValDiv = valToMatch * known.Result;
                        return unknown.Find(newValDiv);
                    }
                    else
                    {
                        Int128 newValDiv = valToMatch / known.Result;
                        return unknown.Find(newValDiv);
                    }
                }


            }
        return Value;
    }
}