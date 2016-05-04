using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ContextQuery = ctx => ctx.AsQueryable<Message>().ProjectToQueryable<MessageDetail>().Where(m => m.Receiver.Id == ReceiverId);
            return ExecuteInternal(context);
        }
    }
}
