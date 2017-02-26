using System.Linq;
using Highway.Data;
using TT.Domain.Messages.DTOs;
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
                var output = ctx.AsQueryable<Message>()
                            .Where(m => m.Id == MessageId)
                            .ProjectToFirstOrDefault<MessageDetail>();

                if (output.Receiver.Id == OwnerId)
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
