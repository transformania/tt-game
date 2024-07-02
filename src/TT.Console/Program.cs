using System.Reflection;
using Oakton;

var executor = CommandExecutor.For(_ =>
{
    _.RegisterCommands(typeof(Program).GetTypeInfo().Assembly);
});

return executor.Execute(args);