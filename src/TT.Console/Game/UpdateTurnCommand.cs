using Oakton;
using Spectre.Console;

namespace TT.Console.Game;

[Description("Performs the turn update against a target server", Name = "update-turn")]
public class UpdateTurnCommand : OaktonAsyncCommand<UpdateTurnInput>
{
    public override async Task<bool> Execute(UpdateTurnInput input)
    {
        return await AnsiConsole.Status()
            .StartAsync("Running turn update...", async ctx =>
            {
                var client = new HttpClient();
                try
                {
                    var response = await client.PostAsync($"{input.TargetServerFlag}/API/WorldUpdate", null);
                    if (!response.IsSuccessStatusCode)
                    {
                        AnsiConsole.MarkupLine($"[red b]Turn update failed![/]");
                        AnsiConsole.MarkupLine(await response.Content.ReadAsStringAsync());
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red b]Turn update failed![/]");
                    AnsiConsole.WriteLine(ex.Message);
                    return false;
                }

                AnsiConsole.MarkupLine($"Turn update [green]successful[/]!");
                return true;
            });
    }
}

public class UpdateTurnInput
{
    [FlagAlias("target-server", 't')]
    [Description("The server, including port, to run the turn update", Name = "target-server")]
    public string TargetServerFlag { get; set; } = "http://localhost:52223";
}