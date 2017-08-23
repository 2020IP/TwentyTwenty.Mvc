#tool "nuget:?package=GitVersion.CommandLine"

GitVersion versionInfo = null;
var target = Argument("target", "Default");
var outputDir = "./artifacts/";
var configuration   = Argument("configuration", "Release");

Task("Clean")
    .Does(() => {
        if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
        CreateDirectory(outputDir);
    });

Task("Version")
    .Does(() => {
        GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
        versionInfo = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });
    });

Task("Restore")
    .IsDependentOn("Version")
    .Does(() => {        
        // Workaround for bad tooling.  See https://github.com/NuGet/Home/issues/4337
        var props = "-p:VersionPrefix=" + versionInfo.MajorMinorPatch + " -p:VersionSuffix=" + versionInfo.PreReleaseLabel + versionInfo.PreReleaseNumber;
        DotNetCoreRestore(new DotNetCoreRestoreSettings
        {
            ArgumentCustomization = args => args.Append(props)
        });
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetCoreBuild(".", new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            VersionSuffix = versionInfo.PreReleaseLabel + versionInfo.PreReleaseNumber,
            ArgumentCustomization = args => args.Append("-p:VersionPrefix=" + versionInfo.MajorMinorPatch),
        });
    });

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = outputDir,
            NoBuild = true,
            Configuration = configuration,
            VersionSuffix = versionInfo.PreReleaseLabel + versionInfo.PreReleaseNumber,
            ArgumentCustomization = args => args.Append("-p:VersionPrefix=" + versionInfo.MajorMinorPatch),
        };

        DotNetCorePack("src/TwentyTwenty.Mvc/", settings);

        System.IO.File.WriteAllLines(outputDir + "artifacts", new[]{
            "nuget:TwentyTwenty.Mvc." + versionInfo.NuGetVersion + ".nupkg",
            "nugetSymbols:TwentyTwenty.Mvc." + versionInfo.NuGetVersion + ".symbols.nupkg",
        //    "releaseNotes:releasenotes.md"
        });

        if (AppVeyor.IsRunningOnAppVeyor)
        {
            foreach (var file in GetFiles(outputDir + "**/*"))
                AppVeyor.UploadArtifact(file.FullPath);
        }
    });

Task("Default")
    .IsDependentOn("Package");

private void GenerateReleaseNotes()
{
    var settings = new GitReleaseNotesSettings
    {
        WorkingDirectory = ".",        
    };

    GitReleaseNotes("./artifacts/releasenotes.md", settings);
}

RunTarget(target);