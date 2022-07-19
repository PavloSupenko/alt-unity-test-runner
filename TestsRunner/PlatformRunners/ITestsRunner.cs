namespace TestsRunner.PlatformRunners;

public interface ITestsRunner
{
    bool IsDeviceConnected(string adbPath, string deviceNumberString, out string deviceId);
    void ReinstallApplication(string adbPath, string apkPath, string deviceId);
    void SetupPortForwarding(string adbPath, string tcpPort, string deviceId);
    void RunApplication(string adbPath, string bundle, string deviceId);
    void RunTests(string unityPath, string projectPath, string testsTreeFilePath, string pathToLogFile);
}
