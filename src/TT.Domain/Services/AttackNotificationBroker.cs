using System;
using System.Linq;

namespace TT.Domain.Services
{
    public interface IAttackNotificationBroker
    {
        event EventHandler<NotificationRaisedEventArgs> NotificationRaised;
        void Notify(int playerId, string message);
    }

    public class AttackNotificationBroker : IAttackNotificationBroker
    {
        private EventHandler<NotificationRaisedEventArgs> notificationRaised;
        public event EventHandler<NotificationRaisedEventArgs> NotificationRaised
        {
            add
            {
                if (notificationRaised == null || !notificationRaised.GetInvocationList().Contains(value))
                    notificationRaised += value;
            }
            remove
            {
                notificationRaised -= value;
            }
        }

        public void Notify(int playerId, string message)
        {
            if (playerId <= 0)
                throw new ArgumentException("Cannot raise attack notification. ", "playerId");

            if(string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Cannot raise attack notification. ", "message");

            OnNotificationRaised(new NotificationRaisedEventArgs(playerId, message));
        }

        private void OnNotificationRaised(NotificationRaisedEventArgs e)
        {
            var handler = notificationRaised;
            if (handler != null) handler(this, e);
        }
    }
}