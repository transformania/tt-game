using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using tfgame.dbModels;

namespace tfgame
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            DomainRegistry.Root = new Root();

            // start duel loop
            //var duelUpdateTic = new System.Timers.Timer(5000);
            //duelUpdateTic.Enabled = true;
            //duelUpdateTic.Elapsed += new ElapsedEventHandler(Duel.RunTick);

        }
    }
}