using System.Collections.Generic;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Queries
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