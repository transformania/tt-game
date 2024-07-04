using System.Diagnostics;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Oakton;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace TT.Console.Database;

public class DatabaseCommand : OaktonAsyncCommand<DatabaseInput>
{
    public override async Task<bool> Execute(DatabaseInput input)
    {
        var stopwatch = Stopwatch.StartNew();

        if (!CheckConfig(input)) return false;

        var services = DatabaseTools.Configure(input.ConfigFile);
        var database = input.DatabaseFlag;
        var status = await CheckStatus(services, database);

        if (!status.IsConStrGood)
        {
            AnsiConsole.MarkupLine("[red b]Connection string is not valid[/]");
            AnsiConsole.MarkupLine("Database modifications cannot proceed. " +
                                   "Check the provided configuration file and ensure [cyan]StatsWebConnection[/] is specified");
            return false;
        }

        switch (input.SubCommand)
        {
            case DatabaseInput.SubCommands.status:
                return true;
            case DatabaseInput.SubCommands.up:
                await Up(services, status, input);
                break;
            case DatabaseInput.SubCommands.migrate:
                Migrate(database, services);
                break;
            case DatabaseInput.SubCommands.recreate:
                await Recreate(services, new DatabaseStatus(true), input);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        stopwatch.Stop();

        AnsiConsole.MarkupLine($"Operation [green]completed[/] in [b]{stopwatch.Elapsed.TotalSeconds} seconds[/]");

        return true;
    }

    private static bool CheckConfig(DatabaseInput input)
    {
        if (!File.Exists(input.ConfigFile))
        {
            AnsiConsole.MarkupLine($"[red b]Config file does not exist:[/] {input.ConfigFile}");
            AnsiConsole.MarkupLine("Check the path to your 'localsettings.json' or 'appsettings.Development.json'");
            return false;
        }

        return true;
    }

    private async Task<DatabaseStatus> CheckStatus(IServiceProvider services, string database)
    {
        return await AnsiConsole.Status()
            .StartAsync($"Checking status of [b]{database}[/]", async context =>
            {
                var status = await DatabaseTools.CheckDatabaseStatus(services, database);

                context.Status($"Status checked");
                AnsiConsole.WriteLine();
                AnsiConsole.Write(RenderStatus(status, database));

                return status;
            });
    }

    private IRenderable RenderStatus(DatabaseStatus status, string database)
    {
        string[] FormatResult(string check, bool condition, string helpText) => condition
            ? [check, "[green]:check_mark:[/]", ""]
            : [check, "[red]X[/]", helpText];

        var table = new Table();
        table.SimpleBorder();
        table.AddColumns("Check", "Status", "Comment");
        table.AddRow(FormatResult("Connection string", status.IsConStrGood, "Check config file. Ensure StatsWebConfig exists"));
        table.AddRow(FormatResult("Database exists", status.Exists, "Run tool with [b]database up <config-file> -s <seed-data-path>[/]"));
        table.AddRow(FormatResult("Database is pre-seeded", status.IsPreSeeded, "Run tool with [b]database up <config-file> -s <seed-data-path>[/]"));
        table.AddRow(FormatResult("Database is migrated", status.IsMigrated, "Run tool with [b]database migrate <config-file>[/]"));
        table.AddRow(FormatResult("Database is seeded", status.IsSeeded, "Run tool with [b]database up <config-file> -s <seed-data-path>[/]"));

        var panel = new Panel(table);
        panel.AsciiBorder();
        panel.Header($"DATABASE STATUS FOR [b]{database}[/]");

        return panel;
    }

    private async Task Up(IServiceProvider services, DatabaseStatus status, DatabaseInput input)
    {
        var database = input.DatabaseFlag;
        await AnsiConsole.Status()
            .StartAsync($"Initialising [b]{database}[/]...", async context =>
            {
                var databasePrefix = $"[b]{database}[/]";
                var connectionString = services.GetConnectionString();
                await using var connection = DatabaseTools.CreateConnection(connectionString);

                if (!status.Exists)
                {
                    AnsiConsole.MarkupLine($"{databasePrefix}: Creating database...");
                    await DatabaseTools.CreateDatabase(connectionString, database);
                    AnsiConsole.MarkupLine($"{databasePrefix}: Database [green]created[/]");
                }

                if (!status.IsPreSeeded)
                {
                    context.Status($"Pre-seeding {databasePrefix}...");
                    if (!PreSeed(input, databasePrefix, connection)) return false;
                }

                if (!status.IsMigrated)
                {
                    context.Status($"Migrating {databasePrefix}...");
                    Migrate(database, services);
                }

                if (!status.IsSeeded)
                {
                    context.Status($"Seeding {databasePrefix}...");

                    if (!Seed(input, connection)) return false;
                }

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"Database {databasePrefix} is [green b]UP![/]");

                return true;
            });
    }

