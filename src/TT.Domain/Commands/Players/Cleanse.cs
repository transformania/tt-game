﻿using System.Data.Entity;
using System.Linq;
using Highway.Data;
using TT.Domain.Entities.LocationLogs;
using TT.Domain.Entities.Players;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Commands.Players
{
    public class Cleanse : DomainCommand<string>
    {

        public int PlayerId { get; set; }
        public BuffBox Buffs { get; set; }
        public bool NoValidate { get; set; }

        public override string Execute(IDataContext context)
        {

            var result = "";

            ContextQuery = ctx =>
            {

                var player = ctx.AsQueryable<Entities.Players.Player>()
                    .Include(p => p.TFEnergies)
                    .Include(p => p.PlayerLogs)
                    .Include(p => p.User)
                    .Include(p => p.User.Achievements)
                    .SingleOrDefault(cr => cr.Id == PlayerId);

                if (player == null)
                    throw new DomainException($"Player with ID '{PlayerId}' could not be found");

                if (!NoValidate)
                {
                    if (player.ActionPoints < PvPStatics.CleanseCost)
                        throw new DomainException("You don't have enough action points to cleanse!");

                    if (player.Mobility != PvPStatics.MobilityFull)
                        throw new DomainException("You must be animate in order to cleanse!");

                    if (player.Mana < PvPStatics.CleanseManaCost)
                        throw new DomainException("You don't have enough mana to cleanse!");

                    if (player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
                        throw new DomainException("You have cleansed and meditated the maximum number of times this update.");
                }

                result = player.Cleanse(Buffs);

                // log statistics only for human players
                if (player.BotId == AIStatics.ActivePlayerBotId)
                {
                    
                    var achievement = player.User.GetAchievement(StatsProcedures.Stat__TimesCleansed);
                    if (achievement != null)
                    {
                        achievement.AddAmount(1);
                        ctx.Update(achievement);
                    }
                    else
                    {
                        ctx.Add(Stat.Create(player.User, 1, StatsProcedures.Stat__TimesCleansed));
                    }
                }

                ctx.Update(player);

                // TODO: Perform this as part of the Cleanse method on the Player entity once known how
                var locationLog = LocationLog.Create(player.Location, $"<span class='playerCleansingNotification'>{player.GetFullName()} cleansed here.</span>", 0);
                ctx.Add(locationLog);

                ctx.Commit();
            };

            ExecuteInternal(context);

            return result;
        }

        protected override void Validate()
        {
            if (PlayerId <= 0)
                throw new DomainException("Player ID is required!");

            if (Buffs == null)
                throw new DomainException("Buffs are required!");
        }

    }
}
