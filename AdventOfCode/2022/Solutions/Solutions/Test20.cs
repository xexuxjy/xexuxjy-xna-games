public class Test20 : BaseTest
{

    public List<int> m_numberList = new List<int>();
    public List<int> m_originalNumberList = new List<int>();

    public override void RunTest()
    {
        TestID = 20;
        IsTestInput = true;

        ReadDataFile();
        foreach(string line in m_dataFileContents)
        {
            m_numberList.Add(int.Parse(line));
            m_originalNumberList.Add(int.Parse(line));
        }

        DebugOutput("Initial arrangement:");
        DebugOutput(string.Join(",",m_originalNumberList));

        for(int i=0;i<m_numberList.Count;i++)
        {
            MoveItem(m_originalNumberList[i]);
        }


        int[] answerIndex = new int[]{1000,2000,3000 };
        int answer = 0;
        foreach(int index in answerIndex)
        {
            int score = m_numberList[index%m_numberList.Count];
            DebugOutput("Index "+index+" : "+score);
            answer += score;
        }

        DebugOutput("Answer is : "+answer);

        WriteDebugInfo();

    }

    // going to be a lot of 'ugly' moving of list data as it stands..

    // maybe a cleverer data structure.
    
    // or remove then re-insert?

    public void MoveItem( int offset)
    {
        int index = m_numberList.IndexOf(offset);

        int newIndex = index + offset;

        bool wrappedBack = false;
        bool wrappedForward = false;

        if(offset < 0)
        {
            while(newIndex < 0)
            {
                wrappedBack = true;
                newIndex+=(m_numberList.Count);
            }
        }
        else
        {
            while(newIndex > m_numberList.Count)
            {
                wrappedForward = true;
                newIndex -= m_numberList.Count;
            }
        }

        m_numberList.RemoveAt(index);
        if(offset < 0)
        {
            newIndex--;
        }

        if(wrappedForward)
        {
            newIndex++;
        }


        if(newIndex < 0)
        {
            m_numberList.Add(offset);
        }
        else
        {
            m_numberList.Insert(newIndex,offset); 
        }

        if(offset != 0)
        {
            int prevIndex = newIndex-1;
            if(prevIndex < 0)
            {
                prevIndex += m_numberList.Count;
            }
            int nextIndex = newIndex+1;
            if(newIndex >= m_numberList.Count)
            {
                newIndex -= m_numberList.Count;
            }

            //DebugOutput(String.Format("{0} moves between {1} {2} ",offset,index,newIndex));
            DebugOutput(String.Format("{0} moves between {1} and {2} :",offset,m_numberList[prevIndex],m_numberList[nextIndex],newIndex));
        }
        else
        {
            DebugOutput(String.Format("{0} does not move: ",offset));
        }


        DebugOutput(String.Join(",",m_numberList));
    }

    public int Forward(int offset,int position)
    {
        // the 'gap' at listcount and  -1 is the same position

    }

}