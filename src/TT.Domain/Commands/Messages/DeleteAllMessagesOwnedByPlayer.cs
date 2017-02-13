using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Commands.Messages
{
    public class DeleteAllMessagesOwnedByPlayer : DomainCommand
    {
        public int OwnerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var deleteMe = ctx.AsQueryable<Message>().Where(cr => cr.Receiver.Id == OwnerId);

                foreach (var m in deleteMe)
                {
                    m.MarkAsDeleted(true);
                }
                
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (OwnerId <= 0)
                throw new DomainException("Owner Id must be greater than 0");
        }
    }
}
