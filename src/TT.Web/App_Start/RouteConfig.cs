using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;
using TT.Web.Attributes;

namespace TT.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("ChatRoom", "Chat/Rooms/{room}", new { controller = "Chat", action = "Index", room = UrlParameter.Optional });

            routes.MapMvcAttributeRoutes(new InheritedDirectRouteProvider());

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "PvP", action = "Play", id = UrlParameter.Optional }
            );
        }

        private class InheritedDirectRouteProvider : DefaultDirectRouteProvider
        {
            protected override IReadOnlyList<IDirectRouteFactory>
                 GetControllerRouteFactories(ControllerDescriptor controllerDescriptor)
            {
                return base.GetControllerRouteFactories(controllerDescriptor)
                        .Concat(controllerDescriptor
                                .GetCustomAttributes(typeof(InheritedRouteAttribute), inherit: true)
                                .Cast<IDirectRouteFactory>())
                        .ToList();
            }
        }
    }

    
}