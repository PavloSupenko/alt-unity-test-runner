namespace AmazonDeviceFarmRunStarter.Arguments;

public static class ArgumentValues
{
    public static readonly Dictionary<string, Argument> Keys = new()
    {
        ["--project"] = Argument.ProjectName, ["-p"] = Argument.ProjectName,

        ["--user-key"] = Argument.UserKey, ["-uk"] = Argument.UserKey,
        ["--user-secret"] = Argument.UserSecret, ["-us"] = Argument.UserSecret,

        ["--upload-application"] = Argument.UploadApplication, ["-ua"] = Argument.UploadApplication,
        ["--application-name"] = Argument.ApplicationName, ["-an"] = Argument.ApplicationName,
        ["--application-platform"] = Argument.ApplicationPlatform, ["-ap"] = Argument.ApplicationPlatform,

        ["--upload-test-specs"] = Argument.UploadTestSpecs, ["-uts"] = Argument.UploadTestSpecs,
        ["--test-specs-name"] = Argument.TestSpecsName, ["-tsn"] = Argument.TestSpecsName,

        ["--upload-test-package"] = Argument.UploadTestPackage, ["-utp"] = Argument.UploadTestPackage,
        ["--test-package-name"] = Argument.TestPackageName, ["-tpn"] = Argument.TestPackageName,
        
        ["--run-name"] = Argument.RunName, ["-r"] = Argument.RunName,
        ["--device-pool-name"] = Argument.DevicePoolName, ["-dp"] = Argument.DevicePoolName,
        ["--timeout"] = Argument.Timeout, ["-t"] = Argument.Timeout,
        
        ["--tests-filter"] = Argument.TestsFilter, ["-f"] = Argument.TestsFilter,
        
        ["--help"] = Argument.Help, ["-h"] = Argument.Help,
    };

    public static readonly Dictionary<Argument, string> Defaults = new()
    {
        [Argument.ProjectName] = "None",
        [Argument.UserKey] = "None",
        [Argument.UserSecret] = "None",
        [Argument.UploadApplication] = "false",
        [Argument.ApplicationPlatform] = "None",
        [Argument.UploadTestPackage] = "false",
        [Argument.UploadTestSpecs] = "false",
        [Argument.Timeout] = "20",
        [Argument.TestsFilter] = "tests/",
    };

    public static readonly Dictionary<Argument, string> Descriptions = new()
    {
        [Argument.ProjectName] = "*Required. Target project name. Not ARN.",
        
        [Argument.UserKey] = "*Required. User key",
        [Argument.UserSecret] = "*Required. User secret",
        
        [Argument.UploadApplication] = "*Optional. Path to application to upload. It will be used as default " +
                                                 "if application name parameter is not set.",
        [Argument.ApplicationName] = "*Optional. Application name ('build.apk', for example) to use in test",
        [Argument.ApplicationPlatform] = "*Required. Application platform ('iOS' or 'Android')",
        
        [Argument.UploadTestPackage] = "*Optional. Path to test package to upload. It will be used as default " +
                                                 "if test package name parameter is not set.",
        [Argument.TestPackageName] = "*Optional. Test package name ('package.zip', for example) to use in test",
        
        [Argument.UploadTestSpecs] = "*Optional. Path to test spec to upload. It will be used as default " +
                                               "if test spec name parameter is not set.",
        [Argument.TestSpecsName] = "*Optional. Test spec name ('android.yml', for example) to use in test",
        
        [Argument.RunName] = "*Required. Test run name.",
        [Argument.DevicePoolName] = "*Required. Device pool name to run on.",
        [Argument.Timeout] = "*Optional. Run timeout in minutes for each device from pool.",
    };
}