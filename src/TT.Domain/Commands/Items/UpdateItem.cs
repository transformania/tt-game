using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Items;

namespace TT.Domain.Commands.Items
{
    public class UpdateItem : DomainCommand
    {

        public int ItemId { get; set; }
        public int? OwnerId { get; set; }
        public string dbLocationName { get; set; }
        public bool IsEquipped { get; set; }
        public DateTime TimeDropped { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var item = ctx.AsQueryable<Item>().SingleOrDefault(cr => cr.Id == ItemId);

                if (item == null)
                    throw new DomainException($"Item with ID {ItemId} could not be found");

                var owner = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(p => p.Id == OwnerId);

                item.Update(this, owner);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            
        }

    }
}
