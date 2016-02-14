using System;
using System.Linq;
using FeatureSwitcher.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Web;
using TT.Web.Models;

[assembly: OwinStartup(typeof(Startup))]

namespace TT.Web
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Features.Are.ConfiguredBy.AppConfig();
            
            // Configure the db context, user manager and role manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

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

            AttackProcedures.LoadCovenantOwnersIntoRAM();

            // set chaos mode 
            IPvPWorldStatRepository repo = new EFPvPWorldStatRepository();
            PvPWorldStat data = repo.PvPWorldStats.FirstOrDefault();
            PvPStatics.ChaosMode = data != null ? data.ChaosMode : false;
            PvPStatics.RoundDuration = data != null ? data.RoundDuration : 5000;
        }
    }
}
