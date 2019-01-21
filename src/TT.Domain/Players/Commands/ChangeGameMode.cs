using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Players.Commands
{
    public class ChangeGameMode : DomainCommand
    {

        public string MembershipId { get; set; }
        public int GameMode { get; set; }

        public bool InChaos { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>().SingleOrDefault(cr => cr.User.Id == MembershipId);
                if (player == null)
                    throw new DomainException($"Player with MembershipID '{MembershipId}' could not be found");

                if (player.GameMode == GameMode)
                    throw new DomainException("You are already in this game mode.");

                if (!InChaos)
                {
                    if (player.GameMode == (int)GameModeStatics.GameModes.PvP && (GameMode == (int)GameModeStatics.GameModes.Superprotection || GameMode == (int)GameModeStatics.GameModes.Protection))
                        throw new DomainException("You cannot leave PvP mode during regular gameplay.");

                    if ((player.GameMode == (int)GameModeStatics.GameModes.Protection || player.GameMode == (int)GameModeStatics.GameModes.Superprotection) && GameMode == (int)GameModeStatics.GameModes.PvP)
                        throw new DomainException("You cannot enter PvP mode during regular gameplay.");
                }

                player.ChangeGameMode(GameMode);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (MembershipId == null)
                throw new DomainException("MembershipID is required!");

            if (GameMode != (int)GameModeStatics.GameModes.Superprotection && GameMode != (int)GameModeStatics.GameModes.Protection && GameMode != (int)GameModeStatics.GameModes.PvP)
                throw new DomainException("Game mode selection is invalid");

        }

    }
}
