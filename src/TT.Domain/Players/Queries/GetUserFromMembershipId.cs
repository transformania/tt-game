using System.Linq;
using Highway.Data;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetUserFromMembershipId : DomainQuerySingle<User>
    {
        public string UserId { get; set; }

        public override User Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<User>()
                    .SingleOrDefault(p => p.Id == UserId);
            };

            return ExecuteInternal(context);
        }
    }
}
