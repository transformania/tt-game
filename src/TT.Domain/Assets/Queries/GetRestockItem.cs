using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;

namespace TT.Domain.Assets.Queries
{
    public class GetRestockItem : DomainQuerySingle<RestockItemDetail>
    {
        public int RestockItemId { get; set; }

        public override RestockItemDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var item = ctx
                    .AsQueryable<RestockItem>()
                    .Include(cr => cr.BaseItem)
                    .FirstOrDefault(cr => cr.Id == RestockItemId);

                return item?.MapToDto();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (RestockItemId <= 0)
                throw new DomainException("RestockItem Id must be greater than 0");
        }
    }
}
