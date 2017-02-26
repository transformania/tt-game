using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetCaptchaEntry : DomainQuerySingle<CaptchaEntryDetail>
    {

        public string UserId { get; set; }

        public override CaptchaEntryDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<CaptchaEntry>()
                           .Where(p => p.User.Id == UserId)
                           .ProjectToFirstOrDefault<CaptchaEntryDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
