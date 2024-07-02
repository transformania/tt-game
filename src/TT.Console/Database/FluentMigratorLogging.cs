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
    protected override void WriteError(string message) => AnsiConsole.MarkupLineInterpolated($"[red]${message}[/]");
    protected override void WriteError(Exception exception) => AnsiConsole.WriteException(exception, ExceptionFormats.ShortenTypes | ExceptionFormats.ShortenMethods);
    protected override void WriteHeading(string message) => AnsiConsole.MarkupLineInterpolated($"[yellow]${message}[/]");
    protected override void WriteEmphasize(string message) => AnsiConsole.MarkupLineInterpolated($"[bold aqua]${message}[/");
    protected override void WriteEmptySql() => AnsiConsole.MarkupLine("[grey]No SQL statement executed.[/]");
    protected override void WriteElapsedTime(TimeSpan timeSpan) => AnsiConsole.MarkupLineInterpolated($"[grey]=> {timeSpan.TotalSeconds}s");
    protected override void WriteSay(string message) => AnsiConsole.MarkupLineInterpolated($"[white]{message}[/]");

    protected override void WriteSql(string sql)
    {
        var message = !string.IsNullOrEmpty(sql) ? sql : "No SQL statement executed.";
        AnsiConsole.MarkupLine($"[grey]{message}[/]");
    }
}