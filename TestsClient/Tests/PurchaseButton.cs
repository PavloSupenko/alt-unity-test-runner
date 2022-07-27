using System.Threading;
using Altom.AltUnityDriver;
using NUnit.Framework;
using TestsClient.BaseTests;


namespace TestsClient.Tests;

public class PurchaseButton : TestBase
{
    [Test]
    public override void Enter()
    {
        var infoButtonObject = altUnityDriver.FindObject(By.NAME, "Subscribe");
        infoButtonObject.Tap(wait: false);
        SaveScreenshot("Click to open");

        var infoPanel = altUnityDriver.WaitForObject(By.NAME, "InAppMenuD_Discount(Clone)");
        Thread.Sleep(2000);
        SaveScreenshot("Opened menu");
        Assert.IsNotNull(infoPanel);
    }

    [Test]
    public override void Exit()
    {
        var exitButtonObject = altUnityDriver.FindObject(By.NAME, "CloseButton");
        exitButtonObject.Tap(wait: false);
        SaveScreenshot("Click to close");

        altUnityDriver.WaitForObjectNotBePresent(By.NAME, "InAppMenuD_Discount(Clone)");
        SaveScreenshot("Closed menu");
        Assert.Pass();
    }
}