using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Queries
{
    public class PlayerOwnsMessage : DomainQuerySingle<bool>
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }

        public override bool Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var output = ctx
                    .AsQueryable<Message>()
                    .Include(m => m.Receiver)
                    .FirstOrDefault(m => m.Id == MessageId);

                if (output?.Receiver.Id == OwnerId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            };

            return ExecuteInternal(context);
        }
    }
}
