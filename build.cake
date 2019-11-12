#tool "nuget:?package=GitVersion.CommandLine&version=5.0.1"

//////////////////////////////////////////////////////////////////////
// CONFIGURATIONS
//////////////////////////////////////////////////////////////////////

var versionPropsTemplate = "./Version.props.template";
var versionProps = "./Version.props";
var sln = "SAHB.GraphQL.Client.sln";

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories("./src/**/bin");
	CleanDirectories("./src/**/obj");
	CleanDirectories("./tests/**/bin");
	CleanDirectories("./tests/**/obj");
});

GitVersion versionInfo = null;
Task("Version")
    .Does(() => 
{
	GitVersion(new GitVersionSettings{
		UpdateAssemblyInfo = false,
		OutputType = GitVersionOutput.BuildServer,
		WorkingDirectory = "."
	});
	versionInfo = GitVersion(new GitVersionSettings{
		UpdateAssemblyInfo = false,
		OutputType = GitVersionOutput.Json,
		WorkingDirectory = "."
	});
		
	// Update version
	var updatedVersionProps = System.IO.File.ReadAllText(versionPropsTemplate)
		.Replace("1.0.0", versionInfo.NuGetVersion);

	System.IO.File.WriteAllText(versionProps, updatedVersionProps);
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    DotNetCoreRestore(sln);
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Version")
	.IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
	var settings = new DotNetCoreBuildSettings
    {
		Configuration = configuration
    };

	DotNetCoreBuild(sln, settings);
});

Task("Publish")
	.IsDependentOn("Build")
	.Does(() =>
{
	var settings = new DotNetCorePublishSettings
    {
		Configuration = configuration
    };

	DotNetCorePublish(sln, settings);
});

Task("Test-CI")
    .Does(() =>
{
	foreach (var test in System.IO.Directory.GetFiles("./tests/", "*.Tests.csproj", SearchOption.AllDirectories))
	{
		var settings = new DotNetCoreTestSettings
		{
			Configuration = configuration,
			NoBuild = true,
			ArgumentCustomization = args=>args.Append("--logger \"trx;LogFileName=TestResults.trx\""),
		};
	
		DotNetCoreTest(test, settings);
	}
});

Task("Test-Format")
	.Does(() =>
{
	DotNetCoreTool(sln, "dotnet-format", "--check --dry-run");
});

Task("Test")
	.IsDependentOn("Build")
	.IsDependentOn("Test-Format")
    .IsDependentOn("Test-CI");

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
