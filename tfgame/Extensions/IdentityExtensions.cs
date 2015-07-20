using System;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace tfgame.Extensions
{
    public static class IdentityExtensions
    {
        public static int GetCurrentUserId(this IIdentity identity)
        {
            string strUserId = identity.GetUserId();
            int intUserId = (strUserId != null) ? Convert.ToInt32(strUserId) : -1;
            return intUserId;
        }
    }
}