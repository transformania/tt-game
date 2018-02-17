using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TT.Web.Services
{
    public interface IHubContextAccessor<THub>
        where THub : IHub
    {
        IHubContext HubContext { get; }
    }
}
