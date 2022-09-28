using System;
using System.Collections.Generic;
using System.Reflection;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Remote;


namespace TestsClient.Drivers;

public class IosExistingDriver : IOSDriver<IOSElement>
{
    private readonly string sessionId;

    public IosExistingDriver(Uri remoteAddress, string sessionId, TimeSpan commandTimeout) : base(remoteAddress, new AppiumOptions(), commandTimeout)
    {
        this.sessionId = sessionId;
        var sessionIdBase = GetType().BaseType.BaseType.BaseType
            .GetField("sessionId", BindingFlags.Instance | BindingFlags.NonPublic);
        
        sessionIdBase.SetValue(this, new SessionId(sessionId));
    }

    protected override Response Execute(string driverCommandToExecute, Dictionary<string, object> parameters)
    {
        if (driverCommandToExecute == DriverCommand.NewSession)
        {
            var resp = new Response();
            resp.Status = OpenQA.Selenium.WebDriverResult.Success;
            resp.SessionId = sessionId;
            resp.Value = new Dictionary<String, Object>();
            return resp;
        }
        var respBase = base.Execute(driverCommandToExecute, parameters);
        return respBase;
    }
}