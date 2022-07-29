namespace TestsRunner.Arguments;

public enum GeneralArguments
{
    Platform,
    TestsTree,
    RunOnDevice,
    BuildPath,
    
    LocalPort,
    DevicePort,
    
    NUnitConsoleApplicationPath,
    NUnitTestsAssemblyPath,
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
    Bundle
}

public enum IosArguments
{
    DeviceName,
    PlatformVersion,
    TeamId,
    SigningId,
}
