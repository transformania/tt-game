﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TT.Domain.Models
{
    public class ChatUser
    {
        private readonly IList<ChatConnection> connections;
        public IEnumerable<ChatConnection> Connections { get { return connections; } }
        public int ConnectionCount { get { return connections.Count; } }

        public string MembershipId { get; private set; }
        public string Name { get; private set; }
        public bool IsDonator { get; private set; }

        public IEnumerable<string> InRooms { get { return connections.Where(x => x.Room != null).Select(x => x.Room).Distinct(); } }

        public ChatUser(string membershipId, string name, bool isDonator = false)
        {
            MembershipId = membershipId;
            Name = name;
            IsDonator = isDonator;

            connections = new List<ChatConnection>();
        }

        public void ConnectedWith(string connectionId)
        {
            if (connections.All(con => con.ConnectionId != connectionId))
                connections.Add(new ChatConnection(connectionId));
        }

        public void DisconnectedWith(string connectionId)
        {
            var connection = connections.SingleOrDefault(con => con.ConnectionId == connectionId);
            if (connection != null)
                connections.Remove(connection);
        }

        public void JoinedRoom(string room, string connectionId)
        {
            var connection = connections.SingleOrDefault(con => con.ConnectionId == connectionId);
            if (connection != null)
                connection.ForRoom(room);
        }

        public void ActiveOn(string connectionId)
        {
            var connection = connections.SingleOrDefault(con => con.ConnectionId == connectionId);
            if (connection != null)
                connection.RecordActivity();
        }

        public void ChangedNameTo(string name)
        {
            Name = name;
        }

        public class ChatConnection
        {
            public string ConnectionId { get; private set; }
            public string Room { get; private set; }
            public DateTime LastActivity { get; private set; }

            public ChatConnection(string connectionId)
            {
                ConnectionId = connectionId;
            }

            public void ForRoom(string room)
            {
                Room = room;
                RecordActivity();
            }

            public void RecordActivity()
            {
                LastActivity = DateTime.UtcNow;
            }
        }
    }
}