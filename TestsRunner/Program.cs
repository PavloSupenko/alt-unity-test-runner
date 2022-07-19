using TestsRunner.PlatformRunners;
using TestsTreeParser.Tree;

namespace TestsRunner;

class Program
{
    private static readonly ITestsRunner AndroidTestsRunner = new AndroidTestsRunner();

    private static void Main(string[] args)
    {
        var generalArgumentsReader =
            new ArgumentsReader<GeneralArguments>(args, ArgumentKeys.GeneralKeys,
                ArgumentKeys.GeneralDefaults, ArgumentKeys.GeneralDescriptions);

        var androidArgumentsReader =
            new ArgumentsReader<AndroidArguments>(args, ArgumentKeys.AndroidKeys,
                ArgumentKeys.AndroidDefaults, ArgumentKeys.AndroidDescriptions);

        if (generalArgumentsReader[GeneralArguments.Help].Equals("true"))
        {
            ShowHelp(generalArgumentsReader, "==== General parameters: ====");
            ShowHelp(androidArgumentsReader, "\n==== Android parameters: ====");
            Console.WriteLine();
            return;
        }

        try
        {
            ExecuteAndroidTests(generalArgumentsReader, androidArgumentsReader);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Something went wrong. Exception: {0}", exception.ToString());
        }
    }

    private static void ExecuteAndroidTests(ArgumentsReader<GeneralArguments> generalArgumentsReader,
        ArgumentsReader<AndroidArguments> androidArgumentsReader)
    {
        if (!AndroidTestsRunner.IsDeviceConnected(
                adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
                deviceNumberString: generalArgumentsReader[GeneralArguments.RunOnDevice],
                out var deviceId))
            return;

        if (generalArgumentsReader[GeneralArguments.SkipInstall].Equals("false"))
        {
            AndroidTestsRunner.ReinstallApplication(
                adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
                apkPath: androidArgumentsReader[AndroidArguments.ApkPath],
                deviceId: deviceId);
        }

        if (generalArgumentsReader[GeneralArguments.SkipPortForward].Equals("false"))
        {
            AndroidTestsRunner.SetupPortForwarding(
                adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
                tcpPort: androidArgumentsReader[AndroidArguments.TcpPort],
                deviceId: deviceId);
        }

        if (generalArgumentsReader[GeneralArguments.SkipRun].Equals("false"))
        {
            AndroidTestsRunner.RunApplication(
                adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
                bundle: androidArgumentsReader[AndroidArguments.Bundle],
                deviceId: deviceId);

            Thread.Sleep(10000);
        }

        if (generalArgumentsReader[GeneralArguments.SkipTests].Equals("false"))
        {
            ParseTestsTree(
                testsTreeJsonPath: generalArgumentsReader[GeneralArguments.TestsTree]);

            AndroidTestsRunner.RunTests(
                unityPath: generalArgumentsReader[GeneralArguments.UnityEditorPath],
                projectPath: generalArgumentsReader[GeneralArguments.ProjectPath],
                testsTreeFilePath: generalArgumentsReader[GeneralArguments.TestsTree],
                pathToLogFile: generalArgumentsReader[GeneralArguments.LogFilePath]);

            DrawTestsTreeResult(
                testsTreeJsonPath: generalArgumentsReader[GeneralArguments.TestsTree],
                logFilePath: generalArgumentsReader[GeneralArguments.LogFilePath]);
        }
    }

    private static void ParseTestsTree(string testsTreeJsonPath)
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
