using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;

namespace TT.Domain.Queries.Item
{
    public class GetItemsAtLocation : DomainQuery<ItemListingDetail>
    {

        public string dbLocationName { get; set; }

        public override IEnumerable<ItemListingDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Entities.Items.Item>().ProjectToQueryable<ItemListingDetail>().Where(i => i.dbLocationName == dbLocationName);
            return ExecuteInternal(context);
        }
    }
}
