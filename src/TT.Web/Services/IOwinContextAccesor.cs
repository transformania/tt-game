using Microsoft.Owin;

namespace TT.Web.Services
{
    public interface IOwinContextAccessor
    {
        IOwinContext CurrentContext { get; }
    }
}