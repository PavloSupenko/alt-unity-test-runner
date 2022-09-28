using System;
using System.Linq;
using System.Net.Http;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using Shared.Serialization;


namespace TestsClient.Drivers;

public class AppiumDriver : IAppiumDriver
{
    private enum DriverPlatform
    {
        Android,
        Ios
    };
    
    private readonly IOSDriver<IOSElement>? iosDriver;
    private readonly AndroidDriver<AndroidElement>? androidDriver;
    private readonly DriverPlatform platform;

    public AppiumDriver()
    {
        var httpClient = new HttpClient();
        var content = httpClient.GetStringAsync("http://127.0.0.1:4723/wd/hub/sessions").Result;
        if (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("No Appium sessions found.");
            return;
        }

        var sessionsData = Newtonsoft.Json.JsonConvert.DeserializeObject<AppiumSessionsListData>(content);
        if (sessionsData == null)
        {
            Console.WriteLine($"Sessions data has incorrect format: {content}");
            return;
        }
        
        var sessionIds = sessionsData.Value
            .Select(sessionData => new {DeviceNumber = sessionData.Capabilities[CustomCapabilityType.TargetDeviceId].ToString(), Data = sessionData})
            .ToDictionary(
                deviceSession => deviceSession.DeviceNumber, 
                deviceSession => deviceSession.Data);

        var targetDeviceNumber = Environment.GetEnvironmentVariable(CustomCapabilityType.TargetDeviceId);
        if (targetDeviceNumber == null)
        {
            Console.WriteLine($"No environment variable:{CustomCapabilityType.TargetDeviceId}");
            return;
        }

        if (!sessionIds.ContainsKey(targetDeviceNumber))
        {
            Console.WriteLine($"No suitable session for device number:{targetDeviceNumber}");
            return;
        }

        var requiredSessionData = sessionIds[targetDeviceNumber];
        var requiredSessionId = requiredSessionData.Id;
        var requiredSessionPlatform = requiredSessionData.Capabilities[MobileCapabilityType.PlatformName].ToString();

        switch (requiredSessionPlatform)
        {
            case "Android":
                platform = DriverPlatform.Android;
                androidDriver = new AndroidExistingDriver(new Uri("http://127.0.0.1:4723/wd/hub"), requiredSessionId, TimeSpan.FromSeconds(600));

                //androidDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                //androidDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(600);
                //androidDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
                break;
            case "iOS":
                platform = DriverPlatform.Ios;
                iosDriver = new IosExistingDriver(new Uri("http://127.0.0.1:4723/wd/hub"), requiredSessionId, TimeSpan.FromSeconds(600));
                
                //iosDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
                //iosDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(600);
                //iosDriver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);
                break;
        }
    }

    public IWebDriver? GetAppiumDriver()
    {
        switch (platform)
        {
            case DriverPlatform.Android:
                return androidDriver;
            case DriverPlatform.Ios:
                return iosDriver;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void ActivateApp()
    {
        switch (platform)
        {
            case DriverPlatform.Android:
                //androidDriver.ActivateApp(androidDriver.);
                break;
            case DriverPlatform.Ios:
                //iosDriver.ActivateApp(null);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void BackgroundApp()
    {
        switch (platform)
        {
            case DriverPlatform.Android:
                androidDriver.BackgroundApp();
                break;
            case DriverPlatform.Ios:
                iosDriver.BackgroundApp();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}