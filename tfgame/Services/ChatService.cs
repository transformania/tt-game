using System;
using tfgame.dbModels;
using tfgame.dbModels.Commands.Player;
using tfgame.dbModels.Models;
using tfgame.Statics;

namespace tfgame.Services
{
    public class ChatService
    {
        private const int OnlineActivityCutoffMinutes = -2;
        
        public void MarkOnlineActivityTimestamp(Player_VM player)
        {
            var markOnlineCutoff = DateTime.UtcNow.AddMinutes(OnlineActivityCutoffMinutes);

            // update the player's "last online" attribute if it's been long enough
            if (player.OnlineActivityTimestamp >= markOnlineCutoff || PvPStatics.AnimateUpdateInProgress) 
                return;

            var cmd = new MarkOnlineActivityTimestamp {Player = player};
            DomainRegistry.Root.Execute(cmd);
        }
    }
}