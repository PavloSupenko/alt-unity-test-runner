using TestsRunner.Arguments;


namespace TestsRunner.PlatformRunners;

public interface ITestsRunner<TArgsEnum, out TDriver> where TArgsEnum : Enum
{
    TDriver Driver { get; }
    void Initialize(ArgumentsReader<GeneralArguments> generalArguments, ArgumentsReader<TArgsEnum> platformArguments);
    
    bool IsDeviceConnected(out string deviceId);
    void SetupPortForwarding(string deviceId);
    void RunApplication(string deviceId, int sleepSeconds);
    void RunTests();
}
