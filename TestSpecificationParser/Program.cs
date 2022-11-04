using Shared.Arguments;
using TestSpecificationParser.Arguments;
using TestSpecificationParser.PlatformInfos;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace TestSpecificationParser;

public static class Program
{
    private static ArgumentsReader<BashScriptBuilderArgument> argumentsReader;
    
    private static void Main(string[] args)
    {
        argumentsReader = new ArgumentsReader<BashScriptBuilderArgument>(args, BashScriptBuilderArgumentValues.Keys,
            BashScriptBuilderArgumentValues.Defaults, BashScriptBuilderArgumentValues.Descriptions);
        
        if (TryShowHelp(argumentsReader))
            return;
        
        using StreamReader reader = new StreamReader(argumentsReader[BashScriptBuilderArgument.YamlFilePath]);
        string specFileContent = reader.ReadToEnd();
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        
        TestSpecification specification = deserializer.Deserialize<TestSpecification>(specFileContent);
        BashScriptBuilder bashScriptBuilder = new BashScriptBuilder(specification);

        bool isCloudRun = bool.Parse(argumentsReader[BashScriptBuilderArgument.IsCloudRun]);
        string deviceNumber = argumentsReader[BashScriptBuilderArgument.DeviceNumber];
        string devicePlatformName = argumentsReader[BashScriptBuilderArgument.DevicePlatformName];
        IDeviceInfo deviceInfo = devicePlatformName.Equals("iOS") ? new IosDeviceInfo() : new AndroidDeviceInfo();
        deviceInfo.IsDeviceConnected(deviceNumber, out var udid, out var platformVersion);

        string bashExecutionScript = bashScriptBuilder.Build
        (
            // We will be using udid as a device name like AWS does.
            deviceName: udid,
            devicePlatform: devicePlatformName,
            artifactsDirectory: argumentsReader[BashScriptBuilderArgument.ArtifactsDirectory],
            deviceId: udid,
            devicePlatformVersion: platformVersion,
            testPackagePath: argumentsReader[BashScriptBuilderArgument.TestPackageDirectory],
            applicationPath: argumentsReader[BashScriptBuilderArgument.ApplicationPath], 
            isCloudRun: isCloudRun);

        using StreamWriter writer = new StreamWriter(argumentsReader[BashScriptBuilderArgument.ShellFilePath]);
        writer.Write(bashExecutionScript);
    }
    
    private static bool TryShowHelp(ArgumentsReader<BashScriptBuilderArgument> argumentsReader)
    {
        if (!argumentsReader.IsTrue(BashScriptBuilderArgument.Help)) 
            return false;

        var showDefaults = true;
        ArgumentsReader<BashScriptBuilderArgument>.ShowHelp(argumentsReader, "==== Parameters: ====", showDefaults);
        Console.WriteLine();

        return true;
    }
}