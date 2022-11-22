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
    
    private static ArgumentsReader<Argument> argumentsReader;

    private static void Main(string[] args)
    {
        argumentsReader = new ArgumentsReader<Argument>(args, ArgumentValues.Keys,
            ArgumentValues.Defaults, ArgumentValues.Descriptions);
        
        if (TryShowHelp(argumentsReader))
            return;
        
        using StreamReader reader = new StreamReader(argumentsReader[Argument.YamlFilePath]);
        string specFileContent = reader.ReadToEnd();
        
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        
        TestSpecification specification = deserializer.Deserialize<TestSpecification>(specFileContent);
        BashScriptBuilder bashScriptBuilder = new BashScriptBuilder(specification);

        bool isCloudRun = bool.Parse(argumentsReader[Argument.IsCloudRun]);

        string devicePlatformName = argumentsReader[Argument.DevicePlatformName];
        IDeviceInfo deviceInfo = devicePlatformName.Equals("iOS") ? new IosDeviceInfo(0) : new AndroidDeviceInfo(5);
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
            artifactsDirectory: argumentsReader[Argument.ArtifactsDirectory],
            realDeviceId: realUdid,
            appiumDeviceId: appiumUdid,
            devicePlatformVersion: platformVersion,
            testPackagePath: argumentsReader[Argument.TestPackageDirectory],
            applicationPath: argumentsReader[Argument.ApplicationPath],
            appiumPort: appiumPort.ToString(),
            altUnityPort: altUnityPort.ToString(),
            wdaIosPort: wdaIosPort.ToString(),
            wdaDerivedPath: argumentsReader[Argument.WdaDerivedDataPath], 
            isCloud: "false", 
            testsFilter: argumentsReader[Argument.TestsFilter]);

        using StreamWriter writer = new StreamWriter(argumentsReader[Argument.ShellFilePath]);
        writer.Write(bashExecutionScript);
    }
    
    private static bool TryShowHelp(ArgumentsReader<Argument> argumentsReader)
    {
        if (!argumentsReader.IsTrue(Argument.Help)) 
            return false;

        var showDefaults = true;
        ArgumentsReader<Argument>.ShowHelp(argumentsReader, "==== Parameters: ====", showDefaults);
        Console.WriteLine();

        return true;
    }
}