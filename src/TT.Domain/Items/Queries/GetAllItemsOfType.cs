using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetAllItemsOfType : DomainQuery<ItemListingDetail>
    {

        public int ItemSourceId { get; set; }

        public override IEnumerable<ItemListingDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Item>().ProjectToQueryable<ItemListingDetail>().Where(i => i.ItemSource.Id == ItemSourceId);
            return ExecuteInternal(context);
        }
    }
}