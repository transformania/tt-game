using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Item;
using TT.Domain.Entities.Items;
using TT.Domain.Entities.Players;

namespace TT.Domain.Commands.Items
{
    public class CreateItem : DomainCommand<int>
    {
        public int ItemSourceId { get; set; }
        public int OwnerId { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var itemSource = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == ItemSourceId);
                if (itemSource == null)
                    throw new DomainException(string.Format("Item Source with Id {0} could not be found", itemSource));

                var player = ctx.AsQueryable<Entities.Players.Player>().SingleOrDefault(t => t.Id == OwnerId);
                
                var item = Item.Create(player, itemSource);

                ctx.Add(item);
                ctx.Commit();

                result = item.Id;
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            
        }

    }
}
