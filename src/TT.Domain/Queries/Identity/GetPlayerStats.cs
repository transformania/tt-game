using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Players;

namespace TT.Domain.Queries.Identity
{
    public class GetPlayerStats : DomainQuery<StatDetail>
    {

        public string OwnerId { get; set; }

        public override IEnumerable<StatDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Stat>()
                           .Where(p => p.Owner.Id == OwnerId)
                           .ProjectToQueryable<StatDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
