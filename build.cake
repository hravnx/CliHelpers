var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var buildVersion = EnvironmentVariable("APPVEYOR_BUILD_VERSION");

var outputDir = "./artifacts/";
var solutionPath = "./CliHelpers.sln";
var projectJson = "./src/CliHelpers/project.json";


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

var versionRx = new System.Text.RegularExpressions.Regex(@"version\""\:\s*(\""[0-9\.\-\*]*\"")");

Task("Package")
    .IsDependentOn("Test")
    .Does(() => {
        EnsureDirectoryExists(outputDir);

        var packSettings = new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = outputDir,
            NoBuild = true
        };
        DotNetCorePack("./src/CliHelpers", packSettings);
    });

Task("Test")
	.IsDependentOn("Build")
	.Does(() =>
    {
		DotNetCoreTest("./test/CliHelpers.Tests");
	});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        MSBuild(solutionPath, activeMsBuildConfig);
    });

Task("Version")
    .Does(() => {
        // if we're building locally do nothing
        if (string.IsNullOrEmpty(buildVersion))
        {
            return;
        }

        Information("patching to {0}", buildVersion);
        // Update project.json
        var updatedProjectJson = System.IO.File.ReadAllText(projectJson);
        updatedProjectJson = versionRx.Replace(updatedProjectJson, "buildVersion" + "\"");
        System.IO.File.WriteAllText(projectJson, updatedProjectJson);
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

