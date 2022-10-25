namespace TestSpecificationParser.Arguments;

public class BashScriptBuilderArgumentValues
{
    public static readonly Dictionary<string, BashScriptBuilderArgument> Keys = new()
    {
        ["--device-name"] = BashScriptBuilderArgument.DeviceName, ["-dn"] = BashScriptBuilderArgument.DeviceName,
        ["--device-udid"] = BashScriptBuilderArgument.DeviceId, ["-udid"] = BashScriptBuilderArgument.DeviceId,
        ["--device-platform"] = BashScriptBuilderArgument.DevicePlatformName, ["-pn"] = BashScriptBuilderArgument.DevicePlatformName,
        ["--device-platform-version"] = BashScriptBuilderArgument.DevicePlatformVersion, ["-pv"] = BashScriptBuilderArgument.DevicePlatformVersion,
        ["--artifacts"] = BashScriptBuilderArgument.ArtifactsDirectory, ["-a"] = BashScriptBuilderArgument.ArtifactsDirectory,
        ["--test-package"] = BashScriptBuilderArgument.TestPackageDirectory, ["-tp"] = BashScriptBuilderArgument.TestPackageDirectory,
        ["--application"] = BashScriptBuilderArgument.ApplicationPath, ["-app"] = BashScriptBuilderArgument.ApplicationPath,
        ["--yaml"] = BashScriptBuilderArgument.YamlFilePath, ["-y"] = BashScriptBuilderArgument.YamlFilePath,
        ["--bash"] = BashScriptBuilderArgument.ShellFilePath, ["-b"] = BashScriptBuilderArgument.ShellFilePath,
        ["--help"] = BashScriptBuilderArgument.Help, ["-h"] = BashScriptBuilderArgument.Help,
    };

    public static readonly Dictionary<BashScriptBuilderArgument, string> Defaults = new();

    public static readonly Dictionary<BashScriptBuilderArgument, string> Descriptions = new()
    {
        [BashScriptBuilderArgument.DeviceName] = "*Required. UDID for iOS device and any name for Android.",
        [BashScriptBuilderArgument.DeviceId] = "*Required. UDID for iOS device and ADB ID for Android.",
        [BashScriptBuilderArgument.DevicePlatformName] = "*Required. Platform name. Android or iOS.",
        [BashScriptBuilderArgument.DevicePlatformVersion] = "*Required. Device Platform version. 11, 14.2 etc.",
        [BashScriptBuilderArgument.ArtifactsDirectory] = "*Required. Directory to save screenshots, logs and report.",
        [BashScriptBuilderArgument.TestPackageDirectory] = "*Required. Unpacked test package directory. tests folder and requirements.txt file are required.",
        [BashScriptBuilderArgument.ApplicationPath] = "*Required. apk or ipa path.",
        [BashScriptBuilderArgument.YamlFilePath] = "*Required. yaml file to regenerate into bash script.",
        [BashScriptBuilderArgument.ShellFilePath] = "*Required. bash result file.",
    };
}