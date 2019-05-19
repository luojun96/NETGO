#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#tool "nuget:?package=GitReleaseNotes&version=0.7.1"

 // compile
var compileConfig = Argument("Configuration", "Release");
var slnFile = "./NETGO.sln";

var artifactsDir = Directory("artifacts");

// unit testing
var artifactsForUnitTestsDir = artifactsDir + Directory("UnitTests");

// acceptance testing
var artifactsForAcceptanceTestingDir = artifactsDir + Directory("AcceptanceTests");

// integration testing
var artifactsForIntegrationTestingDir = artifactsDir + Directory("IntegrationTests");

// benchmark testing
var artifactsForBenchmarkTestsDir = artifactsDir + Directory("BenchmarkTests");

// packaging
var packagesDir = artifactsDir + Directory("Packages");
var releaseNotesFile = packagesDir + File("releasenotes.md");
var artifactsFile = packagesDir + File("artifacts.txt");

/// unstable release/
var nugetFeedUnstableKey = EnvironmentVariable("nuget-apikey-unstable");
var nugetFeedUnstableUploadUrl = "https://www.nuget.org/api/v2/package";
var nugetFeedUnstableSymbolsUploadUrl = "https://www.nuget.org/api/v2/package";

// stable releases
var tagsUrl = "https://api.github.com/repos/luojun96/logon-app/releases/tags/";
var nugetFeedStableKey = EnvironmentVariable ("nuget-apikey-stable");
var nugetFeedStableUploadUrl = "https://www.nuget.org/api/v2/package";
var nugetFeedStableSymbolsUploadUrl = "https://www.nuget.org/api/v2/package";

// internal build variables - don't change these.
var releaseTag = "";
string committedVersion = "0.0.0-dev";
var buildVersion = committedVersion;
GitVersion versioning = null;
var nugetFeedUnstableBranchFilter = "^(develop)$|^(PullRequest/)";

var target = Argument("target", "Default");

Task ("Default")
    .IsDependentOn ("Build");

Task ("Build")
    .IsDependentOn ("RunTests")
    .IsDependentOn ("CreatePackages");

Task ("RunTests")
    .IsDependentOn ("RunUnitTests")
    .IsDependentOn ("RunAcceptanceTests")
    .IsDependentOn ("RunIntegrationTests");

Task("RunUnitTests")
    .Does(() =>
{
    
});

Task("RunAcceptanceTests")
    .Does(() =>
{
    
});

Task("RunIntegrationTests")
    .Does(() =>
{
    
});

Task("CreatePackages")
    .IsDependentOn ("Compile")
    .Does (() => {
        EnsureDirectoryExists(packagesDir);

        CopyFiles("./src/**/Release/NetGo.*.nupkg", packagesDir);

        var projectFiles = GetFiles("./src/**/Release/NetGo.*.nupkg");

		foreach(var projectFile in projectFiles)
		{
			System.IO.File.AppendAllLines(artifactsFile, new[]{
				projectFile.GetFilename().FullPath,
			});
		}

        if (!FileExists(artifactsFile))
        {   
            Information($"{artifactsFile} is not existed.");
            return;
        }

		var artifacts = System.IO.File
			.ReadAllLines(artifactsFile)
			.Distinct();
		
		foreach(var artifact in artifacts)
		{
			var codePackage = packagesDir + File(artifact);

			Information("Created package " + codePackage);
		}
    });

 Task ("Compile")
    .IsDependentOn ("Clean")
    .IsDependentOn ("Version")
    .Does (() => {
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = compileConfig,
        };

        DotNetCoreBuild(slnFile, settings);
    });

 Task ("Clean")
    .Does (() => {
        if (DirectoryExists (artifactsDir)) {
            DeleteDirectory (artifactsDir, new DeleteDirectorySettings {
                Recursive = true,
                Force = true
            });
        }
        CreateDirectory (artifactsDir);
    });

Task ("Version")
    .Does (() => {
        versioning = GetNuGetVersionForCommit();
        var ungetVersion = versioning.NuGetVersion;
        Information("SemVer Version number: " + ungetVersion);

    });
// Executes the task specified in the target argument.
RunTarget(target);

 // Gets nuique nuget version for this commit
private GitVersion GetNuGetVersionForCommit () {

    GitVersion (new GitVersionSettings {
        UpdateAssemblyInfo = false,
        OutputType = GitVersionOutput.BuildServer
    });

    return GitVersion (new GitVersionSettings { OutputType = GitVersionOutput.Json });
}