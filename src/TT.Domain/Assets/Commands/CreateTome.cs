using System.Linq;
using Highway.Data;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;
using TT.Domain.Items.Entities;

namespace TT.Domain.Assets.Commands
{
    public class CreateTome : DomainCommand<int>
    {
        public string Text { get; set; }
        public int BaseItemId { get; set; }

        public override int Execute(IDataContext context)
        {
            int result = 0;

            ContextQuery = ctx =>
            {
                var baseItem = ctx.AsQueryable<ItemSource>().SingleOrDefault(t => t.Id == BaseItemId);
                if (baseItem == null)
                    throw new DomainException($"Base item with Id {BaseItemId} could not be found");

                var tome = Tome.Create(baseItem, Text);

                ctx.Add(tome);
                ctx.Commit();

                result = tome.Id;
            };
      
            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(Text))
                throw new DomainException("No text was provided for the tome");

            if (BaseItemId <= 0)
                throw new DomainException("Base item id must be greater than 0");
        }

    }
}