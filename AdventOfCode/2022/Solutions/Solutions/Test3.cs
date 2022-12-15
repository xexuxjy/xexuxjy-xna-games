using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Test3 : BaseTest
{
    public static char INVALID_CHAR = '*';
    public override void RunTest()
    {
        string filename = InputPath + "puzzle-3-input.txt";

        List<RuckSack> data = new List<RuckSack>();

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                    RuckSack ruckSack = new RuckSack(line);
                    if (ruckSack != null)
                    {
                        data.Add(ruckSack);
                    }
                }
            }
        }

        List<List<RuckSack>> groupList = new List<List<RuckSack>>();
        List<RuckSack> currentGroup = null;
        int groupSize = 3;
        for (int i = 0; i < data.Count; ++i)
        {
            if (i % groupSize == 0)
            {
                currentGroup = new List<RuckSack>();
                groupList.Add(currentGroup);
            }
            currentGroup.Add(data[i]);
        }



        int total = 0;
        int badgeTotal = 0;

        foreach (RuckSack ruckSack in data)
        {
            ruckSack.CheckInput();
            total += ruckSack.Priority;
        }

        foreach (List<RuckSack> group in groupList)
        {
            char common = CommonItem(group);
            Debug.Assert(common != INVALID_CHAR);
            badgeTotal += RuckSack.GetItemPriority(common);

            int ibreak2 = 0;
        }




        int ibreak = 0;

    }

    public static char CommonItem(List<RuckSack> group)
    {
        char result = INVALID_CHAR;
        bool valid = true;

        valid = CheckCharInGroup(group, 'a', out result);
        if (valid)
        {
            return result;
        }
        valid = CheckCharInGroup(group, 'A', out result);

        Debug.Assert(valid);

        return result;
    }

    public static bool CheckCharInGroup(List<RuckSack> group, char startChar, out char result)
    {
        bool valid = true;
        result = INVALID_CHAR;
        for (int i = 0; i < 26; ++i)
        {
            char testChar = (char)((int)startChar + i);
            valid = true;
            foreach (RuckSack ruckSack in group)
            {
                if (!ruckSack.TotalContents.Contains(testChar))
                {
                    valid = false;
                    break;
                }
            }
            if (valid)
            {
                result = testChar;
                break;
            }
        }

        return valid;
    }
}

public class RuckSack
{
    private static Dictionary<char, Tuple<int, int>> PackingDictionary = new Dictionary<char, Tuple<int, int>>();


    private string totalContents;
    private string compartment1;
    private string compartment2;


    public RuckSack(string input)
    {
        int halfLength = input.Length / 2;
        totalContents = input;
        compartment1 = totalContents.Substring(0, halfLength);
        compartment2 = totalContents.Substring(halfLength, halfLength);

        // sort each for efficiency?

        Debug.Assert(compartment1.Length == compartment2.Length);
    }

    public string TotalContents
    {
        get { return totalContents; }
    }

    public int Priority
    {
        get
        {
            HashSet<char> shared = SharedItems;
            Debug.Assert(shared.Count == 1);

            char c = shared.First();
            return GetItemPriority(c);
        }
    }

    public static int GetItemPriority(char c)
    {
        Debug.Assert(c != Test3.INVALID_CHAR);

        if (c >= 'a' && c <= 'z')
        {
            return 1 + (c - 'a');
        }

        return 27 + (c - 'A');

    }


    public HashSet<char> SharedItems
    {
        get
        {
            HashSet<char> shared = new HashSet<char>();
            foreach (char c1 in compartment1)
            {
                foreach (char c2 in compartment2)
                {
                    if (c1 == c2)
                    {
                        shared.Add(c1);
                        break;
                    }
                }
            }
            return shared;
        }
    }


    public void CheckInput()
    {
        foreach (char c in compartment1)
        {
            Tuple<int, int>? value = null;
            PackingDictionary.TryGetValue(c, out value);
            if (value == null)
            {
                value = new Tuple<int, int>(0, 0);
            }
            value = new Tuple<int, int>(value.Item1 + 1, value.Item2);
            PackingDictionary[c] = value;
        }

        foreach (char c in compartment2)
        {
            Tuple<int, int>? value = null;
            PackingDictionary.TryGetValue(c, out value);
            if (value == null)
            {
                value = new Tuple<int, int>(0, 0);
            }
            value = new Tuple<int, int>(value.Item1, value.Item2 + 1);
            PackingDictionary[c] = value;
        }

    }


}