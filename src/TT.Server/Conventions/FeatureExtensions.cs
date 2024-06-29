namespace TT.Server.Conventions;

public static class FeatureExtensions
{
    /// <summary>
    /// Adds MVC to the IServiceCollection and tells MVC where to find views based on feature folders
    /// </summary>
    /// <param name="servicecs"></param>
    public static void AddFeatureConventions(this IServiceCollection servicecs)
    {
        servicecs.AddMvc(o => o.Conventions.Add(new FeatureControllerConvention()))
            .AddRazorOptions((razor =>
            {
                // {0} - Action Name
                // {1} - Controller Name
                // {2} - Feature Name
                // Replace normal view location entirely
                razor.ViewLocationFormats.Clear();
                razor.ViewLocationFormats.Add("/Features/{2}/{1}/{0}.cshtml");
                razor.ViewLocationFormats.Add("/Features/{2}/{0}.cshtml");
                razor.ViewLocationFormats.Add("/Features/Shared/{0}.cshtml");
                razor.ViewLocationExpanders.Add(new FeatureViewConvention());
            }));
    }
}