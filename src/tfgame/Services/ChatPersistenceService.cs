using System.Collections.Generic;
using System.Linq;
using TT.Domain.Models;

namespace tfgame.Services
{
    public class ChatPersistenceService : IChatPersistanceService
    {
        private static readonly object SyncRoot = new object();

        protected static IDictionary<string, ChatUser> Persistence { get; private set; }

        static ChatPersistenceService()
        {
            Persistence = new Dictionary<string, ChatUser>();
        }

        public void TrackConnection(Player_VM player, string connectionId)
        {
            lock (SyncRoot)
            {
                ChatUser user;

                if (Persistence.ContainsKey(player.MembershipId))
                {
                    user = Persistence[player.MembershipId];
                }
                else
                {
                    user = new ChatUser(player.MembershipId, player.GetDescriptor().Item1, player.DonatorLevel > 0);
                    Persistence.Add(player.MembershipId, user);
                }

                user.ConnectedWith(connectionId);
            }
        }

        public void TrackDisconnect(string membershipId, string connectionId)
        {
            if (!Persistence.ContainsKey(membershipId))
                return;

            lock (SyncRoot)
            {
                var user = Persistence[membershipId];
                user.DisconnectedWith(connectionId);

                if (user.ConnectionCount <= 0)
                    Persistence.Remove(membershipId);
            }
        }

        public void TrackRoomJoin(string membershipId, string connectionId, string room)
        {
            if (!Persistence.ContainsKey(membershipId))
                return;

            var user = Persistence[membershipId];
            user.JoinedRoom(room, connectionId);
        }

        public void TrackMessageSend(string membershipId, string connectionId)
        {
            if (!Persistence.ContainsKey(membershipId))
                return;

            var user = Persistence[membershipId];
            user.ActiveOn(connectionId);
        }

        public void TrackPlayerNameChange(string membershipId, string newPlayerName)
        {
            if (!Persistence.ContainsKey(membershipId))
                return;

            Persistence[membershipId].ChangedNameTo(newPlayerName);
        }

        public bool HasNameChanged(string membershipId, string name)
        {
            if (!Persistence.ContainsKey(membershipId))
                return false;

            return Persistence[membershipId].Name != name;
        }

        public string GetRoom(string membershipId, string connectionId)
        {
            if(!Persistence.ContainsKey(membershipId) || !Persistence[membershipId].Connections.Any(x => x.ConnectionId == connectionId))
                return string.Empty;


            var result = Persistence[membershipId].Connections.Single(x => x.ConnectionId == connectionId).Room;

            return result ?? string.Empty;
        }

        public IEnumerable<string> GetRoomsPlayerIsIn(string membershipId)
        {
            return !Persistence.ContainsKey(membershipId) ? new List<string>() : Persistence[membershipId].InRooms ?? new List<string>();
        }

        public IEnumerable<ChatUser> GetUsersInRoom(string room)
        {
            return Persistence.Where(x => x.Value.InRooms.Contains(room)).Select(x => x.Value);
        }
    }
}