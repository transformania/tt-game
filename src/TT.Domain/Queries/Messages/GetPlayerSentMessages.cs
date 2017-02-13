using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Messages
{
    public class GetPlayerSentMessages : DomainQuery<MessageDetail>
    {

        public int SenderId { get; set; }
        public int Take { get; set; }

        public override IEnumerable<MessageDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                    ctx.AsQueryable<Message>()
                        .Where(m => m.Sender.Id == SenderId
                            && m.IsDeleted == false)
                        .OrderByDescending(m => m.Timestamp)
                        .Take(Take)
                        .ProjectToQueryable<MessageDetail>();
                        
            return ExecuteInternal(context);
        }
    }
}
