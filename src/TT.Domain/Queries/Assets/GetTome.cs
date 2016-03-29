using System.Linq;
using TT.Domain.DTOs.Assets;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetTome : Highway.Data.Scalar<TomeDetail>
    {
        public GetTome(int id)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Tome>()
                            .Where(cr => cr.Id == id)
                            .ProjectToFirstOrDefault<TomeDetail>();
            };
        }
    }
}
