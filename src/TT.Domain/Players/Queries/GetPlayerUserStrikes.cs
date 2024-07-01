using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;
using TT.Domain.Players.Mappings;

namespace TT.Domain.Players.Queries
{
    public class GetPlayerUserStrikes : DomainQuerySingle<PlayerUserStrikesDetail>
    {
        public string UserId { get; set; }

        public override PlayerUserStrikesDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var player = ctx
                    .AsQueryable<Player>()
                    .Include(p => p.User.Strikes.Select(s => s.User))
                    .Include(p => p.User.Strikes.Select(s => s.FromModerator))
                    .FirstOrDefault(p => p.User.Id == UserId);

                return player.MapToPlayerStrikesDto();
            };

            return ExecuteInternal(context);
        }
    }
}
