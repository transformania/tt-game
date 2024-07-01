﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Entities;
using TT.Domain.Items.Mappings;
using TT.Domain.Statics;

namespace TT.Domain.Items.Queries
{
    public class GetHighestLevelPlayerItems : DomainQuery<ItemFormerPlayerDetail>
    {
        public int Limit { get; set; }

        public override IEnumerable<ItemFormerPlayerDetail> Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var items = ctx.AsQueryable<Item>()
                    .Include(i => i.ItemSource)
                    .Include(i => i.FormerPlayer)
                    .Include(i => i.FormerPlayer.ItemXP)
                    .Where(i => i.FormerPlayer != null && i.FormerPlayer.BotId == AIStatics.ActivePlayerBotId)
                    .OrderByDescending(i => i.Level)
                    .ThenByDescending(i => i.FormerPlayer.ItemXP.Amount)
                    .Take(Limit)
                    .ToList();

                return items.Select(i => i.MapToFormerPlayerDto()).AsQueryable();
            };

            return ExecuteInternal(context);
        }
    }
}
