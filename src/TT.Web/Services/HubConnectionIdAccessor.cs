using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TT.Web.Services
{
    public class HubConnectionIdAccessor : IHubConnectionIdAccessor
    {
        public string ConnectionId { get; set; }

        string IHubConnectionIdAccessor.ConnectionId => ConnectionId;
    }
}