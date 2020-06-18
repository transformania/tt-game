using Highway.Data;
using MediatR;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TT.Domain.Identity.CommandRequests;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.CommandHandlers
{
    public class ResetSecurityStampHandler : IRequestHandler<ResetSecurityStamp>
    {
        private readonly IDataContext context;

        public ResetSecurityStampHandler(IDataContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(ResetSecurityStamp message, CancellationToken cancellationToken)
        {
            var userQuery = from user in context.AsQueryable<UserSecurityStamp>()
                       where user.Id == message.TargetUserNameId
                       select user;

            var userEntity = await userQuery.FirstAsync(cancellationToken);

            userEntity.ResetSecurityStamp(message);

            await context.CommitAsync();
            return new Unit();
        }
    }
}
