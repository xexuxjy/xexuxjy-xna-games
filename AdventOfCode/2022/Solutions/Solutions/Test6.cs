using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test6 : BaseTest
{
    public static void RunTest()
    {
        string filename = InputPath + "puzzle-5-input.txt";

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                }
            }
        }
    }
}

