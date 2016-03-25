using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Item;
using TT.Domain.DTOs.Assets;

namespace TT.Domain.Commands.Assets
{
    public class CreateTome : DomainCommand<TomeDetail>
    {
        public string Text { get; set; }
        public int BaseItemId { get; set; }

        public override TomeDetail Execute(IDataContext context)
        {

            Validate();

            TomeDetail result = null;

            ContextQuery = ctx =>
            {
                var baseItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException("Base item does not exist");

                var tome = Tome.Create(baseItem, Text);

                ctx.Add(tome);
                ctx.Commit();

                result = new TomeDetail(tome);

            };

      
            ExecuteInternal(context);

            return result;
        }

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new DomainException("No text");
            if (BaseItemId <= 0)
                throw new DomainException("No base item was provided");
        }

    }
}