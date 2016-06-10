
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Messages;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Queries.Messages
{
    public class GetMessage : DomainQuerySingle<MessageDetail>
    {
        public int MessageId { get; set; }
        public int OwnerId { get; set; }

        public override MessageDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var output = ctx.AsQueryable<Message>()
                            .Where(m => m.Id == MessageId)
                            .ProjectToFirstOrDefault<MessageDetail>();

                if (output.Receiver.Id != OwnerId)
                    throw new DomainException(string.Format("Player {0} does not own message {1}", OwnerId, MessageId));

                return output;
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
