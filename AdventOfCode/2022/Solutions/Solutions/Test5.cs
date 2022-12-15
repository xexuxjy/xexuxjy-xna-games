using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test5 : BaseTest
{
    public override void RunTest()
    {
        Stack<string> test = new Stack<string>();
        test.Push("Mark");
        test.Push("Andrew");
        test.Push("Neale");



        string filename = InputPath + "puzzle-5-input.txt";
        List<string> crateLayout = new List<string>();
        List<string> moves = new List<string>();
        List<Stack<string>> crateStacks = new List<Stack<string>>();
        string numbers = null;

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                    if(line.Contains("["))
                    {
                        crateLayout.Add(line);
                    }
                    if(line.StartsWith("move"))
                    {
                        moves.Add(line);
                    }
                    if(line.StartsWith(" 1 "))
                    {
                        numbers = line;
                    }
                }
            }
        }

        int numStacks = int.Parse(numbers.Substring(numbers.Length-3,3));
        for(int i=0;i<numStacks;i++)
        {
            crateStacks.Add(new Stack<string>());
        }

        // reverse layout
        crateLayout.Reverse();

        int crateWidth = 3;
        int crateSpace = 1;

        int index = 0;
        int startIndex = 0;
        int crateIndex = 0;
        foreach(string layout in crateLayout)
        {
            for(int i=0;i<numStacks; i++)
            {
                string crate = layout.Substring((crateWidth*i)+i,crateWidth);
                if(crate.StartsWith("["))
                {
                    crateStacks[i].Push(crate);
                }
            }
        }

        bool is9001 = true;
        List<string> holder9001 = new List<string>();

        foreach(string move in moves)
        {
            string[] moveTokens = move.Split(" ");
            int number = int.Parse(moveTokens[1]);
            int from = int.Parse(moveTokens[3]);
            int to = int.Parse(moveTokens[5]);

            // zero index
            from--;
            to--;

            if(is9001)
            {
                holder9001.Clear();
                for(int i=0;i<number;++i)
                {
                    string val = crateStacks[from].Pop();
                    holder9001.Add(val);
                }

                holder9001.Reverse();

                foreach(string val in holder9001)
                {
                    crateStacks[to].Push(val);
                }

            }
            else
            {
                for(int i=0;i<number;++i)
                {
                    string val = crateStacks[from].Pop();
                    crateStacks[to].Push(val);
                }
            }
        }


        int maxHeight = 0;
        foreach(var cs in crateStacks)
        {
            if(cs.Count > maxHeight)
            {
                maxHeight = cs.Count;
            }
        }

        List<List<string>> display = new List<List<string>>();
        for(int i=0;i<crateStacks.Count;++i)
        {
            List<string> l = new List<string>();
            l.AddRange(crateStacks[i].ToArray());
            //l.Reverse();
            display.Add(l);
        }


        String result = "";
        for(int j=maxHeight-1;j>=0;--j)
        {
            for(int i=0;i<display.Count;++i)
            {
                List<string> l = display[i];
                if(j < l.Count)
                {
                    result += l[j];
                }
                else
                {
                    result += "   ";
                }
                if(i<display.Count-1)
                {
                    result += " ";
                }
            }
            result += "\n";
        }

        string result2="";
        foreach(var s in crateStacks)
        {
            result2 += s.Peek();
        }

        
        int ibreak = 0;
    }
}

