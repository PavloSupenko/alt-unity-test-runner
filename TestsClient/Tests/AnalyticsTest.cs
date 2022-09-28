using System;
using System.Threading;
using NUnit.Framework;


namespace TestsClient;

public class AnalyticsTest : TestBase
{
    private readonly OpenApplication openApplicationTest = new();
    private readonly CloseApplication closeApplicationTest = new();
    private readonly PushAlarmMessage pushAlarmMessageTest = new();
    
    
    [Test]
    public override void Enter()
    {
        for (var i = 1; i <= 20; i++)
        for (var j = 1; j <= 3; j++)
        {
            Console.WriteLine($"Executing session:{i}/20, run:{j}/3");
            OpenAndCloseApplication(i <= 2);
        }

    }

    private void OpenAndCloseApplication(bool isPushMessageAllowed)
    {
        openApplicationTest.Enter();

        if (isPushMessageAllowed)
        {
            Thread.Sleep(TimeSpan.FromSeconds(10));
            pushAlarmMessageTest.Enter();   
        }
        
        Thread.Sleep(TimeSpan.FromSeconds(20));
        closeApplicationTest.Enter();
        Thread.Sleep(TimeSpan.FromSeconds(2));
    }

    [Test]
    public override void Exit()
    {
        
    }
}