using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class DeleteExpiredConsumablesOnGround : DomainCommand
    {

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var cutoff = DateTime.UtcNow.AddMinutes(-PvPStatics.MinutesToDroppedItemDelete);

                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Where(i => i.Owner == null &&
                    i.ItemSource.ItemType == PvPStatics.ItemType_Consumable &&
                    i.ItemSource.Id != PvPStatics.ItemType_DungeonArtifact_Id &&
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

