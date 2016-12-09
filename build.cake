#tool "nuget:?package=GitVersion.CommandLine"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");


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
    .IsDependentOn("Test");

RunTarget(target);

