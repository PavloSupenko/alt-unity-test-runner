using Shared.Arguments;
using AmazonDeviceFarmClient.Client;
using AmazonDeviceFarmClient.Arguments;


namespace AmazonDeviceFarmClient;

public static class Program
{
    private const string TestsFilterVariable = "TESTS_FILTER";
    private static ArgumentsReader<DeviceFarmArgument> argumentsReader;

    
    private static async Task Main(string[] args)
    {
        argumentsReader = new ArgumentsReader<DeviceFarmArgument>(args, DeviceFarmArgumentValues.Keys,
            DeviceFarmArgumentValues.Defaults, DeviceFarmArgumentValues.Descriptions);

        if (TryShowHelp(argumentsReader))
            return;

        DeviceFarmClient deviceFarmClient = CreateDeviceFarmClient
        (
            accessKey: argumentsReader[DeviceFarmArgument.UserKey],
            secretKey: argumentsReader[DeviceFarmArgument.UserSecret]
        );

        string projectName = argumentsReader[DeviceFarmArgument.ProjectName];

        await TryUploadTestPackage(deviceFarmClient, projectName);
        await TryUploadTestSpec(deviceFarmClient, projectName, argumentsReader[DeviceFarmArgument.TestsFilter]);

        ApplicationPlatform platform = 
            argumentsReader[DeviceFarmArgument.ApplicationPlatform].Equals("Android") 
            ? ApplicationPlatform.Android 
            : ApplicationPlatform.Ios;

        await TryUploadApplication(platform, deviceFarmClient, projectName);

        string runName = argumentsReader[DeviceFarmArgument.RunName];
        int timeout = int.Parse(argumentsReader[DeviceFarmArgument.Timeout]);
        
        await ScheduleTestRun(deviceFarmClient, runName, platform, projectName, timeout);
        await deviceFarmClient.WaitTestRun(runName, projectName, TimeSpan.FromSeconds(10));

        var artifactsDirectory = argumentsReader[DeviceFarmArgument.ArtifactsPath];
        await deviceFarmClient.DownloadArtifacts(runName, projectName, artifactsDirectory);
    }

    private static async Task ScheduleTestRun(DeviceFarmClient deviceFarmClient, string runName,
        ApplicationPlatform platform, string projectName, int timeout)
    {
        await deviceFarmClient.ScheduleTestRun(
            runName,
            platform,
            projectName,
            argumentsReader[DeviceFarmArgument.DevicePoolName],
            argumentsReader[DeviceFarmArgument.TestSpecsName],
            timeout);
    }

    private static async Task TryUploadApplication(ApplicationPlatform platform, DeviceFarmClient deviceFarmClient, string projectName)
    {
        if (platform == ApplicationPlatform.Android)
            await deviceFarmClient.UploadAndroidApplication(
                argumentsReader[DeviceFarmArgument.ApplicationName],
                projectName,
                argumentsReader[DeviceFarmArgument.UploadApplication]);
        else
            await deviceFarmClient.UploadIosApplication(
                argumentsReader[DeviceFarmArgument.ApplicationName],
                projectName,
                argumentsReader[DeviceFarmArgument.UploadApplication]);
    }

    private static async Task TryUploadTestSpec(DeviceFarmClient deviceFarmClient, string projectName, string testsFilter)
    {
        if (!argumentsReader.IsExist(DeviceFarmArgument.UploadTestSpecs))
            return;

        string testSpecName = argumentsReader[DeviceFarmArgument.TestSpecsName];
        string testSpecPath = argumentsReader[DeviceFarmArgument.UploadTestSpecs];

        List<string> testSpecLines = (await File.ReadAllLinesAsync(testSpecPath)).ToList();
        string testFilterLine = testSpecLines.First(line => line.Contains($"{TestsFilterVariable}="));
        int testFilerLineIndex = testSpecLines.IndexOf(testFilterLine);

        string[] testFilterVariableData = testFilterLine.Split('=');
        string testFilterVariableName = testFilterVariableData[0];
        string newTestFilterVariableValue = $"\"{testsFilter}\"";
        
        string newTestFilterLine = $"{testFilterVariableName}={newTestFilterVariableValue}";
        testSpecLines[testFilerLineIndex] = newTestFilterLine;

        await File.WriteAllLinesAsync(testSpecPath, testSpecLines);

        await deviceFarmClient.UploadTestSpec(
            testSpecName,
            projectName,
            testSpecPath);
    }

    private static async Task TryUploadTestPackage(DeviceFarmClient deviceFarmClient, string projectName)
    {
        if (!argumentsReader.IsExist(DeviceFarmArgument.UploadTestPackage))
            return;

        await deviceFarmClient.UploadTestPackage(
            argumentsReader[DeviceFarmArgument.TestPackageName],
            projectName,
            argumentsReader[DeviceFarmArgument.UploadTestPackage]);
    }

    private static DeviceFarmClient CreateDeviceFarmClient(string accessKey, string secretKey)
    {
        return new DeviceFarmClient(accessKey, secretKey);
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