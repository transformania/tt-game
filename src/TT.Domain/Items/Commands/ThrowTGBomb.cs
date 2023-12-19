﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.ViewModels;

namespace TT.Domain.Items.Commands
{
    public class ThrowTGBomb : DomainCommand<string>
    {

        public int PlayerId { get; set; }
        public int ItemId { get; set; }
        public BuffBox Buffs { get; set; }
        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Item.ItemSource)
                    .Include(p => p.User)
                    .Include(p => p.User.Stats)
                    .Include(p => p.FormSource)
                    .Include(p => p.FormSource.AltSexFormSource)
                    .FirstOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"player with ID {PlayerId} could not be found.");

                var item = player.Items.FirstOrDefault(i => i.Id == ItemId);

                if (item == null)
                    throw new DomainException($"Item with ID {ItemId} could not be found or does not belong to you.");

                var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

                var affectedPlayers = ctx.AsQueryable<Player>()
                    .Where(p => p.Location == player.Location &&
                     p.Id != PlayerId &&
                     p.GameMode != (int)GameModeStatics.GameModes.Superprotection &&
                     p.InDuel <= 0 &&
                     p.InQuest <= 0 &&
                     p.FormSource.AltSexFormSource != null &&
                     p.LastActionTimestamp > cutoff
                ).Include(p => p.FormSource)
                .Include(p => p.FormSource.AltSexFormSource)
                .ToList();

                // Will the player also dodge?
                var rand = new Random();
                int chance = rand.Next(0, 100);              
                var adjustedChance = chance + (Buffs.Agility() * 0.1) + (Buffs.Luck() * 0.1);

                // Different boosts for normal vs hard
                if (player.InHardmode)
                {
                    adjustedChance /= 2;
                }

                if (adjustedChance < 50)
                {
                    affectedPlayers.Add(player);
                }

                foreach (var p in affectedPlayers)
                {
                    p.ChangeForm(p.FormSource.AltSexFormSource);
                    p.AddLog($"You yelp and feel your body change to that of the opposite sex from <b>{player.GetFullName()}</b>'s use of a TG Splash Orb in your location!", true);
                }

                player.SetOnlineActivityToNow();
                player.UpdateItemUseCounter(1);

                if (affectedPlayers.Any())
                {
                    var xpGained = affectedPlayers.Where(p => p.Level >= player.Level - 2).Sum(p => p.Level < 3 ? 3 : p.Level);

                    player.AddXP(xpGained);

                    // can't do this as a LINQ select due to calling the a.GetFullName() method
                    var playerNames = new List<string>();
                    foreach (var a in affectedPlayers)
                    {
                        playerNames.Add(a.GetFullName());
                    }

                    result = $"You throw your TG Splash Orb and swap the sex of {affectedPlayers.Count()} other mages near you: {ListifyHelper.Listify(playerNames, true)} and gain <b>{xpGained}</b> XP!";
                    player.AddLog(result, false);

                    // log statistics only for human players
                    if (player.BotId == AIStatics.ActivePlayerBotId)
                    {
                        player.User.AddStat(StatsProcedures.Stat__TgOrbVictims, affectedPlayers.Count());
                    }

                }
                else
                {
                    result = "You throw your TG Splash Orb, but unfortunately nobody is affected in this location.";
                }
                var locationLog = LocationLog.Create(player.Location, $"{player.GetFullName()} threw a TG splash orb, affecting {affectedPlayers.Count()} mages.");
                ctx.Add(locationLog);
                ctx.Update(player);

                ctx.Commit();

            };

            ExecuteInternal(context);
            return result; // TODO: use event handler
        }

        protected override void Validate()
        {
            if (PlayerId == 0)
                throw new DomainException("PlayerId is required.");

            if (ItemId == 0)
                throw new DomainException("ItemId is required.");

            if (Buffs == null)
                throw new DomainException("Buffs are missing.");
        }
    }
}
