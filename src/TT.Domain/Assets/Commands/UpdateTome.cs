using System.Linq;
using Highway.Data;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

namespace TT.Domain.Assets.Commands
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
                    throw new DomainException($"Tome with ID {TomeId} was not found");

                var baseItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(u => u.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException($"Base item with ID {BaseItemId} was not found");

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