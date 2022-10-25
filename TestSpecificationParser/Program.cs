using Shared.Arguments;
using TestSpecificationParser.Arguments;
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

        string bashExecutionScript = bashScriptBuilder.Build
        (
            deviceName: argumentsReader[BashScriptBuilderArgument.DeviceName],
            devicePlatform: argumentsReader[BashScriptBuilderArgument.DevicePlatformName],
            artifactsDirectory: argumentsReader[BashScriptBuilderArgument.ArtifactsDirectory],
            deviceId: argumentsReader[BashScriptBuilderArgument.DeviceId],
            devicePlatformVersion: argumentsReader[BashScriptBuilderArgument.DevicePlatformVersion],
            testPackagePath: argumentsReader[BashScriptBuilderArgument.TestPackageDirectory],
            applicationPath: argumentsReader[BashScriptBuilderArgument.ApplicationPath]
        );

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