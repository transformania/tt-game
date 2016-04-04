using System.Collections.Generic;
using Highway.Data;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
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