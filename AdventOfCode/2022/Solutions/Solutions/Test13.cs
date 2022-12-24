using System;
using System.Diagnostics;
using System.Diagnostics.Metrics;

public class Test13 : BaseTest
{
    List<Tuple<string, string>> m_pairList = new List<Tuple<string, string>>();

    public override void RunTest()
    {
        TestID = 13;


        ReadDataFile();

        SolvePuzzle2();

        WriteDebugInfo();
    }

    public void SolvePuzzle1()
    {
        int count=0;
        while (count < m_dataFileContents.Count)
        {
            string line1 = m_dataFileContents[count++];
            string line2 = m_dataFileContents[count++];
            m_pairList.Add(new Tuple<string, string>(line1, line2));
            if (count < m_dataFileContents.Count)
            {
                string empty = m_dataFileContents[count++];
                Debug.Assert("" == empty);
            }
        }

        int total = 0;

        for (int i = 0; i < m_pairList.Count; i++)
        {

            ListNode left = BuildListNode(m_pairList[i].Item1);
            ListNode right = BuildListNode(m_pairList[i].Item2);

            m_debugInfo.Add("== Pair " + (i + 1) + " ==");

            int compare = Compare(left, right, 0);

            if (compare < 0)
            {
                total += (i + 1);
            }
            m_debugInfo.Add("\n");
        }
        m_debugInfo.Add("Final score : " + total);
    }

    public void SolvePuzzle2()
    {
        List<INode> nodes = new List<INode>();

        for(int i=0;i<m_dataFileContents.Count;++i)
        {
            if(m_dataFileContents[i] == "")
            {
                continue;
            }
            nodes.Add(BuildListNode(m_dataFileContents[i]));    
        }

        string decoder2 = "[[2]]";
        string decoder6 = "[[6]]";

        nodes.Add(BuildListNode(decoder2));
        nodes.Add(BuildListNode(decoder6));

        m_debugInfo.Add("Before sort");
        foreach(INode node in nodes) 
        {
            m_debugInfo.Add(node.Debug);
        }
        nodes.Sort((INode left,INode right) => Compare(left,right,0));
        m_debugInfo.Add("After sort");
        
        foreach(INode node in nodes) 
        {
            m_debugInfo.Add(node.Debug);
        }

        int decoder2Index = -1;
        int decoder6Index = -1;

        for(int i=0;i<nodes.Count;++i)
        {
            if(nodes[i].Debug == decoder2)
            {
                decoder2Index = i+1;
            }
            if(nodes[i].Debug == decoder6)
            {
                decoder6Index = i+1;
            }

        }

        m_debugInfo.Add(String.Format("2 at {0} 6 at {1} product = {2}",decoder2Index,decoder6Index,(decoder2Index*decoder6Index)));

        int total = 0;

    }

    public List<Tuple<int, int>> MatchBrackets(string value)
    {
        List<Tuple<int, int>> matchList = new List<Tuple<int, int>>();
        Stack<int> open = new Stack<int>();

        for (int i = 0; i < value.Length; ++i)
        {
            if (value[i] == '[')
            {
                open.Push(i);
            }
            else if (value[i] == ']')
            {
                matchList.Add(new Tuple<int, int>(open.Pop(), i + 1));
            }
        }

        List<Tuple<int, int>> sorted = matchList.OrderBy(x => x.Item1).ToList();
        return sorted;

    }

    ListNode BuildListNode(string line)
    {
        Stack<ListNode> listNodes = new Stack<ListNode>();
        ListNode currentNode = null;
        string currentNumber = "";
        ListNode rootNode = null;

        for (int i = 0; i < line.Length; ++i)
        {
            if (line[i] == '[')
            {
                ListNode newNode = new ListNode();
                if (currentNode != null)
                {
                    currentNode.AddNode(newNode);
                }
                listNodes.Push(newNode);
                currentNode = newNode;

            }
            else if (line[i] == ']')
            {
                INode intNode = GetNumberNode(currentNumber);
                if (intNode != null)
                {
                    currentNode.AddNode(intNode);
                }
                currentNumber = "";
                //currentNode = listNodes.Pop();
                if (listNodes.Count > 1)
                {
                    listNodes.Pop();
                    currentNode = listNodes.Peek();
                }
                else
                {
                    currentNode = listNodes.Pop();
                }
            }
            else if (line[i] >= '0' && line[i] <= '9')
            {
                currentNumber += line[i];
            }
            else if (line[i] == ',')
            {
                INode intNode = GetNumberNode(currentNumber);
                if (intNode != null)
                {
                    currentNode.AddNode(intNode);
                }
                currentNumber = "";
            }
        }



        return currentNode;
    }

