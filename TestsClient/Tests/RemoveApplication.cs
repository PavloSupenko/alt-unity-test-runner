using NUnit.Framework;
using OpenQA.Selenium.Appium.iOS;

namespace TestsClient;

public class RemoveApplication : TestBase
{
    [Test]
    public override void Enter()
    {
        var driver = (IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver();
        driver.RemoveApp("biz.arrowstar.acad.funnyfood");
    }

    [Test]
    public override void Exit()
    {
        
    }
}