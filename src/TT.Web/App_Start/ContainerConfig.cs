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
using MediatR;
using TT.Domain;
using FluentValidation;
using TT.Domain.Validation;
using System;
using Highway.Data;

namespace TT.Web
{
    public class ContainerConfig
    {
        public static void ConfigureContainer(HttpConfiguration httpConfig, IAppBuilder app)
        {
            var container = new Container();
            var webAssembly = typeof(ContainerConfig).Assembly;
            var domainAssembly = typeof(DomainContext).Assembly;

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

            container.Register<IDataContext, DomainContext>(Lifestyle.Scoped);

            container.RegisterMvcControllers(webAssembly);
            container.RegisterWebApiControllers(httpConfig);

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

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            httpConfig.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
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