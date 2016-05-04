using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Commands.Messages
{
    public class DeleteMessage : DomainCommand
    {
        public int MessageId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<Message>().FirstOrDefault(cr => cr.Id == MessageId);

                if (deleteMe == null)
                    throw new DomainException(string.Format("Message with ID {0} was not found", MessageId));

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
