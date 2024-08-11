#tool nuget:?package=NuGet.CommandLine&version=6.5.0

// Default settings
var target = Argument("target", EnvironmentVariable("TT_TARGET") ?? "Build");
var configuration = Argument("configuration", EnvironmentVariable("TT_CONFIGURATION") ?? "Release");

Task("Clean")
    .Does(() => {
        MSBuild("./TT.sln", settings =>
            settings.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithTarget("Clean"));
    }
);

Task("Restore-NuGet-Packages")
    .Does(() => {
        NuGetRestore("./TT.sln");
    }
);

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() => {
    MSBuild("./TT.sln", settings =>
        settings.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal));
    }
);

RunTarget(target);