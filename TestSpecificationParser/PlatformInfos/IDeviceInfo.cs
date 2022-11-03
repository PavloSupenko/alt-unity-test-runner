namespace TestSpecificationParser.PlatformInfos;

public interface IDeviceInfo
{
    bool IsDeviceConnected(string deviceNumberString, out string udid, out string platformVersion);
}