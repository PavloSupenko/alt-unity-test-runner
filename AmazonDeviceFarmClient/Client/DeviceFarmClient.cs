using Amazon;
using Amazon.DeviceFarm;
using Amazon.DeviceFarm.Model;
using Amazon.Runtime;


namespace AmazonDeviceFarmClient.Client;

public class DeviceFarmClient
{
    private readonly Amazon.DeviceFarm.AmazonDeviceFarmClient client;

    public DeviceFarmClient(string accessKey, string secretKey)
    {
        AWSCredentials credentials = new BasicAWSCredentials(accessKey, secretKey);
        
        AmazonDeviceFarmConfig config = new AmazonDeviceFarmConfig() { RegionEndpoint = RegionEndpoint.USWest2 };
        client = new Amazon.DeviceFarm.AmazonDeviceFarmClient(credentials, config);
    }
    
    public async Task UploadAndroidApplication(string name, string projectName, string filePath)
    {
        Upload? uploadData = (await CreateUpload(name, projectName, UploadType.ANDROID_APP)).Upload;
        await UploadFileToFarm(filePath, uploadData.Url);
    }

    public async Task UploadIosApplication(string name, string projectName, string filePath)
    {
        Upload? uploadData = (await CreateUpload(name, projectName, UploadType.IOS_APP)).Upload;
        await UploadFileToFarm(filePath, uploadData.Url);
    }

    public async Task UploadTestSpec(string name, string projectName, string filePath)
    {
        Upload? uploadData = (await CreateUpload(name, projectName, UploadType.APPIUM_PYTHON_TEST_SPEC)).Upload;
        await UploadFileToFarm(filePath, uploadData.Url);
    }

    public async Task UploadTestPackage(string name, string projectName, string filePath)
    {
        Upload? uploadData = (await CreateUpload(name, projectName, UploadType.APPIUM_PYTHON_TEST_PACKAGE)).Upload;
        await UploadFileToFarm(filePath, uploadData.Url);
    }

    public async Task<ScheduleRunResponse> ScheduleTestRun(string runName, ApplicationPlatform platform, 
        string projectName, string devicePoolName, string testSpecName, int timeout)
    {
        string projectArn = (await GetProjectByName(projectName)).Arn;
        string testPackageArn = (await GetLastTestPackage(projectName)).Arn;
        string testSpecArn = (await GetTestSpecByName(projectName, testSpecName)).Arn;
        string devicePoolArn = (await GetDevicePoolByName(devicePoolName, projectName, true)).Arn;
        string applicationArn = (platform == ApplicationPlatform.Android
            ? (await GetLastAndroidApplication(projectName))
            : (await GetLastIosApplication(projectName))).Arn;
        
        return await client.ScheduleRunAsync(new ScheduleRunRequest()
        {
            Name = runName,
            ProjectArn = projectArn,
            AppArn = applicationArn,
            DevicePoolArn = devicePoolArn,
            ExecutionConfiguration = new ExecutionConfiguration()
            {
                VideoCapture = true, JobTimeoutMinutes = timeout
            },
            Test = new ScheduleRunTest()
            {
                Type = TestType.APPIUM_PYTHON, TestPackageArn = testPackageArn, TestSpecArn = testSpecArn
            }
        });
    }

    private Task<ScheduleRunResponse> ScheduleRun(Project project, Upload application, DevicePool devicePool, Upload testPackage, Upload testSpec) =>
        client.ScheduleRunAsync(new ScheduleRunRequest()
        {
            Name = "[Android] testing-running-from-dotnet",
            ProjectArn = project.Arn,
            AppArn = application.Arn,
            DevicePoolArn = devicePool.Arn,
            ExecutionConfiguration = new ExecutionConfiguration()
            {
                VideoCapture = true, JobTimeoutMinutes = 15
            },
            Test = new ScheduleRunTest()
            {
                Type = TestType.APPIUM_PYTHON, TestPackageArn = testPackage.Arn, TestSpecArn = testSpec.Arn
            }
        });

