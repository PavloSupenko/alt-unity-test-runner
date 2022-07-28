namespace TestsRunner.Arguments;

public enum GeneralArguments
{
    Platform,
    TestsTree,
    RunOnDevice,
    
    NUnitConsoleApplicationPath,
    
    SkipPortForward,
    SkipRun,
    SkipTests,
    
    Help,
    Defaults,
}

public enum AndroidArguments
{
    AndroidDebugBridgePath,
    AndroidHomePath,
    JavaHomePath,
    TcpPort,
    ApkPath,
    Bundle
}

public enum IosArguments
{
    TcpPort,
    IpaPath,
    Bundle
}
