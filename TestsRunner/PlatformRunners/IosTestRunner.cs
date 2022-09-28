using System.Diagnostics;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Remote;
using Shared.Processes;
using Shared.Serialization;
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

    public void RunAppiumServer(string hostPlatform)
    {
        var process = "appium";
        var arguments = $"--address 127.0.0.1 --port 4723 --base-path /wd/hub";
        Console.WriteLine($"Executing command: {process} {arguments}");
        processRunner.StartProcess(process, arguments);
        Thread.Sleep(TimeSpan.FromSeconds(60));
    }

    public void StopAppiumServer() => 
        appiumServerProcess?.Kill();

    public void RunAppiumSession(string deviceId, string buildPath, string bundle) =>
        RunAppiumSession(
            ipaPath: buildPath,
            deviceId: deviceId,
            bundle: bundle,
            teamId: iosArgumentsReader[IosArguments.TeamId],
            signingId: iosArgumentsReader[IosArguments.SigningId]);

    private bool GetConnectedDevice(string deviceNumberString, out string deviceId)
    {
        var xcrunPath = "xcrun";
        var arguments = $"xctrace list devices";
        Console.WriteLine($"Executing command: {xcrunPath} {arguments}");

        var resultStrings = processRunner
            .GetProcessOutput(processRunner.StartProcess(xcrunPath, arguments)).ToList();

        var devices = resultStrings
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

    private void GetDeviceInfo(out string deviceName, out string platformVersion)
    {
        var deviceInfo = "ideviceinfo";
        var arguments = string.Empty;
        Console.WriteLine($"Executing command: {deviceInfo} {arguments}");

        var resultStrings = processRunner.GetProcessOutput(processRunner.StartProcess(deviceInfo, arguments)).ToList();

        deviceName = resultStrings.First(line => line.Contains("DeviceName")).Replace("DeviceName: ", string.Empty);
        platformVersion = resultStrings.First(line => line.Contains("ProductVersion")).Replace("ProductVersion: ", string.Empty);
        
        Console.WriteLine($"Found device name: {deviceName}");
        Console.WriteLine($"Found device version: {platformVersion}");
    }

    private void RunAppiumSession(string ipaPath, string bundle, string deviceId, string teamId, string signingId)
    {
        GetDeviceInfo(out var deviceName, out var platformVersion);
        AppiumOptions capabilities = new AppiumOptions();
        
        // Disable timeout session disabling
        capabilities.AddAdditionalCapability(MobileCapabilityType.NewCommandTimeout, 0);
        capabilities.AddAdditionalCapability("appium:appPushTimeout", 1_800_000);
        capabilities.AddAdditionalCapability("appium:wdaConnectionTimeout", 1_800_000);
        capabilities.AddAdditionalCapability("appium:wdaStartupRetryInterval", 1_800_000);
        capabilities.AddAdditionalCapability("appium:waitForIdleTimeout", 1_800);
        
        capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformName, "iOS");
        capabilities.AddAdditionalCapability(MobileCapabilityType.App, ipaPath);
        capabilities.AddAdditionalCapability(MobileCapabilityType.AutomationName, "XCUITest");
        capabilities.AddAdditionalCapability(MobileCapabilityType.DeviceName, deviceName);
        capabilities.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, platformVersion);
        capabilities.AddAdditionalCapability(MobileCapabilityType.Udid, deviceId);
        capabilities.AddAdditionalCapability(IOSMobileCapabilityType.BundleId, bundle);
        capabilities.AddAdditionalCapability("appium:xcodeOrgId", teamId);
        capabilities.AddAdditionalCapability("appium:xcodeSigningId", signingId);
        capabilities.AddAdditionalCapability("appium:showXcodeLog", true);
        capabilities.AddAdditionalCapability(CustomCapabilityType.TargetDeviceId, deviceId);
        
        capabilities.AddAdditionalCapability("appium:noReset", true);
        
        driver = new IOSDriver<IOSElement>(new Uri("http://127.0.0.1:4723/wd/hub"), capabilities, TimeSpan.FromMinutes(30));
        // driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMinutes(30);
        // driver.Manage().Timeouts().PageLoad = TimeSpan.FromMinutes(30);
        // driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromMinutes(30);
    }

    public void StopAppiumSession()
    {
        driver?.Quit();
    }
}
