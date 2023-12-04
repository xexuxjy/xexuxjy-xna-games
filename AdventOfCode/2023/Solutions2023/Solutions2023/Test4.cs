using System.Diagnostics;

public class Test4 : BaseTest
{
    public override void RunTest()
    {
        TestID = 4;
        IsTestInput = false;
        IsPart2 = true;

        ReadDataFile();

        List<Card> cardList = new List<Card>();
        int total = 0;
        foreach (string dataLine in m_dataFileContents)
        {
            Card c = new Card(dataLine);
            cardList.Add(c);
            total += c.Score;
        }

        DebugOutput("Total score is :"+total);

        if (IsPart2)
        {
            foreach (Card c in cardList)
            {
                c.BuildNestedWins(cardList);
            }

            int part2Total = 0;
            foreach (Card c in cardList)
            {
                c.CalculateNestedWins();
            }

            part2Total = 0;
            foreach (Card c in cardList)
            {
                //DebugOutput("Card " + c.Id + " instance total is : " + c.InstanceCount);
                part2Total += c.InstanceCount;
            }

            DebugOutput("Part 2 Total cards  is :" + part2Total);
        }
    }


    private class Card
    {
        private List<int> Win = new List<int>();
        private List<int> Have = new List<int>();
        public int Score;
        public int Id = 0;
        public int NumWins;
        public List<Card> WinCards = new List<Card>();
        public int InstanceCount = 0;
        
        public Card(string line)
        {
            string[] sections = line.Split(new char[] { ':', '|' },StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            Debug.Assert(sections.Length == 3);

            //string[] ids = sections[0].Split(' 'StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            string idVal = sections[0].Replace("Card", "");
            idVal = idVal.Replace(":", "");
            idVal = idVal.Trim();
            
            Id = int.Parse(idVal);
            
            string[] winNumber = sections[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string a in winNumber)
            {
                Win.Add(int.Parse(a));
            }
            string[] haveNumber = sections[2].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            foreach (string a in haveNumber)
            {
                Have.Add(int.Parse(a));
            }
            
            Win.Sort();
            Have.Sort();

            int winTotal = 0;
            for(int i=0;i<Win.Count;i++)
            {
                if (Have.Contains(Win[i]))
                {
                    NumWins++;
                    if (winTotal == 0)
                    {
                        winTotal = 1;
                    }
                    else
                    {
                        winTotal *= 2;
                    }
                }
            }

            Score = winTotal;


        }

        public void BuildNestedWins(List<Card> cardList)
        {
            for (int i = 0; i < NumWins; ++i)
            {
                WinCards.Add(cardList[Id+i]);
            }
        }

        public void CalculateNestedWins()
        {
            InstanceCount++;
            for (int i = 0; i < WinCards.Count; ++i)
            {
                WinCards[i].CalculateNestedWins();
            }
        }
        
        
    }
    
}