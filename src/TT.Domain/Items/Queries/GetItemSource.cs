using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;

namespace TT.Domain.Items.Queries
{
    public class GetItemSource : DomainQuerySingle<ItemSourceDetail>
    {
        public int ItemSourceId { get; set; }

        public override ItemSourceDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var itemSource = ctx
                    .AsQueryable<ItemSource>()
                    .Include(i => i.GivesEffectSource)
                    .FirstOrDefault(p => p.Id == ItemSourceId);

                return itemSource.MapToItemSourceDto();
            };

            return ExecuteInternal(context);
        }
    }
}
