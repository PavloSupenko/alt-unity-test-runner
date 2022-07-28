namespace TestsRunner.Arguments;

static class ArgumentKeys
{
    public static readonly Dictionary<string, GeneralArguments> GeneralKeys = new()
    {
        ["--platform"] = GeneralArguments.Platform,

        ["--tests"] = GeneralArguments.TestsTree,
        ["--run-on-device"] = GeneralArguments.RunOnDevice,
        ["--nunit-console"] = GeneralArguments.NUnitConsoleApplicationPath,

        ["--skip-port-forward"] = GeneralArguments.SkipPortForward,
        ["--skip-server-run"] = GeneralArguments.SkipServerRun,
        ["--skip-session-run"] = GeneralArguments.SkipSessionRun,
        ["--skip-tests"] = GeneralArguments.SkipTests,

        ["--help"] = GeneralArguments.Help,
        ["--defaults"] = GeneralArguments.Defaults,
    };

    public static readonly Dictionary<string, AndroidArguments> AndroidKeys = new()
    {
        ["--adb"] = AndroidArguments.AndroidDebugBridgePath,
        ["--android-home"] = AndroidArguments.AndroidHomePath,
        ["--java-home"] = AndroidArguments.JavaHomePath,
        ["--tcp"] = AndroidArguments.TcpPort,
        ["--apk"] = AndroidArguments.ApkPath,
        ["--bundle"] = AndroidArguments.Bundle,
    };

    public static readonly Dictionary<string, IosArguments> IosKeys = new()
    {
        ["--tcp"] = IosArguments.TcpPort,
        ["--ipa"] = IosArguments.IpaPath,
        ["--bundle"] = IosArguments.Bundle,
    };

    public static readonly Dictionary<GeneralArguments, string> GeneralDefaults = new()
    {
        [GeneralArguments.Platform] = "none",

        [GeneralArguments.TestsTree] = "TestsTreeTemplate.json",
        [GeneralArguments.RunOnDevice] = "1",
        [GeneralArguments.NUnitConsoleApplicationPath] = "none",

        [GeneralArguments.SkipPortForward] = "false",
        [GeneralArguments.SkipServerRun] = "false",
        [GeneralArguments.SkipSessionRun] = "false",
        [GeneralArguments.SkipTests] = "false",

        [GeneralArguments.Help] = "false",
        [GeneralArguments.Defaults] = "false",
    };

    public static readonly Dictionary<AndroidArguments, string> AndroidDefaults = new()
    {
        [AndroidArguments.TcpPort] = "13000",
        [AndroidArguments.AndroidDebugBridgePath] = "D:\\Unity Editors\\2020.3.30f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\SDK\\platform-tools\\adb.exe",
        [AndroidArguments.AndroidHomePath] = "D:\\Unity Editors\\2020.3.30f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\SDK",
        [AndroidArguments.JavaHomePath] = "C:\\Program Files\\Java\\jre1.8.0_321",
        [AndroidArguments.ApkPath] = "D:\\_prjGitHub\\bini-acad-drawing\\bini-acad-drawing\\artifacts\\AltUnit\\bini-acad-drawing.google.develop.apk",
        [AndroidArguments.Bundle] = "com.binibambini.acad.drawing",
    };

    public static readonly Dictionary<IosArguments, string> IosDefaults = new()
    {
        [IosArguments.TcpPort] = "13000",
        [IosArguments.IpaPath] = "bini-acad-drawing.google.apple.ipa",
        [IosArguments.Bundle] = "com.binibambini.acad.drawing",
    };

    public static readonly Dictionary<GeneralArguments, string> GeneralDescriptions = new()
    {
        [GeneralArguments.Platform] = "test platform. Values [ios/android]",

        [GeneralArguments.TestsTree] = "path to tests tree template json file. Value: string",
        [GeneralArguments.RunOnDevice] = "number of device to run tests on. Values: [1..n]",
        [GeneralArguments.NUnitConsoleApplicationPath] = "path to console application to run NUnitv3+ tests. Value: string",

        [GeneralArguments.SkipPortForward] = "skip port forwarding. Values [true/false]",
        [GeneralArguments.SkipServerRun] = "skip running (or opening if it's already ran) application on device. Values [true/false]",
        [GeneralArguments.SkipSessionRun] = "skip installing and running application with initialization of appium session. " +
                                            "Warning: tests that use appium API will not run. Values [true/false]",
        [GeneralArguments.SkipTests] = "skip running tests. Values [true/false]",

        [GeneralArguments.Help] = "show help. Values [true/false]",
        [GeneralArguments.Defaults] = "add default values info to help commands. Only used with --help command. Values [true/false]",
    };

    public static readonly Dictionary<AndroidArguments, string> AndroidDescriptions = new()
    {
        [AndroidArguments.TcpPort] = "port to set up port forwarding. Values [13000, 13001, 13002,...n]",
        [AndroidArguments.AndroidDebugBridgePath] = "adb path. Value: string",
        [AndroidArguments.AndroidHomePath] = "android SDK directory path. Value: string",
        [AndroidArguments.JavaHomePath] = "java home path. Value: string",
        [AndroidArguments.ApkPath] = "apk path. Value: string",
        [AndroidArguments.Bundle] = "bundle. Value: string",
    };

    public static readonly Dictionary<IosArguments, string> IosDescriptions = new()
    {
        [IosArguments.TcpPort] = "port to set up port forwarding. Values [13000, 13001, 13002,...n]",
        [IosArguments.IpaPath] = "ipa path. Value: string",
        [IosArguments.Bundle] = "bundle. Value: string",
    };
}
