namespace TestsRunner.Arguments;

public enum GeneralArguments
{
    Platform,
    UnityEditorPath,
    ProjectPath,
    TestsTree,
    SkipInstall,
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
    TcpPort,
    ApkPath,
    Bundle
}

public enum IosArguments
{
    XcodeRunPath,
    TcpPort,
    IpaPath,
    Bundle
}
