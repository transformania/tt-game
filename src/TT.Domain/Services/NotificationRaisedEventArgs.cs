using System;

namespace TT.Domain.Services
{
    public class NotificationRaisedEventArgs : EventArgs
    {
        public Guid NotificationId { get; private set; }
        public int PlayerId { get; private set; }
        public string Message { get; private set; }

        public NotificationRaisedEventArgs(int playerId, string message)
        {
            NotificationId = Guid.NewGuid();
            PlayerId = playerId;
            Message = message;
        }
    }
}