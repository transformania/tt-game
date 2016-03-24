using System.Linq;
using TT.Domain.DTOs;
using TT.Domain.Entities.Assets;

namespace TT.Domain.Queries.Assets
{
    public class GetTomes : Highway.Data.Query<TomeDetail>
    {
        public GetTomes()
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Tome>().Select(cr => new TomeDetail
                {
                    Id = cr.Id,
                    Text = cr.Text,
                    BaseItem = cr.BaseItem

                });

            };
        }
    }
}