using System;
using System.Collections.Generic;
using tfgame.dbModels;
using tfgame.dbModels.Commands.Player;
using tfgame.dbModels.Models;
using tfgame.Statics;

namespace tfgame.Services
{
    public class ChatService
    {
        private const int OnlineActivityCutoffMinutes = -2;

        public static IDictionary<int, ChatUser> ChatPersistance { get; private set; }

        static ChatService()
        {
            ChatPersistance = new Dictionary<int, ChatUser>();
        }
        
        public void MarkOnlineActivityTimestamp(Player_VM player)
        {
            var markOnlineCutoff = DateTime.UtcNow.AddMinutes(OnlineActivityCutoffMinutes);

            // update the player's "last online" attribute if it's been long enough
            if (player.OnlineActivityTimestamp >= markOnlineCutoff || PvPStatics.AnimateUpdateInProgress) 
                return;

            var cmd = new MarkOnlineActivityTimestamp {Player = player};
            DomainRegistry.Root.Execute(cmd);
        }

        public Tuple<string, string> GetPlayerDescriptorFor(Player_VM player)
        {
            var name = string.Empty;
            var pic = string.Empty;

            if (player.MembershipId == -1)
                return new Tuple<string, string>(name, pic);

            if (ChatStatics.Staff.ContainsKey(player.MembershipId))
            {
                var descriptor = ChatStatics.Staff[player.MembershipId];
                name = descriptor.Item1;
                pic = descriptor.Item2;

                return new Tuple<string, string>(name, pic);
            }

            name = player.GetFullName();

            return new Tuple<string, string>(name, pic);
        }

        public void OnUserConnected(Player_VM player, string connectionId)
        {
            ChatUser user;
            
            if (ChatPersistance.ContainsKey(player.MembershipId))
            {
                user = ChatPersistance[player.MembershipId];
            }
            else
            {
                user = new ChatUser(player.MembershipId, GetPlayerDescriptorFor(player).Item1, player.DonatorLevel > 0);
                ChatPersistance.Add(player.MembershipId, user);
            }

            user.ConnectedWith(connectionId);
        }

        public void OnUserDisconnected(Player_VM player, string connectionId)
        {
            if (!ChatPersistance.ContainsKey(player.MembershipId))
                return;

            var user = ChatPersistance[player.MembershipId];
            user.DisconnectedWith(connectionId);

            if (user.ConnectionCount <= 0)
                ChatPersistance.Remove(player.MembershipId);
        }

        public void OnUserJoinRoom(Player_VM player, string connectionId, string room)
        {
            if (!ChatPersistance.ContainsKey(player.MembershipId))
                return;

            var user = ChatPersistance[player.MembershipId];
            user.JoinedRoom(room, connectionId);
        }

        public void OnUserSentMessage(Player_VM player, string connectionId)
        {
            if (!ChatPersistance.ContainsKey(player.MembershipId))
                return;

            var user = ChatPersistance[player.MembershipId];
            user.ActiveOn(connectionId);
        }
    }
}