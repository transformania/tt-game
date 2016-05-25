using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Players;
using TT.Domain.Entities.Players;

namespace TT.Domain.Queries.Players
{
    public class GetPlayer : DomainQuerySingle<PlayerDetail>
    {
        public int PlayerId { get; set; }

        public override PlayerDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Entities.Players.Player>()
                            .Where(p => p.Id == PlayerId)
                            .ProjectToFirstOrDefault<PlayerDetail>();
            };

            return ExecuteInternal(context);
        }

    }
}
