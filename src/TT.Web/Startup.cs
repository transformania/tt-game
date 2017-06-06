using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Owin;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using TT.Domain;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.World.Queries;
using TT.Web;
using TT.Web.Models;
using TT.Web.Services;

[assembly: OwinStartup(typeof(Startup))]

namespace TT.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = new Container();
            var httpConfig = new HttpConfiguration();

            container.RegisterContainer(httpConfig, app.GetDataProtectionProvider);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, User>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.MapSignalR();

            app.Use(async (context, next) =>
            {
                // check if there is a HttpContext for WebRequestLifestyle to store its scope
                if (HttpContext.Current != null)
                {
                    // capture the owin context for any service dependant on it and store it in the async scoped container
                    // this will use WebRequestLifestyle's cache
                    if (container.GetInstance<IOwinContextAccessor>() is CallContextOwinContextAccessor webrequestCallContextOwinContextAccessor)
                    {
                        webrequestCallContextOwinContextAccessor.CurrentContext = context;
                    }
                }

                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    // capture the owin context for any service dependant on it and store it in the async scoped container
                    // this will use AsyncScopedLifestyle's cache
                    if (container.GetInstance<IOwinContextAccessor>() is CallContextOwinContextAccessor asyncCallContextOwinContextAccessor)
                    {
                        asyncCallContextOwinContextAccessor.CurrentContext = context;
                    }

                    await next();
                }
            });

            app.UseWebApi(httpConfig);

            AttackProcedures.LoadCovenantOwnersIntoRAM();

            // set chaos mode 
            IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
            PvPWorldStat data = repo.PvPWorldStats.FirstOrDefault();
            PvPStatics.ChaosMode = data != null ? data.ChaosMode : false;
            PvPStatics.RoundDuration = data != null ? data.RoundDuration : 5000;
            PvPStatics.AlphaRound = DomainRegistry.Repository.FindSingle(new GetWorld()).RoundNumber;
        }
    }
}
