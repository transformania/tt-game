using System.Diagnostics;
using FluentMigrator.Runner;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Oakton;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace TT.Console.Database;

public class DatabaseCommand : OaktonAsyncCommand<DatabaseInput>
{
    public override async Task<bool> Execute(DatabaseInput input)
    {
        var stopwatch = Stopwatch.StartNew();
        var context = DatabaseCommandContext.FromInput(input);

        if (!CheckConfig(context.ConfigFile)) return false;

        var services = DatabaseTools.Configure(context.ConfigFile);
        var status = await CheckStatus(services, context.Database);

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
                await Up(services, status, context);
                break;
            case DatabaseInput.SubCommands.migrate:
                Migrate(context);
                break;
            case DatabaseInput.SubCommands.rollback:
                Rollback(context);
                break;
            case DatabaseInput.SubCommands.recreate:
                await Recreate(services, context);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        stopwatch.Stop();

        AnsiConsole.MarkupLine($"Operation [green]completed[/] in [b]{stopwatch.Elapsed.TotalSeconds} seconds[/]");

        return true;
    }

    private static bool CheckConfig(string configFile)
    {
        if (!File.Exists(configFile))
        {
            AnsiConsole.MarkupLine($"[red b]Config file does not exist:[/] {configFile}");
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
            ? [check, "[green]OK[/]", ""]
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

    private static async Task Up(IServiceProvider services, DatabaseStatus status, DatabaseCommandContext context)
    {
        await AnsiConsole.Status()
            .StartAsync($"Initialising [b]{context.Database}[/]...", async statusContext =>
            {
                var databasePrefix = $"[b]{context.Database}[/]";
                var connectionString = services.GetConnectionString();
                await using var connection = DatabaseTools.CreateConnection(connectionString);

                if (!status.Exists)
                {
                    AnsiConsole.MarkupLine($"{databasePrefix}: Creating database...");
                    await DatabaseTools.CreateDatabase(connectionString, context.Database);
                    AnsiConsole.MarkupLine($"{databasePrefix}: Database [green]created[/]");
                }

                if (!status.IsPreSeeded)
                {
                    statusContext.Status($"Pre-seeding {databasePrefix}...");
                    if (!PreSeed(context, connection)) return false;
                }

                if (!status.IsMigrated)
                {
                    statusContext.Status($"Migrating {databasePrefix}...");
                    Migrate(context);
                }

                if (!status.IsSeeded)
                {
                    statusContext.Status($"Seeding {databasePrefix}...");

                    if (!Seed(context, connection)) return false;
                }

                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"Database {databasePrefix} is [green b]UP![/]");

                return true;
            });
    }

    private static void Migrate(DatabaseCommandContext context)
    {
        // FluentMigrator seems to cache the output from the VersionInfo table if it is called previously
        // This is a problem if we drop the database as it then thinks it exists when it tries to migrate and goes boom.
        // The solution is to just rebuild the service collection.
        var services = DatabaseTools.Configure(context.ConfigFile);
        var runner = services.GetService<IMigrationRunner>();
        if (runner.HasMigrationsToApplyUp())
            runner.MigrateUp();

        AnsiConsole.MarkupLine($"[b]{context.Database}[/]: Migration [green]complete[/]");
    }

    private static void Rollback(DatabaseCommandContext context)
    {
        var services = DatabaseTools.Configure(context.ConfigFile);
        var runner = services.GetService<IMigrationRunner>();

        try
        {
            runner.Rollback(context.RollBackSteps);
        }
        catch (InvalidOperationException)
        {
            AnsiConsole.MarkupLine("[red b]Rollback failed - only forward migration supported[/]");
            return;
        }

        AnsiConsole.MarkupLine($"[b]{context.Database}[/]: Rollback [green]complete[/]");
    }

    private static bool PreSeed(DatabaseCommandContext context, SqlConnection connection)
    {
        AnsiConsole.MarkupLine($"[b]{context.Database}[/]: Pre-seeding database...");

        var path = Path.Combine(context.SeedDataPath, "PreSeed");
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

        AnsiConsole.MarkupLine($"[b]{context.Database}[/]: Pre-seeding [green]complete[/]");
        return true;
    }

    private static bool Seed(DatabaseCommandContext context, SqlConnection connection)
    {
        var path = context.SeedDataPath;
        if (!Directory.Exists(path))
        {
            AnsiConsole.MarkupLine($"Seed directory [b]{path}[/] does not exist");
            AnsiConsole.MarkupLine("Check the path provided is correct. In most cases it will be [b]src\\SeedData[/]");
            return false;
        }

        var files = new List<string>();
        files.AddRange(Directory.EnumerateFiles(path, "*.sql").Order());
        files.AddRange(Directory.EnumerateFiles(Path.Combine(path, "StoredProcs"), "*.sql"));

        foreach (var file in files)
        {
            DatabaseTools.SeedDatabaseWithFile(connection, file);
        }

        return true;
    }

    private static async Task Recreate(IServiceProvider services, DatabaseCommandContext context)
    {
        if (context.ConfigFile.Contains(".Production"))
        {
            AnsiConsole.MarkupLine($"[red b]Config file targets PRODUCTION:[/] {context.ConfigFile}");
            AnsiConsole.MarkupLine("You probably didn't want to do this. Check the config file provided");
            return;
        }

        var database = context.Database;
        AnsiConsole.MarkupLine($"Preparing to [red b]DROP[/] and [b]RECREATE[/] [b]{database}[/]");

        var confirm = context.SkipConfirmation || AnsiConsole.Prompt(new ConfirmationPrompt($"Are you sure you want to do this?")
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

        await Up(services, new DatabaseStatus(true), context);
    }
}

public class DatabaseInput
{
    public enum SubCommands
    {
        status,
        up,
        migrate,
        rollback,
        recreate,
    }

    [Description("The operation to perform on the database", Name="sub-command")]
    public SubCommands SubCommand { get; set; }

    [FlagAlias("config-file", 'c')]
    [Description("Path to JSON file containing connection string to be used")]
    public string ConfigFileFlag { get; set; }

    [FlagAlias("seed-data", 's')]
    [Description("Seeds database with files in the provided path and performs migrations")]
    public string SeedDataPathFlag { get; set; }

    [FlagAlias("db-name", 'd')]
    [Description("The name of the target database")]
    public string DatabaseFlag { get; set; } = "Stats";

    [FlagAlias("skip-confirmation", 'Y')]
    [Description("Skip confirmation prompts such as when dropping/recreating the database")]
    public bool SkipConfirmationFlag { get; set; }

    [FlagAlias("rollback-steps", 'r')]
    [Description("Number of steps to rollback when used with rollback sub-command")]
    public int RollBackStepsFlag { get; set; } = 1;
}

public class DatabaseCommandContext
{
    public string Database { get; private init; }
    public string ConfigFile { get; private init; }
    public string SeedDataPath { get; private init; }
    public bool SkipConfirmation { get; private init; }
    public int RollBackSteps { get; private init; }

    public static DatabaseCommandContext FromInput(DatabaseInput input)
    {
        return new DatabaseCommandContext
        {
            Database = input.DatabaseFlag,
            ConfigFile = !input.ConfigFileFlag.IsNullOrEmpty() ? input.ConfigFileFlag : FindDefaultConfigFile(),
            SeedDataPath = !input.SeedDataPathFlag.IsNullOrEmpty() ? input.SeedDataPathFlag : FindDefaultSeedData(),
            SkipConfirmation = input.SkipConfirmationFlag,
            RollBackSteps = input.RollBackStepsFlag,
        };
    }

    public static string FindCheckoutRoot(string workingDir)
    {
        return Directory.EnumerateDirectories(workingDir, ".git").Any() ? workingDir : Path.Combine("../", workingDir);
    }

    public static string FindDefaultConfigFile()
    {
        var basePath = FindCheckoutRoot(Environment.CurrentDirectory);
        var localSettings = Path.Combine(basePath, "src/TT.Server/localsettings.json");
        var appSettings = Path.Combine(basePath, "src/TT.Server/appsettings.Development.json");

        var localSettingsExists = File.Exists(localSettings);
        var appSettingsExists = File.Exists(appSettings);

        if (localSettingsExists) return localSettings;
        return appSettingsExists ? appSettings : null;
    }

    public static string FindDefaultSeedData()
    {
        var basePath = FindCheckoutRoot(Environment.CurrentDirectory);
        var seedDataPath = Path.Combine(basePath, "src/SeedData");

        return Directory.Exists(seedDataPath) ? seedDataPath : null;
    }
}