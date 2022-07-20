using TestsRunner.Arguments;
using TestsRunner.Processes;
using TestsTreeParser.Tree;


namespace TestsRunner.PlatformRunners;

public class IosTestRunner : ITestsRunner<IosArguments>
{
    private readonly ProcessRunner processRunner = new();
    private ArgumentsReader<GeneralArguments> generalArgumentsReader;
    private ArgumentsReader<IosArguments> iosArgumentsReader;

    public void Initialize(ArgumentsReader<GeneralArguments> generalArgumentsReader, ArgumentsReader<IosArguments> platformArgumentsReader)
    {
        this.generalArgumentsReader = generalArgumentsReader;
        this.iosArgumentsReader = platformArgumentsReader;
    }

    public bool IsDeviceConnected(out string deviceId) =>
        IsDeviceConnected(
            deviceNumberString: generalArgumentsReader[GeneralArguments.RunOnDevice],
            deviceId: out deviceId);

    public void ReinstallApplication(string deviceId) =>
        ReinstallApplication(
            ipaPath: iosArgumentsReader[IosArguments.IpaPath],
            deviceId: deviceId,
            bundle: iosArgumentsReader[IosArguments.Bundle]);

    public void SetupPortForwarding(string deviceId) =>
        SetupPortForwarding(
            tcpPort: iosArgumentsReader[IosArguments.TcpPort],
            deviceId: deviceId);

    public void RunApplication(string deviceId, int sleepSeconds) =>
        RunApplication(
            ipaPath: iosArgumentsReader[IosArguments.IpaPath],
            deviceId: deviceId,
            sleepSeconds: 10);

    public void RunTests() =>
        RunTests(
            unityPath: generalArgumentsReader[GeneralArguments.UnityEditorPath],
            projectPath: generalArgumentsReader[GeneralArguments.ProjectPath],
            testsTreeFilePath: generalArgumentsReader[GeneralArguments.TestsTree],
            pathToLogFile: generalArgumentsReader[GeneralArguments.LogFilePath]);

    private bool IsDeviceConnected(string deviceNumberString, out string deviceId)
    {
        var xcrunPath = "xcrun";
        var arguments = $"xctrace list devices";
        Console.WriteLine($"Executing command: {xcrunPath} {arguments}");

        var resultsStrings = processRunner
            .GetProcessOutput(processRunner.StartProcess(xcrunPath, arguments)).ToList();

        var devices = resultsStrings
            .Skip(2)
            .Where(line => !string.IsNullOrEmpty(line) && !line.Contains("Simulator"))
            .Select(line => line
                .Split(' ')
                .Last()
                .Replace("(", string.Empty)
                .Replace(")", string.Empty))
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

    private void ReinstallApplication(string ipaPath, string deviceId, string bundle)
    {
        var deviceInstaller = "ideviceinstaller";

        var uninstallArguments = $"-u {deviceId} -U {bundle}";
        var installArguments = $"-u {deviceId} -i \"{ipaPath}\"";

        Console.WriteLine($"Executing command: {deviceInstaller} {uninstallArguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(deviceInstaller, uninstallArguments));

        Console.WriteLine($"Executing command: {deviceInstaller} {installArguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(deviceInstaller, installArguments));
    }

    private void SetupPortForwarding(string tcpPort, string deviceId)
    {
        var proxyPath = "iproxy";
        var arguments = $"-u {deviceId} {tcpPort}:{tcpPort}";
        Console.WriteLine($"Executing command: {proxyPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(proxyPath, arguments));
    }

    private void RunApplication(string ipaPath, string deviceId, int sleepSeconds)
    {
        // Unpack .ipa file
        // todo: We need .app file with "developer" export method. For export fastlane can be used and for getting
        // todo: .app instead of .ipa too.

        // Run application
        var iosDeploy = "ios-deploy";

        // -L — just start and exit lldb | -m — just run without installing | -i — device to work with
        var arguments = $"-i {deviceId} -b {appPath} -m -L";
        Console.WriteLine($"Executing command: {iosDeploy} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(iosDeploy, arguments));
        Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));
    }

    private void RunTests(string unityPath, string projectPath, string testsTreeFilePath, string pathToLogFile)
    {
        var testsTree = TestsTree.DeserializeTree(testsTreeFilePath);
        var testsList = testsTree.GetTestsInvocationList();

        var arguments = $"-projectPath \"{projectPath}\" -executeMethod Editor.AltUnity.AltUnityTestRunnerCustom.RunTestFromCommandLine " +
                        $"-tests {string.Join(" ", testsList)} -logFile \"{pathToLogFile}\" -batchmode -quit";

        Console.WriteLine($"Executing command: {unityPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(unityPath, arguments));
    }

    private void ReinstallApplicationUsingIDeviceInstaller(string ipaPath, string deviceId, string bundle)
    {
        var deviceInstaller = "ideviceinstaller";

        var uninstallArguments = $"-u {deviceId} -U {bundle}";
        var installArguments = $"-u {deviceId} -i \"{ipaPath}\"";

        Console.WriteLine($"Executing command: {deviceInstaller} {uninstallArguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(deviceInstaller, uninstallArguments));

        Console.WriteLine($"Executing command: {deviceInstaller} {installArguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(deviceInstaller, installArguments));
    }

    private void ReinstallApplicationUsingIosDeploy(string appPath, string deviceId)
    {
        var iosDeploy = "ios-deploy";

        // -r — remove app before installing to clear all data | -i — device to work with
        var installArguments = $"-i {deviceId} -b \"{appPath}\" -r";
        Console.WriteLine($"Executing command: {iosDeploy} {installArguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(iosDeploy, installArguments));
    }
}
