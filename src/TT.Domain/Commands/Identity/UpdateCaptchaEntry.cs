using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highway.Data;
using TT.Domain.Entities.Identities;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Commands.Identity
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
                    throw new DomainException(string.Format("CaptchaEntry with Id {0} could not be found", UserId));

                if (AddPassAttempt)
                {
                    entry.AddPassAttempt();
                    entry.UpdateExpirationTimestamp();
                }


                if (AddFailAttempt)
                {
                    entry.AddFailAttempt();
                }

                ctx.Add(entry);
                ctx.Commit();

                result = entry.Id;
            };

            ExecuteInternal(context);

            return result;
        }
    }
}
