using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test6 : BaseTest
{
    public override void RunTest()
    {
        string filename = InputPath + "puzzle-6-input.txt";

        int packetLength = 14;//4

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                    int result = AnalyseLine(line,packetLength);
                    int ibreak = 0;
                }
            }
        }
    }

    public static int AnalyseLine(string line,int packetLength)
    {
        FixedSizedQueue<char> fsq = new FixedSizedQueue<char>(packetLength);
        HashSet<char> duplicates = new HashSet<char>();;

        for(int i=0;i<line.Length;++i)
        {
            duplicates.Clear();
            fsq.Enqueue(line[i]);
            foreach(char c in fsq)
            {
                duplicates.Add(c);
            }

            if(duplicates.Count == packetLength)
            {
                return i+1;
            }
        }
        return -1;
    }


}

