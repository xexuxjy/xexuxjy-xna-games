using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test4 : BaseTest
{
    public static void RunTest()
    {

       string filename = InputPath +"puzzle-4-input.txt";

        List<Section> sections = new List<Section>();
        List<Section> duplicates = new List<Section>();
        List<Section> overlaps = new List<Section>();

        int duplicateCount = 0;
        int overlapCount = 0;

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if(line != null)
                {
                    String[] pairs = line.Split(",");
                    Debug.Assert(pairs.Length == 2);
                    String[] pair1 = pairs[0].Split("-");
                    Debug.Assert(pair1.Length == 2);
                    String[] pair2 = pairs[1].Split("-");
                    Debug.Assert(pair2.Length == 2);

                    Section section1= new Section(){Low = int.Parse(pair1[0]),High=int.Parse(pair1[1]) };
                    Section section2= new Section(){Low = int.Parse(pair2[0]),High=int.Parse(pair2[1]) };

                    sections.Add(section1);
                    sections.Add(section2);

                    if(section1.Contains(section2) || section2.Contains(section1))
                    {
                        duplicateCount++;
                        duplicates.Add(section1);
                        duplicates.Add(section2);
                    }

                    if(section1.Overlaps(section2) || section2.Overlaps(section1))
                    {
                        overlapCount++;
                        overlaps.Add(section1);
                        overlaps.Add(section2);
                    }
                }
            }
        }
        int ibreak = 0;
    }
}



public struct Section
{
    public int Low;
    public int High;

    public bool Contains(Section s)
    {
        return (Low <= s.Low && s.High <= High);
    }

    public bool Overlaps(Section s)
    {
        // handle overlap at bottom end
        bool a = Low <= s.Low && High >= s.Low;
        bool b = Low <= s.High && High >= s.High;


        return a || b;
    }


}