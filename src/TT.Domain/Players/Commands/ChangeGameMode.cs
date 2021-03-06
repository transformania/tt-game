﻿using System;
using System.Linq;
using System.Data.Entity;
using Highway.Data;
using TT.Domain.Exceptions;
using TT.Domain.Players.Entities;
using TT.Domain.Items.Entities;
using TT.Domain.Statics;
using TT.Domain.Procedures;
using TT.Domain.Items.Commands;

namespace TT.Domain.Players.Commands
{
    public class ChangeGameMode : DomainCommand
    {

        public string MembershipId { get; set; }
        public int GameMode { get; set; }

        public bool Force { get; set; }

        public override void Execute(IDataContext context)
        {
            ContextQuery = ctx =>
            {
                var player = ctx.AsQueryable<Player>()
                    .Include(p => p.Items)
                    .Include(p => p.Items.Select(i => i.ItemSource))
                    .SingleOrDefault(cr => cr.User.Id == MembershipId);

                if (player == null)
                    throw new DomainException($"Player with MembershipID '{MembershipId}' could not be found");

                if (player.GameMode == GameMode)
                    throw new DomainException("You are already in this game mode.");

                if (player.GameMode == (int)GameModeStatics.GameModes.PvP && player.IsInDungeon())
                    throw new DomainException("You cannot switch out of PvP mode while you are in the dungeon.");

                if (!Force)
                {
                    var WhenLastCombat = player.LastCombatTimestamp;

                    //Compare combat time stamps to get the most recent.
                    if (player.LastCombatTimestamp < player.LastCombatAttackedTimestamp)
                    {
                        WhenLastCombat = player.LastCombatAttackedTimestamp;
                    }
                    
                    //Grab dates and get the minutes.
                    var GetCombatMinutes = Math.Abs(Math.Floor(WhenLastCombat.Subtract(DateTime.UtcNow).TotalMinutes));

                    //Evaluate crap.
                    if (GetCombatMinutes < 30 && player.GameMode == (int)GameModeStatics.GameModes.PvP)
                        throw new DomainException("You cannot leave PvP mode until you have been out of combat for thirty (30) minutes.");

                    if ((player.GameMode == (int)GameModeStatics.GameModes.Protection || player.GameMode == (int)GameModeStatics.GameModes.Superprotection) && GameMode == (int)GameModeStatics.GameModes.PvP)
                        throw new DomainException("You cannot switch into that mode during regular gameplay.");

                    //Remove a player's Dungeon Points whenever they switch into P/SP
                    player.ClearPvPScore();
                }

                var itemGameMode = GameMode;

                if(itemGameMode == (int)GameModeStatics.GameModes.Superprotection)
                {
                    itemGameMode = (int)GameModeStatics.GameModes.Protection;
                }

                ChangeItemsAndRunesMode(player.Items, itemGameMode);

                var soulboundItems = context.AsQueryable<Item>()
                        .Where(i => i.SoulboundToPlayer != null
                                 && i.SoulboundToPlayer.Id == player.Id)
                        .ToList();
                ChangeItemsAndRunesMode(soulboundItems, itemGameMode);

                player.ChangeGameMode(GameMode);

                ctx.Commit();
            };

            ExecuteInternal(context);
        }

        private static void ChangeItemsAndRunesMode(System.Collections.Generic.IEnumerable<Item> items, int mode)
        {
            foreach (var item in items)
            {
                if (item.PvPEnabled != (int)GameModeStatics.GameModes.Any)
                {
                    item.ChangeGameMode(mode);
                }

                if (item.Runes != null)
                {
                    foreach (var rune in item.Runes)
                    {
                        if (item.PvPEnabled != (int)GameModeStatics.GameModes.Any)
                        {
                            rune.ChangeGameMode(mode);
                        }
                    }
                }
            }
        }

        protected override void Validate()
        {
            if (MembershipId == null)
                throw new DomainException("MembershipID is required!");

            if (GameMode != (int)GameModeStatics.GameModes.Superprotection && GameMode != (int)GameModeStatics.GameModes.Protection && GameMode != (int)GameModeStatics.GameModes.PvP)
                throw new DomainException("Game mode selection is invalid");

        }

    }
}
