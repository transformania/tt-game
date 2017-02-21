using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Players;

namespace TT.Domain.Queries.Players
{
    public class GetPlayerByUserId : DomainQuerySingle<PlayerDetail>
    {
        public string UserId { get; set; }

        public override PlayerDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Entities.Players.Player>()
                            .Where(p => p.User.Id == UserId)
                            .ProjectToFirstOrDefault<PlayerDetail>();
            };

            return ExecuteInternal(context);
        }

    }
}
