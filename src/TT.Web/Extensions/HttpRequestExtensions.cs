using System.Web;
using FeatureSwitch;
using TT.Domain;

namespace TT.Web.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetRealUserHostAddress(this HttpRequestBase request)
        {
            return FeatureContext.IsEnabled<UseCloudflare>() &&
                   !string.IsNullOrEmpty(request.ServerVariables["HTTP_CF_CONNECTING_IP"])
                ? request.ServerVariables["HTTP_CF_CONNECTING_IP"]
                : request.UserHostAddress;
        }
    }
}