using System;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;


namespace TestsClient.Drivers;

public static class ExistingDriver<TDriver, TElement> where TDriver : AppiumDriver<TElement> where TElement : IWebElement 
{
    // public static TDriver Get(Uri remoteAddress, string sessionId) : base(remoteAddress, new AppiumOptions())
    // {
    //     TDriver = new 
    //     
    //     this.sessionId = sessionId;
    //     var sessionIdBase = GetType().BaseType.BaseType.BaseType
    //         .GetField("sessionId", BindingFlags.Instance | BindingFlags.NonPublic);
    //     
    //     sessionIdBase.SetValue(this, new SessionId(sessionId));
    // }
}