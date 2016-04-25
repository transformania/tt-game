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
                return ctx.AsQueryable<Entities.Players.Player>().Select(p => new PlayerDetail
                {
                    Id = p.Id,
                    User = p.User,
                    NPC = p.NPC
                   
                }).FirstOrDefault(p => p.Id == PlayerId);
            };

            return ExecuteInternal(context);
        }
    }
}
