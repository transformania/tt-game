using Microsoft.AspNet.SignalR;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TT.Web.DependencyResolvers
{
    public class SimpleInjectorSignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IServiceProvider _container;

        public SimpleInjectorSignalRDependencyResolver(Container container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            // if Simple Injector is still being configured but SignalR needs a dependency, use the default resolver. If Simple Injector returns null, use the default resolver.
            // this is done because GetService locks the container.
            if (!ContainerConfig.ContainerVerified)
            {
                return base.GetService(serviceType);
            }
            else
            {
                return _container.GetService(serviceType) ?? base.GetService(serviceType);
            }
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            Type collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            IEnumerable<object> simpleInjectorServices;

            if (!ContainerConfig.ContainerVerified)
            {
                return base.GetServices(serviceType);
            }
            else
            {
                simpleInjectorServices = (IEnumerable<object>)_container.GetService(collectionType) ?? Enumerable.Empty<object>();

                return base.GetServices(serviceType).Concat(simpleInjectorServices);
            }            
        }
    }
}