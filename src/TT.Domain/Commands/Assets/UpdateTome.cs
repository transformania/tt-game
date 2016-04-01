using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Item;

namespace TT.Domain.Commands.Assets
{
    public class UpdateTome : DomainCommand
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int BaseItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var tome = ctx.AsQueryable<Tome>().FirstOrDefault(cr => cr.Id == Id);

                var baseItem = ctx.AsQueryable<ItemSource>().Single(u => u.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException("Base item does not exist");

                BaseItemId = baseItem.Id;

                tome.Update(this, baseItem);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new DomainException("No text");

            if (BaseItemId <= 0)
                throw new DomainException("No base item was provided");
        }
    }
}