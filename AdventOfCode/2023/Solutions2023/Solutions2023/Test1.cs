using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test1 : BaseTest
{
    public override void RunTest()
    {
        TestID = 1;
        IsTestInput = true;
        IsPart2 = false;

        ReadDataFile();

        List<string> numbers = new List<string>();
        numbers.Add("one");
        numbers.Add("two");
        numbers.Add("three");
        numbers.Add("four");
        numbers.Add("five");
        numbers.Add("six");
        numbers.Add("seven");
        numbers.Add("eight");
        numbers.Add("nine");



        int total = 0;

        foreach (string line in m_dataFileContents)
        {
            string first = "";
            int firstIndex = int.MaxValue;
            for (int c = 0; c < line.Length; ++c)
            {
                if (Char.IsDigit(line[c]))
                {
                    firstIndex = c;
                    first = ""+line[c];
                    break;
                }
            }

            if (IsPart2)
            {
                string numberName = "";
                for (int i = 0; i < numbers.Count; ++i)
                {
                    int index = line.IndexOf(numbers[i]);
                    if (index >= 0 && index < firstIndex)
                    {
                        numberName = numbers[i];
                        firstIndex = index;
                    }
                }

                if (numberName != "")
                {
                    first = ""+(numbers.IndexOf(numberName) + 1); 
                }
            }

            string last = "";
            int lastIndex = int.MinValue;
            for (int i = line.Length - 1; i >= 0; --i)
            {
                if (char.IsDigit(line[i]))
                {
                    lastIndex = i;
                    last = ""+line[i];
                    break;
                }
            }

            if (IsPart2)
            {
                string numberName = "";
                for (int i = 0; i < numbers.Count; ++i)
                {
                    int index = line.LastIndexOf(numbers[i]);
                    if (index >= 0 && index > lastIndex)
                    {
                        numberName = numbers[i];
                        lastIndex = index;
                    }
                }

                if (numberName != "")
                {
                    last = ""+(numbers.IndexOf(numberName) + 1);
                }
                
                
                
            }
            
            string result = first + last;
            total += int.Parse(result);
        }

        DebugOutput($"Result is {total}");

        WriteDebugInfo();
    }
}