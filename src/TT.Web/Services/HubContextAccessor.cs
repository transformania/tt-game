using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using SimpleInjector;
using System.Threading;

namespace TT.Web.Services
{
    /// <summary>
    /// <para>Allows a service to access a hub's <see cref="IHubContext"/>.</para>
    /// <para>This class is thread-safe and it is suggested by Microsoft that <see cref="IConnectionManager.GetHubContext{T}"/> is called sparingly.</para>
    /// </summary>
    /// <typeparam name="THub"></typeparam>
    public class HubContextAccessor<THub> : IHubContextAccessor<THub>
        where THub : IHub
    {
        IHubContext IHubContextAccessor<THub>.HubContext => container.IsVerifying ? GlobalHost.ConnectionManager.GetHubContext<THub>() : HubContext.Value;

        private readonly Lazy<IHubContext> HubContext;

        private readonly Container container;

        public HubContextAccessor(Container container)
        {
            this.container = container;
            HubContext = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<THub>());
        }
    }
}