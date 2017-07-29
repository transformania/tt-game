using Microsoft.Owin;
using System.Collections.Generic;

namespace TT.Web.Services
{
    public class CallContextOwinContextAccessor : IOwinContextAccessor
    {
        public IOwinContext CurrentContext { get; set; }

        IOwinContext IOwinContextAccessor.CurrentContext => CurrentContext ?? new OwinContext(new Dictionary<string, object>());
    }
}