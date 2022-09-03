using TestsRunner.Arguments;


namespace TestsRunner.PlatformRunners;

public interface ITestsRunner<TArgsEnum> : ITestsRunner where TArgsEnum : Enum
{
    void Initialize(ArgumentsReader<TArgsEnum> platformArguments);
}

public interface ITestsRunner
{
    bool IsDeviceConnected(string deviceNumber, out string deviceId);
    void SetupPortForwarding(string deviceId, string tcpLocalPort, string tcpDevicePort);
    
    void RunAppiumServer(string hostPlatform);
    void StopAppiumServer();
    
    void RunAppiumSession(string deviceId, string deviceNumber, string buildPath, string bundle, int sleepSeconds);
    void StopAppiumSession();
}
