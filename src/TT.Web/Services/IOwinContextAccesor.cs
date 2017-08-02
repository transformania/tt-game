using Microsoft.Owin;

namespace TT.Web.Services
{
    public interface IOwinContextAccessor
    {
        /// <summary>
        /// <para>The current Owin <see cref="IOwinContext"/>.</para>
        /// <para>If null then <see cref="IOwinContextAccessor"/> was resolved outside of an Owin request.</para>
        /// </summary>
        IOwinContext CurrentContext { get; }
    }
}