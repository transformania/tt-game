using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Queries.Identity
{
    public class GetUserStrikes : DomainQuery<StrikeDetail>
    {

        public string UserId { get; set; }

        public override IEnumerable<StrikeDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                    ctx.AsQueryable<Strike>()
                        .Where(m => m.User.Id == UserId)
                        .OrderByDescending(s => s.Timestamp)
                        .ProjectToQueryable<StrikeDetail>();

            return ExecuteInternal(context);
        }
    }
}
