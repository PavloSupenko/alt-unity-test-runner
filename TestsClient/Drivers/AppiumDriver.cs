using System;
using System.Linq;
using System.Net.Http;
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
            .Select(sessionData => new {DeviceNumber = "1", Data = sessionData})
            .ToDictionary(
                deviceSession => deviceSession.DeviceNumber, 
                deviceSession => deviceSession.Data);

        var requiredSessionData = sessionIds["1"];
        var requiredSessionId = requiredSessionData.Id;
        var requiredSessionPlatform = requiredSessionData.Capabilities[MobileCapabilityType.PlatformName];

        switch (requiredSessionPlatform)
        {
            case "Android":
                platform = DriverPlatform.Android;
                androidDriver = new AndroidExistingDriver(new Uri("http://127.0.0.1:4723/wd/hub"), requiredSessionId);
                break;
            case "iOS":
                platform = DriverPlatform.Ios;
                iosDriver = new IosExistingDriver(new Uri("http://127.0.0.1:4723/wd/hub"), requiredSessionId);
                break;
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