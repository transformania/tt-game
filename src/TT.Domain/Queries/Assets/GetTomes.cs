using System.Collections.Generic;
using Highway.Data;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetTomes : DomainQuery<TomeDetail>
    {
        public override IEnumerable<TomeDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Tome>().ProjectToQueryable<TomeDetail>();
            return ExecuteInternal(context);
        }
    }
}