namespace TestsRunner.Arguments;

public enum GeneralArguments
{
    HostPlatform,
    Platform,
    
    TestsTree,
    RunOnDevice,
    BuildPath,
    Bundle,
    
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
}

// todo: make auto-recognizing parameters values
public enum IosArguments
{
    DeviceName,
    PlatformVersion,
    TeamId,
    SigningId,
}
