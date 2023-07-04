public class Test25 : BaseTest
{
    public override void RunTest()
    {
        DateTime startTime = DateTime.Now;

        TestID = 25;
        IsTestInput = false;
        IsPart2 = false;
        ReadDataFile();

        //string bob3 = new ElfNumber(3).GetBob;
        //string bob7 = new ElfNumber(7).GetBob;
        //string bob37 = new ElfNumber(37).GetBob;
        //string bob1257 = new ElfNumber(1257).GetBob;
        //string bob81 = new ElfNumber(8).GetBob;
        //string bob201 = new ElfNumber(201).GetBob;

        long total = 0;

        foreach (string s in m_dataFileContents)
        {
            long val = ElfNumber.FromString(s).Decimal;
            string bob = new ElfNumber(val).GetBob;
            DebugOutput($" {s} converted to {val} and back to {bob} matches {s.Equals(bob)}");
            total += val;
        }

        DebugOutput($"Total as decimal {total} and as elf {new ElfNumber(total).GetBob}");


        double bpElapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        DebugOutput("Elapsed = " + bpElapsed + " ms");


        WriteDebugInfo();
    }


    public class ElfNumber
    {
        private long m_internalVal;

        public static ElfNumber FromString(string s)
        {
            long total = 0;
            string reversed = Reverse(s);
            for (int i = 0; i < reversed.Length; ++i)
            {
                int num = ConvertChar(reversed[i]);
                total += (long)(num * (Math.Pow(5, i)));

            }
            return new ElfNumber(total);
        }

        public ElfNumber(long val)
        {
            m_internalVal = val;
        }




        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public long Decimal
        { get { return m_internalVal; } }

        public string GetBob
        {
            get
            {

                string result = "";

                long valueCopy = m_internalVal;
            
                bool overflow = false;

                while (valueCopy > 0)
                {
                    long rem = valueCopy % 5;
                    long div = valueCopy / 5;

                    if (rem == 0 || rem == 1 || rem == 2)
                    {
                        result += ("" + rem);
                    }
                    else if (rem == 3)
                    {
                        result += "=";
                        overflow = true;
                    }
                    else if (rem == 4)
                    {
                        result += "-";
                        overflow = true;
                    }

                    if(overflow)
                    {
                        div += 1;
                        overflow = false;
                    }


                    valueCopy = div;
                }

                string reverse = Reverse(result);
                return reverse;

            }
        }

        public static int ConvertChar(char c)
        {
            int num = 0;
            switch (c)
            {
                case '2':
                    num = 2; break;
                case '1':
                    num = 1; break;
                case '0':
                    num = 0; break;
                case '-':
                    num = -1; break;
                case '=':
                    num = -2; break;
            }
            return num;
        }

    }
}
