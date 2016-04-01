using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Item;
using TT.Domain.Entities.Item;

namespace TT.Domain.Queries.Item
{
    public class GetItemSource : DomainQuerySingle<ItemSourceDetail>
    {
        public int ItemSourceId { get; set; }

        public override ItemSourceDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<ItemSource>().Select(cr => new ItemSourceDetail
                {
                    Id = cr.Id,
                    DbName = cr.DbName,
                    FriendlyName = cr.FriendlyName,
                    IsUnique = cr.IsUnique
                }).FirstOrDefault(cr => cr.Id == ItemSourceId);
            };

            return ExecuteInternal(context);
        }
    }
}
