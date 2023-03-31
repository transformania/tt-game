using System;
using TT.Domain.Entities;
using TT.Domain.Messages.Commands;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Messages.Entities
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

        public bool IsRead { get; protected set; } // Legacy support.

        public bool ReceiverMarkedAsDeleted { get; protected set; } // TODO:  Remove from Entity and table

        public bool IsDeleted { get; protected set; }
        public bool IsReportedAbusive { get; protected set; }

        private Message() { }

        public static Message Create(Player sender, Player receiver, bool doNotRecycle, CreateMessage cmd)
        {
            var output = new Message
            {
                Sender = sender,
                Receiver = receiver,
                ReadStatus = MessageStatics.Unread,
                Timestamp = DateTime.UtcNow,
                MessageText = cmd.Text,
                ReceiverMarkedAsDeleted = false,
                ConversationId = Guid.NewGuid(),
                IsRead = false,
                DoNotRecycleMe = doNotRecycle

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

        public void MarkAsDeleted(bool isDeleted)
        {
            this.IsDeleted = isDeleted;
        }

        public void MarkAsAbusive(bool isReportedAbusive)
        {
            this.IsReportedAbusive = isReportedAbusive;
            this.DoNotRecycleMe = isReportedAbusive;
        }

        public void MarkAsPartOfAbusiveConversation()
        {
            this.DoNotRecycleMe = true;
        }

    }

    
}
