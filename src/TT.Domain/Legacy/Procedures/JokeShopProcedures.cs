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
        // TODO joke_shop These IDs need to be synced with migration script
        const int FIRST_WARNING_EFFECT = 1006; //
        const int SECOND_WARNING_EFFECT = 1010; //
        const int BANNED_FROM_JOKE_SHOP_EFFECT = 1012; //

        # region Location action hooks

        public static string Search(Player player)
        {
            if (CharacterIsBanned(player))
            {
                var attemptToEject = EjectCharacter(player);

                if (attemptToEject != null)
                {
                    return attemptToEject;
                }
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
                return MildPrank(player);
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

        private static string MildPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            if (roll < 100)
            {
                return MildResourcePrank(player);
            }

            // TODO joke_shop return value
            return "Mild prank";
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

            if (roll < 100)
            {
                return MischievousResourcePrank(player);
            }

            // TODO joke_shop return value
            return "Mischievous prank";
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

            if (roll < 100)
            {
                return MeanResourcePrank(player);
            }

            // TODO joke_shop return value
            return "Mean prank";
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

            return "Your actions attract the attention of the shopkeeper, who bans you from the shop!  " + kickedOutMessage;
        }

        private static string EjectCharacter(Player player)
        {
            var jokeShop = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == LocationsStatics.JOKE_SHOP);

            if (jokeShop == null)
            {
                return null;
            }

            var street = jokeShop.Name_North ?? jokeShop.Name_South ?? jokeShop.Name_East ?? jokeShop.Name_West;

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


        #region Resource pranks

        private static string MildResourcePrank(Player player)
        {
            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            if (roll < 27)  // 27%
            {
                return ChangeHealth(player, (int)(rand.NextDouble() * 125 - 50));
            }
            else if (roll < 54)  // 27%
            {
                return ChangeMana(player, (int)(rand.NextDouble() * 25 - 10));
            }
            else if (roll < 60)  // 6%
            {
                return ChangeActionPoints(player, (int)(rand.NextDouble() * 6 - 3));
            }
            else  // 40%
            {
                return ChangeMoney(player, (int)(rand.NextDouble() * 5 - 2));
            }
        }

        private static string MischievousResourcePrank(Player player)
        {
            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            if (roll < 30)  // 30%
            {
                return ChangeHealth(player, (int)(rand.NextDouble() * 250 - 125));
            }
            else if (roll < 60)  // 30%
            {
                return ChangeMana(player, (int)(rand.NextDouble() * 40 - 20));
            }
            else if (roll < 65)  // 5%
            {
                return ChangeActionPoints(player, (int)(rand.NextDouble() * 30 - 10));
            }
            else if (roll < 95)  // 30%
            {
                return ChangeMoney(player, (int)(rand.NextDouble() * 10 - 5));
            }
            else  // 5%
            {
                return ChangeDungeonPoints(player, 1);
            }
        }

        private static string MeanResourcePrank(Player player)
        {
            var rand = new Random();
            var roll = rand.NextDouble() * 100;

            if (roll < 40)  // 40%
            {
                return ChangeHealth(player, (int)(rand.NextDouble() * 500 - 300));
            }
            else if (roll < 60)  // 25%
            {
                return ChangeMana(player, (int)(rand.NextDouble() * 55 - 35));
            }
            else if (roll < 65)  // 5%
            {
                return ChangeActionPoints(player, (int)(rand.NextDouble() * 60 - 10));
            }
            else if (roll < 95)  // 25%
            {
                return ChangeMoney(player, (int)(rand.NextDouble() * 25 - 15));
            }
            else  // 5%
            {
                return ChangeDungeonPoints(player, (int)(rand.NextDouble() * 10 - 5));
            }
        }

        private static string ChangeHealth(Player player, int amount)
        {
            if(amount >= 0)
            {
                amount++;
            }

            var playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            var before = target.Health;
            target.Health += amount;
            target.NormalizeHealthMana();
            var after = target.Health;
            playerRepo.SavePlayer(target);

            var delta = after - before;
            if (delta == 0)
            {
                return null;
            }

            return $"Health changed by {delta}";  // TODO joke_shop flavor text
        }

        private static string ChangeMana(Player player, int amount)
        {
            if(amount >= 0)
            {
                amount++;
            }

            var playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            var before = target.Mana;
            target.Mana += amount;
            target.NormalizeHealthMana();
            var after = target.Mana;
            playerRepo.SavePlayer(target);

            var delta = after - before;
            if (delta == 0)
            {
                return null;
            }

            return $"Mana changed by {delta}";  // TODO joke_shop flavor text
        }

        private static string ChangeMoney(Player player, int amount)
        {
            if(amount >= 0)
            {
                amount++;
            }

            var playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            var before = target.Money;
            target.Money = Math.Max(0, target.Money + amount);
            var after = target.Money;
            playerRepo.SavePlayer(target);

            var delta = after - before;
            if (delta == 0)
            {
                return null;
            }

            return $"Arpeyjis changed by {delta}";  // TODO joke_shop flavor text
        }

        private static string ChangeActionPoints(Player player, int amount)
        {
            if(amount >= 0)
            {
                amount++;
            }

            var playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            var before = target.ActionPoints;
            target.ActionPoints = Math.Min(Math.Max(0, target.ActionPoints + amount), TurnTimesStatics.GetActionPointLimit());
            var after = target.ActionPoints;
            playerRepo.SavePlayer(target);

            var delta = after - before;
            if (delta == 0)
            {
                return null;
            }

            return $"Action points changed by {delta}";  // TODO joke_shop flavor text
        }

        private static string ChangeDungeonPoints(Player player, int amount)
        {
            if (player.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return null;
            }

            if(amount >= 0)
            {
                amount++;
            }

            var playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            var before = target.PvPScore;
            target.PvPScore = Math.Max(0, target.PvPScore + amount);
            var after = target.PvPScore;
            playerRepo.SavePlayer(target);

            var delta = after - before;
            if (delta == 0)
            {
                return null;
            }

            return $"Dungeon points changed by {delta}";  // TODO joke_shop flavor text
        }

        #endregion
    }
}
