using System.Linq;
using Highway.Data;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

namespace TT.Domain.Assets.Commands
{
    public class CreateRestockItem : DomainCommand<int>
    {
        public int AmountBeforeRestock { get; set; }
        public int AmountToRestockTo { get; set; }
        public int BaseItemId { get; set; }
        public int BotId { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var baseItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException($"Base item with Id {BaseItemId} could not be found");

                var restockItem = RestockItem.Create(baseItem, AmountBeforeRestock, AmountToRestockTo, BotId);

                ctx.Add(restockItem);
                ctx.Commit();

                result = restockItem.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (AmountBeforeRestock < 0)
                throw new DomainException("Minimum amount before restock must be 0");

            if (AmountToRestockTo < 1)
                throw new DomainException("Minimum amount to restock to must be 1");

            if (BaseItemId <= 0)
                throw new DomainException("Base item id must be greater than 0");

        }

    }
}