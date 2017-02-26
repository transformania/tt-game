using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Commands
{
    public class MarkAsRead : DomainCommand
    {

        public int MessageId { get; set; }
        public int ReadStatus { get; set; }
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


                message.MarkAsRead(ReadStatus);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (ReadStatus < 0 || ReadStatus > 2)
                throw new DomainException($"{ReadStatus} is not a valid read status.");
        }

    }
}
