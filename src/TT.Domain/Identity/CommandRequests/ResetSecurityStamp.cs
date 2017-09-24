using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Domain.RequestInterfaces;

namespace TT.Domain.Identity.CommandRequests
{
    public class ResetSecurityStamp : IRequest, IRequestWithUserNameId
    {
        public string TargetUserNameId { get; set; }

        public string UserNameId { get; set; }
    }
}
