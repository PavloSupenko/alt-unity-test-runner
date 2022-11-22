namespace TestSpecificationParser.Arguments;

public class ArgumentValues
{
    public static readonly Dictionary<string, Argument> Keys = new()
    {
        ["--device-platform"] = Argument.DevicePlatformName, ["-pn"] = Argument.DevicePlatformName,
        ["--artifacts"] = Argument.ArtifactsDirectory, ["-a"] = Argument.ArtifactsDirectory,
        ["--test-package"] = Argument.TestPackageDirectory, ["-tp"] = Argument.TestPackageDirectory,
        ["--application"] = Argument.ApplicationPath, ["-app"] = Argument.ApplicationPath,
        ["--yaml"] = Argument.YamlFilePath, ["-y"] = Argument.YamlFilePath,
        ["--bash"] = Argument.ShellFilePath, ["-b"] = Argument.ShellFilePath,
        ["--cloud"] = Argument.ShellFilePath, ["-c"] = Argument.IsCloudRun,
        ["--wda-derived-data-path"] = Argument.ShellFilePath, ["-wda"] = Argument.WdaDerivedDataPath,
        ["--tests-filter"] = Argument.TestsFilter, ["-f"] = Argument.TestsFilter,
        ["--help"] = Argument.Help, ["-h"] = Argument.Help,
    };

    public static readonly Dictionary<Argument, string> Defaults = new()
    {
        [Argument.IsCloudRun] = "false",
    };

    public static readonly Dictionary<Argument, string> Descriptions = new()
    {
        [Argument.DevicePlatformName] = "*Required. Platform name. Android or iOS.",
        [Argument.ArtifactsDirectory] = "*Required. Directory to save screenshots, logs and report.",
        [Argument.TestPackageDirectory] = "*Required. Unpacked test package directory. tests folder and requirements.txt file are required.",
        [Argument.ApplicationPath] = "*Required. Apk or ipa path.",
        [Argument.YamlFilePath] = "*Required. Yaml file to regenerate into bash script.",
        [Argument.IsCloudRun] = "*Required. Should run starts on AWS cloud platform.",
        [Argument.ShellFilePath] = "*Required. Bash result file.",
        [Argument.WdaDerivedDataPath] = "*Required. Path to WebDriverAgent prebuilt derived data.",
        [Argument.TestsFilter] = "*Required. Tests filter according to pytest rules.",
    };
}