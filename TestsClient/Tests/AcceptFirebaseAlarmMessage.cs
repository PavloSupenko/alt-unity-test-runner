using NUnit.Framework;
using OpenQA.Selenium.Appium.iOS;

namespace TestsClient;

public class AcceptFirebaseAlarmMessage : TestBase
{
    [Test]
    public override void Enter()
    {
        var acceptButton = ((IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver()).FindElementByAccessibilityId("OK");
        acceptButton.Click();
    }

    [Test]
    public override void Exit()
    {
        Assert.Pass();
    }
}