using Shared.Processes;


namespace TestSpecificationParser.PlatformInfos;

public class AndroidDeviceInfo : IDeviceInfo
{
    private readonly ProcessRunner processRunner = new();
    
    public bool FindFirstConnectedDevice(out string deviceNumberString, out string udid, out string platformVersion)
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
            deviceNumberString = string.Empty;
            return false;
        }
        
        var appiumProcessesIds = processRunner
            .GetProcessOutput(processRunner.StartProcess("pgrep", "node")).ToList();
        Console.WriteLine($"Found NodeJs processes: {string.Join(',', appiumProcessesIds)}.");

        var appiumProcessesData = appiumProcessesIds.Select(id =>
        {
            var processData = processRunner
                .GetProcessOutput(processRunner.StartProcess("ps", $"-fp {id}"));

            var processDataString = string.Join(' ', processData);
            return processDataString;
        });
        
        Console.WriteLine("Device ID's found:");
        foreach (var id in devices)
            Console.WriteLine(id);

        var freeDevices = devices
            .Where(deviceData => !appiumProcessesData
                .Any(processData => processData.Contains(deviceData)))
            .ToList();
        
        Console.WriteLine("Free device ID's found:");
        foreach (var id in freeDevices)
            Console.WriteLine(id);

        var firstFreeDevice = freeDevices.First();
        var deviceIndex = devices.IndexOf(firstFreeDevice);
        var deviceNumber = deviceIndex + 1;
        deviceNumberString = deviceNumber.ToString();
        
        Console.WriteLine($"Device ID: {firstFreeDevice} will be using as a first free.");

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