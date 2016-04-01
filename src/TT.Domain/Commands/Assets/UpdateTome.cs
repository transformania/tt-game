using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Item;

namespace TT.Domain.Commands.Assets
{
    public class UpdateTome : DomainCommand
    {
        public int TomeId { get; set; }
        public string Text { get; set; }
        public int BaseItemId { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var tome = ctx.AsQueryable<Tome>().SingleOrDefault(cr => cr.Id == TomeId);

                if (tome == null)
                    throw new DomainException(string.Format("Tome with ID {0} was not found", TomeId));

                var baseItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(u => u.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException(string.Format("Base item with ID {0} was not found", BaseItemId));

                tome.Update(this, baseItem);
                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new DomainException("No text was provided for the tome");

            if (TomeId <= 0)
                throw new DomainException("Tome Id must be greater than 0");

            if (BaseItemId <= 0)
                throw new DomainException("Base item id must be greater than 0");
        }
    }
}