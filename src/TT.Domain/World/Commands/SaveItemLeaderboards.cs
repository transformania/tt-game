using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;
using TT.Domain.World.Entities;

namespace TT.Domain.World.Commands
{
    public class SaveItemLeaderboards : DomainCommand
    {
        public int RoundNumber { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var worldData = ctx.AsQueryable<Entities.World>().FirstOrDefault();

                if (worldData == null)
                    throw new DomainException("No world data found.");

                if (worldData.TurnNumber != worldData.RoundDuration)
                    throw new DomainException($"Unable to save Item/Pet leaderboards at this time.  It is turn {worldData.TurnNumber} and needs to be turn {worldData.RoundDuration}.");

                if (worldData.ChaosMode)
                    throw new DomainException("Unable to save Item/Pet leaderboards at this time.  The game is currently in chaos mode.");

                var existingItemEntries =
                    ctx.AsQueryable<ItemLeaderboardEntry>().Any(l => l.RoundNumber == RoundNumber);

                if (existingItemEntries)
                    throw new DomainException($"There are already existing Item/Pet leaderboard entries for round {RoundNumber}.");


                // save item Leaderboards
                var topItems = ctx.AsQueryable<Item>()
                    .Include(p => p.FormerPlayer)
                    .Include(p => p.FormerPlayer.Covenant)
                    .Include(p => p.FormerPlayer.ItemXP)
                    .Include(p => p.ItemSource)
                    .Where(p => p.FormerPlayer != null &&
                        p.FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
                    .OrderByDescending(i => i.Level)
                    .ThenByDescending(i => i.FormerPlayer.ItemXP.Amount)
                    .Take(100)
                    .ToList();

                for (var i = 0; i < topItems.Count; i++)
                {
                    var item = topItems.ElementAt(i);
                    ctx.Add(ItemLeaderboardEntry.Create(i + 1, item, RoundNumber));
                }

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RoundNumber <= 0)
                throw new DomainException("Round Number must be set!");
        }
    }
}
