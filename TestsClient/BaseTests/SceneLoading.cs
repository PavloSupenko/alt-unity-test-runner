using System.Threading;
using NUnit.Framework;


namespace TestsClient.BaseTests;

public class SceneLoading : TestBase
{
    public string SceneName { get; set; }

    [Test]
    public override void Enter()
    {
        altUnityDriver.WaitForCurrentSceneToBe(SceneName);
        Thread.Sleep(2000);
        SaveScreenshot(SceneName);
    }

    [Test]
    public override void Exit()
    {
        // Empty for now.
    }
}