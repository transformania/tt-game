﻿using System.Linq;
using Highway.Data;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Entities;

namespace TT.Domain.Players.Queries
{
    public class GetPlayerByUserId : DomainQuerySingle<PlayerDetail>
    {
        public string UserId { get; set; }

        public override PlayerDetail Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                return ctx.AsQueryable<Player>()
                            .Where(p => p.User.Id == UserId)
                            .ProjectToFirstOrDefault<PlayerDetail>();
            };

            return ExecuteInternal(context);
        }

    }
}
