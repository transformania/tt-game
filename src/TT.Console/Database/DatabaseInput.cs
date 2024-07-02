using Oakton;

namespace TT.Console.Database;

public class DatabaseInput
{
    [Description("Path to JSON file containing connection string to be used")]
    public string ConfigFile { get; set; }

    [Description("The name of the target database")]
    public string Database { get; set; } = "Stats";
}