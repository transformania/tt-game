using System.Web.Mvc;
using System.Web.Routing;
using FeatureSwitch;
using TT.Domain;

namespace TT.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            if (FeatureContext.IsEnabled<ChatV2>())
                routes.MapRoute("Chat", "Chat", new { controller = "Chat", action = "Index" });

            routes.MapRoute("ChatRoom", "Chat/Rooms/{room}", new { controller = "Chat", action = "Index", room = UrlParameter.Optional });

            routes.MapRoute("Home", "", new { controller = "PvP", action = "Play", id = UrlParameter.Optional });

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "PvP", action = "Index", id = UrlParameter.Optional }
            );
        }
    } 
}