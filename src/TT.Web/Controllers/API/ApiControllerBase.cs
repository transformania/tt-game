using System;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace TT.Web.Controllers.API
{
    public class ApiControllerBase : ApiController
    {
        protected Func<string> GetUserId { get; private set; }

        public ApiControllerBase()
        {
            GetUserId = () => User.Identity.GetUserId();
        }

        public void OverrideGetUserId(Func<string> getUserId)
        {
            GetUserId = getUserId;
        }
    }
}