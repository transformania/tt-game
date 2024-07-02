using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using TT.Migrations;

namespace TT.Console.Database;

public static class DatabaseTools
{
    private const string ConnectionStringName = "StatsWebConnection";

    public static bool CreateDatabase(DatabaseInput input)
    {
        using var host = DatabaseUtilityHost.BuildDatabaseUtilityHost(input.ConfigFile);
        var connectionString = host.Services.GetService<IConfiguration>().GetConnectionString(ConnectionStringName);

        var conStr = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        var connection = new SqlConnection(conStr.ConnectionString);
        connection.Open();
        
        using var command = new SqlCommand(
                $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName) CREATE DATABASE {input.Database};",
                connection);
        command.Parameters.Add(new SqlParameter("@DatabaseName", input.Database));

        command.ExecuteNonQuery();

        return true;
    }
    
    public static bool DropDatabase(DatabaseInput input)
    {
        using var host = DatabaseUtilityHost.BuildDatabaseUtilityHost(input.ConfigFile);
        var connectionString = host.Services.GetService<IConfiguration>().GetConnectionString(ConnectionStringName);

        var conStr = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        var connection = new SqlConnection(conStr.ConnectionString);
        connection.Open();
        
        using var command =
            new SqlCommand(
                $"DROP DATABASE IF EXISTS {input.Database};",
                connection);

        command.ExecuteScalar();

        return true;
    }
    
    public static bool SeedDatabase(SeedDatabaseInput input)
    {
        using var host = DatabaseUtilityHost.BuildDatabaseUtilityHost(input.ConfigFile);

        var connectionString = host.Services.GetService<IConfiguration>().GetConnectionString(ConnectionStringName);
        var preSeedFiles = Directory.EnumerateFiles(Path.Combine(input.SeedDataPath, "PreSeed"));
        var seedFiles = Directory.EnumerateFiles(input.SeedDataPath);

        var conStr = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        using var connection = new SqlConnection(conStr.ConnectionString);
        connection.Open();

        AnsiConsole.MarkupLine($"Beginning pre-seeding...");
        // foreach (var seedFile in preSeedFiles)
        // {
        //     AnsiConsole.MarkupLine($"Pre-seeding [white]{seedFile}[/]");
        //     Seed(seedFile, connection);
        // }
        AnsiConsole.MarkupLine($":check_mark: Pre-seeding complete");
        AnsiConsole.MarkupLine($"Beginning seeding...");
        
        foreach (var seedFile in seedFiles)
        {
            AnsiConsole.MarkupLine($"Seeding [white]{seedFile}[/]");
            Seed(seedFile, connection);
        }
        
        return true;

        void Seed(string file, SqlConnection con)
        {
            var script = File.ReadAllText(file);
            foreach (var cmd in script.Split(["GO", "go"], StringSplitOptions.RemoveEmptyEntries))
            {
                using var command = new SqlCommand(cmd, con);
                command.ExecuteNonQuery();
            }
        }
    }
    
    public static bool MigrateUp(DatabaseInput input)
    {
        using var host = DatabaseUtilityHost.BuildDatabaseUtilityHost(input.ConfigFile, (ctx, services) =>
        {
            services.AddFluentMigratorCore()
                .ConfigureRunner(rb =>
                {
                    rb.AddSqlServer2016()
                        .WithGlobalConnectionString(
                            ctx.Configuration.GetConnectionString(ConnectionStringName))
                        .ScanIn(typeof(AddChatRooms).Assembly).For.Migrations();
                })
                .AddLogging(lb => lb.Services.AddSingleton<ILoggerProvider, FluentMigratorAnsiConsoleLoggerProvider>())
                .BuildServiceProvider(false);
        });

        var runner = host.Services.GetService<IMigrationRunner>();
        if (runner == null)
            return false;

        runner.MigrateUp();
        return true;
    }
}