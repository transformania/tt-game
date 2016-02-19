using System.Web.Mvc;
using System.Web.Routing;

namespace TT.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("ChatRoom", "Chat/Rooms/{room}", new {controller = "Chat", action = "Index", room = UrlParameter.Optional });

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "PvP", action = "Play", id = UrlParameter.Optional }
           );

        }
    }
}