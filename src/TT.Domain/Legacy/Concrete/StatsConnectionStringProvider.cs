namespace TT.Domain.Concrete;

/**
 * This is a hack to allow getting the connection string down to StatsContext under .NET 8
 * without breaking compatibility with TT.Web
 */
public static class StatsConnectionStringProvider
{
    public static string ConnectionStringOrName { get; set; }
}