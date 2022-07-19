namespace TestsRunner;

static class ArgumentKeys
{
    public static readonly Dictionary<string, GeneralArguments> GeneralKeys = new()
    {
        ["-project"] = GeneralArguments.ProjectPath,
        ["-unity"] = GeneralArguments.UnityEditorPath,
        ["-logFile"] = GeneralArguments.LogFilePath,
        ["-tests"] = GeneralArguments.TestsTree,
        ["-run-on-device"] = GeneralArguments.RunOnDevice,

        ["-skip-install"] = GeneralArguments.SkipInstall,
        ["-skip-port-forward"] = GeneralArguments.SkipPortForward,
        ["-skip-run"] = GeneralArguments.SkipRun,
        ["-skip-tests"] = GeneralArguments.SkipTests,
    };

    public static readonly Dictionary<string, AndroidArguments> AndroidKeys = new()
    {
        ["-adb"] = AndroidArguments.AndroidDebugBridgePath,
        ["-tcp"] = AndroidArguments.TcpPort,
        ["-apk"] = AndroidArguments.ApkPath,
        ["-bundle"] = AndroidArguments.Bundle,
    };

    public static readonly Dictionary<GeneralArguments, string> GeneralDefaults = new()
    {
        [GeneralArguments.ProjectPath] = "D:\\_prjGitHub\\bini-acad-drawing\\bini-acad-drawing\\src\\acad-bini-drawing",
        [GeneralArguments.UnityEditorPath] = "D:\\Unity Editors\\2020.3.30f1\\Editor\\Unity.exe",
        [GeneralArguments.TestsTree] = "TestsTreeTemplate.json",
        [GeneralArguments.LogFilePath] = "D:\\Unity Editors\\2020.3.30f1\\Editor\\testsLogFile.log",
        [GeneralArguments.RunOnDevice] = "1",

        [GeneralArguments.SkipInstall] = "false",
        [GeneralArguments.SkipPortForward] = "false",
        [GeneralArguments.SkipRun] = "false",
        [GeneralArguments.SkipTests] = "false",
    };

    public static readonly Dictionary<AndroidArguments, string> AndroidDefaults = new()
    {
        [AndroidArguments.TcpPort] = "13000",
        [AndroidArguments.AndroidDebugBridgePath] = "D:\\Unity Editors\\2020.3.30f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\SDK\\platform-tools\\adb.exe",
        [AndroidArguments.ApkPath] = "D:\\_prjGitHub\\bini-acad-drawing\\bini-acad-drawing\\artifacts\\AltUnit\\bini-acad-drawing.google.develop.apk",
        [AndroidArguments.Bundle] = "com.binibambini.acad.drawing",
    };
}
