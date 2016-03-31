using System.Linq;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetTomeByItem : Highway.Data.Scalar<TomeDetail>
    {
        public GetTomeByItem(int id)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Tome>()
                            .Where(cr => cr.BaseItem.Id == id)
                            .ProjectToFirstOrDefault<TomeDetail>();
            };
        }
    }
}
