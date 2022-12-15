using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class BaseTest
{
    public static string InputPath = @"D:\GitHub\xexuxjy\AdventOfCode\2022\Solutions\Data\";

    protected List<string> m_dataFileContents = new List<string>();

    public int TestNum
    {get; set;}

    public String Filename
    {
        get{return InputPath+"puzzle-"+TestNum+"-input.txt"; }
    }

    public void ReadDataFile()
    {
        m_dataFileContents.Clear();
        using (StreamReader sr = new StreamReader(new FileStream(Filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                    m_dataFileContents.Add(line);
                }
            }
        }
    }


    public abstract void RunTest();

}