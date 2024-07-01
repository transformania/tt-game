using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetUserStrikes : DomainQuery<StrikeDetail>
    {

        public string UserId { get; set; }

        public override IEnumerable<StrikeDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                {
                    var strikes = ctx.AsQueryable<Strike>()
                        .Include(s => s.User)
                        .Include(s => s.FromModerator)
                        .Where(m => m.User.Id == UserId)
                        .OrderByDescending(s => s.Timestamp)
                        .ToList();

                    return strikes.Select(s => s.MapToDto()).AsQueryable();
                };

            return ExecuteInternal(context);
        }
    }
}
