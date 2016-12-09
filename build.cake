#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var nuget_push_key = EnvironmentVariable("NUGET_API_KEY");
var packageVersion = EnvironmentVariable("APPVEYOR_BUILD_VERSION");

var outputDir = "./artifacts/";
var solutionPath = "./CliHelpers.sln";

var releaseMsBuildSettings = new MSBuildSettings
		{
			Verbosity = Verbosity.Quiet,
			ToolVersion = MSBuildToolVersion.VS2015,
			Configuration = "Release"
		};

var debugMsBuildSettings = new MSBuildSettings
		{
			Verbosity = Verbosity.Normal,
			ToolVersion = MSBuildToolVersion.VS2015,
			Configuration = "Debug"
		};

var activeMsBuildConfig = configuration == "Debug" ? debugMsBuildSettings : releaseMsBuildSettings;


Task("Publish")
    .IsDependentOn("Package")
    .Does(() => {

        if (string.IsNullOrEmpty(nuget_push_key))
        {
            throw new InvalidOperationException("NUGET_API_KEY not found");
        }

        using(var process = StartAndReturnProcess("./tools/nuget.exe", new ProcessSettings{ Arguments = string.Format("./artifacts/CliHelpers.{0}.nupkg -ApiKey {1}", packageVersion, nuget_push_key) }))
        {
            process.WaitForExit();

            // This should output 0 as valid arguments supplied
            Information("Exit code: {0}", process.GetExitCode());
        }
    });

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        EnsureDirectoryExists(outputDir);
        var packSettings = new DotNetCorePackSettings {
            Configuration = configuration,
            OutputDirectory = outputDir,
            NoBuild = true
        };
        DotNetCorePack("./src/CliHelpers", packSettings);
    });

Task("Test")
	.IsDependentOn("Build")
	.Does(() => {
		DotNetCoreTest("./test/CliHelpers.Tests");
	});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        MSBuild(solutionPath, activeMsBuildConfig);
    });

Task("Restore")
	.Does(() => {
		DotNetCoreRestore(".");
	});

Task("Clean")
    .Does(() => {

        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
        var directories = GetDirectories("./**/bin").Concat(GetDirectories("./**/obj"));
        CleanDirectories(directories);
    });


Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);

