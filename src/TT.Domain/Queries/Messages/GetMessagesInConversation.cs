using System;
using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Messages
{
    public class GetMessagesInConversation : DomainQuery<MessageDetail>
    {

        public Guid? conversationId { get; set; }

        public override IEnumerable<MessageDetail> Execute(IDataContext context)
        {

            ContextQuery = ctx => ctx.AsQueryable<Message>().Where(m => m.ConversationId == conversationId).ProjectToQueryable<MessageDetail>().OrderByDescending(m => m.Timestamp);
            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (conversationId == null)
                throw new DomainException("ConversationId cannot be null");
        }
    }
}
