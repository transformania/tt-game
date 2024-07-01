﻿using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Identity.DTOs;
using TT.Domain.Identity.Entities;

namespace TT.Domain.Identity.Queries
{
    public class GetPlayerStats : DomainQuery<StatDetail>
    {

        public string OwnerId { get; set; }

        public override IEnumerable<StatDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var stats = ctx.AsQueryable<Stat>()
                           .Where(p => p.Owner.Id == OwnerId)
                           .ToList();
                return stats.Select(p => p.MapToDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
