using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Queries
{
    public class GetPlayerReceivedMessages : DomainQuery<MessageDetail>
    {

        public int ReceiverId { get; set; }

        public override IEnumerable<MessageDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Message>()
            .Where(m => m.Receiver.Id == ReceiverId 
                && m.IsDeleted == false)
            .ProjectToQueryable<MessageDetail>().OrderByDescending(m => m.Timestamp);
            return ExecuteInternal(context);
        }
    }
}
