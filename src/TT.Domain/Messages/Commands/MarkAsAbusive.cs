using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Commands
{
    public class MarkAsAbusive : DomainCommand
    {

        public int MessageId { get; set; }
        public int OwnerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var message = ctx.AsQueryable<Message>()
                    .Include(i => i.Receiver)
                    .SingleOrDefault(cr => cr.Id == MessageId);

                if (message == null)
                    throw new DomainException($"Message with ID {MessageId} could not be found");

                if (message.Receiver.Id != OwnerId)
                    throw new DomainException($"Message {MessageId} not owned by player {OwnerId}");


                message.MarkAsAbusive(true);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }
}
