using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TT.Web.Attributes
{
    public sealed class ValidateWebApiAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {
        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            try
            {
                string cookieToken = "";
                string formToken = "";

                if (actionContext.Request.Headers.TryGetValues("RequestVerificationToken", out var tokenHeaders))
                {
                    string[] tokens = tokenHeaders.FirstOrDefault()?.Split(':');
                    if (tokens != null && tokens.Length == 2)
                    {
                        cookieToken = tokens[0].Trim();
                        formToken = tokens[1].Trim();
                    }
                }

                AntiForgery.Validate(cookieToken, formToken);
            }
            catch (System.Web.Mvc.HttpAntiForgeryException)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden, new { Error = "Invalid security token." });

                return Task.FromResult(actionContext.Response);
            }

            return continuation();
        }
    }
}