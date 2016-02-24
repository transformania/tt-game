using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using TT.Domain.Models;
using TT.Domain.Procedures;

namespace TT.Web.Hubs
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