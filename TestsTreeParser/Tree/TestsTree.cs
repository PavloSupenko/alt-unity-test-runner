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
}
