using System.Linq;
using TT.Domain.DTOs.Assets;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetTome : Highway.Data.Scalar<TomeDetail>
    {
        public GetTome(int Id)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Tome>().Select(cr => new TomeDetail
                {
                    Id = cr.Id,
                    Text = cr.Text,
                    BaseItem = new ItemSourceDetail(cr.BaseItem)
                }).FirstOrDefault(cr => cr.Id == Id);
            };
        }
    }
}
