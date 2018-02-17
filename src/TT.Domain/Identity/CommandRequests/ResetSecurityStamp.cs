using MediatR;
using TT.Domain.RequestInterfaces;

namespace TT.Domain.Identity.CommandRequests
{
    public class ResetSecurityStamp : IRequest, IRequestWithUserNameId
    {
        public string TargetUserNameId { get; set; }

        public string UserNameId { get; set; }
    }
}
