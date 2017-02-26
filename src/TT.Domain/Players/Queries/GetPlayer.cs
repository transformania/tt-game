using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetPlayerByBotId : DomainQuerySingle<PlayerDetail>
    {
        public int BotId { get; set; }

        public override PlayerDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Player>()
                            .Where(p => p.BotId == BotId)
                            .ProjectToFirstOrDefault<PlayerDetail>();
            };

            return ExecuteInternal(context);
        }

    }
}
