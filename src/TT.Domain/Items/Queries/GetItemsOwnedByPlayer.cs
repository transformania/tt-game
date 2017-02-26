using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItemsOwnedByPlayer : DomainQuery<ItemDetail>
    {

        public int OwnerId { get; set; }

        public override IEnumerable<ItemDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Item>().ProjectToQueryable<ItemDetail>().Where(i => i.Owner.Id == OwnerId);
            return ExecuteInternal(context);
        }
    }
}