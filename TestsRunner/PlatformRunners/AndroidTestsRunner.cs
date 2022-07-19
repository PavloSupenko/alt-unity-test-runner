using TestsRunner.Processes;

namespace TestsRunner.PlatformRunners;

public class AndroidTestsRunner : ITestsRunner
{
    private ProcessRunner processRunner;

    public bool IsDeviceConnected(string adbPath, string deviceNumberString, out string deviceId)
    {
        var arguments = $"devices";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");

        var resultsStrings = processRunner
            .GetProcessOutput(processRunner.StartProcess(adbPath, arguments)).ToList();

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

    public void ReinstallApplication(string adbPath, string apkPath, string deviceId)
    {
        // var ps = PowerShell.Create();
        // var command = $"&'{adbPath}' -s {deviceId} install -r -g -d \"{apkPath}\"";
        // Console.WriteLine($"Executing command on ps: {command}");
        //
        // ps.AddScript(command);
        // ps.Invoke();
    }

    public void SetupPortForwarding(string adbPath, string tcpPort, string deviceId)
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

    public void RunApplication(string adbPath, string bundle, string deviceId)
    {
        // var ps = PowerShell.Create();
        // var command = $"&'{adbPath}' -s {deviceId} shell am start -n {bundle}/com.unity3d.player.UnityPlayerActivity";
        // Console.WriteLine($"Executing command on ps: {command}");
        //
        // ps.AddScript(command);
        // ps.Invoke();
    }

    public void RunTests(string unityPath, string projectPath, string testsTreeFilePath, string pathToLogFile)
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
}
