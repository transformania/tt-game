﻿using Highway.Data;
using MediatR;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TT.Domain.Identity.Entities;
using TT.Domain.Identity.QueryRequests;

namespace TT.Domain.Identity.QueryRequestHandlers
{
    public class IsValidUserRequestHandler : IRequestHandler<IsValidUserRequest, bool>
    {
        private readonly IDataContext context;

        public IsValidUserRequestHandler(IDataContext context)
        {
            this.context = context;
        }

        public async Task<bool> Handle(IsValidUserRequest message, CancellationToken cancellationToken)
        {
            var userQuery = from u in context.AsQueryable<User>()
                            where u.Id == message.UserNameId
                            select u;

            return await userQuery.AnyAsync(cancellationToken);
        }
    }
}
