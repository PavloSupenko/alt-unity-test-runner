using System.Text;
using Shared.Arguments;
using Shared.Processes;
using Shared.Serialization;
using TestsRunner.Arguments;
using TestsRunner.PlatformRunners;
using TestsTreeParser.Tree;

namespace TestsRunner;

class Program
{
    private static ITestsRunner testsRunner;
    private static ArgumentsReader<GeneralArguments> generalArgumentsReader;

    private static void Main(string[] args)
    {
        generalArgumentsReader = new ArgumentsReader<GeneralArguments>(args, ArgumentKeys.GeneralKeys,
            ArgumentKeys.GeneralDefaults, ArgumentKeys.GeneralDescriptions);

        var androidArgumentsReader =
            new ArgumentsReader<AndroidArguments>(args, ArgumentKeys.AndroidKeys,
                ArgumentKeys.AndroidDefaults, ArgumentKeys.AndroidDescriptions);

        var iosArgumentsReader =
            new ArgumentsReader<IosArguments>(args, ArgumentKeys.IosKeys,
                ArgumentKeys.IosDefaults, ArgumentKeys.IosDescriptions);

        if (TryShowHelp(androidArgumentsReader, iosArgumentsReader)) 
            return;

        try
        {
            testsRunner = CreateTestsRunnerForCurrentPlatform(androidArgumentsReader, iosArgumentsReader);
            ExecuteTests();
        }
        catch (Exception exception)
        {
            Console.WriteLine("Something went wrong. Exception: {0}", exception.ToString());
        }
        finally
        {
            StopSession();
        }
    }

    private static ITestsRunner CreateTestsRunnerForCurrentPlatform(ArgumentsReader<AndroidArguments> androidArgumentsReader,
        ArgumentsReader<IosArguments> iosArgumentsReader)
    {
        switch (generalArgumentsReader[GeneralArguments.Platform])
        {
            case "android":
                var androidTestsRunner = new AndroidTestsRunner();
                androidTestsRunner.Initialize(androidArgumentsReader);
                return androidTestsRunner;
            case "ios":
                var iosTestRunner = new IosTestRunner();
                iosTestRunner.Initialize(iosArgumentsReader);
                return iosTestRunner;
            default:
                throw new KeyNotFoundException("Platform key needed to run tests session. Exit from application.");
        }
    }

    private static bool TryShowHelp(ArgumentsReader<AndroidArguments> androidArgumentsReader, ArgumentsReader<IosArguments> iosArgumentsReader)
    {
        if (!generalArgumentsReader.IsTrue(GeneralArguments.Help)) 
            return false;

        var showDefaults = generalArgumentsReader.IsTrue(GeneralArguments.Defaults);
        ShowHelp(generalArgumentsReader, "==== General parameters: ====", showDefaults);
        ShowHelp(androidArgumentsReader, "==== Android parameters: ====", showDefaults);
        ShowHelp(iosArgumentsReader, "==== iOS parameters: ====", showDefaults);
        Console.WriteLine();

        return true;
    }

    private static void StopSession()
    {
        Console.WriteLine("Closing Appium session and server.");
        testsRunner.StopAppiumSession();
        testsRunner.StopAppiumServer();
    }

    private static void ExecuteTests()
    {
        if (TryGetConnectedDevice(out var deviceId)) 
            return;

        TrySetupPortForwarding(deviceId);
        TryRunAppiumServer();
        TryRunAppiumSession(deviceId);
        TryRunTests(deviceId);
    }

    private static void TryRunTests(string deviceId)
    {
        if (generalArgumentsReader[GeneralArguments.SkipTests].Equals("false"))
        {
            Console.WriteLine("Running tests:");
            PrintParsedTestsTree(testsTreeJsonPath: generalArgumentsReader[GeneralArguments.TestsTree]);

            RunTests(
                testsTreeFilePath: generalArgumentsReader[GeneralArguments.TestsTree],
                consoleRunnerPath: generalArgumentsReader[GeneralArguments.NUnitConsoleApplicationPath],
                systemLog: generalArgumentsReader[GeneralArguments.TestSystemOutputLogFilePath],
                testAssembly: generalArgumentsReader[GeneralArguments.NUnitTestsAssemblyPath],
                deviceId: deviceId);
        }
    }

    private static void TryRunAppiumSession(string deviceId)
    {
        if (generalArgumentsReader[GeneralArguments.SkipSessionRun].Equals("false"))
        {
            Console.WriteLine("Running Appium session:");

            testsRunner.RunAppiumSession(
                deviceId: deviceId,
                bundle: generalArgumentsReader[GeneralArguments.Bundle],
                buildPath: generalArgumentsReader[GeneralArguments.BuildPath]);
        }
    }

