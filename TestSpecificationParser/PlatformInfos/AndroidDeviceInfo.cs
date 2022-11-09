using Shared.Processes;


namespace TestSpecificationParser.PlatformInfos;

public class AndroidDeviceInfo : IDeviceInfo
{
    private readonly ProcessRunner processRunner = new();
    private int deviceNumberShift;

    
    public AndroidDeviceInfo(int deviceNumberShift)
    {
        this.deviceNumberShift = deviceNumberShift;
    }
    
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
        
        if (!freeDevices.Any())
        {
            Console.WriteLine("No free devices to execute tests.");
            udid = string.Empty;
            platformVersion = string.Empty;
            deviceNumberString = string.Empty;
            return false;
        }
        
        Console.WriteLine("Free device ID's found:");
        foreach (var id in freeDevices)
            Console.WriteLine(id);

        var firstFreeDevice = freeDevices.First();
        var deviceIndex = devices.IndexOf(firstFreeDevice);
        var deviceNumber = deviceIndex + 1;
        deviceNumberString = (deviceNumber + deviceNumberShift).ToString();
        
        Console.WriteLine($"Device ID: {firstFreeDevice} will be using as a first free.");
        udid = devices[deviceNumber - 1];

        var versionArguments = $"-s {udid} shell getprop ro.build.version.release";
        Console.WriteLine($"Executing command: {adbPath} {versionArguments}");
        platformVersion = processRunner.GetProcessOutput(processRunner.StartProcess(adbPath, arguments)).First();
        
        Console.WriteLine($"Found device by number: {deviceNumber} with udid: {udid} and Android version: {platformVersion}");
        
        return true;
    }
}