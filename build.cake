#addin "Cake.Json"

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var buildVersion = EnvironmentVariable("APPVEYOR_BUILD_VERSION");

var outputDir = "./artifacts/";
var projectJson = "./src/CliHelpers/project.json";

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
    .IsDependentOn("Version")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration
        };
        DotNetCoreBuild(projectJson, settings);
    });

Task("Version")
    .Does(() => {
        // if we're building locally do nothing
        if (string.IsNullOrEmpty(buildVersion))
        {
            Warning("No version to patch to, skipping...");
            return;
        }

        var jObject = ParseJsonFromFile(projectJson);
        var oldVersion = jObject["version"];
        Information("Patching {0} -> {1}", oldVersion, buildVersion);
        jObject["version"] = buildVersion;

        System.IO.File.WriteAllText(projectJson,jObject.ToString());
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

