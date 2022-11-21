using Shared.Arguments;
using AmazonDeviceFarmClient.Client;
using AmazonDeviceFarmRunAwaiter.Arguments;


namespace AmazonDeviceFarmRunAwaiter;

public static class Program
{
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
        string runName = argumentsReader[Argument.RunName];
        
        await deviceFarmClient.WaitTestRun(runName, projectName, TimeSpan.FromSeconds(10));

        var artifactsDirectory = argumentsReader[Argument.ArtifactsPath];
        await deviceFarmClient.DownloadArtifacts(runName, projectName, artifactsDirectory);
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