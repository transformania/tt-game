using Microsoft.AspNet.SignalR;
using TT.Domain.Procedures;
using TT.Domain.Models;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace tfgame.Chat
{
    public class NoticeHub : Hub
    {

        public Task Connect()
        {
            Player me = PlayerProcedures.GetPlayerFromMembership(Context.User.Identity.GetUserId());

            int minimumDonatorLevelForNotifications = 0;

            if (me.DonatorLevel >= minimumDonatorLevelForNotifications)
            {
                string room = "_" + me.Id;
                return Groups.Add(Context.ConnectionId, room);
            }

            // do nothing...
            return Task.FromResult(0);
           
        }

        public void ReceiveNotice(int playerId, string message)
        {
            string room = "_" + playerId;
            Clients.Group(room).receiveNotice(message);
        }

        

    }
}