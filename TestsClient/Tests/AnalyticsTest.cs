using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium.Appium.iOS;


namespace TestsClient;

public class AnalyticsTest : TestBase
{
    [Test]
    public override void Exit()
    {
    }

    [Test, Timeout(1_000_000)]
    public override void Enter()
    {
        for (var j = 1; j <= 3; j++)
        {
            Console.WriteLine($"Executing run:{j}/3.");
            OpenAndCloseApplication(j <= 2);
        }

        Console.WriteLine("Reinstalling application.");
        RemoveApplication();
    }

    private void OpenAndCloseApplication(bool isPushMessageAllowed)
    {
        Console.WriteLine("Opening application.");
        OpenApplication();

        if (isPushMessageAllowed)
        {
            Console.WriteLine("Waiting for possible push message.");
            Thread.Sleep(TimeSpan.FromSeconds(3));
            SkipPushMessage();   
        }
        
        Console.WriteLine("Waiting for main scene.");
        Thread.Sleep(TimeSpan.FromSeconds(30));
        Console.WriteLine("Closing application.");
        CloseApplication();
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }

    private void OpenApplication()
    {
        var driver = (IOSDriver<IOSElement>) AppiumDriver.GetAppiumDriver();
        driver.LaunchApp();
    }

    private void CloseApplication()
    {
        var driver = (IOSDriver<IOSElement>) AppiumDriver.GetAppiumDriver();
        driver.BackgroundApp();
        Thread.Sleep(TimeSpan.FromSeconds(2));
        driver.TerminateApp("com.mage-app.a.FunnyFoodsLite");
    }

    private void RemoveApplication()
    {
        var driver = (IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver();
        driver.RemoveApp("com.mage-app.a.FunnyFoodsLite");
    }

    private void SkipPushMessage()
    {
        try
        {
            var acceptButton = ((IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver()).FindElementByAccessibilityId("Заборонити");
            
            Console.WriteLine("Skipping push message.");
            acceptButton.Click();
        }
        catch (Exception e)
        {
            Console.WriteLine("No push warning message to skip.");
        }
    }
}