#addin "Cake.SqlServer&version=3.0.0&loaddependencies=true"
#addin nuget:?package=Cake.FileHelpers&version=5.0.0
#addin nuget:?package=System.ValueTuple&version=4.5.0
#addin nuget:?package=Microsoft.Data.SqlClient&version=5.0.1

#tool nuget:?package=NuGet.CommandLine&version=6.5.0
#tool nuget:?package=FluentMigrator.Console&version=3.3.2
#tool nuget:?package=NUnit.ConsoleRunner&version=3.16.3
#tool nuget:?package=OpenCover&version=4.7.1221
#tool nuget:?package=ReportGenerator&version=5.1.20

using static Cake.Common.Tools.ReportGenerator.ReportGeneratorReportType;
using static System.Globalization.CultureInfo;

// Default settings
var target = Argument("target", EnvironmentVariable("TT_TARGET") ?? "Default");
var configuration = Argument("configuration", EnvironmentVariable("TT_CONFIGURATION") ?? "Release");
var dbType = Argument("dbType", EnvironmentVariable("TT_DBTYPE") ?? "localdb_v2").ToLower();
var dbName = Argument("dbName", EnvironmentVariable("TT_DBNAME") ?? "Stats");
var updateUrl = Argument("updateUrl", "http://localhost:52223/API/WorldUpdate");

var isInCI = Convert<bool>(EnvironmentVariable("CI") ?? "false");

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
var connectionStringBuilder = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder()
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
    .Does(() => {
        NuGetRestore("./src/TT.sln");
    }
);

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
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
            var unitCoverage = new FilePath("unit-test-coverage.xml");
            OpenCover(tool => {
               tool.NUnit3("./src/**/bin/" + configuration + "/net48/*.Tests.dll", new NUnit3Settings {
                   Results = new[]  {new NUnit3Result { FileName = "unit-test-result.xml", Transform = "nunit3-junit.xslt" } }
                   });
            },
            unitCoverage,
            new OpenCoverSettings() { ReturnTargetCodeOffset = 0 }
                .WithRegisterDll("Path64")
                .WithFilter("+[TT.Domain]*")
                .WithFilter("-[TT.Web]*")
                .WithFilter("-[TT.Migrations]*")
                .WithFilter("-[TT.Tests]*")
                .WithFilter("-[TT.IntegrationTests]*")
            );
        }
        else
        {
            NUnit3("./src/**/bin/" + configuration + "/net48/*.Tests.dll");
        }
    }
);

Task("Run-Integration-Tests")
    .IsDependentOn("Build")
    .Does(() => {
        using (System.IO.StreamWriter file = new System.IO.StreamWriter($@"./src/TT.IntegrationTests/bin/{configuration}/net48/ConnectionStrings.config"))
        {
            file.WriteLine($"<connectionStrings><add name=\"StatsWebConnection\" providerName=\"System.Data.SqlClient\" connectionString=\"{connectionString}\"/></connectionStrings>");
        }
    
        var platform = new CakePlatform();
        if (platform.Family == PlatformFamily.Windows)
        {
            var integrationCoverage = new FilePath("integration-test-coverage.xml");
            OpenCover(tool => {
               tool.NUnit3("./src/**/bin/" + configuration + "/net48/*.IntegrationTests.dll", new NUnit3Settings {
                   Results = new[]  {new NUnit3Result { FileName = "integration-test-result.xml", Transform = "nunit3-junit.xslt" } }
                   });
            },
            integrationCoverage,
            new OpenCoverSettings() { ReturnTargetCodeOffset = 0 }
                .WithRegisterDll("Path64")
                .WithFilter("+[TT.Domain]*")
                .WithFilter("+[TT.Web]*")
                .WithFilter("-[TT.Migrations]*")
                .WithFilter("-[TT.Tests]*")
                .WithFilter("-[TT.IntegrationTests]*")
            );
        }
        else
        {
            NUnit3("./src/**/bin/" + configuration + "/net48/*.IntegrationTests.dll");
        }
        
        System.IO.File.Delete($@"./src/TT.IntegrationTests/bin/{configuration}/net48/ConnectionStrings.config");
    }
);

Task("Generate-Report")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-Integration-Tests")
    .DoesForEach(new [] { (File("unit-test-coverage.xml"), "unit"), (File("integration-test-coverage.xml"), "integration") }, (tuple) =>
    {
        var (path, testName) = tuple;

        if (FileExists(path))
        {
            ReportGenerator(path, $"coverage/{testName}", new ReportGeneratorSettings(){
                ReportTypes = new List<ReportGeneratorReportType>() { Html, Badges, TextSummary },
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
        var migrationStatus = StartProcess("./tools/FluentMigrator.Console.3.3.2/net461/any/Migrate.exe", new ProcessSettings {
            Arguments = $"--assembly=./src/TT.Migrations/bin/{configuration}/net48/TT.Migrations.dll --dbType=SqlServer2016 --connection=StatsWebConnection --connectionStringConfigPath=./tools/ConnectionStrings.config"
        });
        if (migrationStatus != 0)
        {
            throw new Exception($"FluentMigrator error code {migrationStatus}");
        }

        System.IO.File.Delete(@"./tools/ConnectionStrings.config");
        Information("Applying stored procedures against {0}", dbServer);
        using(var connection = OpenSqlConnection(connectionString))
        {
            ExecuteSqlFile(connection, "./src/TT.Web/Schema/GetPlayerBuffs.sql");
        }
    }
);

Task("Rollback")
    .IsDependentOn("Build")
    .Does(() => {    
    
        Information("Running TT.Migrations rollback using {0}", dbType);

        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"./tools/ConnectionStrings.config"))
        {
            file.WriteLine(string.Format("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<configuration>\n<connectionStrings>\n<add name=\"StatsWebConnection\" providerName=\"System.Data.SqlClient\" connectionString=\"{0}\"/>\n</connectionStrings>\n</configuration>", connectionString));
        }
        var migrationStatus = StartProcess("./tools/FluentMigrator.Console.3.3.2/net461/any/Migrate.exe", new ProcessSettings {
            Arguments = $"--assembly=./src/TT.Migrations/bin/{configuration}/net48/TT.Migrations.dll --task rollback --dbType=SqlServer2016 --connection=StatsWebConnection --connectionStringConfigPath=./tools/ConnectionStrings.config"
        });
        if (migrationStatus != 0)
        {
            throw new Exception($"FluentMigrator error code {migrationStatus}");
        }

        System.IO.File.Delete(@"./tools/ConnectionStrings.config");
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


private static T Convert<T>(string value)
{
    var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
    return (T)converter.ConvertFromInvariantString(value);
}

// Default build, if required Migrates DB and Seeds DB. Skips steps if nothing to do
Task("Default")
    .IsDependentOn("Migrate")
    .IsDependentOn("Seed-DB")
    .IsDependentOn("Run-Unit-Tests");

// Resets to a blank DB, runs full DB migration and DB seed
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


Information("Build settings: Target={0}, Configuration={1}, dbType={2}", target, configuration, dbType);
RunTarget(target);