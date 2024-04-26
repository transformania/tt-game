using System;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
{
    public class SetLockoutTimestamp : DomainCommand<string>
    {
        public string UserId { get; set; }
        public DateTime date { get; set; }
        public String message { get; set; }

        public override string Execute(IDataContext context)
        {

            var output = "";

            ContextQuery = ctx =>
            {

                var user = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == UserId);

                if (user == null)
                    throw new DomainException($"User with Id '{UserId}' does not exist");

                user.SetLockoutEndDateUtc(date);
                user.SetAccountLockoutMessage(message);

                ctx.Update(user);
                ctx.Commit();

                output = $"Lockout date set to {date} for UserId {UserId}";

            };

            ExecuteInternal(context);
            return output;

        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new DomainException("userId is required");
        }
    }
}
