using Shared.Processes;


namespace TestSpecificationParser.PlatformInfos;

public class IosDeviceInfo : IDeviceInfo
{
    private readonly ProcessRunner processRunner = new();
    
    public bool IsDeviceConnected(string deviceNumberString, out string udid, out string platformVersion)
    {
        var xcrunPath = "xcrun";
        var arguments = $"xctrace list devices";
        Console.WriteLine($"Executing command: {xcrunPath} {arguments}");

        var resultStrings = processRunner
            .GetProcessOutput(processRunner.StartProcess(xcrunPath, arguments)).ToList();

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

        var requiredDeviceInfo = devices[deviceNumber - 1];
        udid = requiredDeviceInfo.Udid;
        platformVersion = requiredDeviceInfo.PlatformVersion;
        
        Console.WriteLine($"Found device by number: {deviceNumberString} with udid: {udid} and iOS version: {platformVersion}");
        
        return true;
    }
}