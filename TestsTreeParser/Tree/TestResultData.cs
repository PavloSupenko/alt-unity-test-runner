namespace TestsTreeParser.Tree;

public class TestResultData
{
    public string TestNamePrintLine { get; set; }
    public bool Passed { get; set; }

    public TestResultData(string testNamePrintLine, bool passed)
    {
        TestNamePrintLine = testNamePrintLine;
        Passed = passed;
    }
}
