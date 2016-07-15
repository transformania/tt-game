using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Effects;
using TT.Domain.Entities.Players;
using TT.Domain.Entities.Skills;
using TT.Domain.Entities.TFEnergies;

namespace TT.Domain.Commands.Players
{
    public class DeletePlayer : DomainCommand
    {

        public int PlayerId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var player = ctx.AsQueryable<Entities.Players.Player>()
                .Include(p => p.Items)
                .FirstOrDefault(i => i.Id == PlayerId);

                if (player == null)
                    throw new DomainException(string.Format("Player with ID {0} was not found", PlayerId));

                var spells = ctx.AsQueryable<Skill>().Where(i => i.Owner.Id == PlayerId).ToList();
                var effects = ctx.AsQueryable<Effect>().Where(i => i.Owner.Id == PlayerId).ToList();
                var playerLogs = ctx.AsQueryable<PlayerLog>().Where(i => i.Owner.Id == PlayerId).ToList();
                var tfTenergies = ctx.AsQueryable<TFEnergy>().Where(i => i.Owner.Id == PlayerId).ToList();
                var tfTenergiesCast = ctx.AsQueryable<TFEnergy>().Where(i => i.Caster.Id == PlayerId).ToList();

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
