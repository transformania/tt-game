using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class IsPvPLocked : DomainQuerySingle<bool>
    {
        public string UserId { get; set; }

        public override bool Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>()
                    .FirstOrDefault(m => m.Id == UserId);

                if (user == null)
                {
                    throw new DomainException($"User with Id {UserId} does not exist");
                }

                return user.PvPLock;

            };

            return ExecuteInternal(context);
        }
    }
}