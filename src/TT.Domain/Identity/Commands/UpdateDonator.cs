using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
{ 
    public class UpdateDonator : DomainCommand
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
                    throw new DomainException($"User '{UserId}' could not be found");

                user.UpdateDonator(this);

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

