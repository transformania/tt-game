using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;

namespace TT.Domain.Items.Queries
{
    public class GetItemsAtLocation : DomainQuery<ItemListingDetail>
    {

        public string dbLocationName { get; set; }

        public override IEnumerable<ItemListingDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Where(i => i.dbLocationName == dbLocationName)
                    .ToList();

                return items.Select(i => i.MapToListingDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
