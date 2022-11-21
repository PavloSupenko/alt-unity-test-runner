namespace AmazonDeviceFarmRunAwaiter.Arguments;

public static class ArgumentValues
{
    public static readonly Dictionary<string, Argument> Keys = new()
    {
        ["--project"] = Argument.ProjectName, ["-p"] = Argument.ProjectName,

        ["--user-key"] = Argument.UserKey, ["-uk"] = Argument.UserKey,
        ["--user-secret"] = Argument.UserSecret, ["-us"] = Argument.UserSecret,

        ["--run-name"] = Argument.RunName, ["-r"] = Argument.RunName,
        ["--artifacts-path"] = Argument.ArtifactsPath, ["-a"] = Argument.ArtifactsPath,

        ["--help"] = Argument.Help, ["-h"] = Argument.Help,
    };

    public static readonly Dictionary<Argument, string> Defaults = new()
    {
        [Argument.ProjectName] = "None",
        [Argument.UserKey] = "None",
        [Argument.UserSecret] = "None",
    };

    public static readonly Dictionary<Argument, string> Descriptions = new()
    {
        [Argument.ProjectName] = "*Required. Target project name. Not ARN.",
        
        [Argument.UserKey] = "*Required. User key",
        [Argument.UserSecret] = "*Required. User secret",
        
        [Argument.RunName] = "*Required. Test run name.",
        [Argument.ArtifactsPath] = "*Required. Path to download run artifacts.",
    };
}