using FluentMigrator.Runner;
using FluentMigrator.Runner.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;

namespace TT.Console.Database;

public class FluentMigratorAnsiConsoleLoggerProvider(IOptions<FluentMigratorLoggerOptions> opts) : ILoggerProvider
{
    private readonly FluentMigratorLoggerOptions options = opts.Value;

    public void Dispose() { }

    public ILogger CreateLogger(string categoryName) => new FluentMigratorAnsiConsoleLogger(options);
}

public class FluentMigratorAnsiConsoleLogger(FluentMigratorLoggerOptions options) : FluentMigratorLogger(options)
{
    protected override void WriteError(string message) => AnsiConsole.MarkupLineInterpolated($"[b]Migrator[/]: [red]${message}[/]");
    protected override void WriteError(Exception exception) => AnsiConsole.WriteException(exception, ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods);
    protected override void WriteHeading(string message) => AnsiConsole.MarkupLineInterpolated($"[b]Migrator[/]: [yellow]${message}[/]");
    protected override void WriteEmphasize(string message) => AnsiConsole.MarkupLineInterpolated($"[b]Migrator[/]: [bold aqua]${message}[/");
    protected override void WriteEmptySql() => AnsiConsole.MarkupLine("[b]Migrator[/]: [grey]No SQL statement executed.[/]");
    protected override void WriteElapsedTime(TimeSpan timeSpan) => AnsiConsole.MarkupLineInterpolated($"[b]Migrator[/]: [grey]=> {timeSpan.TotalSeconds}s");
    protected override void WriteSay(string message) => AnsiConsole.MarkupLineInterpolated($"[b]Migrator[/]: [grey]{message}[/]");

    protected override void WriteSql(string sql)
    {
        var message = !string.IsNullOrEmpty(sql) ? sql : "No SQL statement executed.";
        AnsiConsole.MarkupLineInterpolated($"[b]Migrator[/]: [grey]{message}[/]");
    }
}