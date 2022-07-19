using TestsRunner.Arguments;


namespace TestsRunner.PlatformRunners;

public interface ITestsRunner<TArgsEnum> where TArgsEnum : Enum
{
    void Initialize(ArgumentsReader<GeneralArguments> generalArguments, ArgumentsReader<TArgsEnum> platformArguments);
    bool IsDeviceConnected(out string deviceId);
    void ReinstallApplication(string deviceId);
    void SetupPortForwarding(string deviceId);
    void RunApplication(string deviceId, int sleepSeconds);
    void RunTests();
}
