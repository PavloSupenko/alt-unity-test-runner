using System.Diagnostics;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using Shared.Processes;
using TestsRunner.Arguments;
using TestsTreeParser.Tree;


namespace TestsRunner.PlatformRunners;

public class AndroidTestsRunner : ITestsRunner<AndroidArguments, AndroidDriver<AndroidElement>>
{
    private readonly ProcessRunner processRunner = new();
    private ArgumentsReader<GeneralArguments> generalArgumentsReader;
    private ArgumentsReader<AndroidArguments> androidArgumentsReader;
    private AndroidDriver<AndroidElement> driver;
    private Process appiumServerProcess;

    public AndroidDriver<AndroidElement> Driver => driver;

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

    public void SetupPortForwarding(string deviceId) =>
        SetupPortForwarding(
            adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
            tcpPort: androidArgumentsReader[AndroidArguments.TcpPort],
            deviceId: deviceId);

    public void RunAppiumServer() =>
        RunAppiumServer(
            javaHome: androidArgumentsReader[AndroidArguments.JavaHomePath],
            androidHome: androidArgumentsReader[AndroidArguments.AndroidHomePath]);

    public void StopAppiumServer() => 
        appiumServerProcess?.Kill();

    public void RunAppiumSession(string deviceId, int sleepSeconds) =>
        RunAppiumSession(
            apkPath: androidArgumentsReader[AndroidArguments.ApkPath],
            bundle: androidArgumentsReader[AndroidArguments.Bundle],
            sleepSecondsAfterLaunch: sleepSeconds);

    public void StopAppiumSession() => 
        driver?.Quit();

    public void RunTests() =>
        RunTests(
            testsTreeFilePath: generalArgumentsReader[GeneralArguments.TestsTree],
            consoleRunnerPath: generalArgumentsReader[GeneralArguments.NUnitConsoleApplicationPath]);

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

    private void RunAppiumSession(string apkPath, string bundle, int sleepSecondsAfterLaunch)
    {
        InitializeAppiumDriver(apkPath, bundle);
        Thread.Sleep(TimeSpan.FromSeconds(sleepSecondsAfterLaunch));
    }

    private void RunAppiumServer(string javaHome, string androidHome)
    {
        var processRunner = new ProcessRunner();
        var process = "appium";

        var variables = new Dictionary<string, string>()
        {
            ["JAVA_HOME"] = javaHome,
            ["ANDROID_HOME"] = androidHome,
        };
        
        var arguments = $"--address 127.0.0.1 --port 4723 --base-path /wd/hub";
        Console.WriteLine($"Executing command: {process} {arguments}");
        appiumServerProcess = processRunner.StartProcess(process, arguments, variables);
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }

    private void InitializeAppiumDriver(string apkPath, string bundle)
    {
        AppiumOptions capabilities = new AppiumOptions();
        capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
        capabilities.AddAdditionalCapability("appPackage", bundle);
        capabilities.AddAdditionalCapability(MobileCapabilityType.App, apkPath);
        
        // capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, "7.1.1");
        // capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Android Device");
        // capabilities.AddAdditionalCapability("appActivity", "com.instagram.android.activity.MainTabActivity");

        driver = new AndroidDriver<AndroidElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
        driver.LaunchApp();
    }

    private void RunTests(string testsTreeFilePath, string consoleRunnerPath)
    {
        var testsTree = TestsTree.DeserializeTree(testsTreeFilePath);
        var testsList = testsTree.GetTestsInvocationList();

        foreach (var testName in testsList)
        {
            var arguments = $"--test={testName} TestsClient.dll";
            Console.WriteLine($"Executing command: {consoleRunnerPath} {arguments}");
            processRunner.PrintProcessOutput(processRunner.StartProcess(consoleRunnerPath, arguments));
        }
    }
}
