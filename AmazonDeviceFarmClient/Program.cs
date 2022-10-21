using Amazon;
using Amazon.DeviceFarm;
using Amazon.DeviceFarm.Model;
using Amazon.Runtime;


namespace AmazonDeviceFarmClient;

public static class Program
{
    private const string ProjectArn = "arn:aws:devicefarm:us-west-2:457059129408:project:1d6dfb6b-77cc-4ce3-ad9d-76176241c118";
    
    private const string AccessKey = "AKIAWU2WY3BAEOIHKJFI";
    private const string SecretKey = "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";
    
    private const string AndroidSpecName = "android-custom-config.yml";
    private const string IosSpecName = "ios-custom-config.yml";

    private static async Task Main(string[] args)
    {
        AWSCredentials credentials = new BasicAWSCredentials(AccessKey, SecretKey);
        AmazonDeviceFarmConfig config = new AmazonDeviceFarmConfig() { RegionEndpoint = RegionEndpoint.USWest2 };
        Amazon.DeviceFarm.AmazonDeviceFarmClient client = new Amazon.DeviceFarm.AmazonDeviceFarmClient(credentials, config);

        Project project = await GetProject(client, ProjectArn);
        Console.WriteLine($"Found project: {project.Name}");

        List<Upload>? apps = await GetProjectUploads(client, project, UploadType.ANDROID_APP);

        Console.WriteLine("\nFound apps:");
        foreach (var upload in apps) 
            Console.WriteLine("\t - " + upload.Name);
        
        List<Upload>? testPackages = await GetProjectUploads(client, project, UploadType.APPIUM_PYTHON_TEST_PACKAGE);

        Console.WriteLine("\nFound packages:");
        foreach (var package in testPackages) 
            Console.WriteLine("\t - " + package.Name);
        
        List<Upload>? specs = await GetProjectUploads(client, project, UploadType.APPIUM_PYTHON_TEST_SPEC);

        Console.WriteLine("\nFound specs:");
        foreach (var spec in specs) 
            Console.WriteLine("\t - " + spec.Name);

        List<DevicePool>? devicePools = await GetProjectDevicePools(client, project, getCustomPools: true);
        
        Console.WriteLine("\nFound pools:");
        foreach (var pool in devicePools) 
            Console.WriteLine("\t - " + pool.Name);

        var runRequest = (await ScheduleRun(client, project, apps.First(), devicePools.First(), 
            testPackages.First(), specs.First()));
    }

    private static Task<ScheduleRunResponse> ScheduleRun(Amazon.DeviceFarm.AmazonDeviceFarmClient client,
        Project project, Upload application, DevicePool devicePool, Upload testPackage, Upload testSpec) =>
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

    private static async Task<Project> GetProject(IAmazonDeviceFarm client, string projectArn) =>
        (await GetProjectResponse(client, projectArn))
        .Project;
    
    private static Task<GetProjectResponse> GetProjectResponse(IAmazonDeviceFarm client, string projectArn) => 
        client.GetProjectAsync(new GetProjectRequest()
        {
            Arn = projectArn
        });
    
    private static async Task<List<Upload>?> GetProjectUploads(IAmazonDeviceFarm client, Project project, UploadType uploadType) => 
        (await GetProjectUploadsResponse(client, project, uploadType))
        .Uploads
        .OrderByDescending(upload => upload.Created)
        .ToList();
    
    private static Task<ListUploadsResponse> GetProjectUploadsResponse(IAmazonDeviceFarm client, Project project, UploadType uploadType) => 
        client.ListUploadsAsync(new ListUploadsRequest()
        {
            Arn = project.Arn, 
            Type = uploadType
        });
    
    private static async Task<List<DevicePool>?> GetProjectDevicePools(IAmazonDeviceFarm client, Project project, bool getCustomPools) =>
        (await GetProjectDevicePoolsResponse(client, project, getCustomPools))
        .DevicePools;
    
    private static Task<ListDevicePoolsResponse> GetProjectDevicePoolsResponse(IAmazonDeviceFarm client, Project project, bool getCustomPools) => 
        client.ListDevicePoolsAsync(new ListDevicePoolsRequest() 
            {
                Arn = project.Arn, 
                Type = getCustomPools ? DevicePoolType.PRIVATE : DevicePoolType.CURATED
                
            });
}