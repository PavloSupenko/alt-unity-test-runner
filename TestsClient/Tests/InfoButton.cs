using System.Threading;
using Altom.AltUnityDriver;
using NUnit.Framework;


namespace TestsClient;

public class InfoButton : TestBase
{
    [Test]
    public override void Enter()
    {
        var infoButtonObject = AltUnityDriver.FindObject(By.NAME, "Info");
        Thread.Sleep(2000);

        AltUnityDriver.Tap(infoButtonObject.getScreenPosition());
        SaveScreenshot("Click to open");

        var infoPanel = AltUnityDriver.WaitForObject(By.NAME, "InfoMenu(Clone)");
        Thread.Sleep(2000);
        SaveScreenshot("Opened menu");
        Assert.IsNotNull(infoPanel);
    }

    [Test]
    public override void Exit()
    {
        var exitButtonObject = AltUnityDriver.FindObject(By.NAME, "ExitButton");

        AltUnityDriver.Tap(exitButtonObject.getScreenPosition());
        SaveScreenshot("Click to close");

        AltUnityDriver.WaitForObjectNotBePresent(By.NAME, "InfoMenu(Clone)");
        SaveScreenshot("Closed menu");
        Assert.Pass();
    }
}