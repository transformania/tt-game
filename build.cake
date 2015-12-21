var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

Task("Default")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() => {}
);

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("./src/tfgame.sln", new NuGetRestoreSettings {
        Source = new List<string> {
            "https://www.nuget.org/api/v2/",
            "https://www.myget.org/F/roslyn-nightly/"
        }
    });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
    MSBuild("./src/tfgame.sln", new MSBuildSettings()
        .SetConfiguration(configuration)
        .WithProperty("TreatWarningsAsErrors", "False")
        .UseToolVersion(MSBuildToolVersion.NET45)
        .SetVerbosity(Verbosity.Minimal)
        .SetNodeReuse(false));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => {
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll");
});

RunTarget(target);