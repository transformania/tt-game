using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TT.Domain.Identity.QueryRequests
{
    public class IsValidUserRequest : IRequest<bool>
    {
        public string UserNameId { get; set; }
    }
}
