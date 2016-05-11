using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;

namespace TT.Domain.Queries.Item
{
    public class GetAllItemsOfType : DomainQuery<ItemListingDetail>
    {

        public int ItemSourceId { get; set; }

        public override IEnumerable<ItemListingDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Entities.Items.Item>().ProjectToQueryable<ItemListingDetail>().Where(i => i.ItemSource.Id == ItemSourceId);
            return ExecuteInternal(context);
        }
    }
}