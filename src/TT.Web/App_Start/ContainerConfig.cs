using AutoMapper;
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
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using TT.Domain;
using TT.Domain.CreationPolices;
using TT.Domain.Services;
using TT.Domain.Validation;
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
            container.RegisterInstance(new ServiceFactory(((IServiceProvider)container).GetService));

            // Request Handlers
            var requestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(RequestHandler<,>), domainAssembly);
            var voidRequestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(RequestHandler<>), domainAssembly);

            var asyncRequestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(IRequestHandler<,>), domainAssembly);
            var asyncVoidRequestHandlerTypesToRegister = GetAllGenericImplementations(container, typeof(AsyncRequestHandler<>), domainAssembly);

            foreach (var types in requestHandlerTypesToRegister)
            {
                container.Register(typeof(RequestHandler<,>), types);
            }

            foreach (var types in voidRequestHandlerTypesToRegister)
            {
                container.Register(typeof(RequestHandler<>), types);
            }

            foreach (var types in asyncRequestHandlerTypesToRegister)
            {
                container.Register(typeof(IRequestHandler<,>), types);
            }

            foreach (var types in asyncVoidRequestHandlerTypesToRegister)
            {
                container.Register(typeof(AsyncRequestHandler<>), types);
            }

            // PipelineBehaviors
            var pipelineBehaviorTypesToRegister = GetAllGenericImplementations(container, typeof(IPipelineBehavior<,>), domainAssembly);

            container.Collection.Register(typeof(IPipelineBehavior<,>), pipelineBehaviorTypesToRegister);

            // Validators
            var validatorTypes = GetAllGenericImplementations(container, typeof(IValidator<>), domainAssembly)
                .Where(t => !(t.IsGenericType && t.GetGenericTypeDefinition() == typeof(NullValidatorWithResponse<>)))
                .Where(t => t != typeof(NullValidator));

            foreach (var type in validatorTypes)
            {
                var typeGenericArgs = type.GetClosedTypeOf(typeof(IValidator<>)).GenericTypeArguments.First();

                if (!(typeof(IRequest).IsAssignableFrom(typeGenericArgs) || typeGenericArgs.IsClosedTypeOf(typeof(IRequest<>))))
                {
                    continue;
                }

                var boundGenericValidator = typeof(IValidator<>).MakeGenericType(typeGenericArgs);

                container.Register(boundGenericValidator, type);
            }

            container.Register(typeof(NullValidatorWithResponse<>), typeof(NullValidatorWithResponse<>));
            container.Register(typeof(NullValidator), typeof(NullValidator));

            container.ResolveUnregisteredType += (sender, e) =>
            {
                if (e.UnregisteredServiceType.IsGenericType && e.UnregisteredServiceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                {
                    var typeGenericArgs = e.UnregisteredServiceType.GenericTypeArguments.Single();

                    if (typeof(IRequest).IsAssignableFrom(typeGenericArgs))
                    {
                        e.Register(() => container.GetInstance(typeof(NullValidator)));
                    }
                    else                     
                    {
                        var closedRequestTypes = typeGenericArgs.GetClosedTypesOf(typeof(IRequest<>));

                        if (closedRequestTypes.Length == 1)
                        {
                            var responseType = closedRequestTypes.Single().GenericTypeArguments.Single();

                            e.Register(() => container.GetInstance(typeof(NullValidatorWithResponse<>).MakeGenericType(responseType)));
                        }
                    }
                }
            };

            // AutoMapper
            container.Collection.Register(typeof(Profile), typeof(DomainRegistry).Assembly);

            IMapper CreateMapper() => new MapperConfiguration(cfg =>
            {
#pragma warning disable 618
                cfg.CreateMissingTypeMaps = true;
#pragma warning restore 618
                cfg.AddMaps(typeof(DomainRegistry).Assembly);
            }).CreateMapper();

            // This function is called once when the mapper singleton is needed.
            container.RegisterSingleton(CreateMapper);

            // This function is called only when something accesses the static Mapper instance.
            DomainRegistry.SetMapperFunc(container.GetInstance<IMapper>);

            DomainRegistry.Mapper.ConfigurationProvider.AssertConfigurationIsValid();

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