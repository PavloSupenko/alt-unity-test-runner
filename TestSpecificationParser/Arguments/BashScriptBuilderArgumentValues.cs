namespace TestSpecificationParser.Arguments;

public class BashScriptBuilderArgumentValues
{
    public static readonly Dictionary<string, BashScriptBuilderArgument> Keys = new()
    {
        ["--device-platform"] = BashScriptBuilderArgument.DevicePlatformName, ["-pn"] = BashScriptBuilderArgument.DevicePlatformName,
        ["--artifacts"] = BashScriptBuilderArgument.ArtifactsDirectory, ["-a"] = BashScriptBuilderArgument.ArtifactsDirectory,
        ["--test-package"] = BashScriptBuilderArgument.TestPackageDirectory, ["-tp"] = BashScriptBuilderArgument.TestPackageDirectory,
        ["--application"] = BashScriptBuilderArgument.ApplicationPath, ["-app"] = BashScriptBuilderArgument.ApplicationPath,
        ["--yaml"] = BashScriptBuilderArgument.YamlFilePath, ["-y"] = BashScriptBuilderArgument.YamlFilePath,
        ["--bash"] = BashScriptBuilderArgument.ShellFilePath, ["-b"] = BashScriptBuilderArgument.ShellFilePath,
        ["--cloud"] = BashScriptBuilderArgument.ShellFilePath, ["-c"] = BashScriptBuilderArgument.IsCloudRun,
        ["--help"] = BashScriptBuilderArgument.Help, ["-h"] = BashScriptBuilderArgument.Help,
    };

    public static readonly Dictionary<BashScriptBuilderArgument, string> Defaults = new()
    {
        [BashScriptBuilderArgument.IsCloudRun] = "false",
    };

    public static readonly Dictionary<BashScriptBuilderArgument, string> Descriptions = new()
    {
        [BashScriptBuilderArgument.DevicePlatformName] = "*Required. Platform name. Android or iOS.",
        [BashScriptBuilderArgument.ArtifactsDirectory] = "*Required. Directory to save screenshots, logs and report.",
        [BashScriptBuilderArgument.TestPackageDirectory] = "*Required. Unpacked test package directory. tests folder and requirements.txt file are required.",
        [BashScriptBuilderArgument.ApplicationPath] = "*Required. Apk or ipa path.",
        [BashScriptBuilderArgument.YamlFilePath] = "*Required. Yaml file to regenerate into bash script.",
        [BashScriptBuilderArgument.IsCloudRun] = "*Required. Should run starts on AWS cloud platform.",
        [BashScriptBuilderArgument.ShellFilePath] = "*Required. Bash result file.",
    };
}