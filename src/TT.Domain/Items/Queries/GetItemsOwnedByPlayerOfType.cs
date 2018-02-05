using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItemsOwnedByPlayerOfType : DomainQuery<ItemDetail>
    {

        public int OwnerId { private get; set; }
        public string ItemType { private get; set; }

        public override IEnumerable<ItemDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Item>().ProjectToQueryable<ItemDetail>().Where(i => i.Owner != null && i.Owner.Id == OwnerId && i.ItemSource.ItemType == ItemType);
            return ExecuteInternal(context);
        }
    }
}