using static Test24;

public class Test25 : BaseTest
{
    public override void RunTest()
    {
        DateTime startTime = DateTime.Now;

        TestID = 25;
        IsTestInput = true;
        IsPart2 = false;
        ReadDataFile();

        double bpElapsed = DateTime.Now.Subtract(startTime).TotalMilliseconds;
        DebugOutput("Elapsed = " + bpElapsed + " ms");


        WriteDebugInfo();
    }

}