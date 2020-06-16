#addin "nuget:?package=Cake.SqlServer&version=2.2.0"
#addin Cake.FluentMigrator&version=0.4.0
#addin Cake.FileHelpers&version=3.2.1
#addin nuget:?package=SharpZipLib&version=1.2.0
#addin nuget:?package=Cake.Compression&version=0.2.4
#addin nuget:?package=System.ValueTuple&version=4.5.0
#tool "nuget:?package=FluentMigrator.Tools&version=1.6.2"
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.11.1"
#tool "nuget:?package=OpenCover&version=4.7.922"
#tool "nuget:?package=ReportGenerator&version=4.6.1"

using static Cake.Common.Tools.ReportGenerator.ReportGeneratorReportType;
using static System.Globalization.CultureInfo;

// Default settings
var target = Argument("target", EnvironmentVariable("TT_TARGET") ?? "Default");
var configuration = Argument("configuration", EnvironmentVariable("TT_CONFIGURATION") ?? "Release");
var dbType = Argument("dbType", EnvironmentVariable("TT_DBTYPE") ?? "localdb_v2").ToLower();
var dbName = Argument("dbName", EnvironmentVariable("TT_DBNAME") ?? "Stats");
var updateUrl = Argument("updateUrl", "http://localhost:52223/API/WorldUpdate");
var imageUrl = Argument("imageUrl", "https://www.transformaniatime.com/cake/images.zip");

var isInCI = Convert<bool>(EnvironmentVariable("CI") ?? "false");
Uri unitHistoryUri = null;
Uri integrationHistoryUri = null;
if (isInCI)
{
    unitHistoryUri = Convert<Uri>(EnvironmentVariable("TT_UNIT_HISTORY_URI"));
    integrationHistoryUri = Convert<Uri>(EnvironmentVariable("TT_INTEGRATION_HISTORY_URI"));
}

// Dictionary of DB instances and connection strings
var instances = new Dictionary<string,Tuple<string,bool>>()
{
    { "localdb_v2", new Tuple<string,bool>(@"(localdb)\MSSQLLocalDB", true) },
    { "localdb_v1", new Tuple<string,bool>(@"(localdb)\v11.0", true) },
    { "server", new Tuple<string, bool>("localhost", true) },
    { "remoteserver", new Tuple<string, bool>("localhost", false) }
};

var dbServer = Argument("dbServer", EnvironmentVariable("TT_DBSERVER") ?? instances[dbType].Item1);
var dbSecurity = Boolean.Parse(Argument("dbSecurity", EnvironmentVariable("TT_DBSECURITY") ?? instances[dbType].Item2.ToString()));
var dbPassword = new System.Text.StringBuilder(Argument("dbPassword", EnvironmentVariable("TT_DBPASSWORD") ?? ""));
var dbUserId = Argument("dbUserId", EnvironmentVariable("TT_DBUSERID") ?? "newman");

if(!dbSecurity && dbPassword.Length == 0)
{
    ConsoleKeyInfo key;
    Console.Write(string.Format("Enter password for database on \"{0}\": ", dbServer));
    do {
       key = Console.ReadKey(true);

       // Ignore any key out of range.
       if (key.Key == ConsoleKey.Backspace) {
           dbPassword.Length--;
       } else if (key.Key != ConsoleKey.Enter) {
          // Append the character to the password.
          dbPassword.Append(key.KeyChar);
       }
    // Exit if Enter key is pressed.
    } while (key.Key != ConsoleKey.Enter);
    Console.WriteLine();
}
var connectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder
{
    DataSource = dbServer,
    IntegratedSecurity=dbSecurity,
    Password = dbPassword.ToString(),
    UserID = dbUserId
};

var connectionStringNoDb = connectionStringBuilder.ToString();
connectionStringBuilder.InitialCatalog = dbName;
var connectionString = connectionStringBuilder.ToString();

