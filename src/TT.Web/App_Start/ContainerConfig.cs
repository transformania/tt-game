using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using TT.Web.Models;
using Owin;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace TT.Web
{
    public class ContainerConfig
    {
        public static void ConfigureContainer(HttpConfiguration httpConfig, IAppBuilder app)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            container.Register(() =>
            {
                var applicationDbContext = new ApplicationDbContext();
                var userStore = new UserStore<User>(applicationDbContext);
                var dataProtector = app.GetDataProtectionProvider().Create("ASP.NET Identity");
                var dataProtectorTokenProvider = new DataProtectorTokenProvider<User>(dataProtector);

                return new ApplicationUserManager(userStore, dataProtectorTokenProvider);
            }, Lifestyle.Scoped);

            container.Register<ApplicationSignInManager>(Lifestyle.Scoped);

            container.Register(
                () => container.IsVerifying() 
                ? new OwinContext(new Dictionary<string, object>()).Authentication
                : HttpContext.Current.GetOwinContext().Authentication, Lifestyle.Scoped);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterWebApiControllers(httpConfig);

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            httpConfig.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}