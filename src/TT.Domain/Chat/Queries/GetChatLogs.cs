using System;
using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Chat.DTOs;
using TT.Domain.Chat.Entities;

namespace TT.Domain.Chat.Queries
{
    public class GetChatLogs : DomainQuery<ChatLogDetail>
    {

        public string Room { get; set; }
        public string Filter { get; set; }
        public override IEnumerable<ChatLogDetail> Execute(IDataContext context)
        {

            DateTime cutoff;

            if (Filter == "lasth")
            {
                cutoff = DateTime.UtcNow.AddHours(-1);
            }
            else if (Filter == "last4h")
            {
                cutoff = DateTime.UtcNow.AddHours(-4);
            }
            else if (Filter == "last12h")
            {
                cutoff = DateTime.UtcNow.AddHours(-12);
            }
            else if (Filter == "last24h")
            {
                cutoff = DateTime.UtcNow.AddDays(-1);
            }
            else if (Filter == "last48h")
            {
                cutoff = DateTime.UtcNow.AddDays(-2);
            }
            else if (Filter == "last72h")
            {
                cutoff = DateTime.UtcNow.AddDays(-3);
            }
            else
            {
                cutoff = DateTime.UtcNow.AddHours(-1);
            }

            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<ChatLog>().Where(c => c.Room == Room && c.Timestamp >= cutoff).OrderBy(c => c.Timestamp).ProjectToQueryable<ChatLogDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}