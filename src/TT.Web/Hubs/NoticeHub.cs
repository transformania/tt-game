using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using TT.Domain;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Services;
using TT.Web.Services;

namespace TT.Web.Hubs
{
    public class NoticeHub : Hub
    {
        public override Task OnDisconnected()
        {
            DomainRegistry.AttackNotificationBroker.NotificationRaised -= NotificationRaised;

            return base.OnDisconnected();
        }

        private void NotificationRaised(object sender, NotificationRaisedEventArgs args)
        {
            if (DomainRegistry.SentNotifications.Contains(args.NotificationId))
                return;
            
            var player = PlayerProcedures.GetPlayer(args.PlayerId);
            NoticeService.PushAttackNotice(player, args.Message);

            DomainRegistry.SentNotifications.Add(args.NotificationId);
        }

        public Task Connect()
        {
            DomainRegistry.AttackNotificationBroker.NotificationRaised += NotificationRaised;

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