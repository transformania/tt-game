using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Queries.Identity
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
