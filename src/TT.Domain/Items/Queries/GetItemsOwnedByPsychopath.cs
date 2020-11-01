using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItemsOwnedByPsychopath : DomainQuery<ItemDetailForPsycho>
    {

        public int OwnerId { private get; set; }

        public override IEnumerable<ItemDetailForPsycho> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Item>().ProjectToQueryable<ItemDetailForPsycho>().Where(i => i.Owner != null && i.Owner.Id == OwnerId);
            return ExecuteInternal(context);
        }
    }
}