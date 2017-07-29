using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Security;
using TT.Domain.Services;

namespace TT.Web.Services
{
    public class CallContextPrincipalAccessor : IPrincipalAccessor
    {
        /// <summary>
        /// Returns the user for this owin request.
        /// </summary>
        public IPrincipal RequestPrincipal => _owinContextAccessor.CurrentContext.Request.User;

        /// <summary>
        /// Returns the user name ID claim that the request user has.
        /// </summary>
        public string UserNameId => Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";

        /// <summary>
        /// Returns the user name claims that the request user has
        /// </summary>
        public string UserName => Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

        /// <summary>
        /// Returns all claims that the request user has.
        /// </summary>
        public IEnumerable<Claim> Claims => (RequestPrincipal as ClaimsPrincipal)?.Claims ?? Enumerable.Empty<Claim>();

        /// <summary>
        /// Returns all claims that are roles.
        /// </summary>
        public IEnumerable<string> Roles => Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

        private IOwinContextAccessor _owinContextAccessor { get; }

        public CallContextPrincipalAccessor(IOwinContextAccessor owinContextAccessor)
        {
            _owinContextAccessor = owinContextAccessor;
        }
    }
}