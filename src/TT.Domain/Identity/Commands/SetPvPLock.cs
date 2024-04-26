using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
{
    public class SetPvPLock : DomainCommand
    {

        public string UserId { get; set; }
        public string LockoutMessage { get; set; }
        public bool PvPLock { get; set; }

        public override void Execute(IDataContext context)
        {

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == UserId);
                if (user == null)
                    throw new DomainException($"User with Id '{UserId}' could not be found");

                user.SetPvPLockChanges(PvPLock);
                user.SetPvPLockoutMessage(LockoutMessage);

                ctx.Update(user);
                ctx.Commit();

            };

            ExecuteInternal(context);

        }

        protected override void Validate()
        {
            if (string.IsNullOrWhiteSpace(UserId))
                throw new DomainException("UserId is required");
        }
    }
}
