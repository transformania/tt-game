using Oakton;
using Spectre.Console;

namespace TT.Console.Database;

[Description("Creates a new DB", Name = "create-db")]
public class CreateDatabaseCommand : OaktonCommand<DatabaseInput>
{
    public CreateDatabaseCommand()
    {
        Usage("Create default [white]Stats[/] database").Arguments(x => x.ConfigFile);
        Usage("Create named database").Arguments(x => x.ConfigFile, x => x.Database);
    }
    
    public override bool Execute(DatabaseInput input)
    {
        return AnsiConsole.Status()
            .Start($"Creating database [white]{input.Database}[/]...", _ =>
            {
                if (!DatabaseTools.CreateDatabase(input)) return false;
                
                AnsiConsole.MarkupLine($":check_mark: {input.Database} database created");
                return true;
            });
    }
}