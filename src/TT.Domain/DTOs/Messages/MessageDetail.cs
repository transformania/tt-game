using System;
using TT.Domain.DTOs.Players;

namespace TT.Domain.DTOs.Messages
{
    public class MessageDetail
    {
        public int Id { get; set; }
        public PlayerMessageDetail Sender { get; set; }
        public PlayerMessageDetail Receiver { get; set; }

        public int ReadStatus { get; set; }

        public string MessageText { get; set; }

        public DateTime Timestamp { get; set; }

        public Guid? ConversationId { get; set; }
    }
}
