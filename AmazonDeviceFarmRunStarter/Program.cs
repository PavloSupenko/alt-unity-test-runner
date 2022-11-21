using Shared.Arguments;
using AmazonDeviceFarmClient.Client;
using AmazonDeviceFarmRunStarter.Arguments;


namespace AmazonDeviceFarmRunStarter;

public static class Program
{
    private const string TestsFilterVariable = "TESTS_FILTER";
    private static ArgumentsReader<Argument> argumentsReader;

    
    private static async Task Main(string[] args)
    {
        argumentsReader = new ArgumentsReader<Argument>(args, ArgumentValues.Keys,
            ArgumentValues.Defaults, ArgumentValues.Descriptions);

        if (TryShowHelp(argumentsReader))
            return;

        DeviceFarmClient deviceFarmClient = CreateDeviceFarmClient
        (
            accessKey: argumentsReader[Argument.UserKey],
            secretKey: argumentsReader[Argument.UserSecret]
        );

        string projectName = argumentsReader[Argument.ProjectName];

        await TryUploadTestPackage(deviceFarmClient, projectName);
        await TryUploadTestSpec(deviceFarmClient, projectName, argumentsReader[Argument.TestsFilter]);

        ApplicationPlatform platform = 
            argumentsReader[Argument.ApplicationPlatform].Equals("Android") 
            ? ApplicationPlatform.Android 
            : ApplicationPlatform.Ios;

        await TryUploadApplication(platform, deviceFarmClient, projectName);

        string runName = argumentsReader[Argument.RunName];
        int timeout = int.Parse(argumentsReader[Argument.Timeout]);
        
        await ScheduleTestRun(deviceFarmClient, runName, platform, projectName, timeout);
    }

    private static async Task ScheduleTestRun(DeviceFarmClient deviceFarmClient, string runName,
        ApplicationPlatform platform, string projectName, int timeout)
    {
        await deviceFarmClient.ScheduleTestRun(
            runName,
            platform,
            projectName,
            argumentsReader[Argument.DevicePoolName],
            argumentsReader[Argument.TestSpecsName],
            timeout);
    }

    private static async Task TryUploadApplication(ApplicationPlatform platform, DeviceFarmClient deviceFarmClient, string projectName)
    {
        if (platform == ApplicationPlatform.Android)
            await deviceFarmClient.UploadAndroidApplication(
                argumentsReader[Argument.ApplicationName],
                projectName,
                argumentsReader[Argument.UploadApplication]);
        else
            await deviceFarmClient.UploadIosApplication(
                argumentsReader[Argument.ApplicationName],
                projectName,
                argumentsReader[Argument.UploadApplication]);
    }

    private static async Task TryUploadTestSpec(DeviceFarmClient deviceFarmClient, string projectName, string testsFilter)
    {
        if (!argumentsReader.IsExist(Argument.UploadTestSpecs))
            return;

        string testSpecName = argumentsReader[Argument.TestSpecsName];
        string testSpecPath = argumentsReader[Argument.UploadTestSpecs];

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
        if (!argumentsReader.IsExist(Argument.UploadTestPackage))
            return;

        await deviceFarmClient.UploadTestPackage(
            argumentsReader[Argument.TestPackageName],
            projectName,
            argumentsReader[Argument.UploadTestPackage]);
    }

    private static DeviceFarmClient CreateDeviceFarmClient(string accessKey, string secretKey)
    {
        return new DeviceFarmClient(accessKey, secretKey);
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