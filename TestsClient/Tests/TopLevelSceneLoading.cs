using System;


namespace TestsClient;

public class TopLevelSceneLoading : SceneLoading
{
    public TopLevelSceneLoading()
    {
        SceneName = "TopLevelScene";
        WaitingTime = TimeSpan.FromMinutes(1);
    }
}