using Oakton;
using Spectre.Console;

namespace TT.Console.Database;

public class MigrateCommand : OaktonCommand<DatabaseInput>
{
    public MigrateCommand()
    {
        Usage("Migrate default [white]Stats[/] database").Arguments(x => x.ConfigFile);
        Usage("Migrate named database").Arguments(x => x.ConfigFile, x => x.Database);
    }
    
    public override bool Execute(DatabaseInput input)
    {
        return AnsiConsole.Status()
            .AutoRefresh(true)
            .Start("Preparing migration...", ansiContext =>
            {
                AnsiConsole.MarkupLine($"Using [white]{input.ConfigFile}[/]");
                ansiContext.Status("Running migrations...");
                
                return DatabaseTools.MigrateUp(input);
            });
    }
}