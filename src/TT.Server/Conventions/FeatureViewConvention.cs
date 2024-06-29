using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;

namespace TT.Server.Conventions;

/// <summary>
/// A convention for using the 'feature' property on controllers to allow MVC to locate the associated views
/// </summary>
public class FeatureViewConvention : IViewLocationExpander
{
    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context,
        IEnumerable<string> viewLocations)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (viewLocations == null)
        {
            throw new ArgumentNullException(nameof(viewLocations));
        }

        var controllerActionDescriptor = context.ActionContext.ActionDescriptor as ControllerActionDescriptor;
        if (controllerActionDescriptor == null)
        {
            throw new NullReferenceException("ControllerActionDescriptor cannot be null.");
        }

        string featureName = controllerActionDescriptor.Properties["feature"] as string;
        foreach (var location in viewLocations)
        {
            yield return location.Replace("{2}", featureName);
        }
    }

    public void PopulateValues(ViewLocationExpanderContext context) { }
}