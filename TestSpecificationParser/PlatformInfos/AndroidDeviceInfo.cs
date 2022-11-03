using Shared.Processes;


namespace TestSpecificationParser.PlatformInfos;

public class AndroidDeviceInfo : IDeviceInfo
{
    private readonly ProcessRunner processRunner = new();
    
    public bool IsDeviceConnected(string deviceNumberString, out string udid, out string platformVersion)
    {
        var adbPath = "adb";
        var arguments = "devices";
        Console.WriteLine($"Executing command: {adbPath} {arguments}");

        var resultsStrings = processRunner
            .GetProcessOutput(processRunner.StartProcess(adbPath, arguments)).ToList();

        var devices = resultsStrings
            .Where(line => !string.IsNullOrEmpty(line) && !line.StartsWith("List ") && !line.Contains("deamon"))
            .Select(line => line.Split('\t')[0])
            .ToList();

        if (!devices.Any())
        {
            Console.WriteLine("No devices connected to execute tests.");
            udid = string.Empty;
            platformVersion = string.Empty;
            return false;
        }

        var deviceNumber = int.Parse(deviceNumberString);

        Console.WriteLine("Device ID's found:");
        foreach (var id in devices)
            Console.WriteLine(id);

        if (devices.Count < deviceNumber)
        {
            Console.WriteLine("Not enough devices to execute with target device number: {0}.", deviceNumber);
            udid = string.Empty;
            platformVersion = string.Empty;
            return false;
        }

        udid = devices[deviceNumber - 1];

        var versionArguments = $"-s {udid} shell getprop ro.build.version.release";
        Console.WriteLine($"Executing command: {adbPath} {versionArguments}");
        platformVersion = processRunner.GetProcessOutput(processRunner.StartProcess(adbPath, arguments)).First();
        
        Console.WriteLine($"Found device by number: {deviceNumberString} with udid: {udid} and Android version: {platformVersion}");
        
        return true;
    }
}