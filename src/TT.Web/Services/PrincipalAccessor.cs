using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Web.Security;
using TT.Domain.Services;

namespace TT.Web.Services
{
    public class PrincipalAccessor : IPrincipalAccessor
    {
        /// <summary>
        /// Returns the user for this owin or hub request.
        /// </summary>
        /// <remarks>The user will be retrieved from Owin first. If null, the user will be retrieved from a hub request.</remarks>
        public IPrincipal RequestPrincipal => _owinContextAccessor.CurrentContext?.Request.User ?? _hubRequestAccessor.Request?.User ?? Thread.CurrentPrincipal;

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

        private IOwinContextAccessor _owinContextAccessor;
        private readonly IHubRequestAccessor _hubRequestAccessor;

        public PrincipalAccessor(IOwinContextAccessor owinContextAccessor, IHubRequestAccessor hubRequestAccessor)
        {
            _owinContextAccessor = owinContextAccessor;
            _hubRequestAccessor = hubRequestAccessor;
        }
    }
}