using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Queries
{
    public class GetPlayerSentMessages : DomainQuery<MessageDetail>
    {

        public int SenderId { get; set; }
        public int Take { get; set; }

        public override IEnumerable<MessageDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                {
                    var messages = ctx.AsQueryable<Message>()
                        .Include(m => m.Sender)
                        .Include(m => m.Receiver)
                        .Where(m => m.Sender.Id == SenderId
                                    && m.IsDeleted == false)
                        .OrderByDescending(m => m.Timestamp)
                        .Take(Take)
                        .ToList();

                    return messages.Select(m => m.MapToDto()).AsQueryable();
                };
                        
            return ExecuteInternal(context);
        }
    }
}
