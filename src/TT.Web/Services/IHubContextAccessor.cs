﻿using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Web.Services
{
    public interface IHubContextAccessor<THub>
        where THub : IHub
    {
        IHubContext HubContext { get; }
    }
}