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
            case 2:
                CheckTestsPassingStatusFromLogFile(args[0], args[1]);
                break;
            default:
                Console.WriteLine(argumentsCount == 0
                    ? "No arguments."
                    : "No compatible functionalities for more than 2 arguments.");

                break;
        }
    }

    private static void ParseTestsTree(string testsTreeJsonPath)
    {
        var testsTree = TestsTree.DeserializeTree(testsTreeJsonPath);
        var testsList = testsTree.GetTestsInvocationList();

        foreach (var testName in testsList) Console.WriteLine(testName);
    }

    private static void CheckTestsPassingStatusFromLogFile(string testsTreeJsonPath, string logFilePath)
    {
        while (!File.Exists(logFilePath) && !TokenSource.IsCancellationRequested)
            Thread.Sleep(100);

        while (!IsFileReady(logFilePath) && !TokenSource.IsCancellationRequested)
            Thread.Sleep(100);

        if (TokenSource.IsCancellationRequested)
        {
            Console.WriteLine("- Not correct file path");
            return;
        }

        var testsTree = TestsTree.DeserializeTree(testsTreeJsonPath);
        foreach (var testResult in testsTree.GetTestResultsFromLogFile(logFilePath))
        {
            Console.WriteLine((testResult.Passed ? "+ " : "- ") + testResult.TestNamePrintLine);
        }
    }

    private static bool IsFileReady(string filename)
    {
        try
        {
            using (FileStream inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None))
                return inputStream.Length > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