    public INode GetNumberNode(string val)
    {
        IntNode intNode = null;
        int num = -1;
        if (val.Length > 0)
        {
            if (int.TryParse(val, out num))
            {
                intNode = new IntNode();
                intNode.Value = num;
            }
        }
        return intNode;
    }

    private void IndentAndAdd(string data, int indent)
    {
        string result = "";
        for (int i = 0; i < indent; ++i)
        {
            result += "    ";
        }
        result += " - " + data;
        m_debugInfo.Add(result);
    }


    public int Compare(INode left, INode right, int indentLevel)
    {
        ListNode leftLN = left as ListNode;
        ListNode rightLN = right as ListNode;
        IntNode leftIN = left as IntNode;
        IntNode rightIN = right as IntNode;

        IndentAndAdd(String.Format("Comparing {0} and {1}", left.Debug, right.Debug), indentLevel);


        if (leftIN != null && rightIN != null)
        {
            //IndentAndAdd(String.Format("Comparing {0} and {1} {2}", leftIN.Debug, rightIN.Debug, (leftIN.Value < rightIN.Value ? "Correct" : "Incorrect")),indentLevel);
            int val = leftIN.Value.CompareTo(rightIN.Value);
            if (val < 0)
            {
                IndentAndAdd("Left side is smaller so inputs are in the right order", indentLevel);
            }
            else if (val > 0)
            {
                IndentAndAdd("Right side is smaller so inputs are not in the right order", indentLevel);
            }
            return val;
        }
        else if (leftLN != null && rightLN != null)
        {
            for (int i = 0; i < leftLN.Values.Count; ++i)
            {
                if (i < rightLN.Values.Count)
                {
                    int compare = Compare(leftLN.Values[i], rightLN.Values[i], indentLevel + 1);
                    if (compare != 0)
                    {
                        return compare;
                    }
                }
                else
                {
                    // right ran out - not in right order?
                    IndentAndAdd("Right ran out so inputs are not in the right order", indentLevel);
                    return 1;
                }
            }

            // all equals,run out of left in the correct order
            //IndentAndAdd(String.Format("Comparing Lists {0} and {1} {2}", leftLN.Debug, rightLN.Debug, "Left ran out"), indentLevel);
            if (leftLN.Values.Count < rightLN.Values.Count)
            {
                IndentAndAdd("Left ran out so inputs are in the right order", indentLevel);
                return -1;
            }
            return 0;
        }
        else if (leftLN != null && rightIN != null)
        {
            ListNode wrap = new ListNode();
            wrap.AddNode(rightIN);
            return Compare(leftLN, wrap, indentLevel + 1);
        }
        else if (rightLN != null && leftIN != null)
        {
            ListNode wrap = new ListNode();
            wrap.AddNode(leftIN);
            return Compare(wrap, rightLN, indentLevel + 1);

        }

        return 0;
    }

}

public interface INode
{

    public string Debug { get; }


}

public class IntNode : INode
{
    public int Value;
    public string Debug
    {
        get { return "" + Value; }
    }
}

public class ListNode : INode
{
    public List<INode> Values = new List<INode>();
    public void AddNode(INode node)
    {
        Values.Add(node);
    }

    public string Debug
    {
        get
        {
            string val = "[";
            for (int i = 0; i < Values.Count; ++i)
            {
                val += Values[i].Debug;
                if (i < Values.Count - 1)
                {
                    val += ",";
                }
            }
            val += "]";
            return val;
        }
    }
}