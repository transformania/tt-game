using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class DeleteExpiredRunesOnMerchants : DomainCommand
    {
 
        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var lindella = ctx.AsQueryable<Player>().FirstOrDefault(p => p.BotId == AIStatics.LindellaBotId);

                if (lindella == null)
                    throw new DomainException($"Could not find Lindella with BotId {AIStatics.LindellaBotId}");

                var lorekeeper = ctx.AsQueryable<Player>().FirstOrDefault(p => p.BotId == AIStatics.LoremasterBotId);

                if (lorekeeper == null)
                    throw new DomainException($"Could not find Lorekeeper with BotId {AIStatics.LoremasterBotId}");

                var cutoff = DateTime.UtcNow.AddMinutes(-12*PvPStatics.MinutesToDroppedItemDelete);

                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Where(i => i.Owner != null && 
                    (i.Owner.Id == lindella.Id || i.Owner.Id == lorekeeper.Id) &&
                    i.ItemSource.ItemType == PvPStatics.ItemType_Rune &&
                    i.EmbeddedOnItem == null &&
                    i.TimeDropped < cutoff);

                foreach (var i in items)
                {
                    ctx.Remove(i);
                }

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

    }

}

