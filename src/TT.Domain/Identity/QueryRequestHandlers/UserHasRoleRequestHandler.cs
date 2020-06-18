using Highway.Data;
using MediatR;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TT.Domain.Identity.Entities;
using TT.Domain.Identity.QueryRequests;

namespace TT.Domain.Identity.QueryRequestHandlers
{
    public class UserHasRoleRequestHandler : IRequestHandler<UserHasAnyRoleRequest, bool>
    {
        private readonly IDataContext context;

        public UserHasRoleRequestHandler(IDataContext context)
        {
            this.context = context;
        }

        public async Task<bool> Handle(UserHasAnyRoleRequest message, CancellationToken cancellationToken)
        {
            var userQuery = from u in context.AsQueryable<User>()
                            where u.Id == message.UserNameId
                            from r in
                                u.Roles
                            select r.Name;

            var userQueryRoleIntercect = userQuery.Intersect(message.Role);

           return await userQueryRoleIntercect.AnyAsync(cancellationToken);
        }
    }
}
