using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;

namespace TT.Domain.Queries.Item
{
    public class GetItemByVictimName : DomainQuerySingle<ItemDetail>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override ItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Entities.Items.Item>()
                           .Where(i => i.VictimName == FirstName + " " + LastName)
                           .ProjectToFirstOrDefault<ItemDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}