using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Item;
using TT.Domain.Entities.NPCs;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Commands.Assets
{
    public class UpdateRestockItem : DomainCommand
    {
        public int RestockItemId { get; set; }
        public int AmountBeforeRestock { get; set; }
        public int AmountToRestockTo { get; set; }
        public int BaseItemId { get; set; }
        public int NPCId { get; set; }

        public override void Execute(IDataContext context)
        {
            int result = 0;

            ContextQuery = ctx =>
            {

                var restockItem = ctx.AsQueryable<RestockItem>().SingleOrDefault(cr => cr.Id == RestockItemId);

                var baseItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException(string.Format("Base item with Id {0} could not be found", BaseItemId));

                var npc = ctx.AsQueryable<NPC>().SingleOrDefault(t => t.Id == NPCId);
                if (npc == null)
                    throw new DomainException(string.Format("NPC with Id {0} could not be found", BaseItemId));

                restockItem.Update(this, baseItem, npc);
                ctx.Commit();

                result = restockItem.Id;
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (AmountBeforeRestock < 0)
                throw new DomainException("Minimum amount before restock must be 0");

            if (AmountToRestockTo < 1)
                throw new DomainException("Minimum amount to restock to must be 1");

            if (BaseItemId <= 0)
                throw new DomainException("Base item id must be greater than 0");

            if (NPCId <= 0)
                throw new DomainException("NPC id must be greater than 0");
        }

    }
}