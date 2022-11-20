namespace AmazonDeviceFarmClient.Arguments;

public static class DeviceFarmArgumentValues
{
    public static readonly Dictionary<string, DeviceFarmArgument> Keys = new()
    {
        ["--project"] = DeviceFarmArgument.ProjectName, ["-p"] = DeviceFarmArgument.ProjectName,

        ["--user-key"] = DeviceFarmArgument.UserKey, ["-uk"] = DeviceFarmArgument.UserKey,
        ["--user-secret"] = DeviceFarmArgument.UserSecret, ["-us"] = DeviceFarmArgument.UserSecret,

        ["--upload-application"] = DeviceFarmArgument.UploadApplication, ["-ua"] = DeviceFarmArgument.UploadApplication,
        ["--application-name"] = DeviceFarmArgument.ApplicationName, ["-an"] = DeviceFarmArgument.ApplicationName,
        ["--application-platform"] = DeviceFarmArgument.ApplicationPlatform, ["-ap"] = DeviceFarmArgument.ApplicationPlatform,

        ["--upload-test-specs"] = DeviceFarmArgument.UploadTestSpecs, ["-uts"] = DeviceFarmArgument.UploadTestSpecs,
        ["--test-specs-name"] = DeviceFarmArgument.TestSpecsName, ["-tsn"] = DeviceFarmArgument.TestSpecsName,

        ["--upload-test-package"] = DeviceFarmArgument.UploadTestPackage, ["-utp"] = DeviceFarmArgument.UploadTestPackage,
        ["--test-package-name"] = DeviceFarmArgument.TestPackageName, ["-tpn"] = DeviceFarmArgument.TestPackageName,
        
        ["--run-name"] = DeviceFarmArgument.RunName, ["-r"] = DeviceFarmArgument.RunName,
        ["--device-pool-name"] = DeviceFarmArgument.DevicePoolName, ["-dp"] = DeviceFarmArgument.DevicePoolName,
        ["--timeout"] = DeviceFarmArgument.Timeout, ["-t"] = DeviceFarmArgument.Timeout,
        ["--artifacts-path"] = DeviceFarmArgument.ArtifactsPath, ["-a"] = DeviceFarmArgument.ArtifactsPath,
        
        ["--tests-filter"] = DeviceFarmArgument.TestsFilter, ["-f"] = DeviceFarmArgument.TestsFilter,
        
        ["--help"] = DeviceFarmArgument.Help, ["-h"] = DeviceFarmArgument.Help,
    };

    public static readonly Dictionary<DeviceFarmArgument, string> Defaults = new()
    {
        [DeviceFarmArgument.ProjectName] = "None",
        [DeviceFarmArgument.UserKey] = "None",
        [DeviceFarmArgument.UserSecret] = "None",
        [DeviceFarmArgument.UploadApplication] = "false",
        [DeviceFarmArgument.ApplicationPlatform] = "None",
        [DeviceFarmArgument.UploadTestPackage] = "false",
        [DeviceFarmArgument.UploadTestSpecs] = "false",
        [DeviceFarmArgument.Timeout] = "15",
        [DeviceFarmArgument.TestsFilter] = "tests/",
    };

    public static readonly Dictionary<DeviceFarmArgument, string> Descriptions = new()
    {
        [DeviceFarmArgument.ProjectName] = "*Required. Target project name. Not ARN.",
        
        [DeviceFarmArgument.UserKey] = "*Required. User key",
        [DeviceFarmArgument.UserSecret] = "*Required. User secret",
        
        [DeviceFarmArgument.UploadApplication] = "*Optional. Path to application to upload. It will be used as default " +
                                                 "if application name parameter is not set.",
        [DeviceFarmArgument.ApplicationName] = "*Optional. Application name ('build.apk', for example) to use in test",
        [DeviceFarmArgument.ApplicationPlatform] = "*Required. Application platform ('iOS' or 'Android')",
        
        [DeviceFarmArgument.UploadTestPackage] = "*Optional. Path to test package to upload. It will be used as default " +
                                                 "if test package name parameter is not set.",
        [DeviceFarmArgument.TestPackageName] = "*Optional. Test package name ('package.zip', for example) to use in test",
        
        [DeviceFarmArgument.UploadTestSpecs] = "*Optional. Path to test spec to upload. It will be used as default " +
                                               "if test spec name parameter is not set.",
        [DeviceFarmArgument.TestSpecsName] = "*Optional. Test spec name ('android.yml', for example) to use in test",
        
        [DeviceFarmArgument.RunName] = "*Required. Test run name.",
        [DeviceFarmArgument.DevicePoolName] = "*Required. Device pool name to run on.",
        [DeviceFarmArgument.Timeout] = "*Optional. Run timeout in minutes for each device from pool.",
        
        [DeviceFarmArgument.ArtifactsPath] = "*Required. Path to download run artifacts.",
    };
}