using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetAllApprovals : DomainQuery<UserDetail>
    {

        public override IEnumerable<UserDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                {
                    var approved = ctx.AsQueryable<User>().ToList();

                    return approved.Select(r => r.MapToDto()).AsQueryable();
                };

            return ExecuteInternal(context);
        }
    }
}
