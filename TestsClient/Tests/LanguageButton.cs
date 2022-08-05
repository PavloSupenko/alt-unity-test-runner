using System.Threading;
using Altom.AltUnityDriver;
using NUnit.Framework;


namespace TestsClient;

public class LanguageButton : TestBase
{
    [Test]
    public override void Enter()
    {
        TapOnLanguageButton("Click to open", "Opened panel");
    }

    [Test]
    public override void Exit()
    {
        TapOnLanguageButton("Click to close", "Closed panel");
    }

    private void TapOnLanguageButton(string action, string result)
    {
        var languageButton = AltUnityDriver.FindObject(By.NAME, "LangButton");
        Thread.Sleep(2000);

        AltUnityDriver.Tap(languageButton.getScreenPosition());
        SaveScreenshot(action);

        Thread.Sleep(2000);
        SaveScreenshot(result);
        Assert.Pass();
    }
}