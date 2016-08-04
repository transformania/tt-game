using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identities;

namespace TT.Domain.Queries.Identity
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
