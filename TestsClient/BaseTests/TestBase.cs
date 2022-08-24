using System.IO;
using Altom.AltUnityDriver;
using NUnit.Framework;
using OpenQA.Selenium;
using TestsClient.Drivers;


namespace TestsClient;

public abstract class TestBase
{
    protected AltUnityDriver AltUnityDriver;
    protected AppiumDriver AppiumDriver;

    [OneTimeSetUp]
    public void SetUp()
    {
        AppiumDriver = new AppiumDriver();
        AltUnityDriver = new AltUnityDriver();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        AltUnityDriver.Stop();
    }

    public abstract void Enter();
    public abstract void Exit();

    protected void SaveScreenshot(string actionName)
    {
        var testName = GetType().Name;
        SaveScreenshot(testName, actionName);
    }

    private void SaveScreenshot(string testName, string actionName)
    {
        var screenshotPath = Path.Combine("..", "..", "artifacts", "Tests", "Screenshots", testName);
        var screenshotName = actionName + ".png";
        
        if (!Directory.Exists(screenshotPath))
            Directory.CreateDirectory(screenshotPath);

        var screenshotFilePath = Path.Combine(screenshotPath, screenshotName);
        Screenshot screenshot = ((ITakesScreenshot)AppiumDriver.GetAppiumDriver()).GetScreenshot();
        screenshot.SaveAsFile(screenshotFilePath, ScreenshotImageFormat.Png);
    }
}