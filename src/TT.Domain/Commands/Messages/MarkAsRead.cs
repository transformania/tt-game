using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Messages;
using TT.Domain.Statics;

namespace TT.Domain.Commands.Messages
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
                    throw new DomainException(string.Format("Message with ID {0} could not be found", MessageId));

                if (message.Receiver.Id != OwnerId)
                    throw new DomainException(string.Format("Message {0} not owned by player {1}", MessageId, OwnerId));


                message.MarkAsRead(ReadStatus);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (ReadStatus < 0 || ReadStatus > 2)
                throw new DomainException(string.Format("{0} is not a valid read status.", ReadStatus));
        }

    }
}
