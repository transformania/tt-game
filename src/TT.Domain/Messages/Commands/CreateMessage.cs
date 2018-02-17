using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Messages.Commands
{
    public class CreateMessage : DomainCommand<int>
    {
        public string Text { get; set; }
        public int ReceiverId { get; set; }
        public int SenderId { get; set; }
        public MessageDetail ReplyingToThisMessage { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var sender = ctx.AsQueryable<Player>().SingleOrDefault(t => t.Id == SenderId);
                if (sender == null)
                    throw new DomainException($"Sending player with Id {SenderId} could not be found");

                var receiver = ctx.AsQueryable<Player>().SingleOrDefault(t => t.Id == ReceiverId);
                if (receiver == null)
                    throw new DomainException($"Receiving player with Id {ReceiverId} could not be found");

                if (receiver.BotId != AIStatics.ActivePlayerBotId)
                    throw new DomainException("You can't message NPCs.");

                var message = Message.Create(sender, receiver, this);

                ctx.Add(message);
                ctx.Commit();

                result = message.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new DomainException("Text must not be empty or null");
        }
    }
}
