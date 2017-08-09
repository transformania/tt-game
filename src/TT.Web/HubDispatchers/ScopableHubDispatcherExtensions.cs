using Microsoft.AspNet.SignalR;
using Owin;
using System;
using System.Threading.Tasks;

namespace TT.Web.HubDispatchers
{
    public static class ScopableHubDispatcherExtensions
    {
        public static IAppBuilder MapScopableHubConnection(
            this IAppBuilder app,
            Func<IRequest, string, Func<Task>, Task> next,
            string path = "/signalr",
            IDependencyResolver resolver = null,
            HubConfiguration hubConfig = null,
            ConnectionConfiguration conConfig = null)
        {
            if (resolver == null)
            {
                resolver = GlobalHost.DependencyResolver;
            }
            else
            {
                GlobalHost.DependencyResolver = resolver;
            }

            if (hubConfig == null)
            {
                hubConfig = new HubConfiguration();
            }

            if (conConfig == null)
            {
                conConfig = new ConnectionConfiguration();
            }

            hubConfig.Resolver = resolver;
            conConfig.Resolver = resolver;

            Func<ScopableHubDispatcher> ScopableHubDispatcherFactory = () => new ScopableHubDispatcher(next, hubConfig);
            resolver.Register(typeof(ScopableHubDispatcher), ScopableHubDispatcherFactory);

            app.MapSignalR<ScopableHubDispatcher>(path, conConfig);

            return app;
        }
    }
}