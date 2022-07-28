using System.IO;
using Altom.AltUnityDriver;
using NUnit.Framework;


namespace TestsClient;

public abstract class TestBase
{
    public AltUnityDriver altUnityDriver;

    [OneTimeSetUp]
    public void SetUp()
    {
        altUnityDriver = new AltUnityDriver();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        altUnityDriver.Stop();
    }

    public void Initialize(AltUnityDriver driver)
    {
        altUnityDriver = driver;
    }

    public abstract void Enter();
    public abstract void Exit();

    protected void SaveScreenshot(string testName, string actionName)
    {
        var screenshotPath = Path.Combine("..", "..", "artifacts", "Tests", "Screenshots", testName);
        var screenshotName = actionName + ".png";

        if (!Directory.Exists(screenshotPath))
            Directory.CreateDirectory(screenshotPath);

        altUnityDriver.GetPNGScreenshot(Path.Combine(screenshotPath, screenshotName));
    }

    protected void SaveScreenshot(string actionName)
    {
        var testName = GetType().Name;
        SaveScreenshot(testName, actionName);
    }
}