using System.Data.Entity;
using System.Linq;
using Highway.Data;

namespace TT.Domain.Commands.Players
{
    public class ChangeDonatorTier : DomainCommand
    {

        public string UserId { get; set; }
        public int Tier { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>()
                    .Include(p => p.User)
                    .Include(i => i.PlayerLogs)
                    .SingleOrDefault(p => p.User.Id == UserId);

                if (player == null)
                    throw new DomainException($"Player with user ID '{UserId}' could not be found");

                player.SetTier(Tier);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new DomainException("userId is required");

            if (Tier < 0 || Tier > 3)
                throw new DomainException("Tier must be an integer between 0 and 3.");

        }

    }
}