    private static void TryRunAppiumServer()
    {
        if (generalArgumentsReader[GeneralArguments.SkipServerRun].Equals("false"))
        {
            Console.WriteLine("Running Appium server:");
            testsRunner.RunAppiumServer(hostPlatform: generalArgumentsReader[GeneralArguments.HostPlatform]);
        }
    }

    private static void TrySetupPortForwarding(string deviceId)
    {
        if (!generalArgumentsReader[GeneralArguments.SkipPortForward].Equals("false")) 
            return;

        Console.WriteLine("Setup port forwarding:");

        testsRunner.SetupPortForwarding(
            tcpLocalPort: generalArgumentsReader[GeneralArguments.LocalPort],
            tcpDevicePort: generalArgumentsReader[GeneralArguments.DevicePort],
            deviceId: deviceId);
    }

    private static bool TryGetConnectedDevice(out string deviceId)
    {
        if (!generalArgumentsReader.IsExist(GeneralArguments.AutoDetectDevice))
        {
            if (!generalArgumentsReader.IsExist(GeneralArguments.DeviceId))
                throw new KeyNotFoundException("Auto detection disabled and device id not set. Use one of this options.");

            deviceId = generalArgumentsReader[GeneralArguments.DeviceId];
            Console.WriteLine($"Auto detection device disabled. Device id: {deviceId} will be used.");
        }
        else if (!testsRunner.IsDeviceConnected(generalArgumentsReader[GeneralArguments.AutoDetectDevice], out deviceId))
        {
            return true;
        }

        return false;
    }

    private static void PrintParsedTestsTree(string testsTreeJsonPath)
    {
        var testsTree = TestsTree.DeserializeTree(testsTreeJsonPath);
        var testsList = testsTree.GetTestsInvocationList();

        Console.WriteLine("\nOrder of tests to be run parsed from tree file:");
        foreach (var testName in testsList)
            Console.WriteLine(testName);
        
        Console.WriteLine();
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

    private static void RunTests(string testsTreeFilePath, string consoleRunnerPath,
        string systemLog, string testAssembly, string deviceId)
    {
        ProcessRunner processRunner = new ProcessRunner();
        TestsTree testsTree = TestsTree.DeserializeTree(testsTreeFilePath);
        List<string> testsList = testsTree.GetTestsInvocationList();
        Dictionary<string, bool> testsStatus = new Dictionary<string, bool>();

        using StreamWriter sw = new StreamWriter(systemLog,
            new FileStreamOptions()
            {
                Access = FileAccess.Write,
                Mode = FileMode.OpenOrCreate
            });

        foreach (var testName in testsList)
        {
            Console.WriteLine($"Executing test: {testName}");
            var arguments = $"--test={testName} --teamcity {testAssembly}";
            var systemOutput = processRunner
                .GetProcessOutput(processRunner.StartProcess(consoleRunnerPath, arguments, 
                    new Dictionary<string, string>()
                    {
                        [CustomCapabilityType.TargetDeviceId] = deviceId,
                    }))
                .ToList();

            foreach (var outputLine in systemOutput) 
                sw.WriteLine(outputLine);
            
            sw.WriteLine();
            testsStatus.Add(testName, IsTestSuccess(systemOutput));
        }
        
        sw.Close();
        
        DrawTestsTreeResult(TestsTree.DeserializeTree(testsTreeFilePath), testsStatus);
    }

    private static bool IsTestSuccess(IEnumerable<string> logText)
    {
        var testResultLine = logText.FirstOrDefault(line => line.Contains("Overall result"));

        if (string.IsNullOrEmpty(testResultLine))
            return false;

        return testResultLine.Contains("Passed");
    }

    private static void DrawTestsTreeResult(TestsTree tree, Dictionary<string, bool> testsSuccessStatus)
    {
        Console.WriteLine("\nTests results:");
        
        var currentIndent = 1;
        foreach (var testName in tree.GetTestsInvocationList())
        {
            var testPrintLine = new StringBuilder();
            var isTestSuccess = testsSuccessStatus[testName];
            var isEnterTest = testName.EndsWith(".Enter");

            if (isEnterTest)
                currentIndent += 4;

            testPrintLine.Append(isTestSuccess ? "+ " : "- ");
            for (int i = 0; i < currentIndent; i++)
                testPrintLine.Append(" ");

            testPrintLine.Append($"|_{testName}");

            if (!isEnterTest)
                currentIndent -= 4;
            
            Console.WriteLine(testPrintLine.ToString());
        }
        
        Console.WriteLine();
    }
}