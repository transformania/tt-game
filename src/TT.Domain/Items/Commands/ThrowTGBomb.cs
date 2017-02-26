using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Forms.Entities;
using TT.Domain.Players.Entities;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Items.Commands
{
    public class ThrowTGBomb : DomainCommand<string>
    {

        public int PlayerId { get; set; }
        public int ItemId { get; set; }
        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Item.ItemSource)
                    .Include(p => p.FormSource)
                    .Include(p => p.User)
                    .Include(p => p.User.Stats)
                    .FirstOrDefault(p => p.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"player with ID {PlayerId} could not be found.");

                var item = player.Items.FirstOrDefault(i => i.Id == ItemId);

                if (item == null)
                    throw new DomainException($"Item with ID {ItemId} could not be found or does not belong to you.");

                var cutoff = DateTime.UtcNow.AddMinutes(-PvPStatics.OfflineAfterXMinutes);

                var affectedPlayers = ctx.AsQueryable<Player>()
                    .Where(p => p.Location == player.Location &&
                     p.Id != PlayerId &&
                     p.GameMode != GameModeStatics.SuperProtection &&
                     p.InDuel <= 0 &&
                     p.InQuest <= 0 &&
                     (p.FormSource.FriendlyName == "Regular Guy" || p.FormSource.FriendlyName == "Regular Girl") &&
                     p.LastActionTimestamp > cutoff
                ).Include(p => p.FormSource);

                // TODO: Add a nullable FK on FormSource called SexSwapVersion that links to the swapped version, then do this all as a method on the entity
                // TODO: Remove the ToList when the nested query is removed
                foreach (var p in affectedPlayers.ToList())
                {
                    var oldFormSplit = p.FormSource.dbName.Split('_');
                    var newFormName = "";

                    if (oldFormSplit[0] == "woman")
                    {
                        newFormName = "man_" + oldFormSplit[1];
                    }
                    else
                    {
                        newFormName = "woman_" + oldFormSplit[1];
                    }
                    var newsource = ctx.AsQueryable<FormSource>().FirstOrDefault(f => f.dbName == newFormName);
                    p.ChangeForm(newsource);
                    p.AddLog($"You yelp and feel your body change to that of the opposite sex from <b>{player.GetFullName()}</b>'s use of a TG Splash Orb in your location!", true);
                }

                player.SetOnlineActivityToNow();
                player.UpdateItemUseCounter(1);

                if (affectedPlayers.Any())
                {
                    var xpGained = 5*affectedPlayers.Count();
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

                ctx.Update(player);
                ctx.Remove(item);

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
        }
    }
}
