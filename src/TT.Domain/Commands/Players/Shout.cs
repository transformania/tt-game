using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Statics;

namespace TT.Domain.Commands.Players
{
    public class Shout : DomainCommand
    {

        public string UserId { private get; set; }
        public string Message { private get; set; }

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

                if (player.Mobility != PvPStatics.MobilityFull)
                    throw new DomainException("You must be animate in order to shout!");

                if (player.ShoutsRemaining <= 0)
                    throw new DomainException("You can only shout once per turn.");

                Message = Message.Replace("<", "&lt;").Replace(">", "&gt;"); // remove suspicious characters

                var log = LocationLog.Create(player.Location, $"<span class='playerShoutNotification'>{player.GetFullName()} shouted <b>\"{Message}\"</b> here.</span>", 0);

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
