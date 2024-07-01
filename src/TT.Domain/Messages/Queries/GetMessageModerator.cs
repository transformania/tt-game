using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Messages.DTOs;
using TT.Domain.Messages.Entities;

namespace TT.Domain.Messages.Queries
{
    public class GetMessageModerator : DomainQuerySingle<MessageDetail>
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }

        public override MessageDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var output = ctx
                    .AsQueryable<Message>()
                    .Include(m => m.Sender)
                    .Include(m => m.Receiver)
                    .FirstOrDefault(m => m.Id == MessageId);

                if (output?.Receiver.Id != OwnerId)
                    throw new DomainException($"Player {OwnerId} does not own message {MessageId}");

                return output.MapToDto();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (MessageId <= 0)
                throw new DomainException("MessageId must be greater than 0");

            if (OwnerId <= 0)
                throw new DomainException("OwnerId must be greater than 0");
        }
    }
}
