using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Statics;
using TT.Domain.World.Entities;

namespace TT.Domain.World.Commands
{
    public class SavePvPLeaderboards : DomainCommand
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
                    throw new DomainException($"Unable to save PvP leaderboards at this time.  It is turn {worldData.TurnNumber} and needs to be turn {worldData.RoundDuration}.");

                if (worldData.ChaosMode)
                    throw new DomainException("Unable to save PvP leaderboards at this time.  The game is currently in chaos mode.");

                var existingPvPEntries =
                    ctx.AsQueryable<PvPLeaderboardEntry>().Any(l => l.RoundNumber == RoundNumber);

                if (existingPvPEntries)
                    throw new DomainException($"There are already existing PvP leaderboard entries for round {RoundNumber}.");

                // save PvP Leaderboards
                var pvpPlayers = ctx.AsQueryable<Players.Entities.Player>()
                    .Include(p => p.FormSource)
                    .Include(p => p.Covenant)
                    .Where(p => p.GameMode == (int)GameModeStatics.GameModes.PvP &&
                        p.BotId == AIStatics.ActivePlayerBotId)
                    .OrderByDescending(p => p.PvPScore)
                    .ThenByDescending(p => p.Level)
                    .ThenByDescending(p => p.XP)
                    .Take(100)
                    .ToList();

                for (var i = 0; i < pvpPlayers.Count; i++)
                {
                    var player = pvpPlayers.ElementAt(i);
                    ctx.Add(PvPLeaderboardEntry.Create(i+1, player, RoundNumber));
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
