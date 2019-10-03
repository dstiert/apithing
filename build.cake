#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=OpenCover&version=4.7.922"
#tool "nuget:?package=ReportGenerator&version=4.1.4"
#tool "nuget:?package=coveralls.io&version=1.4.2"
#addin "nuget:?package=Cake.Coveralls&version=0.10.0"
#addin "nuget:?package=SharpZipLib&Version=1.1.0"
#addin "nuget:?package=Cake.Compression&version=0.2.3"
#addin "Cake.FileHelpers&version=3.2.0"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var solution = GetFiles("*.sln").First();

//////////////////////////////////////////////////////////////////////
// APPVEYOR SETUP
//////////////////////////////////////////////////////////////////////

if(AppVeyor.IsRunningOnAppVeyor)
{
    Setup(context =>
    {       
        // Pass version info to AppVeyor
        GitVersion(new GitVersionSettings{
            ArgumentCustomization = args => args.Append("-verbosity Warn"),
            OutputType = GitVersionOutput.BuildServer,
            NoFetch = true
        });
    });
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories("**/bin");
    CleanDirectories("**/obj");
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(solution);
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    MSBuild(solution, new MSBuildSettings {
        Configuration = configuration,
        EnvironmentVariables = new Dictionary<string, string> 
        {  
            { "GitVersion_NoFetchEnabled", "true" }
        },
        ArgumentCustomization = arg => arg.AppendSwitch("/p:DebugType","=","Full")
    });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    OpenCover(
        tool => tool.DotNetCoreVSTest("**/bin/**/*unittests.dll"),
        new FilePath("./coverage.xml"),
        new OpenCoverSettings { ReturnTargetCodeOffset = 0, OldStyle = true }.WithFilter("+[*apithing*]apithing*").WithFilter("-[*Tests*]*")
    );

    if(AppVeyor.IsRunningOnAppVeyor)
    {
        CoverallsIo("./coverage.xml", new CoverallsIoSettings {
            RepoToken = EnvironmentVariable("COVERALLS_TOKEN")
        });
    }
    else
    {
        ReportGenerator("./coverage.xml", "./coverage", new ReportGeneratorSettings {
            ReportTypes = new[] { ReportGeneratorReportType.HtmlSummary }
        });
    }
});

Task("Publish")
    .IsDependentOn("Restore")
    .Does(() =>{
        var settings = new DotNetCorePublishSettings
        {
            Configuration = configuration,
            OutputDirectory = "./apithing/bin/publish",
            NoRestore = true,
            EnvironmentVariables = new Dictionary<string, string> 
            {  
                { "GitVersion_NoFetchEnabled", "true" }
            },
        };

        DotNetCorePublish("apithing/apithing.csproj", settings);
        CreateDirectory("./artifacts/");
        ZipCompress($"./apithing/bin/publish", "./artifacts/publish.zip");
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Publish");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
