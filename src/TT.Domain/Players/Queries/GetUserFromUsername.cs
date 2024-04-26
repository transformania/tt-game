using System.Linq;
using Highway.Data;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetUserFromUsername : DomainQuerySingle<User>
    {
        public string Username { get; set; }

        public override User Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<User>()
                    .SingleOrDefault(p => p.UserName == Username);
            };

            return ExecuteInternal(context);
        }
    }
}
