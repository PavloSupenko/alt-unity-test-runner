namespace TestsRunner.Arguments;

public enum GeneralArguments
{
    Platform,
    TestsTree,
    RunOnDevice,
    
    LocalPort,
    DevicePort,
    
    NUnitConsoleApplicationPath,
    TestSystemOutputLogFilePath,
    
    SkipPortForward,
    SkipServerRun,
    SkipSessionRun,
    SkipTests,
    
    Help,
    Defaults,
}

public enum AndroidArguments
{
    AndroidDebugBridgePath,
    AndroidHomePath,
    JavaHomePath,
    ApkPath,
    Bundle
}

public enum IosArguments
{
    IpaPath,
    DeviceName,
    PlatformVersion,
    TeamId,
    SigningId,
}
