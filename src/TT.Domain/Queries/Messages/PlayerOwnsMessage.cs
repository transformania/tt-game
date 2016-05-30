using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Messages
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
