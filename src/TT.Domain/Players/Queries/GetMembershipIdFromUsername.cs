using System.Linq;
using Highway.Data;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetMembershipIdFromUsername : DomainQuerySingle<string>
    {
        public string Username { get; set; }

        public override string Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>()
                    .SingleOrDefault(p => p.UserName == Username);

                if (user == null)
                {
                    return$"No Account found for '{Username}'";
                }

                return user.Id;
            };

            return ExecuteInternal(context);
        }
    }
}
