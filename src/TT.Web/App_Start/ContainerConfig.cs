using FluentValidation;
using Highway.Data;
using MediatR;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TT.Domain;
using TT.Domain.Services;
using TT.Web.Models;
using TT.Web.Services;

namespace TT.Web
{
    public static class ContainerConfig
    {
        static Assembly webAssembly = typeof(ContainerConfig).Assembly;
        static Assembly domainAssembly = typeof(DomainContext).Assembly;

        public static void RegisterContainer(this Container container, HttpConfiguration httpConfig, Func<IDataProtectionProvider> dataProtectionProviderFactory)
        {
            // use AsyncScopedLifestyle when web api requests a dependency,
            // otherwise use WebRequestLifestyle when MVC requests a dependency.
            container.Options.DefaultScopedLifestyle = Lifestyle.CreateHybrid(
                defaultLifestyle: new AsyncScopedLifestyle(),
                fallbackLifestyle: new WebRequestLifestyle());

            // MVC
            container.RegisterMvcControllers(webAssembly);
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // WebApi
            container.RegisterWebApiControllers(httpConfig);
            WebApiConfig.Register(httpConfig);
            httpConfig.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // Owin
            container.Register<IOwinContextAccessor, CallContextOwinContextAccessor>(Lifestyle.Scoped);
            container.Register<IPrincipalAccessor, CallContextPrincipalAccessor>(Lifestyle.Scoped);

            container.Register(() =>
            {
                var applicationDbContext = new ApplicationDbContext();
                var userStore = new UserStore<User>(applicationDbContext);
                var dataProtector = dataProtectionProviderFactory().Create("ASP.NET Identity");
                var dataProtectorTokenProvider = new DataProtectorTokenProvider<User>(dataProtector);

                return new ApplicationUserManager(userStore, dataProtectorTokenProvider);
            }, Lifestyle.Scoped);

            container.Register<ApplicationSignInManager>(Lifestyle.Scoped);

            container.Register(() => container.GetInstance<IOwinContextAccessor>().CurrentContext.Authentication, Lifestyle.Scoped);

            container.Register<IDataContext, DomainContext>(Lifestyle.Scoped);

            // Mediator
            container.RegisterSingleton<IMediator, Mediator>();
            container.RegisterSingleton(new SingleInstanceFactory(container.GetInstance));
            container.RegisterSingleton(new MultiInstanceFactory(container.GetAllInstances));

            // Request Handlers
            var requestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(IRequestHandler<,>), domainAssembly);
            var voidRequestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(IRequestHandler<>), domainAssembly);

            foreach (var types in requestHandlerTypesToRegister)
            {
                container.Register(typeof(IRequestHandler<,>), types);
            }

            foreach (var types in voidRequestHandlerTypesToRegister)
            {
                container.Register(typeof(IRequestHandler<>), types);
            }

            // PipelineBehaviors
            var pipelineBehaviorTypesToRegister = GetAllGenericImplementations(container, typeof(IPipelineBehavior<,>), domainAssembly);

            container.RegisterCollection(typeof(IPipelineBehavior<,>), pipelineBehaviorTypesToRegister);

            // Validators
            container.RegisterCollection(typeof(IValidator<>), domainAssembly);

            container.Verify();
        }

        private static IEnumerable<Type> GetAllGenericImplementations<T>(Container container, params Assembly[] assemblies)
        {
            return GetAllGenericImplementations(container, typeof(T), assemblies);
        }

        private static IEnumerable<Type> GetAllGenericImplementations(Container container, Type serviceType, params Assembly[] assemblies)
        {
            return container.GetTypesToRegister(
                serviceType,
                assemblies,
                new TypesToRegisterOptions
                {
                    IncludeGenericTypeDefinitions = true
                });
        }
    }
}