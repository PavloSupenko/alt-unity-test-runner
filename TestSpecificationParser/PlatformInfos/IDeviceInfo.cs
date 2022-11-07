namespace TestSpecificationParser.PlatformInfos;

public interface IDeviceInfo
{
    bool FindFirstConnectedDevice(out string deviceNumberString, out string udid, out string platformVersion);
}