using Microsoft.AspNet.SignalR;

namespace TT.Web.Services
{
    public interface IHubRequestAccessor
    {
        /// <summary>
        /// <para>The current SignalR <see cref="IRequest"/>.</para>
        /// <para>If null then <see cref="IHubRequestAccessor"/> was resolved outside of a SignalR hub request.</para>
        /// </summary>
        IRequest Request { get; }
    }
}
