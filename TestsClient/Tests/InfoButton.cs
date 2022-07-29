using System.Threading;
using Altom.AltUnityDriver;
using NUnit.Framework;


namespace TestsClient;

public class InfoButton : TestBase
{
    [Test]
    public override void Enter()
    {
        var infoButtonObject = altUnityDriver.FindObject(By.NAME, "Info");
        Thread.Sleep(2000);

        altUnityDriver.Tap(infoButtonObject.getScreenPosition());
        SaveScreenshot("Click to open");

        var infoPanel = altUnityDriver.WaitForObject(By.NAME, "InfoMenu(Clone)");
        Thread.Sleep(2000);
        SaveScreenshot("Opened menu");
        Assert.IsNotNull(infoPanel);
    }

    [Test]
    public override void Exit()
    {
        var exitButtonObject = altUnityDriver.FindObject(By.NAME, "ExitButton");

        altUnityDriver.Tap(exitButtonObject.getScreenPosition());
        SaveScreenshot("Click to close");

        altUnityDriver.WaitForObjectNotBePresent(By.NAME, "InfoMenu(Clone)");
        SaveScreenshot("Closed menu");
        Assert.Pass();
    }
}