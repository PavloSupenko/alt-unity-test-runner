﻿using TestsRunner.Arguments;
using TestsRunner.Processes;
using TestsTreeParser.Tree;


namespace TestsRunner.PlatformRunners;

public class AndroidTestsRunner : ITestsRunner<AndroidArguments>
{
    private readonly ProcessRunner processRunner = new();
    private ArgumentsReader<GeneralArguments> generalArgumentsReader;
    private ArgumentsReader<AndroidArguments> androidArgumentsReader;

    public void Initialize(ArgumentsReader<GeneralArguments> generalArgumentsReader, ArgumentsReader<AndroidArguments> platformArgumentsReader)
    {
        this.generalArgumentsReader = generalArgumentsReader;
        this.androidArgumentsReader = platformArgumentsReader;
    }

    public bool IsDeviceConnected(out string deviceId) =>
        IsDeviceConnected(
            adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
            deviceNumberString: generalArgumentsReader[GeneralArguments.RunOnDevice],
            deviceId: out deviceId);

    public void ReinstallApplication(string deviceId) =>
        ReinstallApplication(
            adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
            apkPath: androidArgumentsReader[AndroidArguments.ApkPath],
            deviceId: deviceId);

    public void SetupPortForwarding(string deviceId) =>
        SetupPortForwarding(
            adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
            tcpPort: androidArgumentsReader[AndroidArguments.TcpPort],
            deviceId: deviceId);

    public void RunApplication(string deviceId, int sleepSeconds) =>
        RunApplication(
            adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
            bundle: androidArgumentsReader[AndroidArguments.Bundle],
            deviceId: deviceId,
            sleepSeconds: sleepSeconds);

    public void RunTests() =>
        RunTests(
            unityPath: generalArgumentsReader[GeneralArguments.UnityEditorPath],
            projectPath: generalArgumentsReader[GeneralArguments.ProjectPath],
            testsTreeFilePath: generalArgumentsReader[GeneralArguments.TestsTree],
            pathToLogFile: generalArgumentsReader[GeneralArguments.LogFilePath]);

    private bool IsDeviceConnected(string adbPath, string deviceNumberString, out string deviceId)
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

    private void ReinstallApplication(string adbPath, string apkPath, string deviceId)
    {
        var arguments = $"-s {deviceId} install -r -g -d \"{apkPath}\"";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(adbPath, arguments));
    }

    private void SetupPortForwarding(string adbPath, string tcpPort, string deviceId)
    {
        ResetPortForwarding(adbPath, deviceId);
        InstallPortForwarding(adbPath, tcpPort, deviceId);
    }

    private void ResetPortForwarding(string adbPath, string deviceId)
    {
        var arguments = $"-s {deviceId} forward --remove-all";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(adbPath, arguments));
    }

    private void InstallPortForwarding(string adbPath, string tcpPort, string deviceId)
    {
        var arguments = $"-s {deviceId} forward tcp:{tcpPort} tcp:{tcpPort}";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(adbPath, arguments));
    }

    private void RunApplication(string adbPath, string bundle, string deviceId, int sleepSeconds)
    {
        var arguments = $"-s {deviceId} shell am start -n {bundle}/com.unity3d.player.UnityPlayerActivity";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(adbPath, arguments));
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
}