    private void Migrate(string database, IServiceProvider services)
    {
        var runner = services.GetService<IMigrationRunner>();
        runner.LoadVersionInfoIfRequired();
        runner.MigrateUp();
        AnsiConsole.MarkupLine($"[b]{database}[/]: Migration [green]complete[/]");
    }

    private static bool PreSeed(DatabaseInput input, string databasePrefix, SqlConnection connection)
    {
        AnsiConsole.MarkupLine($"{databasePrefix}: Pre-seeding database...");

        var path = Path.Combine(input.SeedDataPathFlag, "PreSeed");
        if (!Directory.Exists(path))
        {
            AnsiConsole.MarkupLine($"Pre-seed directory [b]{path}[/] does not exist");
            AnsiConsole.MarkupLine("Check the path provided is correct. In most cases it will be [b]src\\SeedData[/]");
            return false;
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.sql"))
        {
            DatabaseTools.SeedDatabaseWithFile(connection, file);
        }

        AnsiConsole.MarkupLine($"{databasePrefix}: Pre-seeding [green]complete[/]");
        return true;
    }

    private static bool Seed(DatabaseInput input, SqlConnection connection)
    {
        var path = input.SeedDataPathFlag;
        if (!Directory.Exists(path))
        {
            AnsiConsole.MarkupLine($"Seed directory [b]{path}[/] does not exist");
            AnsiConsole.MarkupLine("Check the path provided is correct. In most cases it will be [b]src\\SeedData[/]");
            return false;
        }

        foreach (var file in Directory.EnumerateFiles(path, "*.sql").Order())
        {
            DatabaseTools.SeedDatabaseWithFile(connection, file);
        }

        return true;
    }

    private async Task Recreate(IServiceProvider services, DatabaseStatus status, DatabaseInput input)
    {
        if (input.ConfigFile.Contains(".Production"))
        {
            AnsiConsole.MarkupLine($"[red b]Config file targets PRODUCTION:[/] {input.ConfigFile}");
            AnsiConsole.MarkupLine("You probably didn't want to do this. Check the config file provided");
            return;
        }

        var database = input.DatabaseFlag;
        AnsiConsole.MarkupLine($"Preparing to [red b]DROP[/] and [b]RECREATE[/] [b]{database}[/]");

        var confirm = input.SkipConfirmationFlag || AnsiConsole.Prompt(new ConfirmationPrompt($"Are you sure you want to do this?")
        {
            DefaultValue = false
        });
        if (!confirm)
        {
            AnsiConsole.MarkupLine($"[yellow b]Aborted![/] - No changes made");
            return;
        }

        var connectionString = services.GetConnectionString();
        await DatabaseTools.DropDatabaseAsync(connectionString, database);
        AnsiConsole.MarkupLine($"[b]{database}[/] [yellow]dropped[/]");

        await Up(services, status, input);
    }
}

public class DatabaseInput
{
    public enum SubCommands
    {
        status,
        up,
        migrate,
        recreate,
    }

    [Description("The operation to perform on the databsae", Name="sub-command")]
    public SubCommands SubCommand { get; set; }

    [Description("Path to JSON file containing connection string to be used", Name = "config")]
    public string ConfigFile { get; set; }

    [FlagAlias("seed-data", 's')]
    [Description("Seeds database with files in the provided path and performs migrations")]
    public string SeedDataPathFlag { get; set; }

    [FlagAlias("db-name", 'd')]
    [Description("The name of the target database")]
    public string DatabaseFlag { get; set; } = "Stats";

    [FlagAlias("skip-confirmation", 'Y')]
    [Description("Skip confirmation prompts such as when dropping/recreating the database")]
    public bool SkipConfirmationFlag { get; set; }
}