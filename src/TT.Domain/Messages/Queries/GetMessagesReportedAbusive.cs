using System.Collections.Generic;
using System.Data.Entity;
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
            ContextQuery = ctx =>
            {
                var messages = ctx.AsQueryable<Message>()
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.IsReportedAbusive == true)
                    .OrderByDescending(m => m.Timestamp)
                    .ToList();

                return messages.Select(m => m.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }

    }
}
