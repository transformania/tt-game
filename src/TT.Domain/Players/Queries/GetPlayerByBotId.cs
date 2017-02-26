using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetPlayer : DomainQuerySingle<PlayerDetail>
    {
        public int PlayerId { get; set; }

        public override PlayerDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Player>()
                            .Where(p => p.Id == PlayerId)
                            .ProjectToFirstOrDefault<PlayerDetail>();
            };

            return ExecuteInternal(context);
        }

    }
}
