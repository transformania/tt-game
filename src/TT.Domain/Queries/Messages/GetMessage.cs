
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Messages
{
    public class GetMessage : DomainQuerySingle<MessageDetail>
    {
        public int MessageId { get; set; }

        public override MessageDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Message>()
                            .Where(m => m.Id == MessageId)
                            .ProjectToFirstOrDefault<MessageDetail>();
            };

            return ExecuteInternal(context);
        }

    }
}
