using MediatR;

namespace TT.Domain.Identity.QueryRequests
{
    public class IsValidUserRequest : IRequest<bool>
    {
        public string UserNameId { get; set; }
    }
}
