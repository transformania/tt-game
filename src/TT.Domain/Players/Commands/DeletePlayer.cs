using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Effects.Entities;
using TT.Domain.Entities.Skills;
using TT.Domain.Entities.TFEnergies;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.TFEnergies.Entities;

namespace TT.Domain.Players.Commands
{
    public class DeletePlayer : DomainCommand
    {

        public int PlayerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var player = ctx.AsQueryable<Player>()
                .Include(p => p.Items)
                .FirstOrDefault(i => i.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID {PlayerId} was not found");

                var spells = ctx.AsQueryable<Skill>().Where(i => i.Owner.Id == PlayerId).ToList();
                var effects = ctx.AsQueryable<Effect>().Where(i => i.Owner.Id == PlayerId).ToList();
                var playerLogs = ctx.AsQueryable<PlayerLog>().Where(i => i.Owner.Id == PlayerId).ToList();
                var tfTenergies = ctx.AsQueryable<TFEnergies.Entities.TFEnergy>().Where(i => i.Owner.Id == PlayerId).ToList();
                var tfTenergiesCast = ctx.AsQueryable<TFEnergies.Entities.TFEnergy>().Where(i => i.Caster.Id == PlayerId).ToList();

                foreach (var s in spells)
                {
                    ctx.Remove(s);
                }

                foreach (var e in effects)
                {
                    ctx.Remove(e);
                }

                foreach (var l in playerLogs)
                {
                    ctx.Remove(l);
                }
                foreach (var e in tfTenergies)
                {
                    ctx.Remove(e);
                }
                foreach (var e in tfTenergiesCast)
                {
                    ctx.Remove(e);
                }

                player.DropAllItems();

                ctx.Remove(player);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
           
        }
    }
}
