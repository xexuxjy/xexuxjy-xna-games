public class Test8 : BaseTest
{
    int m_gridWidth = 0;
    int m_gridHeight = 0;
    int[,] m_treeGrid = null;


    public override void RunTest()
    {
        string filename = InputPath + "puzzle-8-input.txt";

        List<string> treeLines = new List<string>();

        using (StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open)))
        {
            while (!sr.EndOfStream)
            {
                string? line = sr.ReadLine();
                if (line != null)
                {
                    treeLines.Add(line);
                }
            }
        }

        if (treeLines.Count > 0)
        {
            m_gridWidth = treeLines[0].Length;
            m_gridHeight = treeLines.Count;

            m_treeGrid = new int[m_gridWidth, m_gridHeight];
            for (int i = 0; i < m_gridHeight; i++)
            {
                for (int j = 0; j < m_gridWidth; ++j)
                {
                    m_treeGrid[i, j] = int.Parse("" + treeLines[i][j]);
                }
            }
        }
        
        int maxScore = 0;

        int total = 0;
        for(int y=0;y<m_gridHeight; y++)
        {
            for(int x=0;x<m_gridWidth; x++)
            {
                if(IsVisibleFromOutside(x,y))
                {
                    total++;
                }

                int viewScore = CalcViewingScore(x,y);
                if(viewScore > maxScore)
                {
                    maxScore = viewScore;
                }

            }
        }


        int a = CalcViewingScore(2,3);

        int ibreak = 0;

    }

    public bool IsVisibleFromOutside(int x, int y)
    {
        // is it on the outside?
        if(x == 0 || y == 0 || x == m_gridWidth-1 || y == m_gridHeight-1)
        {
            return true;
        }

        int treeHeight = m_treeGrid[y, x];

        bool visibleLeft = true;
        for (int i = 0; i < x; ++i)
        {
            if (m_treeGrid[y, i] >= treeHeight)
            {
                visibleLeft = false;
                break;
            }
        }

        bool visibleRight = true;
        for (int i = x+1; i < m_gridWidth; ++i)
        {
            if (m_treeGrid[y, i] >= treeHeight)
            {
                visibleRight = false;
                break;
            }
        }

        bool visibleTop = true;
        for (int i = 0; i < y; ++i)
        {
            if (m_treeGrid[i, x] >= treeHeight)
            {
                visibleTop = false;
                break;
            }
        }

        bool visibleBottom = true;
        for (int i = y+1; i < m_gridHeight; ++i)
        {
            if (m_treeGrid[i, x] >= treeHeight)
            {
                visibleBottom = false;
                break;
            }
        }

        return visibleTop || visibleBottom || visibleLeft || visibleRight;

    }


    private bool CountAndStop(int height,int val,ref int count)
    {
        if(val < height)
        {
            count++;
        }
        if(val >= height)
        {
            return true;
        }
        return false;
    }

    public int CalcViewingScore(int x, int y)
    {
        int treeHeight = m_treeGrid[y, x];

        int topScore = 0;
        if(y > 0)
        {
            for(int i = y-1;i>=0;i--)
            {
                if(CountAndStop(treeHeight,m_treeGrid[i,x],ref topScore))
                {
                    // include the tree that stopped us
                    topScore++;
                    break;
                }
            }
        }

        int leftScore = 0;
        if(x > 0)
        {
            for(int i = x-1;i>=0;i--)
            {
                if(CountAndStop(treeHeight,m_treeGrid[y,i],ref leftScore))
                {
                    leftScore++;
                    break;
                }
            }
        }

        int bottomScore = 0;
        if(y > 0)
        {
            for(int i = y+1;i<m_gridHeight;i++)
            {
                if(CountAndStop(treeHeight,m_treeGrid[i,x],ref bottomScore))
                {
                    bottomScore++;
                    break;
                }
            }
        }


        int rightScore = 0;
        if(x < m_gridWidth)
        {
            for(int i = x+1;i<m_gridWidth;i++)
            {
                if(CountAndStop(treeHeight,m_treeGrid[y,i],ref rightScore))
                {
                    rightScore++;
                    break;
                }
            }
        }



        return topScore*bottomScore*leftScore*rightScore;

    }

}
