namespace TT.Web.Services
{
    public class HubConnectionIdAccessor : IHubConnectionIdAccessor
    {
        public string ConnectionId { get; set; }

        string IHubConnectionIdAccessor.ConnectionId => ConnectionId;
    }
}