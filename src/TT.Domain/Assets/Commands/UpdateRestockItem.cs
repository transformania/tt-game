using System.Linq;
using Highway.Data;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

namespace TT.Domain.Assets.Commands
{
    public class UpdateRestockItem : DomainCommand
    {
        public int RestockItemId { get; set; }
        public int AmountBeforeRestock { get; set; }
        public int AmountToRestockTo { get; set; }
        public int BaseItemId { get; set; }
        public int BotId { get; set; }

        public override void Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {

                var restockItem = ctx.AsQueryable<RestockItem>().SingleOrDefault(cr => cr.Id == RestockItemId);
                if (restockItem == null)
                    throw new DomainException($"Restock item with Id {RestockItemId} could not be found");

                var baseItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException($"Base item with Id {BaseItemId} could not be found");

                restockItem.Update(this, baseItem, BotId);
                ctx.Commit();

                result = restockItem.Id;
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RestockItemId == 0)
                throw new DomainException("RestockItemId must be set");

            if (AmountBeforeRestock < 0)
                throw new DomainException("Minimum amount before restock must be 0");

            if (AmountToRestockTo < 1)
                throw new DomainException("Minimum amount to restock to must be 1");

            if (BaseItemId <= 0)
                throw new DomainException("Base item id must be greater than 0");
        }

    }
}