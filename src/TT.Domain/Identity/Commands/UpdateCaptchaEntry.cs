using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
{
    public class UpdateCaptchaEntry : DomainCommand<int>
    {
        public string UserId { get; set; }
        public bool AddPassAttempt { get; set; }
        public bool AddFailAttempt { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {

                var entry = ctx.AsQueryable<CaptchaEntry>().SingleOrDefault(t => t.User.Id == UserId);

                if (entry == null)
                    throw new DomainException($"CaptchaEntry with Id {UserId} could not be found");

                if (AddPassAttempt)
                {
                    entry.AddPassAttempt();
                    entry.UpdateExpirationTimestamp();
                }


                if (AddFailAttempt)
                {
                    entry.AddFailAttempt();
                }

                ctx.Update(entry);
                ctx.Commit();

                result = entry.Id;
            };

            ExecuteInternal(context);

            return result;
        }
    }
}
