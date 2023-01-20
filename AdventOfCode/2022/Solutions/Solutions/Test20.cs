using static System.Runtime.InteropServices.JavaScript.JSType;

public class Test20 : BaseTest
{

    public List<(int,int)> m_numberList = new List<(int,int)>();

    public override void RunTest()
    {
        TestID = 20;
        IsTestInput = false;

        ReadDataFile();
        int count = 0;
        foreach (string line in m_dataFileContents)
        {
            m_numberList.Add(((count++),int.Parse(line)));
        }

        // need to handle multiples of the same number.
        // not even sure we can do that unless we tie them togetehr as things 
        // will get re-ordered.
        // knew it was too easy....
        // wrap an object and it's original position together should work.



        DebugOutput("Initial arrangement:",true);
        DebugOutput(string.Join(",", m_numberList.Select(x=>x.Item2)),true);

        for (int i = 0; i < m_numberList.Count; i++)
        {
            MoveItem(i);
        }


        int[] answerIndex = new int[] { 1000, 2000, 3000 };
        int answer = 0;
        int zeroIndex = m_numberList.FindIndex(x=>x.Item2 == 0);
        foreach (int index in answerIndex)
        {
            int adjustedIndex = zeroIndex + index;
            //int newIndex = GetWrappedValue(index);
            //int score = m_numberList[newIndex];
            int score = m_numberList[adjustedIndex%m_numberList.Count].Item2;
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

        int newIndex = index + item.Item2;
        newIndex = GetWrappedValue(newIndex);

        if(newIndex < 0)
        {
            int ibreak =0 ;
        }

        m_numberList.RemoveAt(index);

        if (newIndex > m_numberList.Count)
        {
            newIndex = 0;
            //newIndex = m_numberList.Count;
        }


        m_numberList.Insert(newIndex, item);

        if (item.Item2 != 0)
        {
            int prevIndex = newIndex - 1;
            if (prevIndex < 0)
            {
                prevIndex += m_numberList.Count;
            }
            int nextIndex = newIndex + 1;
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

    public int GetWrappedValue(int newIndex)
    {
        int original = newIndex;
        bool wrappedBack = false;
        bool wrappedForward = false;
        bool addToEnd = false;

        if(original == 9999)
        {
            int ibreak = 0;
        }

        if (newIndex == 0)
        {
            addToEnd = true;
            newIndex = m_numberList.Count - 1;
        }
        else
        {
            while (newIndex < 0)
            {
                wrappedBack = true;
                newIndex += (m_numberList.Count);
            }
            while (newIndex >= m_numberList.Count)
            {
                wrappedForward = true;
                newIndex -= m_numberList.Count;
            }

            if (wrappedBack)
            {
                newIndex--;
            }
            if (wrappedForward)
            {
                newIndex++;
            }
        }

        if(original == newIndex)
        {
            int ibreak = 0;
        }

        if(newIndex == m_numberList.Count)
        {
            newIndex--;
        }
        if (newIndex < 0)
        {
            int ibreak =0 ;
            newIndex = 0;//m_numberList.Count - 1;
        }
        return newIndex;

    }


}