Task("Clean")
    .Does(() => {
        MSBuild("./src/TT.sln", settings =>
            settings.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithTarget("Clean"));
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
    MSBuild("./src/TT.sln", settings =>
        settings.SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal));
    }
);

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() => {
        var platform = new CakePlatform();
        if (platform.Family == PlatformFamily.Windows)
        {
            var unitCoverage = new FilePath("unitCoverage.xml");
            OpenCover(tool => {
               tool.NUnit3("./src/**/bin/" + configuration + "/net472/*.Tests.dll");
            },
            unitCoverage,
            new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
                .WithFilter("+[TT.Domain]*")
                .WithFilter("-[TT.Web]*")
                .WithFilter("-[TT.Migrations]*")
                .WithFilter("-[TT.Tests]*")
                .WithFilter("-[TT.IntegrationTests]*")
            );
        }
        else
        {
            NUnit3("./src/**/bin/" + configuration + "/net472/*.Tests.dll");
        }
    }
);

Task("Run-Integration-Tests")
    .IsDependentOn("Build")
    .Does(() => {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"./src/TT.IntegrationTests/bin/{configuration}/net472/ConnectionStrings.config"))
        {
            file.WriteLine($"<connectionStrings><add name=\"StatsWebConnection\" providerName=\"System.Data.SqlClient\" connectionString=\"{connectionString}\"/></connectionStrings>");
        }
    
        var platform = new CakePlatform();
        if (platform.Family == PlatformFamily.Windows)
        {
            var integrationCoverage = new FilePath("integrationCoverage.xml");
            OpenCover(tool => {
               tool.NUnit3("./src/**/bin/" + configuration + "/net472/*.IntegrationTests.dll");
            },
            integrationCoverage,
            new OpenCoverSettings { ReturnTargetCodeOffset = 0 }
                .WithFilter("+[TT.Domain]*")
                .WithFilter("+[TT.Web]*")
                .WithFilter("-[TT.Migrations]*")
                .WithFilter("-[TT.Tests]*")
                .WithFilter("-[TT.IntegrationTests]*")
            );
        }
        else
        {
            NUnit3("./src/**/bin/" + configuration + "/net472/*.IntegrationTests.dll");
        }
        
        System.IO.File.Delete($@"./src/TT.IntegrationTests/bin/{configuration}/net472/ConnectionStrings.config");
    }
);

Task("Generate-Report")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-Integration-Tests")
    .DoesForEach(new [] { (File("unitCoverage.xml"), "unit", unitHistoryUri), (File("integrationCoverage.xml"), "integration", integrationHistoryUri) }, (tuple) =>
    {
        var (path, testName, historyUri) = tuple;

        if (FileExists(path))
        {
            bool TryDownload(out FilePath historyPath)
            {
                try 
                {
                    historyPath = DownloadFile(historyUri);
                }
                catch (AggregateException ex)
                when(ex.GetBaseException() is System.Net.Http.HttpRequestException)
                {
                    Warning($"{testName} coverage history not found.");
                    historyPath = null;
                    return false;
                }

                return true;
            }

            if (isInCI && TryDownload(out FilePath result))
            {
                try 
                {
                    GZipUncompress(result, new DirectoryPath($"coverage/{testName}/history"));
                }
                catch (Exception ex)
                {
                    Error(ex);
                    throw;
                }
            }

            ReportGenerator(path, $"coverage/{testName}", new ReportGeneratorSettings(){
                ReportTypes = new List<ReportGeneratorReportType>() { Html, Badges, TextSummary },
                HistoryDirectory = new DirectoryPath($"coverage/{testName}/history")
            });

            foreach(var line in FileReadLines(new FilePath($"coverage/{testName}/Summary.txt")))
            {
                if (string.IsNullOrEmpty(line))
                    break;

                if (line == "Summary")
                    Information($"{CurrentCulture.TextInfo.ToTitleCase(testName)} Test Summary");
                else
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("Line coverage:"))
                        Information($"  {CurrentCulture.TextInfo.ToTitleCase(testName)} " + trimmed.ToLower());
                    else
                        Information(line);
                }
            }
            
            if (isInCI)
            {
                try 
                {
                    GZipCompress(new DirectoryPath($"coverage/{testName}/history"), new FilePath($"coverage/{testName}/history.tar.gz"), 9);
                }
                catch (Exception ex)
                {
                    Error(ex);
                    throw;
                }

                if (FileExists($"coverage/{testName}/history.tar.gz"))
                {
                    DeleteDirectory(new DirectoryPath($"coverage/{testName}/history"), new DeleteDirectorySettings
                    {
                        Recursive = true
                    });
                }
            }
        }
    });

