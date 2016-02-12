using System.Linq;
using System.Text.RegularExpressions;
using Highway.Data;
using TT.Domain.Entities.Assets;
using TT.Domain.Entities.Items;
using TT.Domain.Entities.Identity;
using TT.Domain.DTOs;

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
            // assert Id is valid int
        }

    }
}