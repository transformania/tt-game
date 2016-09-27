
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Identities;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Commands.Identity
{
    public class CreateDonator : DomainCommand
    {

        public string UserId { get; set; }
        public int Tier { get; set; }
        public int ActualDonationAmount { get; set; }
        public string SpecialNotes { get; set; }
        public string PatreonName { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {

                var user = ctx.AsQueryable<User>().Include(d => d.Donator).SingleOrDefault(t => t.Id == UserId);

                if (user == null)
                    throw new DomainException(string.Format("User '{0}' could not be found", UserId));

                var oldUser =
                    ctx.AsQueryable<User>()
                        .Include(d => d.Donator)
                        .Where(u => u.Donator != null && u.Donator.Owner.Id == UserId)
                        .SingleOrDefault(t => t.Id == UserId);

                if (oldUser != null)
                    throw new DomainException(
                        string.Format("User {0} already has a donation entry.  Please update that instead.",
                            oldUser.UserName));

                user.CreateDonator(this);

                ctx.Update(user);
                ctx.Commit();

            };

            ExecuteInternal(context);


        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new DomainException("userId is required");
        }
    }
}
