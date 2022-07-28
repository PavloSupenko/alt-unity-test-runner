using System;
using TestsClient.BaseTests;


namespace TestsClient.Tests;

public class TopLevelSceneLoading : SceneLoading
{
    public TopLevelSceneLoading()
    {
        SceneName = "TopLevelScene";
        WaitingTime = TimeSpan.FromMinutes(1);
    }
}