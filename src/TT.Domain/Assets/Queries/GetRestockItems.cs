using System.Collections.Generic;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Queries
{
    public class GetRestockItems : DomainQuery<RestockItemDetail>
    {
        public override IEnumerable<RestockItemDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<RestockItem>().ProjectToQueryable<RestockItemDetail>();
            return ExecuteInternal(context);
        }
    }
}