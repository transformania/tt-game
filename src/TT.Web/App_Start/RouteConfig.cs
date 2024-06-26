using System.Web.Mvc;
using System.Web.Routing;
using TT.Web.Services;

namespace TT.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes, IFeatureService featureService)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            if (featureService.IsFeatureEnabled(Features.ChatV2))
                routes.MapRoute("Chat", "chat", MVC.Chat.Index());

            routes.MapRoute("ChatRoom", "chat/rooms/{room}", MVC.Chat.Index(null).AddRouteValue("room", UrlParameter.Optional));

            routes.MapRoute("Home", "", MVC.PvP.Play().AddRouteValue("id", UrlParameter.Optional));

            // Not using T4MVC on this route because there is no Index() in PvP
            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "pvp", action = "index", area = "", id = UrlParameter.Optional }
            );
        }
    } 
}