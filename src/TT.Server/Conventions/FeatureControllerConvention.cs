using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace TT.Server.Conventions;

/// <summary>
/// Adds a 'feature' property to controllers to allow controllers in feature folders to be found by MVC
/// </summary>
public class FeatureControllerConvention : IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        controller.Properties.Add("feature", GetFeatureName(controller.ControllerType));
    }

    private static string GetFeatureName(TypeInfo controllerType)
    {
        var tokens = controllerType.FullName!.Split('.');
        if (tokens.All(t => t != "Features"))
            return "";
        var featureName = tokens
            .SkipWhile(t => !t.Equals("features", StringComparison.CurrentCultureIgnoreCase))
            .Skip(1)
            .Take(1)
            .FirstOrDefault();

        return featureName;
    }
}

