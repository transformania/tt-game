using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TT.Console.Database;

public static class DatabaseUtilityHost
{
    public static IHost BuildDatabaseUtilityHost(string configFile, Action<HostBuilderContext, IServiceCollection> configureServices = null)
    {
        var builder = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration => configuration.AddJsonFile(configFile, false));

        if (configureServices != null)
            builder.ConfigureServices(configureServices);

        return builder.Build();
    }
}