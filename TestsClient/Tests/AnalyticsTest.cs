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

    [Test]
    public override void Enter()
    {
        for (var i = 1; i <= 20; i++)
        {
            for (var j = 1; j <= 3; j++)
            {
                Console.WriteLine($"Executing session:{i}/20, run:{j}/3.");
                OpenAndCloseApplication(i <= 2);
            }
            
            Console.WriteLine("Reinstalling application.");
            RemoveApplication();
        }
    }

    private void OpenAndCloseApplication(bool isPushMessageAllowed)
    {
        OpenApplication();

        if (isPushMessageAllowed)
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));
            SkipPushMessage();   
        }
        
        Thread.Sleep(TimeSpan.FromSeconds(20));
        CloseApplication();
        Thread.Sleep(TimeSpan.FromSeconds(2));
    }

    private void OpenApplication()
    {
        var driver = (IOSDriver<IOSElement>) AppiumDriver.GetAppiumDriver();
        driver.LaunchApp();
    }

    private void CloseApplication()
    {
        var driver = (IOSDriver<IOSElement>) AppiumDriver.GetAppiumDriver();
        driver.TerminateApp("biz.arrowstar.acad.funnyfood");
    }

    private void RemoveApplication()
    {
        var driver = (IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver();
        driver.RemoveApp("biz.arrowstar.acad.funnyfood");
    }

    private void SkipPushMessage()
    {
        try
        {
            var acceptButton = ((IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver()).FindElementByAccessibilityId("Заборонити");
            acceptButton.Click();
        }
        catch (Exception e)
        {
            Console.WriteLine("No push warning message to skip.");
        }
    }
}