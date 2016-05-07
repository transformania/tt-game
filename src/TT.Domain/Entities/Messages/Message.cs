using System;
using TT.Domain.Commands.Messages;
using TT.Domain.Entities.Players;

namespace TT.Domain.Entities.Messages
{
    public class Message : Entity<int>
    {

        public Player Sender { get; protected set; }
        public Player Receiver { get; protected set; }

        public bool IsRead { get; protected set; }
        public int ReadStatus { get; protected set; }

        public string MessageText { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        public bool DoNotRecycleMe { get; protected set; }
        public int ConversationId { get; protected set; }

        private Message() { }

        public static Message Create(Player sender, Player receiver, CreateMessage cmd)
        {
            return new Message
            {
                Sender = sender,
                Receiver = receiver,
                IsRead = cmd.IsRead,
                ReadStatus = cmd.ReadStatus,
                Timestamp = DateTime.UtcNow,
                MessageText = cmd.Text,
                DoNotRecycleMe = cmd.DoNotRecycleMe,
                
                ConversationId = cmd.ConversationId,
                
            };
        }

    }

    
}
