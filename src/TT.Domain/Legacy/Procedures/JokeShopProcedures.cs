﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Legacy.Procedures
{
    public static class JokeShopProcedures
    {
        // Potential pranks
        // Random animate TF, body swap, tg, turned into psycho, body and name swap, temp inan, inan keeping inventory
        // Summon a psycho/doppelganger
        // Mind control e.g. force move
        // Ban
        // Nothing
        // Find or spend cash
        // Curse
        // Teleport - kick out, town (rooted?), dungeon (rooted?) in combat
        // Bounty
        // Name change/family
        // Join coven
        // Shrink to SP item
        // Invisibility
        // Silenced
        // Blinded
        // Disorientated - move in random direction
        // Psycho nip
        // Challenge
        // Dice roll
        // Act as form MC

        // TODO joke_shop These IDs need to be synced with migration script
        const int FIRST_WARNING_EFFECT = 1006; //
        const int SECOND_WARNING_EFFECT = 1010; //
        const int BANNED_FROM_JOKE_SHOP_EFFECT = 1012; //

        # region Location action hooks

        public static string Search(Player player)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            var rand = new Random();

            // TODOD joke_shop Re-enable regular search chance
            /*
            // Decide whether this is a regular or a prank search
            if (rand.NextDouble() < 0.75)
            {
                return null;
            }
            */

            var roll = rand.NextDouble() * 100;

            if (roll < 60)  // 60% chance:
            {
                return MildPrank();
            }
            else if (roll < 80)  // 20% chance:
            {
                return MischievousPrank(player);
            }
            else if (roll < 87)  // 7% chance:
            {
                return MeanPrank(player);
            }

            // 13% chance:
            return BanCharacter(player);
        }

        public static string Meditate(Player player)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            return null;
        }

        public static string Cleanse(Player player)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            return null;
        }

        public static string SelfRestore(Player player)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            return null;
        }

        #endregion

        #region Prank selection

        private static string MildPrank()
        {
            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            // TODO joke_shop select prank

            // TODO joke_shop return value
            return "Minor prank";
            //return null;
        }

        private static string MischievousPrank(Player player)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            // TODO joke_shop select prank

            // TODO joke_shop return value
            return "Regular prank";
            //return null;
        }

        private static string MeanPrank(Player player)
        {
            var warning = EnsurePlayerIsWarnedTwice(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            // TODO joke_shop select prank

            // TODO joke_shop return value
            return "Major prank";
            //return null;
        }

        #endregion

        #region Warnings and access controls

        private static string EnsurePlayerIsWarned(Player player)
        {
            if (!EffectProcedures.PlayerHasEffect(player, FIRST_WARNING_EFFECT))
            {
                EffectProcedures.GivePerkToPlayer(FIRST_WARNING_EFFECT, player);

                var logMessage = "Beware!  This cursed joke shop is a dangerous place!  If you stay here too long anything could happen.  Maybe you should keep your nose out of trouble and leave now?";
                PlayerLogProcedures.AddPlayerLog(player.Id, logMessage, false);
                return logMessage;
            }

            // Already recently warned
            return null;
        }

        private static string EnsurePlayerIsWarnedTwice(Player player)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            if (!EffectProcedures.PlayerHasEffect(player, SECOND_WARNING_EFFECT))
            {
                EffectProcedures.GivePerkToPlayer(SECOND_WARNING_EFFECT, player);

                var logMessage = "This is your final warning!  This cursed joke shop does not play by the normal rules of Sunnyglade.  If you stay here too long you could be risking your very soul!  Get out now - while you still can!";
                PlayerLogProcedures.AddPlayerLog(player.Id, logMessage, true);
                return logMessage;
            }

            // Already recently reminded
            return null;
        }

        public static bool CharacterIsBanned(Player player)
        {
            return EffectProcedures.PlayerHasActiveEffect(player, BANNED_FROM_JOKE_SHOP_EFFECT);
        }

        private static string BanCharacter(Player player)
        {
            if (EffectProcedures.PlayerHasEffect(player, BANNED_FROM_JOKE_SHOP_EFFECT))
            {
                return "Already banned!";
                //return null;
            }

            var kickedOutMessage = EjectCharacter(player);
            EffectProcedures.GivePerkToPlayer(BANNED_FROM_JOKE_SHOP_EFFECT, player);

            return "Your searching attracts the attention of the shopkeeper, who bans you from the shop!  " + kickedOutMessage;
        }

        private static string EjectCharacter(Player player)
        {
            var jokeShop = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == LocationsStatics.JOKE_SHOP);

            if (jokeShop == null)
            {
                return null;
            }

            var street = jokeShop.Name_North;
            street = (street == null) ? jokeShop.Name_South : street;
            street = (street == null) ? jokeShop.Name_East : street;
            street = (street == null) ? jokeShop.Name_West : street;

            if (street == null)
            {
                return null;
            }

            var streetTile = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == street);

            if (streetTile == null)
            {
                return null;
            }

            var leavingMessage = $"{player.GetFullName()} is kicked out of the {jokeShop.Name} towards {streetTile.Name}";
            var enteringMessage = $"{player.GetFullName()} lands here with a thud after being kicked out of the {jokeShop.Name}";

            var playerLog = $"You are kicked out of the <b>{jokeShop.Name}</b> and land in <b>{streetTile.Name}</b>.";

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            // user.dbLocationName = street;  // TODO joke_shop disabled while testing
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, playerLog, false);
            LocationLogProcedures.AddLocationLog(LocationsStatics.JOKE_SHOP, leavingMessage);
            LocationLogProcedures.AddLocationLog(street, enteringMessage);

            return null;
            // return playerLog;  // TODO joke_shop disabled while testing
        }

        #endregion

    }
}
