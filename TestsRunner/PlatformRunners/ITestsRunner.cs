using TestsRunner.Arguments;


namespace TestsRunner.PlatformRunners;

public interface ITestsRunner<TArgsEnum, out TDriver> where TArgsEnum : Enum
{
    void Initialize(ArgumentsReader<GeneralArguments> generalArguments, ArgumentsReader<TArgsEnum> platformArguments);
    
    bool IsDeviceConnected(out string deviceId);
    void SetupPortForwarding(string deviceId, string tcpLocalPort, string tcpDevicePort);
    
    void RunAppiumServer();
    void StopAppiumServer();
    
    void RunAppiumSession(string deviceId, int sleepSeconds);
    void StopAppiumSession();
}
