using System.Text.RegularExpressions;

public class Test2 : BaseTest
{
    public override void RunTest()
    {
        TestID = 2;
        IsTestInput =false;
        IsPart2 = true;

        ReadDataFile();

        int redLimit = 12;
        int greenLimit = 13;
        int blueLimit = 14;
        
        Regex regex = new Regex(@"\d+ \w+");
        List<Game> allGames = new List<Game>();
        int gameId = 0;
        foreach (string line in m_dataFileContents)
        {
            gameId++;
            Game g = new Game();
            g.GameId = gameId;
            allGames.Add(g);
            string[] tokens = line.Split(new char[] { ':', ';' }, StringSplitOptions.RemoveEmptyEntries);
            // first token is game.
            for(int i=1;i<tokens.Length;++i)
            {
                g.SetData(tokens[i]);
            }
        }

        int total = 0;
        foreach (Game g in allGames)
        {
            if (IsPart2)
            {
                total += g.Power;
            }
            else
            {
                if (g.IsPossible(redLimit, greenLimit, blueLimit))
                {
                    //DebugOutput($"Game {g.GameId} is possible");
                    total += g.GameId;
                }
                
            }
        }
        DebugOutput("Final total is : "+total);
    }

    public class Game
    {
        public int GameId;
        public int Red;
        public int Blue;
        public int Green;

        public int Total => Red + Green + Blue;
        public int Power => Red * Green * Blue;
        
        public bool IsPossible(int redLimit, int greenLimit, int blueLimit)
        {
            return Red <= redLimit && Green <= greenLimit && Blue <= blueLimit;
        }
        
        
        public void SetData(string data)
        {
            string[] results = data.Split(',',StringSplitOptions.TrimEntries);

            foreach (string subresult in results)
            {
                string[] pair = subresult.Split(' ');
                int number = int.Parse(pair[0]);
                string colour = pair[1];

                if (colour == "red")
                {
                    Red = Math.Max(Red,number);
                }
                else if (colour == "green")
                {
                    Green = Math.Max(Green,number);
                }
                if (colour == "blue")
                {
                    Blue = Math.Max(Blue,number);
                }
            }
        }
    }
}

