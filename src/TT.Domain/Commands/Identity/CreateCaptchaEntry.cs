using System.Linq;
using Highway.Data;
using TT.Domain.Entities.Identities;
using TT.Domain.Entities.Identity;
using TT.Domain.Statics;

namespace TT.Domain.Commands.Identity
{
    public class CreateCaptchaEntry : DomainCommand<int>
    {

        public string UserId { get; set; }

        public override int Execute(IDataContext context)
        {
            var result = 0;

            ContextQuery = ctx =>
            {
                var user = ctx.AsQueryable<User>().SingleOrDefault(t => t.Id == UserId);
                if (user == null)
                    throw new DomainException(string.Format("User with Id {0} could not be found", UserId));

                var entry = CaptchaEntry.Create(user);

                ctx.Add(entry);
                ctx.Commit();

                result = entry.Id;
            };

            ExecuteInternal(context);

            return result;
        }
    }
    
}
