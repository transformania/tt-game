using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Messages;

namespace TT.Domain.Commands.Messages
{
    public class DeletePlayerExpiredMessages : DomainCommand
    {
        public int OwnerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var cutoff = DateTime.UtcNow.AddDays(-3);

                var deleteMe = ctx.AsQueryable<Message>()
                    .Where(m => m.Receiver.Id == OwnerId &&
                    m.Timestamp < cutoff &&
                    m.DoNotRecycleMe == false);

                foreach (var m in deleteMe)
                {
                    ctx.Remove(m);
                }

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (OwnerId <= 0)
                throw new DomainException("Owner Id must be greater than 0");
        }
    }
}
