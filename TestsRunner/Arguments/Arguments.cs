namespace TestsRunner.Arguments;

public enum GeneralArguments
{
    Platform,
    UnityEditorPath,
    ProjectPath,
    TestsTree,
    LogFilePath,
    SkipPortForward,
    SkipRun,
    SkipTests,
    RunOnDevice,
    Help,
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
