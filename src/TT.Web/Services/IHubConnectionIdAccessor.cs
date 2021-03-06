﻿namespace TT.Web.Services
{
    public interface IHubConnectionIdAccessor
    {
        /// <summary>
        /// <para>The current SignalR ConnectionId string.</para>
        /// <para>If null then <see cref="IHubConnectionIdAccessor"/> was resolved outside of a SignalR hub request.</para>
        /// </summary>
        string ConnectionId { get; }
    }
}
