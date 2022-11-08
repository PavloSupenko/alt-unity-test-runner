using Shared.Arguments;
using TestSpecificationParser.Arguments;
using TestSpecificationParser.PlatformInfos;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


namespace TestSpecificationParser;

public static class Program
{
    private const int DefaultAltUnityPort = 13000;
    private const int DefaultAppiumPort = 4723;
    private const int DefaultWdaIosPort = 8100;
    
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

        string devicePlatformName = argumentsReader[BashScriptBuilderArgument.DevicePlatformName];
        IDeviceInfo deviceInfo = devicePlatformName.Equals("iOS") ? new IosDeviceInfo() : new AndroidDeviceInfo();
        deviceInfo.FindFirstConnectedDevice(out var deviceNumber, out var realUdid, out var platformVersion);
        
        int deviceNumberDecimal = int.Parse(deviceNumber);
        int appiumPort = DefaultAppiumPort + deviceNumberDecimal;
        int altUnityPort = DefaultAltUnityPort + deviceNumberDecimal;
        int wdaIosPort = DefaultWdaIosPort + deviceNumberDecimal;

        string appiumUdid = isCloudRun
            ? realUdid.Replace("-", "")
            : realUdid;

        string bashExecutionScript = bashScriptBuilder.Build
        (
            // We will be using udid as a device name like AWS does.
            deviceName: realUdid,
            devicePlatform: devicePlatformName,
            artifactsDirectory: argumentsReader[BashScriptBuilderArgument.ArtifactsDirectory],
            realDeviceId: realUdid,
            appiumDeviceId: appiumUdid,
            devicePlatformVersion: platformVersion,
            testPackagePath: argumentsReader[BashScriptBuilderArgument.TestPackageDirectory],
            applicationPath: argumentsReader[BashScriptBuilderArgument.ApplicationPath],
            appiumPort: appiumPort.ToString(),
            altUnityPort: altUnityPort.ToString(),
            wdaIosPort: wdaIosPort.ToString(),
            wdaDerivedPath: argumentsReader[BashScriptBuilderArgument.WdaDerivedDataPath], 
            isCloud: "false");

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