Task("Migrate")
    .IsDependentOn("Build")
    .IsDependentOn("PreSeed-DB")
    .Does(() => {    
    
        Information("Running TT.Migrations using {0}", dbType);

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"./tools/ConnectionStrings.config"))
        {
            file.WriteLine(string.Format("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<configuration>\n<connectionStrings>\n<add name=\"StatsWebConnection\" providerName=\"System.Data.SqlClient\" connectionString=\"{0}\"/>\n</connectionStrings>\n</configuration>", connectionString));
        }
        FluentMigrator(new FluentMigratorSettings
        {
            Connection = "StatsWebConnection",
            ConnectionStringConfigPath ="./tools/ConnectionStrings.config",
            Provider = "sqlserver",
            Assembly = "./src/TT.Migrations/bin/" + configuration + "/net472/TT.Migrations.dll"
        });
        System.IO.File.Delete(@"./tools/ConnectionStrings.config");
        Information("Applying stored procedures against {0}", dbServer);
        using(var connection = OpenSqlConnection(connectionString))
        {
            ExecuteSqlFile(connection, "./src/TT.Web/Schema/GetPlayerBuffs.sql");
        }
    }
);

Task("Drop-DB")
    .WithCriteria(() => !(dbType == "remoteserver"))
    .ContinueOnError()
    .Does(() => {
        Information("Dropping database using {0}", dbServer);
        
        if (FileExists("seeded.flg"))
            System.IO.File.Delete("seeded.flg");
        DropDatabase(connectionString, dbName);
    }
);

Task("PreSeed-DB")
    .WithCriteria(() => !FileExists("seeded.flg") && !(dbType == "remoteserver"))
    .Does(() => {
        CreateDatabase(connectionStringNoDb, dbName);
        var seedScripts = GetFiles("src/SeedData/PreSeed/*.sql");
        
        using(var connection = OpenSqlConnection(connectionString))
        {
            foreach(var script in seedScripts)
            {
                ExecuteSqlFile(connection, script);
            }
        }
    }
);

Task("Seed-DB")
    .WithCriteria(() => !FileExists("seeded.flg") && !(dbType == "remoteserver"))
    .Does(() => {
        var seedScripts = GetFiles("src/SeedData/*.sql");
        
        using(var connection = OpenSqlConnection(connectionString))
        {
            foreach(var script in seedScripts)
            {
                ExecuteSqlFile(connection, script);
            }
        }
        System.IO.File.Create("seeded.flg");
    }
);

Task("Turn-Update")
    .Does(() => {
        using (var wc = new System.Net.WebClient())
        {
            Information("POSTing to " + updateUrl);
            wc.UploadString(updateUrl, "");
        }
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

private static T Convert<T>(string value)
{
    var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
    return (T)converter.ConvertFromInvariantString(value);
}

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
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-Integration-Tests")
    .IsDependentOn("Generate-Report");

// Drops, re-migrates and re-seeds the DB
Task("Recreate-DB")
    .IsDependentOn("Drop-DB")
    .IsDependentOn("Default");

// Drops images and re-seeds them
Task("Recreate-Images")
    .IsDependentOn("Drop-Images")
    .IsDependentOn("Default");

Information("Build settings: Target={0}, Configuration={1}, dbType={2}", target, configuration, dbType);
RunTarget(target);