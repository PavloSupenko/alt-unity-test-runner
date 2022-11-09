using Amazon.DeviceFarm.Model;
using Shared.Arguments;
using AmazonDeviceFarmClient.Client;
using AmazonDeviceFarmClient.Arguments;


namespace AmazonDeviceFarmClient;

public static class Program
{
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

        var projectName = argumentsReader[DeviceFarmArgument.ProjectName];

        if (argumentsReader.IsExist(DeviceFarmArgument.UploadTestPackage))
            await deviceFarmClient.UploadTestPackage(
                argumentsReader[DeviceFarmArgument.TestPackageName],
                projectName,
                argumentsReader[DeviceFarmArgument.UploadTestPackage]);
        
        if (argumentsReader.IsExist(DeviceFarmArgument.UploadTestSpecs))
            await deviceFarmClient.UploadTestSpec(
                argumentsReader[DeviceFarmArgument.TestSpecsName],
                projectName,
                argumentsReader[DeviceFarmArgument.UploadTestSpecs]);
        
        bool isAndroid = argumentsReader[DeviceFarmArgument.ApplicationPlatform].Equals("Android");
        ApplicationPlatform platform = isAndroid ? ApplicationPlatform.Android : ApplicationPlatform.Ios;
        
        if (isAndroid)
            await deviceFarmClient.UploadAndroidApplication(
                argumentsReader[DeviceFarmArgument.ApplicationName],
                projectName,
                argumentsReader[DeviceFarmArgument.UploadApplication]);
        else
            await deviceFarmClient.UploadIosApplication(
                argumentsReader[DeviceFarmArgument.ApplicationName],
                projectName,
                argumentsReader[DeviceFarmArgument.UploadApplication]);

        var runName = argumentsReader[DeviceFarmArgument.RunName];
        var timeout = int.Parse(argumentsReader[DeviceFarmArgument.Timeout]);
        
        await deviceFarmClient.ScheduleTestRun(
            runName,
            platform,
            projectName,
            argumentsReader[DeviceFarmArgument.DevicePoolName],
            argumentsReader[DeviceFarmArgument.TestSpecsName],
            timeout);
        
        await deviceFarmClient.WaitTestRun(runName, projectName, TimeSpan.FromSeconds(10));

        var artifactsDirectory = argumentsReader[DeviceFarmArgument.ArtifactsPath];

        await deviceFarmClient.DownloadArtifacts(runName, projectName, artifactsDirectory);
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