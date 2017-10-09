using Highway.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Domain.Identity.CommandRequests;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.CommandHandlers
{
    public class ResetSecurityStampHandler : IAsyncRequestHandler<ResetSecurityStamp>
    {
        private readonly IDataContext context;

        public ResetSecurityStampHandler(IDataContext context)
        {
            this.context = context;
        }

        public async Task Handle(ResetSecurityStamp message)
        {
            var userQuery = from user in context.AsQueryable<UserSecurityStamp>()
                       where user.Id == message.TargetUserNameId
                       select user;

            var userEntity = await userQuery.FirstAsync();

            userEntity.ResetSecurityStamp(message);

            await context.CommitAsync();
        }
    }
}
