using Oakton;
using Spectre.Console;

namespace TT.Console.Database;

[Description("Creates a new DB", Name = "drop-db")]
public class DropDatabaseCommand : OaktonCommand<DatabaseInput>
{
    public DropDatabaseCommand()
    {
        Usage("Drop default [white]Stats[/] database").Arguments(x => x.ConfigFile);
        Usage("Drop named database").Arguments(x => x.ConfigFile, x => x.Database);
    }
    
    public override bool Execute(DatabaseInput input)
    {
        return AnsiConsole.Status()
            .Start($"Dropping database [white]{input.Database}[/]...", _ =>
            {
                if (!DatabaseTools.DropDatabase(input)) return false;
                
                AnsiConsole.MarkupLine($":check_mark: {input.Database} database dropped");
                return true;
            });
    }
}