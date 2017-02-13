using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Messages;
using TT.Domain.Statics;

namespace TT.Domain.Queries.Messages
{
    public class GetUnreadMessageCountByPlayer : DomainQuerySingle<int>
    {
        public int OwnerId { get; set; }

        public override int Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var count = ctx.AsQueryable<Message>()
                    .Count(m => m.Receiver.Id == OwnerId &&
                                m.ReadStatus == MessageStatics.Unread &&
                                m.IsDeleted == false
                                );

                return count;
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (OwnerId <= 0)
                throw new DomainException("OwnerId must be greater than 0");
        }
    }
}
