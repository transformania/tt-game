using System.Linq;
using Highway.Data;

namespace TT.Domain.Commands.Players
{
    public class ChangeRPMode : DomainCommand
    {

        public string MembershipId { get; set; }
        public bool InRPMode { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(cr => cr.User.Id == MembershipId);
                if (player == null)
                    throw new DomainException($"Player with MembershipID '{MembershipId}' could not be found");

                player.ChangeRPMode(InRPMode);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (MembershipId == null)
                throw new DomainException("MembershipID is required!");
        }

    }
}
