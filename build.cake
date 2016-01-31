// Default settings
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var dbType = Argument("dbType", "localdb_v1").ToLower();

// Dictionary of DB instances and connection strings
var instances = new Dictionary<string,Tuple<string,string>>()
{
    { "localdb_v2", new Tuple<string,string>(@"(localdb)\MSSQLLocalDB", @"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=Stats; Integrated Security=SSPI") },
    { "localdb_v1", new Tuple<string,string>(@"(localdb)\v11.0", @"Data Source=(LocalDb)\v11.0; Initial Catalog=Stats; Integrated Security=SSPI;") },
    { "server", new Tuple<string, string>("localhost", "Data Source=localhost; Initial Catalog=Stats; Integrated Security=true;") } 
};

Task("Clean")
    .Does(() => {
        CleanDirectories(new DirectoryPath[] { Directory("./src/TT.Web/bin"), Directory("./src/TT.Tests/bin/"), Directory("./src/TT.Domain/bin/") + Directory(configuration) });   
    }
);

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("./src/TT.sln", new NuGetRestoreSettings {
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
    MSBuild("./src/TT.sln", new MSBuildSettings()
        .SetConfiguration(configuration)
        .UseToolVersion(MSBuildToolVersion.NET45)
        .SetVerbosity(Verbosity.Minimal)
        .SetNodeReuse(false));
    }
);

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => {
        NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll");
    }
);

Task("Migrate")
    .IsDependentOn("Build")
    .Does(() => {
        CopyFile("src/packages/EntityFramework.6.1.0/tools/Migrate.exe","src/TT.Web/bin/Migrate.exe");
        
        Information("Running migrations using {0}", instances[dbType].Item2);
        
        using(var process = StartAndReturnProcess("src/TT.Web/bin/Migrate.exe", new ProcessSettings 
        { 
            Arguments = "TT.Web.dll /connectionProviderName=\"System.Data.SqlClient\" /connectionString=\"" + instances[dbType].Item2 + "\"" 
        }))
        {
            process.WaitForExit();

            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception("Migration failed");
        }
        
        Information("Applying stored procedures against {0}", instances[dbType].Item1);
                
        using(var process = StartAndReturnProcess("sqlcmd", new ProcessSettings { Arguments = @"-i src\TT.Web\Schema\GetPlayerBuffs.sql -S " + instances[dbType].Item1 }))
        {
            process.WaitForExit();
            
            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception("Stored procedure scripts failed");
        }
    }
);

Task("Drop-DB")
    .Does(() => {
        Information("Dropping database using {0}", instances[dbType].Item1);
        
        if (FileExists("seeded.flg"))
            System.IO.File.Delete("seeded.flg");
                
        var sql = "ALTER DATABASE [Stats] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [Stats];";
        
        using(var process = StartAndReturnProcess("sqlcmd", new ProcessSettings { Arguments = "-b -Q \""+sql+"\" -S " + instances[dbType].Item1 }))
        {
            process.WaitForExit();
            
            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception(string.Format("Faled to drop Stats database using {0}", instances[dbType].Item1));
        } 
    }
);

Task("Seed-DB")
    .WithCriteria(() => !FileExists("seeded.flg"))
    .Does(() => {
        var seedScripts = GetFiles("src/SeedData/*.sql");
        
        foreach(var script in seedScripts)
        {
            using(var process = StartAndReturnProcess("sqlcmd", new ProcessSettings { Arguments = "-i \""+script+"\" -S " + instances[dbType].Item1 }))
            {
                process.WaitForExit();
                
                var exitCode = process.GetExitCode();
                if (exitCode > 0)
                    throw new Exception(string.Format("Faled to run {0}", script));
            }
        }
        
        System.IO.File.Create("seeded.flg");
    }
);

Task("Default")
    .IsDependentOn("Migrate")
    .IsDependentOn("Seed-DB")
    .IsDependentOn("Run-Unit-Tests");
    
Task("Recreate-DB")
    .IsDependentOn("Drop-DB")
    .IsDependentOn("Default");

Information("Build settings: Target={0}, Configuration={1}, dbType={2}", target, configuration, dbType);
RunTarget(target);