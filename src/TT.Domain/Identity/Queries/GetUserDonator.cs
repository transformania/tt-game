using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetUserDonator : DomainQuerySingle<UserDonatorDetail>
    {

        public int Id { get; set; }

        public override UserDonatorDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<User>()
                           .Where(p => p.Donator.Id == Id)
                           .ProjectToFirstOrDefault<UserDonatorDetail>();
            };

            return ExecuteInternal(context);
        }
    }
}
