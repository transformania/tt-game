using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace TT.Domain.Services
{
    public interface IPrincipalAccessor
    {
        /// <summary>
        /// Returns the principal.
        /// </summary>
        IPrincipal RequestPrincipal { get; }

        /// <summary>
        /// Returns the user ID.
        /// </summary>
        string UserNameId { get; }

        /// <summary>
        /// Returns the user name.
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// Returns the roles that the principal has.
        /// </summary>
        IEnumerable<string> Roles { get; }

        /// <summary>
        /// Returns the claims that the principal.
        /// </summary>
        IEnumerable<Claim> Claims { get; }
    }
}
