using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Effects.Entities;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Exceptions;
using TT.Domain.Legacy.Procedures.JokeShop;
using TT.Domain.Players.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Players.Commands
{
    public class Shout : DomainCommand
    {

        public string UserId { private get; set; }
        public string Message { private get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.User)
                    .Include(i => i.PlayerLogs)
                    .SingleOrDefault(p => p.User.Id == UserId);

                if (player == null)
                    throw new DomainException($"Player with user ID '{UserId}' could not be found");

                if (player.Mobility != PvPStatics.MobilityFull)
                    throw new DomainException("You must be animate in order to shout!");

                if (player.ShoutsRemaining <= 0)
                    throw new DomainException("You can only shout once per turn.");

                if (CharacterPrankProcedures.HUSHED_EFFECT.HasValue)
                {
                    // Also consider players with a temporary change in bot ID to be active so not to autolock
                    var hushed = ctx.AsQueryable<Effect>()
                                        .Where(e => e.EffectSource.Id == CharacterPrankProcedures.HUSHED_EFFECT.Value &&
                                                    e.Owner.Id == player.Id &&
                                                    e.Duration > 0)
                                        .Any();

                    if (hushed)
                        throw new DomainException("You have been hushed and cannot currently shout.");
                }


                Message = Message.Replace("<", "&lt;").Replace(">", "&gt;"); // remove suspicious characters

                var log = LocationLog.Create(player.Location, $"<span class='playerShoutNotification'>{player.GetFullName()} shouted <b>\"{Message}\"</b> here.</span>");

                player.Shout(Message);
                ctx.Update(player);
                ctx.Add(log);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new DomainException("userId is required");

            if (string.IsNullOrWhiteSpace(Message))
                throw new DomainException("A shout message is required");

            if (Message.Length > 100)
                throw new DomainException("A shout must contain 100 characters or fewer.");

        }

    }
}
