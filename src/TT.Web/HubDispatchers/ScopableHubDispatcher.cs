using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace TT.Web.HubDispatchers
{
    /// <summary>
    /// <para>This class extends <see cref="HubDispatcher"/> to intercept the <see cref="IRequest"/> and connectionId from a hub's method call and passes it to <see cref="next"/>.</para>
    /// <para>The <see cref="next"/> function also receives the original hub method task to perform pipeline-like actions by returning it as a <see cref="Task"/> within the <see cref="next"/> function.</para>
    /// <para>Very simply put, this class takes the base tasks and wraps them using a function similar to an Owin middleware function.</para>
    /// </summary>
    public class ScopableHubDispatcher : HubDispatcher
    {
        private Func<IRequest, string, Func<Task>, Task> next;

        public ScopableHubDispatcher(Func<IRequest, string, Func<Task>, Task> next, HubConfiguration configuration) : base(configuration)
        {
            this.next = next;
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            return next(request, connectionId, () => base.OnReceived(request, connectionId, data));
        }

        protected override Task OnConnected(IRequest request, string connectionId)
        {
            return next(request, connectionId, () => base.OnConnected(request, connectionId));
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled)
        {
            return next(request, connectionId, () => base.OnDisconnected(request, connectionId, stopCalled));
        }

        protected override Task OnReconnected(IRequest request, string connectionId)
        {
            return next(request, connectionId, () => base.OnReconnected(request, connectionId));
        }
    }
}