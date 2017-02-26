using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Commands
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
                    throw new DomainException($"User with Id {UserId} could not be found");

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
