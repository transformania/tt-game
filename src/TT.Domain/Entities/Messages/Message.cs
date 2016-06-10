using System;
using TT.Domain.Commands.Messages;
using TT.Domain.Entities.Players;
using TT.Domain.Statics;

namespace TT.Domain.Entities.Messages
{
    public class Message : Entity<int>
    {

        public Player Sender { get; protected set; }
        public Player Receiver { get; protected set; }

        public int ReadStatus { get; protected set; }

        public string MessageText { get; protected set; }

        public DateTime Timestamp { get; protected set; }

        public bool DoNotRecycleMe { get; protected set; }
        public Guid? ConversationId { get; protected set; }

        public bool ReceiverMarkedAsDeleted { get; protected set; } // TODO:  Remove from Entity and table

        private Message() { }

        public static Message Create(Player sender, Player receiver, CreateMessage cmd)
        {
            var output = new Message
            {
                Sender = sender,
                Receiver = receiver,
                ReadStatus = MessageStatics.Unread,
                Timestamp = DateTime.UtcNow,
                MessageText = cmd.Text,
                ReceiverMarkedAsDeleted = false,
                ConversationId = Guid.NewGuid()

            };

            if (sender.DonatorLevel == 3 || receiver.DonatorLevel == 3)
            {
                output.DoNotRecycleMe = true;
            }

            if (cmd.ReplyingToThisMessage != null)
            {
                output.ConversationId = cmd.ReplyingToThisMessage.ConversationId;
            }

            return output;
        }

        public void MarkAsRead(int isRead)
        {
            this.ReadStatus = isRead;
        }

    }

    
}
