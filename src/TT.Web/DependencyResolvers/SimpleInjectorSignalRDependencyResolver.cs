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
            return _container.GetService(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            Type collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            IEnumerable<object> simpleInjectorServices = (IEnumerable<object>)_container.GetService(collectionType) ?? Enumerable.Empty<object>();

            return base.GetServices(serviceType).Concat(simpleInjectorServices);
        }
    }
}