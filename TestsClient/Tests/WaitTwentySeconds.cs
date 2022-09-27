using System;
using System.Threading;
using NUnit.Framework;


namespace TestsClient;

public class WaitTwentySeconds : TestBase
{
    [Test]
    public override void Enter()
    {
        Thread.Sleep(TimeSpan.FromSeconds(20));
    }

    [Test]
    public override void Exit()
    {
        
    }
}