using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetCurrentCarryWeight : DomainQuerySingle<int>
    {
        public int PlayerId { get; set; }

        public override int Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Items.Select(i => i.ItemSource))
                    .FirstOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with PlayerId '{PlayerId}' not found");

                return player.GetCurrentCarryWeight();

            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("Player ID is required!");

        }
    }
}
