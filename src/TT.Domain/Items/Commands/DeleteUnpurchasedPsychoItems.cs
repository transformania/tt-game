using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class DeleteUnpurchasedPsychoItems : DomainCommand
    {
        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {

                var lindella =
                    context.AsQueryable<Players.Entities.Player>()
                        .FirstOrDefault(p => p.BotId == AIStatics.LindellaBotId);

                if (lindella == null)
                    throw new DomainException($"Could not find Lindella with BotId {AIStatics.LindellaBotId}");

                var wuffie =
                    context.AsQueryable<Players.Entities.Player>().FirstOrDefault(p => p.BotId == AIStatics.WuffieBotId);

                if (wuffie == null)
                    throw new DomainException($"Could not find Wuffie with BotId {AIStatics.WuffieBotId}");

                var cutoff = DateTime.UtcNow.AddDays(-3);

                
                var query = ctx.AsQueryable<Item>()
                    .Where(i => i.FormerPlayer.BotId == AIStatics.PsychopathBotId &&
                                i.Owner != null &&
                                (i.Owner.Id == lindella.Id || i.Owner.Id == wuffie.Id)
                                && i.TimeDropped < cutoff)
                    .Include(i => i.Runes)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.FormerPlayer.Effects)
                    .Include(i => i.FormerPlayer.Skills)
                    .Include(i => i.FormerPlayer.PlayerLogs)
                    .Include(i => i.FormerPlayer.TFEnergies)
                    .Include(i => i.FormerPlayer.TFEnergiesCast)
                    .Take(50) // for performance reasons don't run too many deletions at once, otherwise this command will take forever
                    .ToList();

                foreach (var p in query.Select(q => q.FormerPlayer))
                {
                    foreach (var x in p.Effects.ToList())
                    {
                        ctx.Remove(x);
                    }
                    foreach (var x in p.Skills.ToList())
                    {
                        ctx.Remove(x);
                    }
                    foreach (var x in p.PlayerLogs.ToList())
                    {
                        ctx.Remove(x);
                    }
                    foreach (var x in p.TFEnergies.ToList())
                    {
                        ctx.Remove(x);
                    }
                    foreach (var x in p.TFEnergiesCast.ToList())
                    {
                        ctx.Remove(x);
                    }
                    ctx.Remove(p);

                }

                context.Commit();
                foreach (var i in query)
                {
                    i.RemoveRunes();
                    ctx.Remove(i);
                }

                context.Commit();

            };
            ExecuteInternal(context);

        }
    }
}
