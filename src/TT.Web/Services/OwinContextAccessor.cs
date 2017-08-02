using Microsoft.Owin;
using System.Collections.Generic;

namespace TT.Web.Services
{
    public class OwinContextAccessor : IOwinContextAccessor
    {
        public IOwinContext CurrentContext { get; set; }

        IOwinContext IOwinContextAccessor.CurrentContext => CurrentContext ?? new OwinContext(new Dictionary<string, object>());
    }
}