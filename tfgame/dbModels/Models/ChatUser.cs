using System.Collections.Generic;

namespace tfgame.dbModels.Models
{
    public class ChatUser
    {
        private readonly IList<string> connections;
        public IEnumerable<string> Connections { get { return connections; } }
        public int ConnectionCount { get { return connections.Count; } }

        public int MembershipId { get; private set; }
        public string Name { get; private set; }
        
        public ChatUser(int membershipId, string name)
        {
            MembershipId = membershipId;
            Name = name;
            connections = new List<string>();
        }

        public void ConnectedWith(string connectionId)
        {
            if (!connections.Contains(connectionId))
                connections.Add(connectionId);
        }

        public void DisconnectedWith(string connectionId)
        {
            if (connections.Contains(connectionId))
                connections.Remove(connectionId);
        }
    }
}