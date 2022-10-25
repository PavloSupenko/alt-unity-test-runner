using Shared.Arguments;
using AmazonDeviceFarmClient.Client;
using AmazonDeviceFarmClient.Arguments;


namespace AmazonDeviceFarmClient;

public static class Program
{
    private const string AndroidSpecName = "android-custom-config.yml";
    private const string IosSpecName = "ios-custom-config.yml";
    
    private static ArgumentsReader<DeviceFarmArgument> argumentsReader;

    private static async Task Main(string[] args)
    {
        argumentsReader = new ArgumentsReader<DeviceFarmArgument>(args, DeviceFarmArgumentValues.Keys,
            DeviceFarmArgumentValues.Defaults, DeviceFarmArgumentValues.Descriptions);

        if (TryShowHelp(argumentsReader))
            return;

        DeviceFarmClient deviceFarmClient = new DeviceFarmClient(
            argumentsReader[DeviceFarmArgument.UserKey],
            argumentsReader[DeviceFarmArgument.UserSecret]);

        await deviceFarmClient.UploadTestPackage(
            "markers-from-sdk.zip",
            argumentsReader[DeviceFarmArgument.ProjectName],
            @"D:\Tests\_prj\test-package\more-skippables-2.zip");

        await deviceFarmClient.ScheduleTestRun(
            "[Android] test-run-from-sdk",
            ApplicationPlatform.Android,
            argumentsReader[DeviceFarmArgument.ProjectName],
            "[Android] random-device",
            AndroidSpecName,
            10);
    }

    private static bool TryShowHelp(ArgumentsReader<DeviceFarmArgument> argumentsReader)
    {
        if (!argumentsReader.IsTrue(DeviceFarmArgument.Help)) 
            return false;

        var showDefaults = true;
        ArgumentsReader<DeviceFarmArgument>.ShowHelp(argumentsReader, "==== Parameters: ====", showDefaults);
        Console.WriteLine();

        return true;
    }
}