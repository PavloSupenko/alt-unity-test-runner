using System.Diagnostics;
using TestsTreeParser.Tree;


namespace TestsRunner;

class Program
{
    private static void Main(string[] args)
    {
        var generalArgumentReader =
            new ArgumentsReader<GeneralArguments>(args, ArgumentKeys.GeneralKeys, ArgumentKeys.GeneralDefaults);

        var androidArgumentReader =
            new ArgumentsReader<AndroidArguments>(args, ArgumentKeys.AndroidKeys, ArgumentKeys.AndroidDefaults);

        try
        {
            ExecuteAndroidTests(generalArgumentReader, androidArgumentReader);
        }
        catch (Exception exception)
        {
            Console.WriteLine("Something went wrong. Exception: {0}", exception.ToString());
        }
    }

    private static void ExecuteAndroidTests(ArgumentsReader<GeneralArguments> generalArgumentReader,
        ArgumentsReader<AndroidArguments> androidArgumentReader)
    {
        if (!IsDeviceConnected(
                adbPath: androidArgumentReader[AndroidArguments.AndroidDebugBridgePath],
                deviceNumberString: generalArgumentReader[GeneralArguments.RunOnDevice],
                out string deviceId))
            return;

        if (generalArgumentReader[GeneralArguments.SkipInstall].Equals("false"))
        {
            ReinstallApplication(
                adbPath: androidArgumentReader[AndroidArguments.AndroidDebugBridgePath],
                apkPath: androidArgumentReader[AndroidArguments.ApkPath],
                deviceId: deviceId);
        }

        if (generalArgumentReader[GeneralArguments.SkipPortForward].Equals("false"))
        {
            SetupPortForwarding(
                adbPath: androidArgumentReader[AndroidArguments.AndroidDebugBridgePath],
                tcpPort: androidArgumentReader[AndroidArguments.TcpPort],
                deviceId: deviceId);
        }

        if (generalArgumentReader[GeneralArguments.SkipRun].Equals("false"))
        {
            RunApplication(
                adbPath: androidArgumentReader[AndroidArguments.AndroidDebugBridgePath],
                bundle: androidArgumentReader[AndroidArguments.Bundle],
                deviceId: deviceId);

            Thread.Sleep(10000);
        }

        if (generalArgumentReader[GeneralArguments.SkipTests].Equals("false"))
        {
            RunTests(
                unityPath: generalArgumentReader[GeneralArguments.UnityEditorPath],
                projectPath: generalArgumentReader[GeneralArguments.ProjectPath],
                testsTreeFilePath: generalArgumentReader[GeneralArguments.TestsTree],
                pathToLogFile: generalArgumentReader[GeneralArguments.LogFilePath]);

            DrawTestsTreeResult(
                testsTreeFilePath: generalArgumentReader[GeneralArguments.TestsTree],
                pathToLogFile: generalArgumentReader[GeneralArguments.LogFilePath]);
        }
    }

    private static bool IsDeviceConnected(string adbPath, string deviceNumberString, out string deviceId)
    {
        var arguments = $"devices";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");

        var resultsStrings = GetProcessOutput(StartProcess(adbPath, arguments)).ToList();

        var devices = resultsStrings
            .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("List "))
            .Select(line => line.Split('\t')[0])
            .ToList();

        if (!devices.Any())
        {
            Console.WriteLine("No devices connected to execute tests.");
            deviceId = string.Empty;
            return false;
        }

        var deviceNumber = int.Parse(deviceNumberString);


        Console.WriteLine("Device ID's found:");
        foreach (var id in devices)
            Console.WriteLine(id);

        if (devices.Count < deviceNumber)
        {
            Console.WriteLine("Not enough devices to execute with target device number: {0}.", deviceNumber);
            deviceId = string.Empty;
            return false;
        }

        deviceId = devices[deviceNumber - 1];
        return true;
    }

    private static void ReinstallApplication(string adbPath, string apkPath, string deviceId)
    {
        // var ps = PowerShell.Create();
        // var command = $"&'{adbPath}' -s {deviceId} install -r -g -d \"{apkPath}\"";
        // Console.WriteLine($"Executing command on ps: {command}");
        //
        // ps.AddScript(command);
        // ps.Invoke();
    }

    private static void SetupPortForwarding(string adbPath, string tcpPort, string deviceId)
    {
        // var ps = PowerShell.Create();
        // var command1 = $"&'{adbPath}' -s {deviceId} forward --remove-all";
        // var command2 = $"&'{adbPath}' forward tcp:{tcpPort} tcp:{tcpPort}";
        // Console.WriteLine($"Executing command 1 on ps: {command1}");
        // Console.WriteLine($"Executing command 2 on ps: {command2}");
        //
        // ps.AddScript(command1);
        // ps.AddScript(command2);
        // ps.Invoke();
    }

    private static void RunApplication(string adbPath, string bundle, string deviceId)
    {
        // var ps = PowerShell.Create();
        // var command = $"&'{adbPath}' -s {deviceId} shell am start -n {bundle}/com.unity3d.player.UnityPlayerActivity";
        // Console.WriteLine($"Executing command on ps: {command}");
        //
        // ps.AddScript(command);
        // ps.Invoke();
    }

    private static void RunTests(string unityPath, string projectPath, string testsTreeFilePath, string pathToLogFile)
    {
        // var ps = PowerShell.Create();
        // var testsTree = TestsTree.DeserializeTree(testsTreeFilePath);
        // var testsList = testsTree.GetTestsInvocationList();
        //
        // var command =
        //     $"&'{unityPath}' -projectPath \"{projectPath}\" -executeMethod Editor.AltUnity.AltUnityTestRunnerCustom.RunTestFromCommandLine " +
        //     $"-tests {string.Join(" ", testsList)} -logFile \"{pathToLogFile}\" -batchmode -quit";
        //
        // Console.WriteLine($"Executing command on ps: {command}");
        //
        // ps.AddScript(command);
        // ps.Invoke();
    }

    private static void DrawTestsTreeResult(string testsTreeFilePath, string pathToLogFile)
    {
        // Thread.Sleep(10000);
        //
        // while (!File.Exists(pathToLogFile))
        //     Thread.Sleep(100);
        //
        // Console.WriteLine("Log files detected at path: {0}.", pathToLogFile);
        //
        // while (!IsFileReady(pathToLogFile))
        //     Thread.Sleep(100);
        //
        // Console.WriteLine("Log files filled at path: {0}.", pathToLogFile);
        // Console.WriteLine("Tests passing hierarchy:\n");
        //
        // var testsTree = TestsTree.DeserializeTree(testsTreeFilePath);
        //
        // foreach (var testResult in testsTree.GetTestResultsFromLogFile(pathToLogFile))
        // {
        //     Console.ForegroundColor = testResult.Passed ? ConsoleColor.Yellow : ConsoleColor.DarkRed;
        //     Console.WriteLine(testResult.TestNamePrintLine);
        // }
        //
        // Console.WriteLine();
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

    private static Process StartProcess(string processPath, string arguments)
    {
        var process = new Process();

        var startInfo = new ProcessStartInfo
        {
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Minimized,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            FileName = processPath,
            Arguments = arguments
        };

        process.StartInfo = startInfo;
        process.Start();

        return process;
    }

    private static IEnumerable<string> GetProcessOutput(Process process)
    {
        var output = new List<string>();

        while (!process.StandardOutput.EndOfStream)
        {
            var line = process.StandardOutput.ReadLine();

            if (line.Length > 0)
                output.Add(line);
        }

        process.WaitForExit();
        process.StandardError.ReadToEnd();

        return output;
    }
}
