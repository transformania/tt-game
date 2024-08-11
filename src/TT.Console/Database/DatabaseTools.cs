using System.Text.RegularExpressions;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using TT.Migrations;

namespace TT.Console.Database;

public record DatabaseStatus(
    bool Success,
    bool IsConStrGood = false,
    bool Exists = false,
    bool IsPreSeeded = false,
    bool IsMigrated = false,
    bool IsSeeded = false
    );

public static class DatabaseTools
{
    public const string ConnectionStringName = "StatsWebConnection";

    public static IServiceProvider Configure(string configFile)
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration => configuration.AddJsonFile(configFile, false))
            .ConfigureServices((ctx, services) =>
            {
                services.AddFluentMigratorCore()
                    .ConfigureRunner(rb =>
                    {
                        rb.AddSqlServer2016()
                            .WithGlobalConnectionString(
                                ctx.Configuration.GetConnectionString(ConnectionStringName))
                            .ScanIn(typeof(AddChatRooms).Assembly).For.Migrations();
                    })
                    .AddLogging(lb => lb.Services
                        .RemoveAll(typeof(ILogger))
                        .RemoveAll(typeof(ILoggerProvider))
                        .AddSingleton<ILoggerProvider, FluentMigratorAnsiConsoleLoggerProvider>())
                    .BuildServiceProvider(false);
            });

        return builder.Build().Services;
    }

    public static string GetConnectionString(this IServiceProvider services) =>
        services.GetService<IConfiguration>().GetConnectionString(ConnectionStringName);

    public static SqlConnection CreateConnection(string connectionString)
    {
        var conStr = new SqlConnectionStringBuilder(connectionString) { InitialCatalog = "master" };
        var connection = new SqlConnection(conStr.ConnectionString);
        connection.Open();

        return connection;
    }

    public static async Task<bool> CreateDatabase(string connectionString, string database)
    {
        await using var connection = CreateConnection(connectionString);
        await using var command = new SqlCommand(
                $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = @DatabaseName) CREATE DATABASE {database};",
                connection);
        command.Parameters.Add(new SqlParameter("@DatabaseName", database));

        await command.ExecuteNonQueryAsync();

        return true;
    }
    
    public static async Task<bool> DropDatabaseAsync(string connectionString, string database)
    {
        await using var connection = CreateConnection(connectionString);

        var cmd = $@"IF (SELECT DB_ID(@DatabaseName)) IS NOT NULL
               BEGIN
                    ALTER DATABASE {database} SET OFFLINE WITH ROLLBACK IMMEDIATE;
                    ALTER DATABASE {database} SET ONLINE;
                    DROP DATABASE {database};
                END";

        await using var command = new SqlCommand(cmd, connection);
        command.Parameters.AddWithValue("@DatabaseName", database);

        await command.ExecuteScalarAsync();

        return true;
    }

    public static void SeedDatabaseWithFile(SqlConnection connection, string file)
    {
        try
        {
            var script = File.ReadAllText(file);
            var commandStrings = Regex.Split(
                script,
                @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase,
                TimeSpan.FromSeconds(5)
            );
            var chunks = commandStrings.Length;

            AnsiConsole.MarkupLine($"Seeding [b]{file}[/] ({chunks} chunk{(chunks > 1 ? "s" : "")})");

            foreach (var cmd in commandStrings.Where(s => !s.IsNullOrEmpty()))
            {
                var command = new SqlCommand(cmd, connection);
                command.ExecuteNonQuery();
            }

            AnsiConsole.MarkupLine($"Seeding [b]{file}[/] [green]completed[/]");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed seeding[/] [b]{file}[/]:");
            AnsiConsole.WriteException(ex);
        }
    }

    public static async Task<DatabaseStatus> CheckDatabaseStatus(IServiceProvider services, string database)
    {
        try
        {
            var connectionString = services.GetConnectionString();
            if (connectionString.IsNullOrEmpty())
                return new DatabaseStatus(true);

            await using var connection = CreateConnection(connectionString);
            await using var existsCmd = new SqlCommand($"SELECT database_id FROM sys.databases WHERE Name = '{database}';", connection);
            await using var isPreSeededCmd = new SqlCommand($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Players'", connection);
            await using var isSeededCmd = new SqlCommand(@"
            USE Stats;
            IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AspNetUserRoles')
                BEGIN
                    SELECT COUNT(*) FROM AspNetUserRoles
                END
            ELSE
                BEGIN
                    SELECT -1
                END;
            ", connection);

            var exists = await existsCmd.ExecuteScalarAsync() != null;
            if (!exists)
                return new DatabaseStatus(true, true);

            var preSeedResult = (int)await isPreSeededCmd.ExecuteScalarAsync();
            if (preSeedResult > 0)
                return new DatabaseStatus(true, true, true);

            var runner = services.GetService<IMigrationRunner>();
            var hasMigrationsPending = runner.HasMigrationsToApplyUp();
            var seededResult = await isSeededCmd.ExecuteScalarAsync();
            var isSeeded = seededResult != null && (int)seededResult > 0;

            return new DatabaseStatus(true, true, true, true, !hasMigrationsPending, isSeeded);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Failed to check DB status[/]");
            AnsiConsole.WriteException(ex);
            return new DatabaseStatus(false);
        }
    }
}