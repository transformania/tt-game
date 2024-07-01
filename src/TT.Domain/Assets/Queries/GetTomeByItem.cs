using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Assets.DTOs;
using TT.Domain.Assets.Entities;
using TT.Domain.Exceptions;

namespace TT.Domain.Assets.Queries
{
    public class GetTomeByItem : DomainQuerySingle<TomeDetail>
    {
        public int ItemSourceId { get; set; }

        public override TomeDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var tome = ctx
                    .AsQueryable<Tome>()
                    .Include(cr => cr.BaseItem)
                    .FirstOrDefault(cr => cr.BaseItem.Id == ItemSourceId);

                return tome?.MapToDto();
            };

            return ExecuteInternal(context);
        }

        protected override void Validate()
        {
            if (ItemSourceId <= 0)
                throw new DomainException("ItemSourceID must be a number greater than 0");
        }
    }
}
