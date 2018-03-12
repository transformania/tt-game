using System;
using TT.Domain.Players.DTOs;

namespace TT.Domain.Messages.DTOs
{
    public class MessageDetail
    {
        public int MessageId { get; set; }
        public PlayerMessageDetail Sender { get; set; }
        public PlayerMessageDetail Receiver { get; set; }

        public int ReadStatus { get; set; }

        public string MessageText { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid? ConversationId { get; set; }
    }
}
