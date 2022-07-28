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
            var showDefaults = generalArgumentsReader[GeneralArguments.Defaults].Equals("true");
            ShowHelp(generalArgumentsReader, "==== General parameters: ====", showDefaults);
            ShowHelp(androidArgumentsReader, "==== Android parameters: ====", showDefaults);
            ShowHelp(iosArgumentsReader, "==== iOS parameters: ====", showDefaults);
            Console.WriteLine();
            
            return;
        }

        try
        {
            switch (generalArgumentsReader[GeneralArguments.Platform])
            {
                case "android":
                    ExecuteTests(AndroidTestsRunner, generalArgumentsReader);
                    break;
                case "ios":
                    ExecuteTests(IosTestsRunner, generalArgumentsReader);
                    break;
                default:
                    Console.WriteLine("Platform key needed to run test session. Exit from application.");
                    break;
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("Something went wrong. Exception: {0}", exception.ToString());
        }
        finally
        {
            // todo: write a little bit properly and safe
            switch (generalArgumentsReader[GeneralArguments.Platform])
            {
                case "android":
                    StopSession(AndroidTestsRunner);
                    break;
                case "ios":
                    StopSession(IosTestsRunner);
                    break;
            }
        }
    }

    private static void StopSession<TArgsEnum, TDriver>(ITestsRunner<TArgsEnum, TDriver> testRunner) where TArgsEnum : Enum
    {
        testRunner.StopAppiumServer();
        testRunner.StopAppiumSession();
    }

    private static void ExecuteTests<TArgsEnum, TDriver>(ITestsRunner<TArgsEnum, TDriver> testRunner, ArgumentsReader<GeneralArguments> generalArgumentsReader) where TArgsEnum : Enum
    {
        if (!testRunner.IsDeviceConnected(out var deviceId))
            return;

        if (generalArgumentsReader[GeneralArguments.SkipPortForward].Equals("false"))
            testRunner.SetupPortForwarding(deviceId: deviceId);
        
        if (generalArgumentsReader[GeneralArguments.SkipServerRun].Equals("false"))
            testRunner.RunAppiumServer();

        if (generalArgumentsReader[GeneralArguments.SkipSessionRun].Equals("false"))
            testRunner.RunAppiumSession(deviceId: deviceId, sleepSeconds: 10);

        if (generalArgumentsReader[GeneralArguments.SkipTests].Equals("false"))
        {
            PrintParsedTestsTree(testsTreeJsonPath: generalArgumentsReader[GeneralArguments.TestsTree]);
            testRunner.RunTests();
            DrawTestsTreeResult(testsTreeJsonPath: generalArgumentsReader[GeneralArguments.TestsTree]);
        }
    }

    private static void PrintParsedTestsTree(string testsTreeJsonPath)
    {
        var testsTree = TestsTree.DeserializeTree(testsTreeJsonPath);
        var testsList = testsTree.GetTestsInvocationList();

        foreach (var testName in testsList)
            Console.WriteLine(testName);
    }

    private static void DrawTestsTreeResult(string testsTreeJsonPath)
    {
        // var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        //
        // if (tokenSource.IsCancellationRequested)
        // {
        //     Console.WriteLine("- Not correct file path");
        //     return;
        // }
        //
        // var testsTree = TestsTree.DeserializeTree(testsTreeJsonPath);
        // foreach (var testResult in testsTree.GetTestResultsFromLogFile(logFilePath))
        // {
        //     Console.WriteLine((testResult.Passed ? "+ " : "- ") + testResult.TestNamePrintLine);
        // }
    }

    private static void ShowHelp<TArgsEnum>(ArgumentsReader<TArgsEnum> argumentsReader, string header, bool showDefaults) 
        where TArgsEnum : Enum
    {
        Console.WriteLine(header);
        foreach (TArgsEnum argumentValue in Enum.GetValues(typeof(TArgsEnum)))
        {
            var argumentHelp = argumentsReader.GetHelp(argumentValue);
            Console.WriteLine($"    [{argumentHelp.switchName}]  —  {argumentHelp.description}");
            
            if (showDefaults)
                Console.WriteLine($"        default value:{argumentsReader[argumentValue]}");
        }
    }
}
