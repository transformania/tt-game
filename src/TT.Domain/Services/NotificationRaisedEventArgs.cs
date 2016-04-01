using System;

namespace TT.Domain.Services
{
    public class NotificationRaisedEventArgs : EventArgs
    {
        public int PlayerId { get; private set; }
        public string Message { get; private set; }

        public NotificationRaisedEventArgs(int playerId, string message)
        {
            PlayerId = playerId;
            Message = message;
        }
    }
}