using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highway.Data;
using TT.Domain.DTOs.Players;
using TT.Domain.Statics;

namespace TT.Domain.Queries.Players
{
    public class GetPlayerBusDetail : DomainQuerySingle<PlayerBusDetail>
    {
        public int playerId { get; set; }

        public override PlayerBusDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Entities.Players.Player>()
                            .Where(p => p.Id == playerId)
                            .ProjectToFirstOrDefault<PlayerBusDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
