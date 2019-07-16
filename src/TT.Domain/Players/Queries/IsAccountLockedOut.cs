using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Players.Queries
{
    public class IsAccountLockedOut : DomainQuerySingle<bool>
    {
        public string userId { get; set; }

        public override bool Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>()
                    .FirstOrDefault(m => m.Id == userId);

              if (user==null)
                    throw new DomainException($"User {userId} does not exist.");

                return user.LockoutEndDateUtc == null ? false : user.LockoutEndDateUtc > DateTime.UtcNow;
            };

            return ExecuteInternal(context);
        }
    }
}
