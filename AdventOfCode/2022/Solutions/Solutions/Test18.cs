public class Test18 : BaseTest
{
    public List<IntVector3> m_positions = new List<IntVector3>();
    public Dictionary<IntVector3,List<IntVector3>> m_touchingMap =  new Dictionary<IntVector3,List<IntVector3>>();

    public override void RunTest()
    {
        TestID = 18;
        IsTestInput = false;
        ReadDataFile();

        

        foreach(string line in m_dataFileContents)
        {
            string[] tokens = line.Split(',');
            IntVector3 position= new IntVector3(int.Parse(tokens[0]),int.Parse(tokens[1]),int.Parse(tokens[2]));
            m_positions.Add(position);
        }

        for(int i=0;i<m_positions.Count;++i)
        {
            IntVector3 pos = m_positions[i];
            m_touchingMap[pos] = new List<IntVector3>();

            for(int j=i+1;j<m_positions.Count;j++)
            {
                if(DoCubesTouch(pos,m_positions[j]))
                {
                    m_touchingMap[pos].Add(m_positions[j]); 
                }
            }
        }

        int freeSides = 0;
        foreach(IntVector3 pos in m_positions)
        {
            m_debugInfo.Add("Cube : "+pos+" - "+string.Join(' ',m_touchingMap[pos]));
            freeSides += (6-(m_touchingMap[pos].Count*2));
            
        }
        m_debugInfo.Add("Total area : "+freeSides);


        WriteDebugInfo();
    }

    public bool DoCubesTouch(IntVector3 cube1,IntVector3 cube2)
    {
        return cube1.ManhattanDistance(cube2) == 1;
    }

    

    
}