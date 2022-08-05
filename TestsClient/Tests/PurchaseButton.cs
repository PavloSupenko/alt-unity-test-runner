using System.Threading;
using Altom.AltUnityDriver;
using NUnit.Framework;


namespace TestsClient;

public class PurchaseButton : TestBase
{
    [Test]
    public override void Enter()
    {
        var infoButtonObject = AltUnityDriver.FindObject(By.NAME, "Subscribe");
        infoButtonObject.Tap(wait: false);
        SaveScreenshot("Click to open");

        var infoPanel = AltUnityDriver.WaitForObject(By.NAME, "InAppMenuD_Discount(Clone)");
        Thread.Sleep(2000);
        SaveScreenshot("Opened menu");
        Assert.IsNotNull(infoPanel);
    }

    [Test]
    public override void Exit()
    {
        var exitButtonObject = AltUnityDriver.FindObject(By.NAME, "CloseButton");
        exitButtonObject.Tap(wait: false);
        SaveScreenshot("Click to close");

        AltUnityDriver.WaitForObjectNotBePresent(By.NAME, "InAppMenuD_Discount(Clone)");
        SaveScreenshot("Closed menu");
        Assert.Pass();
    }
}