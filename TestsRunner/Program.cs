using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using TestsRunner.Arguments;
using TestsRunner.PlatformRunners;
using TestsTreeParser.Tree;

namespace TestsRunner;

class Program
{
    private static readonly ITestsRunner<AndroidArguments, AndroidDriver<AndroidElement>> AndroidTestsRunner = new AndroidTestsRunner();
    private static readonly ITestsRunner<IosArguments, IOSDriver<IOSElement>> IosTestsRunner = new IosTestRunner();

    private static void Main(string[] args)
    {
        var generalArgumentsReader =
            new ArgumentsReader<GeneralArguments>(args, ArgumentKeys.GeneralKeys,
                ArgumentKeys.GeneralDefaults, ArgumentKeys.GeneralDescriptions);

        var androidArgumentsReader =
            new ArgumentsReader<AndroidArguments>(args, ArgumentKeys.AndroidKeys,
                ArgumentKeys.AndroidDefaults, ArgumentKeys.AndroidDescriptions);

        var iosArgumentsReader =
            new ArgumentsReader<IosArguments>(args, ArgumentKeys.IosKeys,
                ArgumentKeys.IosDefaults, ArgumentKeys.IosDescriptions);

        AndroidTestsRunner.Initialize(generalArgumentsReader, androidArgumentsReader);
        IosTestsRunner.Initialize(generalArgumentsReader, iosArgumentsReader);

        if (generalArgumentsReader[GeneralArguments.Help].Equals("true"))
        {
            ShowHelp(generalArgumentsReader, "==== General parameters: ====");
            ShowHelp(androidArgumentsReader, "==== Android parameters: ====");
            ShowHelp(iosArgumentsReader, "==== iOS parameters: ====");
            Console.WriteLine();
            return;
        }

        try
        {
            var platform = generalArgumentsReader[GeneralArguments.Platform];

            switch (platform)
            {
                case "android":
                    ExecuteTests(AndroidTestsRunner, generalArgumentsReader);
                    break;
                case "ios":
                    ExecuteTests(IosTestsRunner, generalArgumentsReader);
                    break;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("Something went wrong. Exception: {0}", exception.ToString());
        }
    }

    private static void ExecuteTests<TArgsEnum, TDriver>(ITestsRunner<TArgsEnum, TDriver> testRunner, ArgumentsReader<GeneralArguments> generalArgumentsReader) where TArgsEnum : Enum
    {
        if (!testRunner.IsDeviceConnected(out var deviceId))
            return;

        if (generalArgumentsReader[GeneralArguments.SkipPortForward].Equals("false"))
            testRunner.SetupPortForwarding(deviceId: deviceId);

        if (generalArgumentsReader[GeneralArguments.SkipRun].Equals("false"))
            testRunner.RunApplication(deviceId: deviceId, sleepSeconds: 10);

        if (generalArgumentsReader[GeneralArguments.SkipTests].Equals("false"))
        {
            PrintParsedTestsTree(testsTreeJsonPath: generalArgumentsReader[GeneralArguments.TestsTree]);
            testRunner.RunTests();
            DrawTestsTreeResult(
                testsTreeJsonPath: generalArgumentsReader[GeneralArguments.TestsTree],
                logFilePath: generalArgumentsReader[GeneralArguments.LogFilePath]);
        }
    }

    private static void PrintParsedTestsTree(string testsTreeJsonPath)
    {
        var testsTree = TestsTree.DeserializeTree(testsTreeJsonPath);
        var testsList = testsTree.GetTestsInvocationList();

        foreach (var testName in testsList)
            Console.WriteLine(testName);
    }

    private static void DrawTestsTreeResult(string testsTreeJsonPath, string logFilePath)
    {
        var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        while (!File.Exists(logFilePath) && !tokenSource.IsCancellationRequested)
            Thread.Sleep(100);

        while (!IsFileReady(logFilePath) && !tokenSource.IsCancellationRequested)
            Thread.Sleep(100);

        if (tokenSource.IsCancellationRequested)
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
            using var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
            return inputStream.Length > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private static void ShowHelp<TArgsEnum>(ArgumentsReader<TArgsEnum> argumentsReader, string header) where TArgsEnum : Enum
    {
        Console.WriteLine(header);
        foreach (TArgsEnum argumentValue in Enum.GetValues(typeof(TArgsEnum)))
        {
            var argumentHelp = argumentsReader.GetHelp(argumentValue);
            Console.WriteLine($"    [{argumentHelp.switchName}]  —  {argumentHelp.description}");
            Console.WriteLine($"        value:{argumentsReader[argumentValue]}");
        }
    }
}
