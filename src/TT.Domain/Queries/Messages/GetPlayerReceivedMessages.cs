using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Messages
{
    public class GetPlayerReceivedMessages : DomainQuery<MessageDetail>
    {

        public int ReceiverId { get; set; }

        public override IEnumerable<MessageDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Message>().Where(m => m.Receiver.Id == ReceiverId).ProjectToQueryable<MessageDetail>().OrderByDescending(m => m.Timestamp);
            return ExecuteInternal(context);
        }
    }
}