    private async Task<DevicePool?> GetDevicePoolByName(string name, string projectName, bool getCustomPools)
    {
        List<DevicePool>? pools = (await GetDevicePoolsResponse(projectName, getCustomPools)).DevicePools;
        DevicePool? targetPool = pools.First(pool => pool.Name.Equals(name));
        return targetPool;
    }

    private async Task<List<DevicePool>?> GetDevicePools(string projectName, bool getCustomPools) =>
        (await GetDevicePoolsResponse(projectName, getCustomPools))
        .DevicePools;

    private async Task<ListDevicePoolsResponse> GetDevicePoolsResponse(string projectName, bool getCustomPools)
    {
        string projectArn = (await GetProjectByName(projectName)).Arn;
        return await client.ListDevicePoolsAsync(new ListDevicePoolsRequest()
        {
            Arn = projectArn,
            Type = getCustomPools ? DevicePoolType.PRIVATE : DevicePoolType.CURATED
        });
    }

    private async Task<CreateUploadResponse> CreateUpload(string name, string projectName, UploadType type)
    {
        string projectArn = (await GetProjectByName(projectName)).Arn;
        return await client.CreateUploadAsync(new CreateUploadRequest()
        {
            Name = name, 
            ProjectArn = projectArn, 
            Type = type
        });
    }

    private async Task<Upload?> GetLastAndroidApplication(string projectName) => 
        await GetLastUpload(projectName, UploadType.ANDROID_APP);

    private async Task<Upload?> GetLastIosApplication(string projectName) => 
        await GetLastUpload(projectName, UploadType.IOS_APP);

    private async Task<Upload?> GetLastTestPackage(string projectName) => 
        await GetLastUpload(projectName, UploadType.APPIUM_PYTHON_TEST_PACKAGE);

    private async Task<Upload?> GetTestSpecByName(string projectName, string name)
    {
        List<Upload>? specs = await GetUploads(projectName, UploadType.APPIUM_PYTHON_TEST_SPEC);
        Upload targetSpec = specs.First(spec => spec.Name.Equals(name));
        return targetSpec;
    }

    private async Task<Upload?> GetLastUpload(string projectName, UploadType uploadType)
    {
        var uploads = await GetUploads(projectName, uploadType);
        if (uploads == null || !uploads.Any())
            return null;

        var sortedUploads = uploads.OrderByDescending(upload => upload.Created);
        return sortedUploads.First();
    }

    private async Task<List<Upload>?> GetUploads(string projectName, UploadType uploadType)
    {
        string projectArn = (await GetProjectByName(projectName)).Arn;
        return (await GetUploadsResponse(projectArn, uploadType)).Uploads;
    }

    private Task<ListUploadsResponse> GetUploadsResponse(string projectArn, UploadType uploadType) => 
        client.ListUploadsAsync(new ListUploadsRequest()
        {
            Arn = projectArn, 
            Type = uploadType
        });

    private async Task<Project> GetProjectByName(string projectName)
    {
        List<Project>? projects = (await client.ListProjectsAsync(new ListProjectsRequest())).Projects;
        Project targetProject = projects.First(project => project.Name.Equals(projectName));
        return targetProject;
    }

    private async Task<Project> GetProjectByArn(string projectArn) =>
        (await GetProjectResponse(projectArn))
        .Project;

    private Task<GetProjectResponse> GetProjectResponse(string projectArn) => 
        client.GetProjectAsync(new GetProjectRequest()
        {
            Arn = projectArn
        });

    private static async Task UploadFileToFarm(string filePath, string loadingUrl)
    {
        using (var httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(15) })
        {
            using (var request = new HttpRequestMessage(new HttpMethod("PUT"), loadingUrl))
            {
                request.Content = new ByteArrayContent(File.ReadAllBytes(filePath));
                var response = await httpClient.SendAsync(request);
            }
        }
    }
}