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

    public string Build(string deviceName, string devicePlatform, string artifactsDirectory, string deviceId,
        string devicePlatformVersion, string testPackagePath, string applicationPath, bool isCloudRun)
    {
        StringBuilder scriptContent = new StringBuilder();
        AddBashHeader(scriptContent);
        AddExports(scriptContent, deviceName, devicePlatform, artifactsDirectory, deviceId, devicePlatformVersion, 
            testPackagePath, applicationPath, isCloudRun);
        AddPhases(scriptContent, specification.Phases);

        return scriptContent.ToString();
    }

    private void AddBashHeader(StringBuilder scriptContent)
    {
        scriptContent.AppendLine("#!/bin/bash");
    }

    /// <remark>
    /// Strange thing is that XCUITest on AWS device farm log into appiumlog.log file that device id contains no dashes
    /// But local machine works fine with dashes. Local Appium version is 1.22.3 and AWS is 1.22.2
    /// </remark>
    private void AddExports(StringBuilder scriptContent, string deviceName, string devicePlatform, 
        string artifactsDirectory, string deviceId, string devicePlatformVersion, string testPackagePath,
        string applicationPath, bool isCloudRun)
    {
        // For users of Appium versions 1.15.0 and higher, your Appium version requires that the UDID of the device not contain any "-" characters
        // So, we will create a new environment variable of the UDID specifically for Appium based on your Appium version
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_DEVICE_UDID_FOR_APPIUM", isCloudRun 
            ? deviceId.Replace("-", "") 
            : deviceId);
        
        AddEnvironmentVariable(scriptContent, "DEVICEFARM_DEVICE_UDID", deviceId);

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