using System.Linq;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Item;

namespace TT.Domain.Queries.Item
{
    public class GetItemSources : Highway.Data.Query<ItemSourceDetail>
    {
        public GetItemSources()
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<ItemSource>().Select(cr => new ItemSourceDetail
                {
                    Id = cr.Id,
                    DbName = cr.DbName,
                    FriendlyName = cr.FriendlyName,
                    IsUnique = cr.IsUnique
                });

            };
        }
    }
}