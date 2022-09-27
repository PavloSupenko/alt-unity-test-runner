using NUnit.Framework;
using OpenQA.Selenium.Appium.iOS;

namespace TestsClient;

public class PushAlarmMessage: TestBase
{
    [Test]
    public override void Enter()
    {
        var acceptButton = ((IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver()).FindElementByAccessibilityId("Заборонити");
        acceptButton.Click();
    }

    [Test]
    public override void Exit()
    {
        Assert.Pass();
    }
}