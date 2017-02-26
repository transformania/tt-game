using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItemByVictimName : DomainQuerySingle<ItemDetail>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override ItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Item>()
                           .Where(i => i.VictimName == FirstName + " " + LastName)
                           .ProjectToFirstOrDefault<ItemDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}