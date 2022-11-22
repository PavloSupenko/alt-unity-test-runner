namespace TestsHtmlReportCreator.Arguments;

public static class ArgumentValues
{
    public static readonly Dictionary<string, Argument> Keys = new()
    {
        ["--local-android-files"] = Argument.LocalAndroidPath, ["-la"] = Argument.LocalAndroidPath,
        ["--local-ios-files"] = Argument.LocalIosPath, ["-li"] = Argument.LocalIosPath,
        ["--remote-android-files"] = Argument.RemoteAndroidPath, ["-ra"] = Argument.RemoteAndroidPath,
        ["--remote-ios-files"] = Argument.RemoteIosPath, ["-ri"] = Argument.RemoteIosPath,
        
        ["--report-path"] = Argument.ReportPath, ["-r"] = Argument.ReportPath,
    };

    public static readonly Dictionary<Argument, string> Defaults = new()
    {
    };

    public static readonly Dictionary<Argument, string> Descriptions = new()
    {
        [Argument.LocalAndroidPath] = "*Required. Path to android test result files on local farm.",
        [Argument.LocalIosPath] = "*Required. Path to ios test result files on local farm.",
        [Argument.RemoteAndroidPath] = "*Required. Path to android test result files on remote farm.",
        [Argument.RemoteIosPath] = "*Required. Path to ios test result files on remote farm.",
        [Argument.ReportPath] = "*Required. Path to generated HTML document.",
    };
}