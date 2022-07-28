using TestsTreeParser.Tree;
namespace TestsTreeParser;

class Program
{
    private static readonly CancellationTokenSource TokenSource = new(TimeSpan.FromSeconds(10));

    private static void Main(string[] args)
    {
        var argumentsCount = args.Length;
        switch (argumentsCount)
        {
            case 1:
                ParseTestsTree(args[0]);
                break;
        }
    }

    private static void ParseTestsTree(string testsTreeJsonPath)
    {
        var testsTree = TestsTree.DeserializeTree(testsTreeJsonPath);
        var testsList = testsTree.GetTestsInvocationList();

        foreach (var testName in testsList) 
            Console.WriteLine(testName);
    }
}
