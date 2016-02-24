// Default settings
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var dbType = Argument("dbType", "localdb_v1").ToLower();
var imageUrl = Argument("imageUrl", "http://www.transformaniatime.com/Images/PvP.zip");

// Dictionary of DB instances and connection strings
var instances = new Dictionary<string,Tuple<string,string>>()
{
    { "localdb_v2", new Tuple<string,string>(@"(localdb)\MSSQLLocalDB", @"Data Source=(LocalDb)\MSSQLLocalDB; Initial Catalog=Stats; Integrated Security=SSPI") },
    { "localdb_v1", new Tuple<string,string>(@"(localdb)\v11.0", @"Data Source=(LocalDb)\v11.0; Initial Catalog=Stats; Integrated Security=SSPI;") },
    { "server", new Tuple<string, string>("localhost", "Data Source=localhost; Initial Catalog=Stats; Integrated Security=true;") } 
};

var dbServer = Argument("dbServer",instances[dbType].Item1);
var connectionString = Argument("connectionString",instances[dbType].Item2);
var reallyDropDb = Argument("reallyDropDb", "no");

Task("Clean")
    .Does(() => {
        CleanDirectories(new DirectoryPath[] { 
            Directory("./src/TT.Web/bin"), 
            Directory("./src/TT.Tests/bin/"), 
            Directory("./src/TT.Domain/bin/"),
            Directory("./src/TT.Migrations/bin/"),
        });   
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
    .IsDependentOn("Migrate-EF")
    .IsDependentOn("Seed-DB")
    .Does(() => {    
    
        Information("Running TT.Migrations using {0}", connectionString);
                  
        using(var process = StartAndReturnProcess("tools/FluentMigrator.Tools/tools/AnyCPU/40/Migrate.exe", new ProcessSettings 
        { 
            Arguments = "/db sqlserver /connection=\"" + connectionString + "\" /target=\"./src/TT.Migrations/bin/" + configuration + "/TT.Migrations.dll\"" 
        }))
        {
            process.WaitForExit();

            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception("Migration failed");
        }              
    }
);

Task("Migrate-EF")
    .Does(() => {
        CopyFile("src/packages/EntityFramework.6.1.3/tools/Migrate.exe","src/TT.Web/bin/Migrate.exe");
        
        Information("Running TT.Web migrations using {0}", connectionString);
        
        using(var process = StartAndReturnProcess("src/TT.Web/bin/Migrate.exe", new ProcessSettings 
        { 
            Arguments = "TT.Web.dll /connectionProviderName=\"System.Data.SqlClient\" /connectionString=\"" + connectionString + "\"" 
        }))
        {
            process.WaitForExit();

            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception("Migration failed");
        }
        
        Information("Applying stored procedures against {0}", dbServer);
                
        using(var process = StartAndReturnProcess("sqlcmd", new ProcessSettings { Arguments = @"-i src\TT.Web\Schema\GetPlayerBuffs.sql -S " + dbServer }))
        {
            process.WaitForExit();
            
            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                throw new Exception("Stored procedure scripts failed");
        } 
    }
);

Task("Migrate-FM")
    .Does(() => {
    
    }
);

Task("Drop-DB")
    .WithCriteria(() => reallyDropDb.ToLower() == "yes")
    .ContinueOnError()
    .Does(() => {
        Information("Dropping database using {0}", dbServer);
        
        if (FileExists("seeded.flg"))
            System.IO.File.Delete("seeded.flg");
                
        var sql = "ALTER DATABASE [Stats] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [Stats];";
        
        using(var process = StartAndReturnProcess("sqlcmd", new ProcessSettings { Arguments = "-b -Q \""+sql+"\" -S " + dbServer }))
        {
            process.WaitForExit();
            
            var exitCode = process.GetExitCode();
            if (exitCode > 0)
                Warning(string.Format("Faled to drop Stats database using {0}", dbServer));
        } 
    }
);

Task("Seed-DB")
    .WithCriteria(() => !FileExists("seeded.flg"))
    .Does(() => {
        var seedScripts = GetFiles("src/SeedData/*.sql");
        
        foreach(var script in seedScripts)
        {
            using(var process = StartAndReturnProcess("sqlcmd", new ProcessSettings { Arguments = "-i \""+script+"\" -S " + dbServer }))
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

Task("Seed-Images")
    .WithCriteria(() => !FileExists("images.flg"))
    .Does(() => {
        var seedImages = DownloadFile(imageUrl);
        Unzip(seedImages, "./src/TT.Web/Images/PvP");
        DeleteFile(seedImages);
        System.IO.File.Create("images.flg");
     }
);

Task("Drop-Images")
    .Does(() => {
        if (FileExists("images.flg"))
            System.IO.File.Delete("images.flg");
        CleanDirectory("./src/TT.Web/Images/PvP");
    }
);

// Default build, if required Migrates DB, Seeds DB and Seeds images. Skips steps if nothing to do
Task("Default")
    .IsDependentOn("Migrate")
    .IsDependentOn("Seed-DB")
    .IsDependentOn("Seed-Images")
    .IsDependentOn("Run-Unit-Tests");

// Resets to a blank DB, runs full DB migration and DB seed but doesn't seed images
Task("CI-Build")
    .IsDependentOn("Drop-DB")
    .IsDependentOn("Migrate")
    .IsDependentOn("Seed-DB")
    .IsDependentOn("Run-Unit-Tests");

// Drops, re-migrates and re-seeds the DB
Task("Recreate-DB")
    .IsDependentOn("Drop-DB")
    .IsDependentOn("Default")
    .Does(() => {
        if (reallyDropDb != "yes")
            Warning("Database was not dropped, 'reallyDropDb' was not set to 'yes'");  
    }
);

// Drops images and re-seeds them
Task("Recreate-Images")
    .IsDependentOn("Drop-Images")
    .IsDependentOn("Default");

Information("Build settings: Target={0}, Configuration={1}, dbType={2}, reallyDropDb={3}", target, configuration, dbType, reallyDropDb);
RunTarget(target);