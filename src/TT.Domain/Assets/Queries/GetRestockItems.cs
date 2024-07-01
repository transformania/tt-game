using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Queries
{
    public class GetRestockItems : DomainQuery<RestockItemDetail>
    {
        public override IEnumerable<RestockItemDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var items = ctx.AsQueryable<RestockItem>()
                    .Include(cr => cr.BaseItem)
                    .ToList();
                return items.Select(cr => cr.MapToDto()).AsQueryable();
            };
            return ExecuteInternal(context);
        }
    }
}