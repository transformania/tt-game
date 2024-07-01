using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;

namespace TT.Domain.Assets.Queries
{
    public class GetTomes : DomainQuery<TomeDetail>
    {
        public override IEnumerable<TomeDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var tomes = ctx.AsQueryable<Tome>().Include(cr => cr.BaseItem).ToList();
                return tomes.Select(cr => cr.MapToDto()).AsQueryable();
            };
            return ExecuteInternal(context);
        }
    }
}