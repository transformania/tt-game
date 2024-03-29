﻿using System.Collections.Generic;
using System.Linq;
using Highway.Data;
using TT.Domain.Combat.DTOs;

namespace TT.Domain.Combat.Queries
{
    public class GetMindControlsForPlayer : DomainQuery<VictimMindControlDetail>
    {

        public int OwnerId { get; set; }

        public override IEnumerable<VictimMindControlDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx => ctx.AsQueryable<Entities.MindControl.VictimMindControl>().ProjectToQueryable<VictimMindControlDetail>().Where(i => i.Victim.Id == OwnerId);
            return ExecuteInternal(context);
        }

    }
}
