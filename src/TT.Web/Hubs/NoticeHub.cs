using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using TT.Domain;
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

            var me = PlayerProcedures.GetPlayerFromMembership(Context.User.Identity.GetUserId());
            return Groups.Add(Context.ConnectionId, GetPlayerRoomId(me.Id));
        }

        public void ReceiveNotice(int playerId, string message)
        {
            Clients.Group(GetPlayerRoomId(playerId)).receiveNotice(message);
        }

        private string GetPlayerRoomId(int playerId)
        {
            return $"_{playerId}";
        }

        

    }
}