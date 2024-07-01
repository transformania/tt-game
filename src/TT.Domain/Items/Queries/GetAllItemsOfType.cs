using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;

namespace TT.Domain.Items.Queries
{
    public class GetAllItemsOfType : DomainQuery<ItemListingDetail>
    {

        public int ItemSourceId { get; set; }

        public override IEnumerable<ItemListingDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Where(i => i.ItemSource.Id == ItemSourceId)
                    .ToList();

                return items.Select(i => i.MapToListingDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}