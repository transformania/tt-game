using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Commands.Identity
{
    public class AddStrike : DomainCommand
    {

        public string UserId { get; set; }
        public string ModeratorId { get; set; }
        public string Reason { get; set; }

        public int Round { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == UserId);
                if (user == null)
                    throw new DomainException($"User with Id '{UserId}' could not be found");

                var moderator = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == ModeratorId);
                if (moderator == null)
                    throw new DomainException($"Moderator with Id '{ModeratorId}' could not be found");

                var player = ctx.AsQueryable<Entities.Players.Player>()
                    .Include(p => p.PlayerLogs)
                    .SingleOrDefault(p => p.User.Id == UserId);

                if (player != null)
                {
                    player.AddLog($"<b class='bad'>You have received a strike against your account from a moderator.  Reason cited: {Reason}.</b>", true);
                }

                var entry = Strike.Create(user, moderator, Reason, Round);

                ctx.Add(entry);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Reason))
                throw new DomainException("Reason for strike is required");

            if (Round <= 0)
                throw new DomainException("Round must be a positive integer greater than 0");
        }
    }

}
