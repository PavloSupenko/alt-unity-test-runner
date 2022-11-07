using System.Text;


namespace TestSpecificationParser;

public class BashScriptBuilder
{
    private readonly TestSpecification specification;
    
    /// <summary>
    /// avm (Appium version manager) and nvm (NodeJs version manager) commands will not be included
    /// to locally executed bash script.
    /// Locally only one version of node and appium will be installed.
    /// </summary>
    private readonly string[] notAllowedLocallyCommands = {
        "avm",
        "nvm",
        "export APPIUM_VERSION",
    };

    public BashScriptBuilder(TestSpecification specification)
    {
        this.specification = specification;
    }

    public string Build(string deviceNumber, string deviceName, string devicePlatform, 
        string artifactsDirectory, string realDeviceId, string appiumDeviceId, string devicePlatformVersion, string testPackagePath,
        string applicationPath, string appiumPort, string altUnityPort, string wdaIosPort)
    {
        StringBuilder scriptContent = new StringBuilder();
        AddBashHeader(scriptContent);

        AddExports
        (
            scriptContent: scriptContent,
            deviceNumber: deviceNumber,
            deviceName: deviceName,
            devicePlatform: devicePlatform,
            artifactsDirectory: artifactsDirectory,
            realDeviceId: realDeviceId,
            appiumDeviceId: appiumDeviceId,
            devicePlatformVersion: devicePlatformVersion,
            testPackagePath: testPackagePath,
            applicationPath: applicationPath,
            appiumPort: appiumPort,
            altUnityPort: altUnityPort,
            wdaIosPort: wdaIosPort
        );
        
        AddPhases(scriptContent, specification.Phases);

        return scriptContent.ToString();
    }

    private void AddBashHeader(StringBuilder scriptContent)
    {
        scriptContent.AppendLine("#!/bin/bash");
    }

    /// <remark>
    /// XCUITest driver on AWS device farm does not support device id with dashes (it can be found in appiumlog.log)
    /// But local machine works fine with dashes. Tested with local Appium version 1.22.3+ and AWS 1.22.2
    /// </remark>
    private void AddExports(StringBuilder scriptContent, string deviceNumber, string deviceName, string devicePlatform, 
        string artifactsDirectory, string realDeviceId, string appiumDeviceId, string devicePlatformVersion, string testPackagePath,
        string applicationPath, string appiumPort, string altUnityPort, string wdaIosPort)
    {
        AddEnvironmentVariable(scriptContent, "DEVICE_NUMBER", deviceNumber);
        AddEnvironmentVariable(scriptContent, "APPIUM_PORT", appiumPort);
        AddEnvironmentVariable(scriptContent, "ALT_UNITY_PORT", altUnityPort);
        AddEnvironmentVariable(scriptContent, "WDA_PORT", wdaIosPort);
        
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_DEVICE_UDID_FOR_APPIUM", appiumDeviceId);
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_DEVICE_UDID", realDeviceId);

        AddEnvironmentVariable(scriptContent, "DEVICEFARM_DEVICE_NAME", deviceName);
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_DEVICE_PLATFORM_NAME", devicePlatform);
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_LOG_DIR", artifactsDirectory);
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_DEVICE_OS_VERSION", devicePlatformVersion);
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_TEST_PACKAGE_PATH", testPackagePath);
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_APP_PATH", applicationPath);
    }

    private void AddEnvironmentVariable(StringBuilder scriptContent, string name, string value)
    {
        scriptContent.AppendLine($"export {name}={value}");
    }

    private void AddPhases(StringBuilder scriptContent, Dictionary<string, TestPhase> phases)
    {
        foreach (var phase in phases) 
            AddMethod(scriptContent, phase.Key, phase.Value.Commands);

        foreach (var phase in phases) 
            scriptContent.AppendLine(phase.Key);
    }

    private void AddMethod(StringBuilder scriptContent, string methodName, List<string>? commands)
    {
        scriptContent.AppendLine(string.Empty);
        scriptContent.AppendLine($"{methodName}()");
        scriptContent.AppendLine("{");

        scriptContent.AppendLine("echo ' '");
        scriptContent.AppendLine($"echo '########### Entering phase {methodName} ###########'");
        scriptContent.AppendLine("echo ' '");

        if (commands != null)
            foreach (var command in commands)
            {
                if (notAllowedLocallyCommands.Any(notAllowedCommand => command.Contains(notAllowedCommand)))
                    continue;
                
                scriptContent.AppendLine($"echo '{command}'");
                scriptContent.AppendLine(command);
            }
        
        scriptContent.AppendLine("}");
        scriptContent.AppendLine(string.Empty);
    }
}