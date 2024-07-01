using System.Data.Entity;
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
                var donator = ctx
                    .AsQueryable<User>()
                    .Include(m => m.Donator)
                    .FirstOrDefault(p => p.Donator.Id == Id);

                return donator?.MapToDonatorDto();
            };

            return ExecuteInternal(context);
        }
    }
}
