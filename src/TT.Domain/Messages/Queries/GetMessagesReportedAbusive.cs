using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Queries
{
    public class GetMessagesReportedAbusive : DomainQuery<MessageDetail>
    {

        public override IEnumerable<MessageDetail> Execute(IDataContext context)
        {

            ContextQuery = ctx => ctx.AsQueryable<Message>()
            .Where(m => m.IsReportedAbusive == true)
            .ProjectToQueryable<MessageDetail>().OrderByDescending(m => m.Timestamp);
            return ExecuteInternal(context);
        }

    }
}
