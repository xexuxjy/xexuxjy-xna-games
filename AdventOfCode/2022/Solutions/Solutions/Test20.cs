using static System.Runtime.InteropServices.JavaScript.JSType;

public class Test20 : BaseTest
{

    public List<(int,long)> m_numberList = new List<(int,long)>();

    public override void RunTest()
    {
        TestID = 20;
        IsTestInput = false ;
        IsPart2 = true;

        ReadDataFile();
        int count = 0;
        long decryptionKey = IsPart2?811589153:1;

        foreach (string line in m_dataFileContents)
        {
            m_numberList.Add(((count++),long.Parse(line)*decryptionKey));
        }

        // need to handle multiples of the same number.
        // not even sure we can do that unless we tie them togetehr as things 
        // will get re-ordered.
        // knew it was too easy....
        // wrap an object and it's original position together should work.


        DebugOutput("Initial arrangement:",true);
        DebugOutput(string.Join(",", m_numberList.Select(x=>x.Item2)),true);

        int numMixes = IsPart2?10:1;

        for(int mix = 0; mix < numMixes;mix++)
        {
            for (int i = 0; i < m_numberList.Count; i++)
            {
                MoveItem(i);
            }
        }

        int[] answerIndex = new int[] { 1000, 2000, 3000 };
        long answer = 0;
        int zeroIndex = m_numberList.FindIndex(x=>x.Item2 == 0);
        foreach (int index in answerIndex)
        {
            int adjustedIndex = zeroIndex + index;
            long score = m_numberList[adjustedIndex%m_numberList.Count].Item2;
            DebugOutput("Index " + index + " : " + score);
            answer += score;
        }

        DebugOutput("Answer is : " + answer);

        WriteDebugInfo();

    }

    // going to be a lot of 'ugly' moving of list data as it stands..

    // maybe a cleverer data structure.

    // or remove then re-insert?

    public void MoveItem(int position)
    {
        var item = m_numberList.Find(x=>x.Item1== position);
        int index = m_numberList.IndexOf(item);

        long newIndex = index + item.Item2;
        newIndex = GetWrappedValue(newIndex);

        m_numberList.RemoveAt(index);


        m_numberList.Insert((int)newIndex, item);

        if (item.Item2 != 0)
        {
            int prevIndex = (int)newIndex - 1;
            if (prevIndex < 0)
            {
                prevIndex += m_numberList.Count;
            }
            int nextIndex = (int)newIndex + 1;
            if (nextIndex >= m_numberList.Count)
            {
                nextIndex -= m_numberList.Count;
            }
            DebugOutput(string.Format("{0} moves between {1} and {2} :", item.Item2, m_numberList[prevIndex].Item2, m_numberList[nextIndex].Item2, newIndex),true);
        }
        else
        {
            DebugOutput(string.Format("{0} does not move: ",item.Item2), true);
        }


        DebugOutput(string.Join(",", m_numberList.Select(x=>x.Item2)),true);
    }


    public long GetWrappedValue(long newIndex)
    {
        newIndex = (newIndex) % (m_numberList.Count - 1);

        if (newIndex < 0)
        {
            newIndex = m_numberList.Count + newIndex - 1;
        }
        return newIndex;

    }



}