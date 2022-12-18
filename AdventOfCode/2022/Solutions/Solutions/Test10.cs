using System.Text;
using System.Xml.Linq;
using static Test10;

public class Test10 : BaseTest
{
    CPU m_cpu;
    CRT m_crt;

    public override void RunTest()
    {
        TestID = 10;
        ReadDataFile();
        m_cpu = new CPU();
        m_cpu.Init();
        m_cpu.AddSamplePoint(20);
        m_cpu.AddSamplePoint(60);
        m_cpu.AddSamplePoint(100);
        m_cpu.AddSamplePoint(140);
        m_cpu.AddSamplePoint(180);
        m_cpu.AddSamplePoint(220);

        m_crt = new CRT();
        m_crt.Init(m_cpu);

        foreach(string line in m_dataFileContents)
        {
            while(!m_cpu.ReadyForInstruction)
            {
                m_crt.Tick();
                m_cpu.Tick();
            }
            ParseLine(line);
        }
         
        // finish operation
        while(!m_cpu.ReadyForInstruction)
        {
            m_crt.Tick();
            m_cpu.Tick();
        }
     
        using (StreamWriter sw = new StreamWriter(new FileStream(InputPath + "puzzle-" + TestID + "-debug.txt", FileMode.Create)))
        {
            sw.WriteLine(m_crt.DebugInfo());
        }


        int ibreak = 0;
    }

    public void ParseLine(string line)
    {
        if(line != null)
        {
            string[] tokens = line.Split(" ");
            if(tokens.Length == 1)
            {
                if(tokens[0] == CPU.OP_NOOP)
                {
                    m_cpu.NoOp();
                }
            }
            else if(tokens.Length == 2)
            {
                if(tokens[0].StartsWith(CPU.OP_ADD))
                {
                    int val = int.Parse(tokens[1]);
                    m_cpu.AddRegister(CPU.REGISTER_X,val);
                }
            }
        }
    }

}

public class CRT
{
    private int m_cycleCount = 0;
    private int m_drawingPosition = 0;
    private int m_width;
    private int m_height;


    private char[] m_screenOutput;

    private CPU m_cpu;

    public void Init(CPU cpu)
    {
        m_width = 40;
        m_height = 6;
        m_screenOutput = new char[m_width*m_height];
        m_cpu = cpu;
    }

    public void Tick()
    {
        m_cycleCount++;

        int registerValue = m_cpu.GetRegisterValue(CPU.REGISTER_X);
        if(SpriteVisible(registerValue))
        {
            m_screenOutput[m_drawingPosition] = '#';
        }
        else
        {
            m_screenOutput[m_drawingPosition] = '.';
        }
        m_drawingPosition++;
        if(m_drawingPosition >= m_screenOutput.Length)
        {
            m_drawingPosition = 0;
        }
    }

    public int CycleCount
    {
        get{return m_cycleCount; }
    }

    public int PixelPosition
    {
        get{return m_drawingPosition; }
    }


    public bool SpriteVisible(int registerValue)
    {
        int val = m_drawingPosition ;
        int remainder = val % m_width;

        if(registerValue == remainder)
        {
            return true;
        }
        if(registerValue-1 == remainder)
        {
            return true;
        }
        if(registerValue+1 == remainder)
        {
            return true;
        }
        return false;
    }

    public String DebugInfo()
    {
        StringBuilder sb = new StringBuilder();

        for(int y=0;y<m_height;++y)
        {
            for(int x=0;x<m_width;++x)
            {
                sb.Append(m_screenOutput[(y*m_width) + x]);
            }
            sb.AppendLine();
        }
        sb.AppendLine();
        return sb.ToString();
    }

}


public class CPU
{
    private Dictionary<string,Register> m_registers =  new Dictionary<string, Register>();
    private int m_cycleCount = 0;
    private int m_currentOperationCount = 0;

    public const string REGISTER_X = "X";
    public const string OP_ADD = "add";
    public const string OP_NOOP = "noop";
    public const string OP_EMPTY = "";


    private string m_currentOperation = "";
    private Register m_currentRegister = null;
    private int m_currentOperationValue = 0;

    private List<int> m_samplePoints = new List<int>();


    private int m_totalSampled = 0;

    public int TotalSampled
    {
        get{return m_totalSampled;}
    }

    public void Init()
    {
        m_registers.Clear();
        m_registers.Add(REGISTER_X,new Register(){Name=REGISTER_X,Value = 1 });
    }

    public void AddSamplePoint(int val)
    {
        m_samplePoints.Add(val);
    }

    public bool ReadyForInstruction
    {
        get{return m_currentOperationCount == 0; }
    }


    public int SignalStrength
    {
        get{return m_registers[REGISTER_X].Value * CycleCount; }
    }

    public int CycleCount
    {
        get{return m_cycleCount; }
    }

    public void Tick()
    {
        m_cycleCount++;

        if(m_samplePoints.Contains(m_cycleCount))
        {
            m_totalSampled += SignalStrength;
        }


        m_currentOperationCount--;
        m_currentOperationCount = Math.Max(m_currentOperationCount,0);
        if(m_currentOperationCount == 0 && m_currentOperation != OP_EMPTY)
        {
            if(m_currentOperation == OP_ADD)
            {
                m_currentRegister.Value += m_currentOperationValue;
                m_currentRegister = null;
            }
            m_currentOperation = OP_EMPTY;
        }


    }

    public int GetRegisterValue(string name)
    { return m_registers[name].Value; }

    public void AddRegister(string name,int value)
    {
        if(ReadyForInstruction)
        {
            m_currentOperationCount = 2;
            m_currentOperation = OP_ADD;
            m_currentRegister = m_registers[name];
            m_currentOperationValue = value;
        }

    }

    public void NoOp()
    {
        if(ReadyForInstruction)
        {
            m_currentOperationCount = 1;
            m_currentOperation = OP_NOOP;
        }

    }




}


public class Register
{
    public string Name;
    public int Value;
}