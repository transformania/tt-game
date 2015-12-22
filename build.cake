var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var dbType = Argument("dbType", "localdb_v1").ToLower();

Task("Default")
    .IsDependentOn("Migrate")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() => {}
);

Task("Clean")
    .Does(() => {
        CleanDirectories(new DirectoryPath[] { Directory("./src/tfgame/bin"), Directory("./src/tfgame.tests/bin/") + Directory(configuration) });   
});

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
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
    MSBuild("./src/tfgame.sln", new MSBuildSettings()
        .SetConfiguration(configuration)
        .UseToolVersion(MSBuildToolVersion.NET45)
        .SetVerbosity(Verbosity.Minimal)
        .SetNodeReuse(false));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => {
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll");
});

Task("Migrate")
    .IsDependentOn("Build")
    .Does(() => {
        var instances = new Dictionary<string,Tuple<string,string>>()
        {
            { "localdb_v2", new Tuple<string,string>(@"(localdb)\MSSQLLocalDB", @"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=Stats; Integrated Security=SSPI") },
            { "localdb_v1", new Tuple<string,string>(@"(localdb)\v11.0", @"Data Source=(LocalDb)\v11.0; Initial Catalog=Stats; Integrated Security=SSPI;") },
            { "server", new Tuple<string, string>("localhost", "Data Source=localhost; Initial Catalog=Stats; Integrated Security=true;") } 
        };
    
        CopyFile("src/packages/EntityFramework.6.1.0/tools/Migrate.exe","src/tfgame/bin/Migrate.exe");
        
        Information("Running migrations using {0}", instances[dbType].Item2);
        
        using(var process = StartAndReturnProcess("src/tfgame/bin/Migrate.exe", new ProcessSettings 
        { 
            Arguments = "tfgame.dll /connectionProviderName=\"System.Data.SqlClient\" /connectionString=\"" + instances[dbType].Item2 + "\"" 
        }))
        {
            process.WaitForExit();

            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception("Migration failed");
        }
        
        Information("Applying stored procedures against {0}", instances[dbType].Item1);
              
        using(var process = StartAndReturnProcess("sqlcmd", new ProcessSettings { Arguments = @"-i src\tfgame\Schema\GetPlayerBuffs.sql -S " + instances[dbType].Item1 }))
        {
            process.WaitForExit();
            
            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception("Stored procedure scripts failed");
        }
    });

Information("dbType: {0}", dbType);
RunTarget(target);