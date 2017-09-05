using FluentValidation;
using Highway.Data;
using MediatR;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR.Hubs;
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
using TT.Domain.CreationPolices;
using TT.Domain.Services;
using TT.Web.LifestyleSelectionBehaviors;
using TT.Web.Models;
using TT.Web.Services;

namespace TT.Web
{
    public static class ContainerConfig
    {
        private static readonly Assembly webAssembly = typeof(ContainerConfig).Assembly;
        private static readonly Assembly domainAssembly = typeof(DomainContext).Assembly;

        public static void RegisterContainer(this Container container, HttpConfiguration httpConfig, Func<IDataProtectionProvider> dataProtectionProviderFactory)
        {
            // use AsyncScopedLifestyle when web api requests a dependency,
            // otherwise use WebRequestLifestyle when MVC requests a dependency.
            container.Options.DefaultScopedLifestyle = Lifestyle.CreateHybrid(
                defaultLifestyle: new AsyncScopedLifestyle(),
                fallbackLifestyle: new WebRequestLifestyle());

            container.Options.LifestyleSelectionBehavior 
                = new AttributeBasedLifestyleSelectionBehavior(
                    new Dictionary<IEnumerable<Type>, CreationPolicy>
                    {
                        { container.GetTypesToRegister(typeof(IValidator<>), new []{ domainAssembly }), CreationPolicy.Singleton }
                    });

            // MVC
            container.RegisterMvcControllers(webAssembly);
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // WebApi
            container.RegisterWebApiControllers(httpConfig);
            WebApiConfig.Register(httpConfig);
            httpConfig.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);

            // SignalR
            // batch registration for IHub
            var IHubImplementationTypes = GetAllGenericImplementations<IHub>(container, webAssembly);

            foreach (var hubType in IHubImplementationTypes)
            {
                container.Register(hubType, hubType, Lifestyle.Scoped);
            }

            container.Register<IHubConnectionIdAccessor, HubConnectionIdAccessor>(Lifestyle.Scoped);

            container.Register<IHubRequestAccessor, HubRequestAccessor>(Lifestyle.Scoped);

            container.Register(typeof(IHubContextAccessor<>), typeof(HubContextAccessor<>), Lifestyle.Singleton);

            // Owin
            container.Register<IOwinContextAccessor, OwinContextAccessor>(Lifestyle.Scoped);
            container.Register<IPrincipalAccessor, PrincipalAccessor>(Lifestyle.Scoped);

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
            container.RegisterSingleton(new SingleInstanceFactory(((IServiceProvider)container).GetService));
            container.RegisterSingleton(new MultiInstanceFactory(container.GetAllInstances));

            // Request Handlers
            var requestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(IRequestHandler<,>), domainAssembly);
            var voidRequestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(IRequestHandler<>), domainAssembly);

            var asyncRequestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(IAsyncRequestHandler<,>), domainAssembly);
            var asyncVoidRequestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(IAsyncRequestHandler<>), domainAssembly);

            foreach (var types in requestHandlerTypesToRegister)
            {
                container.Register(typeof(IRequestHandler<,>), types);
            }

            foreach (var types in voidRequestHandlerTypesToRegister)
            {
                container.Register(typeof(IRequestHandler<>), types);
            }

            foreach (var types in asyncRequestHandlerTypesToRegister)
            {
                container.Register(typeof(IAsyncRequestHandler<,>), types);
            }

            foreach (var types in asyncVoidRequestHandlerTypesToRegister)
            {
                container.Register(typeof(IAsyncRequestHandler<>), types);
            }

            // PipelineBehaviors
            var pipelineBehaviorTypesToRegister = GetAllGenericImplementations(container, typeof(IPipelineBehavior<,>), domainAssembly);

            container.RegisterCollection(typeof(IPipelineBehavior<,>), pipelineBehaviorTypesToRegister);

            // Validators
            var validatorTypes = GetAllGenericImplementations(container, typeof(IValidator<>), domainAssembly);

            foreach (var type in validatorTypes)
            {
                container.Register(typeof(IValidator<>), type);
            }

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