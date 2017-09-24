using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Domain.Identity.QueryRequests
{
    public class UserHasAnyRoleRequest : IRequest<bool>
    {
        public string UserNameId { get; set; }

        public IEnumerable<string> Role { get; set; }
    }
}
