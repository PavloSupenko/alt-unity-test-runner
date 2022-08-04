using System.Diagnostics;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using Shared.Processes;
using TestsRunner.Arguments;


namespace TestsRunner.PlatformRunners;

public class AndroidTestsRunner : ITestsRunner<AndroidArguments>
{
    private readonly ProcessRunner processRunner = new();
    private ArgumentsReader<AndroidArguments> androidArgumentsReader;
    private AndroidDriver<AndroidElement> driver;
    private Process appiumServerProcess;
    
    
    public void Initialize(ArgumentsReader<AndroidArguments> platformArgumentsReader) => 
        androidArgumentsReader = platformArgumentsReader;

    public bool IsDeviceConnected(string deviceNumber, out string deviceId) =>
        IsDeviceConnected(
            adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
            deviceNumberString: deviceNumber,
            deviceId: out deviceId);

    public void SetupPortForwarding(string deviceId, string tcpLocalPort, string tcpDevicePort) =>
        SetupPortForwarding(
            adbPath: androidArgumentsReader[AndroidArguments.AndroidDebugBridgePath],
            tcpLocalPort: tcpLocalPort,
            tcpDevicePort: tcpDevicePort,
            deviceId: deviceId);

    public void RunAppiumServer() =>
        RunAppiumServer(
            javaHome: androidArgumentsReader[AndroidArguments.JavaHomePath],
            androidHome: androidArgumentsReader[AndroidArguments.AndroidHomePath]);

    public void StopAppiumServer() => 
        appiumServerProcess?.Kill();

    public void RunAppiumSession(string deviceId, string buildPath, string bundle, int sleepSeconds)
    {
        InitializeAppiumDriver(buildPath, bundle, deviceId);
        Thread.Sleep(TimeSpan.FromSeconds(sleepSeconds));
    }

    public void StopAppiumSession() => 
        driver?.Quit();

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

    private void SetupPortForwarding(string adbPath, string tcpLocalPort, string tcpDevicePort, string deviceId)
    {
        ResetPortForwarding(adbPath, deviceId);
        InstallPortForwarding(adbPath, tcpLocalPort, tcpDevicePort, deviceId);
    }

    private void ResetPortForwarding(string adbPath, string deviceId)
    {
        var arguments = $"-s {deviceId} forward --remove-all";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(adbPath, arguments));
    }

    private void InstallPortForwarding(string adbPath, string tcpLocalPort, string tcpDevicePort, string deviceId)
    {
        var arguments = $"-s {deviceId} forward tcp:{tcpLocalPort} tcp:{tcpDevicePort}";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");
        processRunner.PrintProcessOutput(processRunner.StartProcess(adbPath, arguments));
    }

    private void RunAppiumServer(string javaHome, string androidHome)
    {
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

    private void InitializeAppiumDriver(string apkPath, string bundle, string deviceId)
    {
        AppiumOptions capabilities = new AppiumOptions();
        
        // Disable timeout session disabling
        capabilities.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, 0);
        capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "Android");
        capabilities.AddAdditionalCapability("appPackage", bundle);
        capabilities.AddAdditionalCapability(MobileCapabilityType.App, apkPath);
        capabilities.AddAdditionalCapability(MobileCapabilityType.Udid, deviceId);
        
        driver = new AndroidDriver<AndroidElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities, TimeSpan.FromMinutes(5));
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
    }
}
