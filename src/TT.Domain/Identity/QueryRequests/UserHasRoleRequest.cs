using MediatR;
using System.Collections.Generic;

namespace TT.Domain.Identity.QueryRequests
{
    public class UserHasAnyRoleRequest : IRequest<bool>
    {
        public string UserNameId { get; set; }

        public IEnumerable<string> Role { get; set; }
    }
}
