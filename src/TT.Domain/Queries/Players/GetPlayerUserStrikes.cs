using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Players;

namespace TT.Domain.Queries.Players
{
    public class GetPlayerUserStrikes : DomainQuerySingle<PlayerUserStrikesDetail>
    {
        public string UserId { get; set; }

        public override PlayerUserStrikesDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Entities.Players.Player>()
                            .Where(p => p.User.Id == UserId)
                            .ProjectToFirstOrDefault<PlayerUserStrikesDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
