using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetAllUsers : DomainQuery<UserDetail>
    {
        public override IEnumerable<UserDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                {
                    var users = ctx.AsQueryable<User>().ToList();

                    return users.Select(r => r.MapToDto()).AsQueryable();
                };

            return ExecuteInternal(context);
        }
    }
}
