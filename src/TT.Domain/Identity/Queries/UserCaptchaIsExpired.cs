using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class UserCaptchaIsExpired : DomainQuerySingle<bool>
    {
        public string UserId { get; set; }

        public override bool Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var output = ctx.AsQueryable<CaptchaEntry>()
                            .Where(m => m.User.Id == UserId)
                            .ProjectToFirstOrDefault<CaptchaEntryDetail>();

                if (output == null)
                {
                    throw new DomainException($"User with Id {UserId} has no CaptchaEntry");
                }

                if (output.ExpirationTimestamp < DateTime.UtcNow)
                    return true;

                return false;

            };

            return ExecuteInternal(context);
        }
    }
}
