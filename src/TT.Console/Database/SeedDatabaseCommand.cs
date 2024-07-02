using Oakton;
using Spectre.Console;

namespace TT.Console.Database;

[Description("Creates a new DB", Name = "seed-db")]
public class SeedDatabaseCommand : OaktonCommand<SeedDatabaseInput>
{
    public SeedDatabaseCommand()
    {
        Usage("Seed default [white]Stats[/] database").Arguments(x => x.ConfigFile, x => x.SeedDataPath);
        Usage("Seed named database").Arguments(x => x.ConfigFile, x => x.SeedDataPath, x => x.Database);
    }
    
    public override bool Execute(SeedDatabaseInput input)
    {
        return AnsiConsole.Status()
            .Start($"Creating database [white]{input.Database}[/]...", _ =>
            {
                if (!DatabaseTools.SeedDatabase(input)) return false;
                
                AnsiConsole.MarkupLine($":check_mark: {input.Database} database seeded");
                return true;
            });
    }
}

public class SeedDatabaseInput : DatabaseInput
{
    [Description("Path to seed data scripts", Name = "seed-data")]
    public string SeedDataPath { get; set; }
} 