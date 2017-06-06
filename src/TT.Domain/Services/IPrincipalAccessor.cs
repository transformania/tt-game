using System.Collections.Generic;
using System.Security.Principal;

namespace TT.Domain.Services
{
    public interface IPrincipalAccessor
    {
        IPrincipal RequestPrincipal { get; }
        string RequestUserId { get; }
        IEnumerable<string> Roles { get; }
    }
}
