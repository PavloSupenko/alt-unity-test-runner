using NUnit.Framework;
using OpenQA.Selenium.Appium.iOS;

namespace TestsClient;

public class RemoveApplication : TestBase
{
    [Test]
    public override void Enter()
    {
        var driver = (IOSDriver<IOSElement>)AppiumDriver.GetAppiumDriver();
        driver.RemoveApp("com.mage-app.a.FunnyFoodsLite");
    }

    [Test]
    public override void Exit()
    {
        
    }
}