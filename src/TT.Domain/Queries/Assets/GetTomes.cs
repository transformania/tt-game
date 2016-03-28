using System.Collections.Generic;
using System.Linq;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetTomes : Highway.Data.Query<TomeDetail>
    {
        public GetTomes()
        {
            ContextQuery = ctx => ctx.AsQueryable<Tome>().ProjectToQueryable<TomeDetail>();
        }
    }
}