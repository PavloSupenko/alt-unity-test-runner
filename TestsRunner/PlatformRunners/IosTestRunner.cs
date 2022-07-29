using System.Diagnostics;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using Shared.Processes;
using TestsRunner.Arguments;


namespace TestsRunner.PlatformRunners;

public class IosTestRunner : ITestsRunner<IosArguments>
{
    private readonly ProcessRunner processRunner = new();
    private ArgumentsReader<IosArguments> iosArgumentsReader;
    private IOSDriver<IOSElement> driver;
    private Process appiumServerProcess;

    
    public void Initialize(ArgumentsReader<IosArguments> platformArgumentsReader) => 
        iosArgumentsReader = platformArgumentsReader;

    public bool IsDeviceConnected(string deviceNumber, out string deviceId) =>
        GetConnectedDevice(
            deviceNumberString: deviceNumber,
            deviceId: out deviceId);

    public void SetupPortForwarding(string deviceId, string tcpLocalPort, string tcpDevicePort)
    {
        var proxyPath = "iproxy";
        var arguments = $"-u {deviceId} {tcpLocalPort}:{tcpDevicePort}";
        Console.WriteLine($"Executing command: {proxyPath} {arguments}");
        processRunner.StartProcess(proxyPath, arguments);
        Thread.Sleep(TimeSpan.FromSeconds(2));
    }

    public void RunAppiumServer()
    {
        var process = "appium";
        var arguments = $"--address 127.0.0.1 --port 4723 --base-path /wd/hub";
        Console.WriteLine($"Executing command: {process} {arguments}");
        processRunner.StartProcess(process, arguments);
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }

    public void StopAppiumServer() => 
        appiumServerProcess?.Kill();

    public void RunAppiumSession(string deviceId, string buildPath, int sleepSeconds) =>
        RunAppiumSession(
            ipaPath: buildPath,
            deviceId: deviceId,
            deviceName: iosArgumentsReader[IosArguments.DeviceName],
            platformVersion: iosArgumentsReader[IosArguments.PlatformVersion],
            teamId: iosArgumentsReader[IosArguments.TeamId],
            signingId: iosArgumentsReader[IosArguments.SigningId]);

    private bool GetConnectedDevice(string deviceNumberString, out string deviceId)
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

    private void RunAppiumSession(string ipaPath, string deviceId, string deviceName, string platformVersion, string teamId, string signingId)
    {
        AppiumOptions capabilities = new AppiumOptions();
        
        // Disable timeout session disabling
        capabilities.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, 0);
        
        capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "iOS");
        capabilities.AddAdditionalCapability(MobileCapabilityType.App, ipaPath);
        capabilities.AddAdditionalCapability(MobileCapabilityType.AutomationName, "XCUITest");
        capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, deviceName);
        capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, platformVersion);
        capabilities.AddAdditionalCapability(MobileCapabilityType.Udid, deviceId);
        capabilities.AddAdditionalCapability("appium:xcodeOrgId", teamId);
        capabilities.AddAdditionalCapability("appium:xcodeSigningId", signingId);
        capabilities.AddAdditionalCapability("appium:showXcodeLog", true);
        
        driver = new IOSDriver<IOSElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
    }

    public void StopAppiumSession() => 
        driver?.Quit();
}
