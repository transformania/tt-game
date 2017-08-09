using Microsoft.AspNet.SignalR;

namespace TT.Web.Services
{
    public class HubRequestAccessor : IHubRequestAccessor
    {
        public IRequest Request { get; set; }

        IRequest IHubRequestAccessor.Request => Request;
    }
}