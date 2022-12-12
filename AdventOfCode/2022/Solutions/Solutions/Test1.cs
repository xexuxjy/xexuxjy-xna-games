using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Test1 : BaseTest
{
    public static void RunTest()
    {
        string filename = InputPath +" puzzle-1-input.txt";
        List<Elf> elves = new List<Elf>();

        string line = null;
        Elf elf = new Elf();
        elves.Add(elf);
        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if(line == "")
                {
                    elf = new Elf();
                    elves.Add(elf);
                }
                else
                {
                    int num;
                    if (int.TryParse(line, out num))
                    {
                        elf.AddItem(num);
                    }
                }
            }
        }

        var ordered = elves.OrderByDescending(x=>x.Total);
        Elf maxElf = ordered.First();

        int numElves  = 3;
        int backupTotal = 0;

        var orderedSlice = ordered.Take(numElves);
        foreach(Elf elf1 in orderedSlice)
        {
            backupTotal+=elf1.Total;
        }


        int maxVal = maxElf.Total;
        int ibreak = 0;
    }

}




public class Elf
{
    static int count = 0;

    public Elf()
    {
        Name = "Elf-" + (count++);
    }

    public void AddItem(int val)
    {
        FoodItems.Add(val);
        Total += val;
    }

    public string Name;
    public List<int> FoodItems = new List<int>();
    public int Total;
}
