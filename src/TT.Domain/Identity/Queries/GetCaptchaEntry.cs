using System.Data.Entity;
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
                var entry = ctx
                    .AsQueryable<CaptchaEntry>()
                    .Include(p => p.User)
                    .FirstOrDefault(p => p.User.Id == UserId);

                return entry?.MapToDto();
            };

            return ExecuteInternal(context);
        }
    }
}
