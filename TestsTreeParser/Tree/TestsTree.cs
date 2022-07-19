using System.Text;
using Newtonsoft.Json;


namespace TestsTreeParser.Tree;

public class TestsTree
{
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string TestsRootNamespace;

    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public List<TestsTreeNode> Tests;

    public List<string> GetTestsInvocationList()
    {
        var order = new List<string>();

        foreach (var test in Tests)
            AddNodeSubtestsToList(test, order);

        return order;
    }

    private void AddNodeSubtestsToList(TestsTreeNode testsTreeNode, List<string> order)
    {
        order.Add($"{TestsRootNamespace}.{testsTreeNode.Test}.Enter");

        if (testsTreeNode.SubTests != null)
            foreach (var test in testsTreeNode.SubTests)
                AddNodeSubtestsToList(test, order);

        order.Add($"{TestsRootNamespace}.{testsTreeNode.Test}.Exit");
    }

    public static TestsTree DeserializeTree(string testsTreeFilePath)
    {
        if (!File.Exists(testsTreeFilePath))
        {
            Console.WriteLine("Tests tree file not found on path: {0}.", testsTreeFilePath);
            return null;
        }

        var fileText = File.ReadAllText(testsTreeFilePath);
        var testsTree = JsonConvert.DeserializeObject<TestsTree>(fileText);

        if (testsTree == null)
        {
            Console.WriteLine("The tests tree file is empty or fill incorrectly.");
            return null;
        }

        return testsTree;
    }

    public List<TestResultData> GetTestResultsFromLogFile(string logFilePath)
    {
        var logText = File.ReadAllLines(logFilePath);
        var testsData = new List<TestResultData>();

        var currentIndent = 1;
        foreach (var testName in GetTestsInvocationList())
        {
            var testPrintLine = new StringBuilder();
            var isTestSuccess = IsTestSuccess(logText, testName);
            var isEnterTest = testName.EndsWith(".Enter");

            if (isEnterTest)
                currentIndent += 4;

            for (int i = 0; i < currentIndent; i++)
                testPrintLine.Append(" ");

            testPrintLine.Append($"|_{testName}");

            if (!isEnterTest)
                currentIndent -= 4;

            testsData.Add(new TestResultData(testPrintLine.ToString(), isTestSuccess));
        }

        return testsData;
    }

    private static bool IsTestSuccess(IList<string> logText, string testName)
    {
        var testResultLine = logText.FirstOrDefault(line => line.Contains(testName) && line.Contains(" TEST "));

        if (string.IsNullOrEmpty(testResultLine))
            return false;

        return testResultLine.Contains("PASSED");
    }
}
