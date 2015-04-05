using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class NoticeProcedures
    {


        public static void PushNotice(Player player, string message)
        {
            PushNotice(player.Id, message);
        }

        public static void PushNotice(int playerId, string message)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<tfgame.Chat.NoticeHub>();
            context.Clients.Group("_" + playerId).receiveNotice(message);
        }

    }
}