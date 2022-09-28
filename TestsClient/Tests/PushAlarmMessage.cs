using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium.Appium.iOS;

namespace TestsClient;

public class PushAlarmMessage: TestBase
{
    [Test]
    public override void Enter()
    {
        Thread.Sleep(TimeSpan.FromSeconds(10));

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

    [Test]
    public override void Exit()
    {
        Assert.Pass();
    }
}