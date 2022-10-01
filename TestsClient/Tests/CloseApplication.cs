using NUnit.Framework;
using OpenQA.Selenium.Appium.iOS;

namespace TestsClient;

/// <summary>
/// This test methods need appium:noReset=true capability to not reinstall application on launch
/// </summary>
public class CloseApplication : TestBase
{
    [Test]
    public override void Enter()
    {
        var driver = (IOSDriver<IOSElement>) AppiumDriver.GetAppiumDriver();
        driver.TerminateApp("com.mage-app.a.FunnyFoodsLite");
    }

    [Test]
    public override void Exit()
    {
        
    }
}