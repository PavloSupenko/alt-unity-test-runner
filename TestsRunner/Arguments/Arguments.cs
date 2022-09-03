namespace TestsRunner.Arguments;

public enum GeneralArguments
{
    HostPlatform,
    Platform,

    AutoDetectDevice,
    DeviceId,
    
    TestsTree,
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
