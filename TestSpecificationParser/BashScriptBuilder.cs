using System.Text;


namespace TestSpecificationParser;

public class BashScriptBuilder
{
    // This is custom variables we should define in spec file
    private const string IsCloudRun = "IS_CLOUD_RUN";
    private const string AppiumPort = "APPIUM_PORT";
    private const string AltUnityPort = "ALT_UNITY_PORT";
    private const string WdaPort = "WDA_PORT";
    private const string DeviceIdForAppium = "DEVICEFARM_DEVICE_UDID_FOR_APPIUM";
    private const string WdaDerivedPath = "DEVICEFARM_WDA_DERIVED_DATA_PATH";
    private const string TestsFilter = "TESTS_FILTER";
    
    // This variables are provided from environment of AWS device farm
    private const string DeviceId = "DEVICEFARM_DEVICE_UDID";
    private const string DeviceName = "DEVICEFARM_DEVICE_NAME";
    private const string DevicePlatformName = "DEVICEFARM_DEVICE_PLATFORM_NAME";
    private const string LogDirectory = "DEVICEFARM_LOG_DIR";
    private const string DevicePlatformVersionVariable = "DEVICEFARM_DEVICE_OS_VERSION";
    private const string TestPackagePath = "DEVICEFARM_TEST_PACKAGE_PATH";
    private const string ApplicationPath = "DEVICEFARM_APP_PATH";

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
        $"{AppiumPort}=",
        $"{AltUnityPort}=",
        $"{WdaPort}=",
        $"{DeviceIdForAppium}=",
        $"{WdaDerivedPath}=",
        $"{IsCloudRun}=",
        $"{TestsFilter}=",
    };

    public BashScriptBuilder(TestSpecification specification)
    {
        this.specification = specification;
    }

    public string Build(string deviceName, string devicePlatform,
        string artifactsDirectory, string realDeviceId, string appiumDeviceId, string devicePlatformVersion,
        string testPackagePath, string applicationPath, string appiumPort, string altUnityPort, string wdaIosPort,
        string wdaDerivedPath, string isCloud, string testsFilter)
    {
        StringBuilder scriptContent = new StringBuilder();
        AddBashHeader(scriptContent);

        AddExports
        (
            scriptContent: scriptContent,
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
            wdaIosPort: wdaIosPort, 
            wdaDerivedPath: wdaDerivedPath, 
            isCloud: isCloud,
            testsFiler: testsFilter);
        
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
    private void AddExports(StringBuilder scriptContent, string deviceName, string devicePlatform,
        string artifactsDirectory, string realDeviceId, string appiumDeviceId, string devicePlatformVersion,
        string testPackagePath, string applicationPath, string appiumPort, string altUnityPort, string wdaIosPort, 
        string wdaDerivedPath, string isCloud, string testsFiler)
    {
        AddEnvironmentVariable(scriptContent, TestsFilter, testsFiler);
        
        AddEnvironmentVariable(scriptContent, AppiumPort, appiumPort);
        AddEnvironmentVariable(scriptContent, AltUnityPort, altUnityPort);
        AddEnvironmentVariable(scriptContent, WdaPort, wdaIosPort);
        AddEnvironmentVariable(scriptContent, DeviceIdForAppium, appiumDeviceId);
        AddEnvironmentVariable(scriptContent, WdaDerivedPath, wdaDerivedPath);
        AddEnvironmentVariable(scriptContent, IsCloudRun, isCloud);
        
        AddEnvironmentVariable(scriptContent, DeviceId, realDeviceId);
        AddEnvironmentVariable(scriptContent, DeviceName, deviceName);
        AddEnvironmentVariable(scriptContent, DevicePlatformName, devicePlatform);
        AddEnvironmentVariable(scriptContent, LogDirectory, artifactsDirectory);
        AddEnvironmentVariable(scriptContent, DevicePlatformVersionVariable, devicePlatformVersion);
        AddEnvironmentVariable(scriptContent, TestPackagePath, testPackagePath);
        AddEnvironmentVariable(scriptContent, ApplicationPath, applicationPath);
    }

    private void AddEnvironmentVariable(StringBuilder scriptContent, string name, string value)
    {
        if (string.IsNullOrEmpty(value))
            value = "\"Unknown\"";
        
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