using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Commands.Messages
{
    public class DeleteMessage : DomainCommand
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<Message>()
                .Include(i => i.Receiver)
                .FirstOrDefault(cr => cr.Id == MessageId);

                if (deleteMe == null)
                    throw new DomainException($"Message with ID {MessageId} was not found");

                if (OwnerId == 0)
                    throw new DomainException("OwnerId is required to delete a message");

                if (deleteMe.Receiver.Id != OwnerId)
                    throw new DomainException($"Message {MessageId} not owned by player {OwnerId}");

                ctx.Remove(deleteMe);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (MessageId <= 0)
                throw new DomainException("Message Id must be greater than 0");
        }
    }
}
