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
                    throw new DomainException(string.Format("Message with ID {0} was not found", MessageId));

                if (deleteMe.Receiver.Id != OwnerId)
                    throw new DomainException(string.Format("Message {0} not owned by player {1}", MessageId, OwnerId));

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
