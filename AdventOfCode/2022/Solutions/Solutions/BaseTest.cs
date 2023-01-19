using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class BaseTest
{
    public static string InputPath = @"D:\GitHub\xexuxjy\AdventOfCode\2022\Solutions\Data\";

    protected List<string> m_dataFileContents = new List<string>();
    protected List<string> m_debugInfo = new List<string>();

    public int TestID
    { get; set; }

    public bool IsTestInput
    { get; set; }

    public bool IsPart2
    { get; set; }

    public String Filename
    {
        get { return InputPath + "puzzle-" + TestID + "-" + (IsTestInput ? "test-" : "") + "input.txt"; }
    }

    public String DebugFilename
    {
        get { return InputPath + "puzzle-" + TestID + "-" + (IsTestInput ? "test-" : "") + "debug.txt"; }
    }

    public void WriteDebugInfo()
    {
        using (StreamWriter sw = new StreamWriter(new FileStream(DebugFilename, FileMode.Create)))
        {
            foreach (string line in m_debugInfo)
            {
                sw.WriteLine(line);
            }
        }
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

    public void DebugOutput(string s)
    {
        m_debugInfo.Add(s);
        System.Console.WriteLine(s);
    }

    public abstract void RunTest();

}