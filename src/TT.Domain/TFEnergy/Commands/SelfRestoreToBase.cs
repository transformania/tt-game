using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.TFEnergy.Commands
{
    public class SelfRestoreToBase : DomainCommand<string>
    {

        public int PlayerId { get; set; }
        public BuffBox Buffs { get; set; }

        public override string Execute(IDataContext context)
        {
            var result = "";

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                .Include(p => p.FormSource)
                    .Include(p => p.OriginalFormSource)
                    .Include(p => p.SelfRestoreEnergy)
                .SingleOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID {PlayerId} could not be found");

                if (player.ActionPoints < (decimal) PvPStatics.SelfRestoreAPCost)
                    throw new DomainException($"You don't have enough action points to do this.  You have {player.ActionPoints} and need {PvPStatics.SelfRestoreAPCost}.");

                if (player.Mana < (decimal)PvPStatics.SelfRestoreManaCost)
                    throw new DomainException($"You don't have enough mana to do this.  You have {player.Mana} and need {PvPStatics.SelfRestoreManaCost}.");

                if (player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
                {
                    throw new DomainException("You have cleansed and meditated too many times this turn.");
                }

                result = player.AddSelfRestoreEnergy(Buffs);

                ctx.Update(player);
                ctx.Commit();

            };

            ExecuteInternal(context);

            return result;

        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("Player Id is required");

            if (Buffs == null)
                throw new DomainException("Buffs are required.");
        }
    }

}

