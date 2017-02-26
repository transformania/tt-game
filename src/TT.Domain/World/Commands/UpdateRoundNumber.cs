using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Statics;

namespace TT.Domain.World.Commands
{
    public class UpdateRoundNumber : DomainCommand
    {
        public string RoundNumber { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var game = ctx.AsQueryable<Entities.World>().First();
                if (game == null)
                    throw new DomainException("World data does not exist yet!");

                if (game.TurnNumber != 0 && game.TurnNumber != PvPStatics.RoundDuration)
                    throw new DomainException("Round renaming can only be done at turn 0 or the maximum round turn.");

                if (!game.ChaosMode)
                    throw new DomainException("Round renaming can only be done in chaos mode.");

                game.SetRoundNumber(RoundNumber);
                ctx.Update(game);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RoundNumber.IsNullOrWhiteSpace())
                throw new DomainException("Round Number must be set!");
        }
    }
}
