using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetPlayerByUserId : DomainQuerySingle<PlayerDetail>
    {
        public string UserId { get; set; }

        public override PlayerDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var player = ctx
                    .AsQueryable<Player>()
                    .Include(p => p.User)
                    .Include(p => p.ItemXP)
                    .FirstOrDefault(p => p.User.Id == UserId);

                return player?.MapToDto();
            };

            return ExecuteInternal(context);
        }

    }
}
