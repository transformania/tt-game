using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetUserDonators : DomainQuery<UserDonatorDetail>
    {

        public int MinimumTier { get; set; }

        public override IEnumerable<UserDonatorDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                {
                    var donators = ctx.AsQueryable<User>()
                        .Include(m => m.Donator)
                        .Where(m => m.Donator != null)
                        .Where(m => m.Donator.Tier >= MinimumTier)
                        .ToList();

                    return donators.Select(m => m.MapToDonatorDto()).AsQueryable();
                };

            return ExecuteInternal(context);
        }
    }
}
