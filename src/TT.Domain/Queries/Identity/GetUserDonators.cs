﻿using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.DTOs.Identity;
using TT.Domain.Entities.Identity;

namespace TT.Domain.Queries.Identity
{
    public class GetUserDonators : DomainQuery<UserDonatorDetail>
    {

        public int MinimumTier { get; set; }

        public override IEnumerable<UserDonatorDetail> Execute(IDataContext context)
        {
            ContextQuery =
                ctx =>
                    ctx.AsQueryable<User>()
                        .Where(m => m.Donator != null)
                        .Where(m => m.Donator.Tier >= MinimumTier)
                        .ProjectToQueryable<UserDonatorDetail>();

            return ExecuteInternal(context);
        }
    }
}