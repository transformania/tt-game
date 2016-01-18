using System.Web.Script.Serialization;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class NoticeProcedures
    {
        public const string PushType__PlayerLog = "log";
        public const string PushType__PlayerMessage = "message";
        public const string PushType__Attack = "attack";

        public static void PushNotice(Player player, string message, string type)
        {
            PushNotice(player.Id, message, type);
        }

        public static void PushNotice(int playerId, string message, string type)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<tfgame.Chat.NoticeHub>();
            string group = "_" + playerId;

            // message type
            if (type == PushType__PlayerLog)
            {
                var obj = new 
                {
                    jMessage = "NOTICE:  " + message,
                    
                };
                var json = new JavaScriptSerializer().Serialize(obj);
                context.Clients.Group(group).receiveNotice(json);
            }
            else if (type == PushType__PlayerMessage)
            {
                var obj = new
                {
                    jMessage = message,
                    type = PushType__PlayerMessage,
                };
                var json = new JavaScriptSerializer().Serialize(obj);
                context.Clients.Group(group).receiveNotice(json);
            }
            else if (type == PushType__Attack)
            {
                var obj = new
                {
                    jMessage = "YOU HAVE BEEN ATTACKED:  " + message,
                };
                var json = new JavaScriptSerializer().Serialize(obj);
                context.Clients.Group(group).receiveNotice(json);
            }

           
        }

        public static void PushAttackNotice(Player player, string message)
        {
            var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<tfgame.Chat.NoticeHub>();
            string group = "_" + player.Id;
            var obj = new
            {
                type = PushType__Attack,
                jMessage = message,
                wp = player.Health,
                maxwp = player.MaxHealth,
                mana = player.Mana,
                maxmana = player.MaxMana,
            };
            var json = new JavaScriptSerializer().Serialize(obj);
            context.Clients.Group(group).receiveNotice(json);

        }



    }
}