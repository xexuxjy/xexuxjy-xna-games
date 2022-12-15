using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Test2 : BaseTest
{
    public override void RunTest()
    {
        bool strategy = true;

        string filename = InputPath +" puzzle-2-input.txt";

        List<RPSRound> rounds = new List<RPSRound>();

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if(line != null)
                {
                    RPSRound round = RPSRound.FromString(line,strategy);
                    rounds.Add(round);
                }
            }
        }

        int total = 0;
        foreach (RPSRound round in rounds)
        {
            total+= round.Score;
        }

        int ibreak = 0;
    }

}


public class RPSRound
{
    RPS play;
    RPS response;
    int totalScore;
    string[] tokens= null;

    public static RPSRound FromString(string input,bool strategy=false)
    {
        RPSRound rpsRound = new RPSRound();
        string[] tokens = input.Split(' ');
        rpsRound.tokens = tokens;

        Debug.Assert(tokens.Length == 2);
        if (tokens[0] == "A")
        {
            rpsRound.play = RPS.Rock;
        }
        else if (tokens[0] == "B")
        {
            rpsRound.play = RPS.Paper;
        }
        else if (tokens[0] == "C")
        {
            rpsRound.play = RPS.Scissors;
        }
        else
        {
            Debug.Assert(false, "Unexpected play : " + tokens[0]);
        }

        if (tokens[1] == "X")
        {
            if(strategy)
            {
                rpsRound.response = GetLose(rpsRound.play);
            }
            else
            {
                rpsRound.response = RPS.Rock;
            }
        }
        else if (tokens[1] == "Y")
        {
            if(strategy)
            {
                rpsRound.response = rpsRound.play;
            }
            else
            {
                rpsRound.response = RPS.Paper;
            }
        }
        else if (tokens[1] == "Z")
        {
            if(strategy)
            {
                rpsRound.response = GetWin(rpsRound.play);
            }
            else
            {
            rpsRound.response = RPS.Scissors;
            }
        }
        else
        {
            Debug.Assert(false, "Unexpected response : " + tokens[1]);
        }
        return rpsRound;
    }
    public bool IsWin
    {
        get
        {
            return response == GetWin(play);
        }
    }

    public bool IsLose
    {
        get
        {
            return response == GetLose(play);
        }
    }

    public static RPS GetWin(RPS input)
    {
        if(input == RPS.Rock)
        {
            return RPS.Paper;
        }
        if(input == RPS.Paper)
        {
            return RPS.Scissors;
        }
        return RPS.Rock;
    }

    public static RPS GetLose(RPS input)
    {
        if(input == RPS.Rock)
        {
            return RPS.Scissors;
        }
        if(input == RPS.Paper)
        {
            return RPS.Rock;
        }
        return RPS.Paper;
    }


    public bool IsDraw
    { get { return play == response; } }
    public int Score
    {
        get
        {
            int total = 0;
            if(IsWin)
            {
                total += 6;
            }
            else if (IsDraw)
            {
                total += 3;
            }

            if(response == RPS.Rock)
            {
                total += 1;
            }
            if(response == RPS.Paper)
            {
                total += 2;
            }
            if(response == RPS.Scissors)
            {
                total += 3;
            }
            return total;
        }
    }


}

public enum RPS
{
    Rock,
    Paper,
    Scissors
}