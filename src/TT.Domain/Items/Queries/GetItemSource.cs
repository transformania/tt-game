using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;

namespace TT.Domain.Items.Queries
{
    public class GetItemSource : DomainQuerySingle<ItemSourceDetail>
    {
        public int ItemSourceId { get; set; }

        public override ItemSourceDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<ItemSource>()
                           .Where(p => p.Id == ItemSourceId)
                           .ProjectToFirstOrDefault<ItemSourceDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
