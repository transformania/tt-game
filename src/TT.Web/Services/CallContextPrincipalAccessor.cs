using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Security;
using TT.Domain.Services;

namespace TT.Web.Services
{
    public class CallContextPrincipalAccessor : IPrincipalAccessor
    {
        private IOwinContextAccessor _owinContextAccessor { get; }

        public IPrincipal RequestPrincipal => _owinContextAccessor.CurrentContext.Request.User;

        public string RequestUserId => RequestPrincipal?.Identity.GetUserId() ?? "";

        public IEnumerable<string> Roles => (RequestPrincipal as RolePrincipal)?.GetRoles() ?? new string[] { };

        public CallContextPrincipalAccessor(IOwinContextAccessor owinContextAccessor)
        {
            _owinContextAccessor = owinContextAccessor;
        }
    }
}