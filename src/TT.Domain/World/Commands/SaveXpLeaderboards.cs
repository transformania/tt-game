using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Statics;
using TT.Domain.World.Entities;

namespace TT.Domain.World.Commands
{
    public class SaveXpLeaderboards : DomainCommand
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
                    throw new DomainException($"Unable to save XP leaderboards at this time.  It is turn {worldData.TurnNumber} and needs to be turn {worldData.RoundDuration}.");

                if (worldData.ChaosMode)
                    throw new DomainException("Unable to save XP leaderboards at this time.  The game is currently in chaos mode.");

                var existingXPEntries =
                    ctx.AsQueryable<XpLeaderboardEntry>().Any(l => l.RoundNumber == RoundNumber);

                if (existingXPEntries)
                    throw new DomainException($"There are already existing XP leaderboard entries for round {RoundNumber}.");

                // save XP Leaderboards
                var xpPlayers = ctx.AsQueryable<Players.Entities.Player>()
                    .Include(p => p.FormSource)
                    .Include(p => p.Covenant)
                    .Where(p => p.BotId == AIStatics.ActivePlayerBotId)
                    .OrderByDescending(p => p.Level)
                    .ThenByDescending(p => p.XP)
                    .Take(100)
                    .ToList();

                for (var i = 0; i < xpPlayers.Count; i++)
                {
                    var player = xpPlayers.ElementAt(i);
                    ctx.Add(XpLeaderboardEntry.Create(i + 1, player, RoundNumber));
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
