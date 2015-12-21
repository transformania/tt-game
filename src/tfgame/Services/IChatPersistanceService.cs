using System.Collections.Generic;
using tfgame.dbModels.Models;

namespace tfgame.Services
{
    public interface IChatPersistanceService
    {
        void TrackConnection(Player_VM player, string toString);
        void TrackDisconnect(string membershipId, string connectionId);
        void TrackRoomJoin(string membershipId, string connectionId, string room);
        void TrackMessageSend(string membershipId, string connectionId);
        void TrackPlayerNameChange(string membershipId, string newPlayerName);
        bool HasNameChanged(string membershipId, string name);
        IEnumerable<string> GetRoomsPlayerIsIn(string membershipId);
        string GetRoom(string membershipId, string connectionId);
        IEnumerable<ChatUser> GetUsersInRoom(string room);
    }
}