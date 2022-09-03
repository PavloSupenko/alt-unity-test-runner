namespace TestsRunner.Arguments;

static class ArgumentKeys
{
    public static readonly Dictionary<string, GeneralArguments> GeneralKeys = new()
    {
        ["--host-platform"] = GeneralArguments.HostPlatform,
        ["--platform"] = GeneralArguments.Platform,
        ["--build"] = GeneralArguments.BuildPath,
        
        ["--bundle"] = GeneralArguments.Bundle,

        ["--auto-detect-device"] = GeneralArguments.AutoDetectDevice,
        ["--device-id"] = GeneralArguments.DeviceId,

        ["--tests"] = GeneralArguments.TestsTree,
        ["--nunit-console"] = GeneralArguments.NUnitConsoleApplicationPath,
        ["--nunit-assembly"] = GeneralArguments.NUnitTestsAssemblyPath,
        ["--log"] = GeneralArguments.TestSystemOutputLogFilePath,
        
        ["--port-local"] = GeneralArguments.LocalPort,
        ["--port-device"] = GeneralArguments.DevicePort,

        ["--skip-port-forward"] = GeneralArguments.SkipPortForward,
        ["--skip-server-run"] = GeneralArguments.SkipServerRun,
        ["--skip-session-run"] = GeneralArguments.SkipSessionRun,
        ["--skip-tests"] = GeneralArguments.SkipTests,

        ["--help"] = GeneralArguments.Help,
        ["--defaults"] = GeneralArguments.Defaults,
        
        //* Short versions *//
        
        ["-hp"] = GeneralArguments.HostPlatform,
        ["-p"] = GeneralArguments.Platform,
        ["-b"] = GeneralArguments.BuildPath,
        
        ["-app"] = GeneralArguments.Bundle,
        
        ["-n"] = GeneralArguments.AutoDetectDevice,
        ["-i"] = GeneralArguments.DeviceId,

        ["-t"] = GeneralArguments.TestsTree,
        ["-c"] = GeneralArguments.NUnitConsoleApplicationPath,
        ["-a"] = GeneralArguments.NUnitTestsAssemblyPath,
        ["-l"] = GeneralArguments.TestSystemOutputLogFilePath,
        
        ["-ah"] = GeneralArguments.LocalPort,
        ["-ad"] = GeneralArguments.DevicePort,

        ["-sp"] = GeneralArguments.SkipPortForward,
        ["-ss"] = GeneralArguments.SkipServerRun,
        ["-sr"] = GeneralArguments.SkipSessionRun,
        ["-st"] = GeneralArguments.SkipTests,

        ["-h"] = GeneralArguments.Help,
        ["-d"] = GeneralArguments.Defaults,
    };

    public static readonly Dictionary<string, AndroidArguments> AndroidKeys = new()
    {
        ["--adb"] = AndroidArguments.AndroidDebugBridgePath,
        ["--android-home"] = AndroidArguments.AndroidHomePath,
        ["--java-home"] = AndroidArguments.JavaHomePath,
    };

    public static readonly Dictionary<string, IosArguments> IosKeys = new()
    {
        ["--device-name"] = IosArguments.DeviceName,
        ["--ios-version"] = IosArguments.PlatformVersion,
        ["--signing-id"] = IosArguments.SigningId,
        ["--team-id"] = IosArguments.TeamId,
    };

    public static readonly Dictionary<GeneralArguments, string> GeneralDefaults = new()
    {
        [GeneralArguments.HostPlatform] = "osx",

        [GeneralArguments.TestsTree] = "TestsTreeTemplate.json",
        [GeneralArguments.TestSystemOutputLogFilePath] = "tests-log.log",
        
        [GeneralArguments.LocalPort] = "13000",
        [GeneralArguments.DevicePort] = "13000",

        [GeneralArguments.SkipPortForward] = "false",
        [GeneralArguments.SkipServerRun] = "false",
        [GeneralArguments.SkipSessionRun] = "false",
        [GeneralArguments.SkipTests] = "false",

        [GeneralArguments.Help] = "false",
        [GeneralArguments.Defaults] = "false",
    };

    public static readonly Dictionary<AndroidArguments, string> AndroidDefaults = new()
    {
        [AndroidArguments.AndroidDebugBridgePath] = "D:\\Unity Editors\\2020.3.30f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\SDK\\platform-tools\\adb.exe",
        [AndroidArguments.AndroidHomePath] = "D:\\Unity Editors\\2020.3.30f1\\Editor\\Data\\PlaybackEngines\\AndroidPlayer\\SDK",
        [AndroidArguments.JavaHomePath] = "C:\\Program Files\\Java\\jre1.8.0_321",
    };

    // todo: Get device name and iOS version from device udid
    public static readonly Dictionary<IosArguments, string> IosDefaults = new()
    {
        [IosArguments.DeviceName] = "Pavlo’s iPhone",
        [IosArguments.PlatformVersion] = "14.2",
        [IosArguments.SigningId] = "Apple Development",
        [IosArguments.TeamId] = "J86B8Y8A5P",
    };

    public static readonly Dictionary<GeneralArguments, string> GeneralDescriptions = new()
    {
        [GeneralArguments.HostPlatform] = "computer platform. Values [osx/windows]",
        [GeneralArguments.Platform] = "test platform. Values [ios/android]",
        [GeneralArguments.BuildPath] = "path to .apk for android / .ipa or .app for iOS. Value: string",
        
        [GeneralArguments.AutoDetectDevice] = "number of device to auto detect to run tests on. Values: [1..n]",
        [GeneralArguments.DeviceId] = "device id to run tests on. Used when auto detection is not allowed. Value: string",

        [GeneralArguments.TestsTree] = "path to tests tree template json file. Value: string",
        [GeneralArguments.NUnitConsoleApplicationPath] = "path to console application to run NUnitv3+ tests. Value: string",
        [GeneralArguments.NUnitTestsAssemblyPath] = "path to console tests assembly. Value: string",
        
        [GeneralArguments.Bundle] = "application bundle. Value: string",
        
        [GeneralArguments.TestSystemOutputLogFilePath] = "file to save NUnit output logs. Value: string",
        
        [GeneralArguments.LocalPort] = "local machine port to set up port forwarding. Values [13000, 13001, 13002,...n]",
        [GeneralArguments.DevicePort] = "connected device port to set up port forwarding. Values [13000, 13001, 13002,...n]",

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
        [AndroidArguments.AndroidHomePath] = "android SDK directory path. Value: string",
        [AndroidArguments.JavaHomePath] = "java home path. Value: string",
        [AndroidArguments.AndroidDebugBridgePath] = "adb path. Value: string",
    };

    public static readonly Dictionary<IosArguments, string> IosDescriptions = new()
    {
        [IosArguments.DeviceName] = "device name. Value: string",
        [IosArguments.PlatformVersion] = "version of iOS. Value: string",
        [IosArguments.SigningId] = "signing id. Value: string",
        [IosArguments.TeamId] = "team id. Value: string",
    };
}
