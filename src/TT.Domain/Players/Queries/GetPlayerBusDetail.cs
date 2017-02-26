using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetPlayerBusDetail : DomainQuerySingle<PlayerBusDetail>
    {
        public int playerId { get; set; }

        public override PlayerBusDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Player>()
                            .Where(p => p.Id == playerId)
                            .ProjectToFirstOrDefault<PlayerBusDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
