using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Queries
{
    public class GetMessagesInConversation : DomainQuery<MessageDetail>
    {

        public Guid? conversationId { get; set; }

        public override IEnumerable<MessageDetail> Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var messages = ctx.AsQueryable<Message>()
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .Where(m => m.ConversationId == conversationId &&
                                m.IsDeleted == false)
                    .OrderByDescending(m => m.Timestamp)
                    .ToList();

                return messages.Select(m => m.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (conversationId == null)
                throw new DomainException("ConversationId cannot be null");
        }
    }
}
