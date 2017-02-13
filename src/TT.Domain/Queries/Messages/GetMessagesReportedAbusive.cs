using System;
using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Messages
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
