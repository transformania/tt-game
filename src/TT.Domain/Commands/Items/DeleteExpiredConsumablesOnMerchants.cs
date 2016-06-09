using System;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Items;
using TT.Domain.Statics;

namespace TT.Domain.Commands.Items
{
    public class DeleteExpiredConsumablesOnMerchants : DomainCommand
    {

        public int LindellaId { get; set; }
        public int LorekeeperId { get; set; }

        //List<Item> possibleToDelete = itemsRepo.Items.Where(i => (i.dbLocationName != "" && i.OwnerId == -1) || (i.OwnerId == merchantId || i.OwnerId == skaldyrId) && i.dbName != PvPStatics.ItemType_DungeonArtifact).ToList();


        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {

                var lindella = ctx.AsQueryable<Entities.Players.Player>().FirstOrDefault(p => p.Id == LindellaId);

                if (lindella == null)
                    throw new DomainException(string.Format("Could not find Lindella with Id {0}", LindellaId));

                var lorekeeper = ctx.AsQueryable<Entities.Players.Player>().FirstOrDefault(p => p.Id == LorekeeperId);

                if (lorekeeper == null)
                    throw new DomainException(string.Format("Could not find Lorekeeper with Id {0}", LorekeeperId));

                var cutoff = DateTime.UtcNow.AddMinutes(-12*PvPStatics.MinutesToDroppedItemDelete);

                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Where(i => (i.Owner.Id == LindellaId || i.Owner.Id == LorekeeperId) &&
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

