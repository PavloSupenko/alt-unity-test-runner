﻿using System;
using System.Threading;
using NUnit.Framework;


namespace TestsClient.BaseTests;

public class SceneLoading : TestBase
{
    public string SceneName { get; protected set; }
    public TimeSpan WaitingTime { get; protected set; }

    [Test]
    public override void Enter()
    {
        altUnityDriver.WaitForCurrentSceneToBe(SceneName, WaitingTime.Seconds);
        Thread.Sleep(2000);
        SaveScreenshot(SceneName);
    }

    [Test]
    public override void Exit()
    {
        // Empty for now.
    }
}