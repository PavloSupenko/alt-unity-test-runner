using Shared.Processes;


namespace TestSpecificationParser.PlatformInfos;

public class IosDeviceInfo : IDeviceInfo
{
    private readonly ProcessRunner processRunner = new();
    private int deviceNumberShift;

    
    public IosDeviceInfo(int deviceNumberShift)
    {
        this.deviceNumberShift = deviceNumberShift;
    }
    
    public bool FindFirstConnectedDevice(out string deviceNumberString, out string udid, out string platformVersion)
    {
        var xcrunPath = "xcrun";
        var arguments = $"xctrace list devices";
        Console.WriteLine($"Executing command: {xcrunPath} {arguments}");

        var resultStrings = processRunner
            .GetProcessOutput(processRunner.StartProcess(xcrunPath, arguments)).ToList();
        
        Console.WriteLine($"Device listing returns: \n{string.Join('\n', resultStrings)}");

        var devices = resultStrings
            .Skip(2)
            .Where(line => !string.IsNullOrEmpty(line) && !line.Contains("Simulator"))
            .Select(line =>
            {
                var deviceIdAndPlatform = line
                    .Split(' ')
                    .Reverse()
                    .Take(2)
                    .Select(info => info.Replace("(", string.Empty).Replace(")", string.Empty))
                    .ToArray();

                return new { Udid = deviceIdAndPlatform[0], PlatformVersion = deviceIdAndPlatform[1]};
            })
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
                .Any(processData => processData.Contains(deviceData.Udid)))
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

        var requiredDeviceInfo = devices[deviceNumber - 1];
        udid = requiredDeviceInfo.Udid;
        platformVersion = requiredDeviceInfo.PlatformVersion;
        
        Console.WriteLine($"Found device by number: {deviceNumber} with udid: {udid} and iOS version: {platformVersion}");
        
        return true;
    }
}