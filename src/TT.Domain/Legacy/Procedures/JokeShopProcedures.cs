﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;

namespace TT.Domain.Legacy.Procedures
{
    public static class JokeShopProcedures
    {
        // Get the IDs for the desired effects in a cross-database safe way
        private static readonly int? FIRST_WARNING_EFFECT = EffectWithName("effect_Joke_Shop_Warned");
        private static readonly int? SECOND_WARNING_EFFECT = EffectWithName("effect_Joke_Shop_Warned_Twice");
        private static readonly int? BANNED_FROM_JOKE_SHOP_EFFECT = EffectWithName("effect_Joke_Shop_Banned");

        private static readonly int[] BOOST_EFFECTS = EffectsWithNamesStarting("effect_Joke_Shop_Boost_");
        private static readonly int[] PENALTY_EFFECTS = EffectsWithNamesStarting("effect_Joke_Shop_Penalty_");

        private static readonly int? ROOT_EFFECT = EffectWithName("effect_Joke_Shop_Penalty_Mobility");
        private static readonly int? SNEAK_REVEAL_1 = EffectWithName("effect_Joke_Shop_Track_1");
        private static readonly int? SNEAK_REVEAL_2 = EffectWithName("effect_Joke_Shop_Track_2");
        private static readonly int? SNEAK_REVEAL_3 = EffectWithName("effect_Joke_Shop_Track_3");

        private static readonly int? AUTO_RESTORE_EFFECT = EffectWithName("effect_Joke_Shop_Auto_Restore");
        private static readonly int? INSTINCT_EFFECT = EffectWithName("effect_Joke_Shop_MC_Instinct");

        private const string LIMITED_MOBILITY = "immobile";

        private static readonly List<FormDetail> STABLE_FORMS = CandidateForms();

        public static readonly int[] MISCHIEVOUS_FORMS = {215, 221, 438};
        public static readonly int[] CATS_AND_NEKOS = {39, 100, 385, 434, 504, 572, 575, 668, 673, 681, 703, 713, 733, 752, 761, 806, 849, 851, 855, 987, 991, 1034, 1060, 1098, 1105, 1188, 1202};
        public static readonly int[] DOGS = {34, 359, 552, 667, 911, 912, 995, 1043, 1074, 1108, 1123, 1187};
        public static readonly int[] RODENTS = {70, 143, 205, 271, 278, 279, 317, 318, 319, 522, 772, 1077};
        public static readonly int[] TREES = {50, 741};
        public static readonly int[] STRIPPERS = {153, 719, 880};
        public static readonly int[] DRONES = {715, 930, 951, 1039, 1050};
        public static readonly int[] SHEEP = {204, 950, 1022, 1035, 1198};
        public static readonly int[] MAIDS = {65};  // TODO joke_shop more maid forms

        internal class FormDetail
        {
            public int FormSourceId { get; }
            public string FriendlyName { get; }
            public string Category { get; }

            public FormDetail(int formSourceId, string friendlyName, string category)
            {
                FormSourceId = formSourceId;
                FriendlyName = friendlyName;
                Category = category;
            }
        }

        private static List<FormDetail> CandidateForms()
        {
            IDbStaticSkillRepository skillsRepo = new EFDbStaticSkillRepository();
            var learnableSpells = skillsRepo.DbStaticSkills.Where(spell => spell.IsLive == "live" && spell.IsPlayerLearnable).Select(spell => new {spell.FormSourceId}).ToList();

            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var forms = formsRepo.DbStaticForms.Select(form => new {form.FriendlyName, Immobile = form.MoveActionPointDiscount < -5, form.Id, form.MobilityType, form.ItemSourceId});

            var learnableForms = learnableSpells.Join(forms, spell => spell.FormSourceId, form => form.Id, (spell, form) => new {form.Id, form.FriendlyName, form.Immobile, form.MobilityType, form.ItemSourceId});

            IDbStaticItemRepository itemsRepo = new EFDbStaticItemRepository();
            var itemTypes = itemsRepo.DbStaticItems.Select(i => new {i.Id, i.ItemType}).ToList();

            var formDetails = learnableForms.Join(itemTypes, form => form.ItemSourceId, itemType => itemType.Id, (form, item) => new FormDetail(form.Id, form.FriendlyName, item.ItemType)).ToList();
            formDetails.AddRange(learnableForms.Where(form => form.ItemSourceId == null && form.MobilityType == PvPStatics.MobilityFull).Select(form => new FormDetail(form.Id, form.FriendlyName, form.Immobile ? LIMITED_MOBILITY : form.MobilityType)));

            return formDetails;
        }

        private static List<FormDetail> Forms(Func<FormDetail, bool> predicate)
        {
            return STABLE_FORMS.Where(predicate).ToList();
        }

        private static int? EffectWithName(string dbName)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.dbName == dbName).FirstOrDefault()?.Id;
        }

        private static int[] EffectsWithNamesStarting(string prefix)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.dbName.StartsWith(prefix)).Select(e => e.Id).ToArray();
        }

        internal static void RunEffectActions(List<Effect> temporaryEffects)
        {
            // Undo temporary TFs
            if (AUTO_RESTORE_EFFECT.HasValue)
            {
                var playersToRestore = temporaryEffects.Where(e => e.EffectSourceId == AUTO_RESTORE_EFFECT.Value && e.Duration == 0).Select(e => e.OwnerId);
                foreach (var player in playersToRestore)
                {
                    UndoTemporaryForm(player);
                }
            }

            // TODO joke_shop call this later on in world update?
            if (INSTINCT_EFFECT.HasValue)
            {
                var playersToControl = temporaryEffects.Where(e => e.EffectSourceId == INSTINCT_EFFECT.Value).Select(e => e.OwnerId).ToList();
                InstinctProcedures.ActOnInstinct(playersToControl);
            }
        }

        #region Location action hooks

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
            if (rand.Next(3) != 0)  // Attempt a prank 1 time in 3, else normal search
            {
                return null;
            }
            */

            var roll = 101;  // TODO joke_shop temp // rand.Next(100);

            if (roll < 60)  // 60% chance:
            {
                return IdentityChange(player);
                //return MildPrank(player);
            }
            else if (roll < 80)  // 20% chance:
            {
                return TransformToMindControlledForm(player);
                // return MischievousPrank(player);
            }
            else if (roll < 87)  // 7% chance:
            {
                return MeanPrank(player);
            }

            // 13% chance:
            //return BanCharacter(player);

            // TODO joke_shop debug temp
            EnsurePlayerIsWarned(player);
            EnsurePlayerIsWarnedTwice(player);
            TryAnimateTransform(player, MAIDS[0]);
            //TryAnimateTransform(player, new Random().Next(2) == 0 ? CATS_AND_NEKOS[0] : RODENTS[0]);
            GiveEffect(player, INSTINCT_EFFECT);
            return "DEBUG DONE";
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

            // Mechanism through which a player can elect to restore their original name
            if (player.Health >= player.MaxHealth)
            {
                return RestoreName(player);
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
            var roll = rand.Next(100);

            if (roll < 100)
            {
                // return MildResourcePrank(player);
                // return MildLocationPrank(player);
                // return MildQuotasAndTimerPrank(player);
                // return MildTransformationPrank(player);
                return MildEffectsPrank(player);
                // return DiceGame(player);

                // return AnimateTransform(player);
                //return ImmobileTransform(player, false);
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
            var roll = rand.Next(100);

            if (roll < 100)
            {
                // return MischievousResourcePrank(player);
                // return MischievousLocationPrank(player);
                // return MischievousQuotasAndTimerPrank(player);
                // return MischievousTransformationPrank(player);
                // return MischievousTransformationPrank(player);
                return MischievousEffectsPrank(player);
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
            var roll = rand.Next(100);

            if (roll < 100)
            {
                // return MeanResourcePrank(player);
                // return MeanLocationPrank(player);
                // return MeanQuotasAndTimerPrank(player);
                return MeanEffectsPrank(player);
            }

            // TODO joke_shop return value
            return "Mean prank";
            //return null;
        }

        #endregion

        #region Warnings and access controls

        private static bool PlayerHasBeenWarned(Player player)
        {
            return FIRST_WARNING_EFFECT.HasValue && EffectProcedures.PlayerHasEffect(player, FIRST_WARNING_EFFECT.Value);
        }

        private static bool PlayerHasBeenWarnedTwice(Player player)
        {
            return SECOND_WARNING_EFFECT.HasValue && EffectProcedures.PlayerHasEffect(player, SECOND_WARNING_EFFECT.Value);
        }

        private static string EnsurePlayerIsWarned(Player player)
        {
            if (!PlayerHasBeenWarned(player))
            {
                if (!FIRST_WARNING_EFFECT.HasValue)
                {
                    return "Bug: Unable to give player first warning";
                }

                EffectProcedures.GivePerkToPlayer(FIRST_WARNING_EFFECT.Value, player);

                // TODO joke_shop Put this message in the effect and return the string from GivePerk, logging is handled by that
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

            if (!PlayerHasBeenWarnedTwice(player))
            {
                if (!SECOND_WARNING_EFFECT.HasValue)
                {
                    return "Bug: Unable to give player first warning";
                }

                EffectProcedures.GivePerkToPlayer(SECOND_WARNING_EFFECT.Value, player);

                // TODO joke_shop Put this message in the effect and return the string from GivePerk, will still need to add important log message for pop-up
                var logMessage = "This is your final warning!  This cursed joke shop does not play by the normal rules of Sunnyglade.  If you stay here too long you could be risking your very soul!  Get out now - while you still can!";
                PlayerLogProcedures.AddPlayerLog(player.Id, logMessage, true);
                return logMessage;
            }

            // Already recently reminded
            return null;
        }

        public static bool CharacterIsBanned(Player player)
        {
            return BANNED_FROM_JOKE_SHOP_EFFECT.HasValue && EffectProcedures.PlayerHasActiveEffect(player, BANNED_FROM_JOKE_SHOP_EFFECT.Value);
        }

        private static string BanCharacter(Player player)
        {
            if (!BANNED_FROM_JOKE_SHOP_EFFECT.HasValue)
            {
                return "Bug: Unable to ban player";
            }

            if (EffectProcedures.PlayerHasEffect(player, BANNED_FROM_JOKE_SHOP_EFFECT.Value))
            {
                return "Already banned!";  // TODO joke_shop return null after testing
                //return null;
            }

            var kickedOutMessage = EjectCharacter(player);

            // TODO joke_shop Put this message in the effect and return the string from GivePerk
            EffectProcedures.GivePerkToPlayer(BANNED_FROM_JOKE_SHOP_EFFECT.Value, player);

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

        public static void EjectOfflineCharacters()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

            var playerRepo = new EFPlayerRepository();
            var players = playerRepo.Players.Where(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                                                        p.Mobility == PvPStatics.MobilityFull &&
                                                        p.LastActionTimestamp < cutoff &&
                                                        p.InDuel <= 0 &&
                                                        p.InQuest <= 0 &&
                                                        p.BotId == AIStatics.ActivePlayerBotId);

            foreach (var player in players)
            {
                PlayerLogProcedures.AddPlayerLog(player.Id, "You wake with a start as you land on the sidewalk.  \"Hey!  No sleeping in my shop!  This isn't a hotel!\" shouts the shopkeeper.", true);
                EjectCharacter(player);

                if (PlayerHasBeenWarnedTwice(player))
                {
                    ResetActivityTimer(player);
                    ChangeHealth(player, -50);
                    ChangeMana(player, -10);
                }
                else if (PlayerHasBeenWarned(player))
                {
                    ResetActivityTimer(player, 0.5);
                    ChangeHealth(player, -25);
                    ChangeMana(player, -5);
                }
            }
        }

        #endregion


        #region Resource pranks

        private static string MildResourcePrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 27)  // 27%
            {
                return ChangeHealth(player, rand.Next(-50, 75));
            }
            else if (roll < 54)  // 27%
            {
                return ChangeMana(player, rand.Next(-10, 15));
            }
            else if (roll < 60)  // 6%
            {
                return ChangeActionPoints(player, rand.Next(-3, 3));
            }
            else  // 40%
            {
                return ChangeMoney(player, rand.Next(-2, 3));
            }
        }

        private static string MischievousResourcePrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 30)  // 30%
            {
                return ChangeHealth(player, rand.Next(-125, 125));
            }
            else if (roll < 60)  // 30%
            {
                return ChangeMana(player, rand.Next(-20, 20));
            }
            else if (roll < 65)  // 5%
            {
                return ChangeActionPoints(player, rand.Next(-10, 20));
            }
            else if (roll < 95)  // 30%
            {
                return ChangeMoney(player, rand.Next(-5, 5));
            }
            else  // 5%
            {
                return ChangeDungeonPoints(player, 1);
            }
        }

        private static string MeanResourcePrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 40)  // 40%
            {
                return ChangeHealth(player, rand.Next(-300, 200));
            }
            else if (roll < 60)  // 25%
            {
                return ChangeMana(player, rand.Next(-35, 20));
            }
            else if (roll < 65)  // 5%
            {
                return ChangeActionPoints(player, rand.Next(-30, 30));
            }
            else if (roll < 95)  // 25%
            {
                return ChangeMoney(player, rand.Next(-15, 10));
            }
            else  // 5%
            {
                return ChangeDungeonPoints(player, rand.Next(-5, 5));
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

        #region Location pranks

        private static string MildLocationPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return TeleportToOverworld(player, false, rand.Next(2) == 0);
            }
            else if (roll < 30)  // 20%
            {
                return TeleportToDungeon(player, 0);
            }
            else if (roll < 40)  // 10%
            {
                return TeleportToBar(player, false);
            }
            else if (roll < 46)  // 6%
            {
                return TeleportToFriendlyNPC(player);
            }
            else if (roll < 50)  // 4%
            {
                return TeleportToQuest(player);
            }
            else if (roll < 75)  // 25%
            {
                return RunAway(player);
            }
            else  // 25%
            {
                return WanderAimlessly(player);
            }
        }

        private static string MischievousLocationPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 25)  // 25%
            {
                return TeleportToOverworld(player, true, true);
            }
            else if (roll < 50)  // 25%
            {
                return TeleportToDungeon(player, rand.Next(1, 4));
            }
            else if (roll < 75)  // 25%
            {
                return TeleportToBar(player, true);
            }
            else  // 25%
            {
                return TeleportToHostileNPC(player, false);
            }
        }

        private static string MeanLocationPrank(Player player)
        {
            return TeleportToHostileNPC(player, true);
        }

        private static bool Teleport(Player player, string location, bool logLocations)
        {
            if (location == null)
            {
                return false;
            }

            var destination = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == location);

            if (destination == null)
            {
                return false;
            }
            
            if (player.InDuel > 0 || player.InQuest > 0 || player.MindControlIsActive || player.MoveActionPointDiscount < -TurnTimesStatics.GetActionPointReserveLimit())
            {
                // TODO joke_shop Also block if too many items, player.Items.Count(i => !i.IsEquipped) > GetMaxInventorySize()
                return false;
            }
            
            if (destination.dbName.Contains("dungeon_"))
            {
                SkillProcedures.GiveSkillToPlayer(player.Id, PvPStatics.Dungeon_VanquishSpellSourceId);
            }

            PlayerProcedures.MovePlayer_InstantNoLog(player.Id, location);

            if (logLocations)
            {
                LocationLogProcedures.AddLocationLog(LocationsStatics.JOKE_SHOP, $"Strange forces propel {player.GetFullName()} off to {destination.Name}!");
            }
            else
            {
                LocationLogProcedures.AddLocationLog(LocationsStatics.JOKE_SHOP, $"{player.GetFullName()} is whisked off to a faraway place!");
            }
            LocationLogProcedures.AddLocationLog(location, $"{player.GetFullName()} is surprised to find they are suddenly here!");
            PlayerLogProcedures.AddPlayerLog(player.Id, $"You are sent to {destination.Name}", false);

            return true;
        }

        private static string TeleportToOverworld(Player player, bool root, bool curse)
        {
            var location = LocationsStatics.GetRandomLocationNotInDungeonOr(LocationsStatics.JOKE_SHOP);

            if (!Teleport(player, location, new Random().Next(2) == 0))
            {
                return null;
            }

            if (root)
            {
                root = GiveEffect(player, ROOT_EFFECT) != null;
            }

            if (curse && !root)
            {
                ApplyLocalCurse(player, location);
            }

            return "Teleport to overworld"; // TODO joke_shop flavor text
        }

        private static string TeleportToDungeon(Player player, int meanness)
        {
            if (!PlayerProcedures.CheckAllowedInDungeon(player, out _))
            {
                return null;
            }

            var lastAttackTimeAgo = Math.Abs(DateTime.UtcNow.Subtract(player.GetLastCombatTimestamp()).TotalMinutes);
            if (lastAttackTimeAgo < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
            {
                return null;
            }

            var location = LocationsStatics.GetRandomLocation_InDungeon();

            if (!Teleport(player, location, new Random().Next(2) == 0))
            {
                return null;
            }

            if (meanness % 2 == 1)
            {
                ResetCombatTimer(player);
            }

            if (meanness >= 2)
            {
                GiveEffect(player, ROOT_EFFECT);
            }

            return "Teleport to dungeon"; // TODO joke_shop flavor text
        }

        private static string TeleportToFriendlyNPC(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(6);

            int npc = 0;
            // View view = null;

            switch(roll)
            {
                case 0:
                    npc = AIStatics.BartenderBotId;
                    // view = MVC.NPC.TalkToBartender("none");
                    break;
                case 1:
                    npc = AIStatics.LoremasterBotId;
                    // view = MVC.NPC.TalkToLorekeeper();
                    break;
                case 2:
                    npc = AIStatics.SoulbinderBotId;
                    // view = MVC.NPC.TalkToSoulbinder();
                    break;
                case 3:
                    npc = AIStatics.JewdewfaeBotId;
                    // view = MVC.NPC.TalkWithJewdewfae();
                    break;
                case 4:
                    npc = AIStatics.LindellaBotId;
                    // view = MVC.NPC.TradeWithMerchant("shirt");
                    break;
                case 5:
                    npc = AIStatics.WuffieBotId;
                    // view = MVC.NPC.TradeWithPetMerchant();
                    break;
            }

            var npcPlayer = TeleportToNPC(player, npc);
            if (npcPlayer == null)
            {
                return null;
            }

            if (npcPlayer.BotId == AIStatics.JewdewfaeBotId)
            {
                var encounter = BossProcedures_Jewdewfae.GetFairyChallengeInfoAtLocation(npcPlayer.dbLocationName);  // interface currently restricts to one encounter per location
                TryAnimateTransform(player, encounter.RequiredFormSourceId);
            }

            // TODO joke_shop Redirect to the NPC's talk/trade page
            return $"The bejeweled eyes of a strage ornament begin to glow as a raspy sucking voice echoes throughout the room:  \"Maybe you should talk to <b>{npcPlayer.GetFullName()}</b>?\"  Then the room fades away.";
        }

        private static string TeleportToHostileNPC(Player player, bool attack)
        {
            var targetForm = -1;

            IPlayerRepository playerRepo = new EFPlayerRepository();

            // Bosses
            var hostiles = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull && (
                                                         p.BotId == AIStatics.BimboBossBotId ||
                                                         p.BotId == AIStatics.DonnaBotId ||
                                                         p.BotId == AIStatics.FaebossBotId ||
                                                         p.BotId == AIStatics.MotorcycleGangLeaderBotId ||
                                                         p.BotId == AIStatics.MouseBimboBotId ||
                                                         p.BotId == AIStatics.MouseNerdBotId ||
                                                         p.BotId == AIStatics.FemaleRatBotId ||
                                                         p.BotId == AIStatics.MaleRatBotId ||
                                                         p.BotId == AIStatics.ValentineBotId)).ToList();

            // Minibosses
            if (hostiles == null || hostiles.IsEmpty())
            {
                hostiles = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull && (
                                                         p.BotId == AIStatics.MinibossExchangeProfessorId ||
                                                         p.BotId == AIStatics.MinibossFiendishFarmhandId ||
                                                         p.BotId == AIStatics.MinibossGroundskeeperId ||
                                                         p.BotId == AIStatics.MinibossLazyLifeguardId ||
                                                         p.BotId == AIStatics.MinibossPopGoddessId ||
                                                         p.BotId == AIStatics.MinibossPossessedMaidId ||
                                                         p.BotId == AIStatics.MinibossSeamstressId ||
                                                         p.BotId == AIStatics.MinibossSororityMotherId)).ToList();
            }

            var rand = new Random();
            Player npcPlayer = null;
            if (hostiles != null && hostiles.Any())
            {
                npcPlayer = hostiles[rand.Next(hostiles.Count())];
            }

            // Psychopaths
            if (npcPlayer == null)
            {
                npcPlayer = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull &&
                                                          p.BotId == AIStatics.PsychopathBotId &&
                                                          p.Level <= player.Level)
                                              .OrderByDescending(p => p.Level).FirstOrDefault();
            }

            if (npcPlayer == null)
            {
                return null;
            }

            // Turn into form needed to attack
            if (npcPlayer.BotId == AIStatics.MouseBimboBotId)
            {
                targetForm = BossProcedures_Sisters.NerdSpellFormSourceId;
            }
            else if (npcPlayer.BotId == AIStatics.MouseNerdBotId)
            {
                targetForm = BossProcedures_Sisters.BimboSpellFormSourceId;
            }

            if (targetForm != -1)
            {
                TryAnimateTransform(player, targetForm);
            }

            // Move to same tile as NPC
            if (!Teleport(player, npcPlayer.dbLocationName, rand.Next(2) == 0))
            {
                return null;
            }

            if (attack)
            {
                var spells = SkillProcedures.AvailableSkills(player, npcPlayer, true);
                if (spells != null && spells.Any())
                {
                    var spellList = spells.ToArray();
                    var spell = spellList[rand.Next(spellList.Count())];

                    // Note we do not apply the full gamut of preconditions of a manual attack present in the controller
                    AttackProcedures.AttackSequence(player, npcPlayer, spell);
                }
            }
            
            return "Teleport to hostile NPC";  // TODO joke_shop Add flavor text
        }

        private static Player TeleportToNPC(Player player, int npc)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var npcPlayer = playerRepo.Players.FirstOrDefault(p => p.BotId == npc);

            if (npcPlayer == null || npcPlayer.dbLocationName == null)
            {
                return null;
            }

            if (!Teleport(player, npcPlayer.dbLocationName, new Random().Next(2) == 0))
            {
                return null;
            }

            return npcPlayer;
        }

        private static string TeleportToBar(Player player, bool root)
        {
            // Not all getaways can be clean..
            string bar = "tavern_counter";

            if(!Teleport(player, bar, true))
            {
                return null;
            }

            if (root)
            {
                GiveEffect(player, ROOT_EFFECT);
            }
            else
            {
                ApplyLocalCurse(player, bar);
            }

            return "Teleport to bar";  // TODO joke_shop Flavor text
        }

        private static string TeleportToQuest(Player player)
        {
            var lastAttackTimeAgo = Math.Abs(DateTime.UtcNow.Subtract(player.GetLastCombatTimestamp()).TotalMinutes);
            if (lastAttackTimeAgo < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
            {
                return null;
            }

            var quests = QuestProcedures.GetAllAvailableQuestsForPlayer(player, PvPWorldStatProcedures.GetWorldTurnNumber());

            if (quests == null || !quests.Any())
            {
                return null;
            }

            var rand = new Random();
            var index = rand.Next(quests.Count());
            var quest = quests.ToArray()[index];

            if (!Teleport(player, quest.dbName, rand.Next(2) == 0))
            {
                return null;
            }

            return $"You notice a gold chalice on the shelf, its engraving obscured by dirt.  You decide to blow the dust off and a cloud fills the room.  A frail man with a long white beard and crooked staff emerges from the mist.  \"So, it's a quest you seek?\" comes his shrill, wheezing voice.  \"Well, I have just the thing.  Seek out your victory, young mage.\"  He hands you a scroll.  At the top it is written <b>{quest.Name}</b>.  As you take it you feel yourself transported to a far-off place...";
        }

        private static string RunAway(Player player)
        {
            var destination = LocationsStatics.GetRandomLocationNotInDungeonOr(LocationsStatics.JOKE_SHOP);

            if (MovePlayer(player, destination, 15) == null)
            {
                return null;
            }

            return "Run away";  // TODO joke_shop flavor text
        }

        internal static string MovePlayer(Player player, string destination, int maxSpacesToMove, Action<Player, string> callback = null)
        {
            var start = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);
            var end = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == destination);

            if (destination == null || start == null || end == null)
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            var pathTiles = PathfindingProcedures.GetMovementPath(start, end);
            var costPerTile = 1 - target.MoveActionPointDiscount;

            // Cap distance, plus don't exceed number of tiles or available AP
            var maxDistance = Math.Floor(target.ActionPoints / costPerTile);
            var spacesToMove = (int)(Math.Min(maxDistance, pathTiles.Count()));
            spacesToMove = Math.Min(maxSpacesToMove, spacesToMove);

            if (spacesToMove == 0)
            {
                return null;
            }

            var stoppingTile = pathTiles[spacesToMove - 1];
            PlayerProcedures.MovePlayerMultipleLocations(player, stoppingTile, spacesToMove * costPerTile, callback: callback);

            return stoppingTile;
        }

        private static string WanderAimlessly(Player player)
        {
            var rand = new Random();

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            var name = target.GetFullName();

            var costPerTile = 1 - target.MoveActionPointDiscount;
            var maxDistance = Math.Floor(target.ActionPoints / costPerTile);
            var spacesToMove = (int)Math.Min(maxDistance, rand.Next(10, 16));

            string nextTileId = player.dbLocationName;
            var nextTile = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == nextTileId);

            for (var i = spacesToMove; i > 0; i--)
            {
                var currentTile = nextTile;

                switch (rand.Next(4))
                {
                    case 0:
                        nextTileId = currentTile.Name_North ?? currentTile.Name_South ?? currentTile.Name_East ?? currentTile.Name_West;
                        break;
                    case 1:
                        nextTileId = currentTile.Name_South ?? currentTile.Name_East ?? currentTile.Name_West ?? currentTile.Name_North;
                        break;
                    case 2:
                        nextTileId = currentTile.Name_East ?? currentTile.Name_West ?? currentTile.Name_North ?? currentTile.Name_South;
                        break;
                    case 3:
                        nextTileId = currentTile.Name_West ?? currentTile.Name_North ?? currentTile.Name_South ?? currentTile.Name_East;
                        break;
                }

                nextTile = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == nextTileId);

                LocationLogProcedures.AddLocationLog(currentTile.dbName, name + " left toward " + nextTile.Name);
                LocationLogProcedures.AddLocationLog(nextTileId, name + " entered from " + currentTile.Name);
            }

            PlayerProcedures.MovePlayerMultipleLocations(player, nextTileId, spacesToMove * costPerTile);

            return "Wander aimlessly";  // TODO joke_shop flavor text
        }

        #endregion

        #region Effects pranks

        private static string MildEffectsPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, ROOT_EFFECT, 1);
            }
            else if (roll < 60)  // 50%
            {
                return GiveRandomEffect(player, BOOST_EFFECTS);
            }
            else if (roll < 90)  // 30%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS);
            }
            else  // 10%
            {
                return GiveEffect(player, SNEAK_REVEAL_1);
            }
        }

        private static string MischievousEffectsPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, ROOT_EFFECT, 2);
            }
            else if (roll < 40)  // 30%
            {
                return GiveRandomEffect(player, BOOST_EFFECTS);
            }
            else if (roll < 75)  // 35%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS);
            }
            else if (roll < 95)  // 20%
            {
                return GiveEffect(player, INSTINCT_EFFECT);
            }
            else  // 5%
            {
                return GiveEffect(player, SNEAK_REVEAL_2);
            }
        }

        private static string MeanEffectsPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 10)  // 10%
            {
                return GiveEffect(player, ROOT_EFFECT, 4);
            }
            else if (roll < 90)  // 80%
            {
                return GiveRandomEffect(player, PENALTY_EFFECTS);
            }
            else  // 10%
            {
                return GiveEffect(player, SNEAK_REVEAL_3);
            }
        }

        private static string GiveEffect(Player player, int? effect, int duration = 3)
        {
            if (!effect.HasValue || EffectProcedures.PlayerHasEffect(player, effect.Value))
            {
                return null;
            }

            return EffectProcedures.GivePerkToPlayer(effect.Value, player, Duration: duration, Cooldown: duration);
        }

        private static string GiveRandomEffect(Player player, int[] effects)
        {
            if (effects.IsEmpty())
            {
                return null;
            }

            var effect = effects[new Random().Next(effects.Count())];

            if (EffectProcedures.PlayerHasEffect(player, effect))
            {
                return null;
            }

            return EffectProcedures.GivePerkToPlayer(effect, player);
        }

        private static string ApplyLocalCurse(Player player, string dbLocationName)
        {
            var effects = EffectStatics.GetEffectGainedAtLocation(dbLocationName).ToArray();

            if (effects.Any())
            {
                var effect = effects[new Random{}.Next(effects.Count())];

                if (!EffectProcedures.PlayerHasEffect(player, effect.Id))
                {
                    return GiveEffect(player, effect.Id);
                }
            }

            return null;
        }

        #endregion

        #region Quotas and timers pranks

        private static string MildQuotasAndTimerPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 50)  // 50%
            {
                // Half-way through combat timer
                return ResetCombatTimer(player, 0.5);
            }
            else  // 50%
            {
                // Clear combat timer
                ResetCombatTimer(player, 1);
                return "As you peruse the shelves of the joke shop all thoughts of battle leave your mind.";
            }
        }

        private static string MischievousQuotasAndTimerPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 20)  // 20%
            {
                return ChangeAttacks(player, rand.Next(-1, 1));
            }
            else if (roll < 40)  // 20%
            {
                return ChangeCleanseMeditates(player, rand.Next(-1, 1));
            }
            else if (roll < 60)  // 20%
            {
                return ChangeItemUses(player, rand.Next(-1, 1));
            }
            else  // 40%
            {
                return ResetCombatTimer(player);
            }
        }

        private static string MeanQuotasAndTimerPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 40)  // 40%
            {
                return BlockAttacks(player);
            }
            else if (roll < 75)  // 35%
            {
                return BlockCleanseMeditates(player);
            }
            else  // 25%
            {
                return BlockItemUses(player);
            }
        }

        private static string ChangeAttacks(Player player, int delta)
        {
            if (delta >= 0)
            {
                delta++;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate += delta;
            playerRepo.SavePlayer(player);

            return "Attack counter changed";  // TODO joke_shop flavor text
        }

        private static string BlockAttacks(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.TimesAttackingThisUpdate = PvPStatics.MaxAttacksPerUpdate;
            playerRepo.SavePlayer(player);

            return "Attack counter blocked";  // TODO joke_shop flavor text
        }

        private static string ChangeCleanseMeditates(Player player, int delta)
        {
            if (delta >= 0)
            {
                delta++;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.CleansesMeditatesThisRound += delta;
            playerRepo.SavePlayer(player);

            return "Meditate counter changed";  // TODO joke_shop flavor text
        }

        private static string BlockCleanseMeditates(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.CleansesMeditatesThisRound = PvPStatics.MaxCleansesMeditatesPerUpdate;
            playerRepo.SavePlayer(player);

            return "Meditate counter blocked";  // TODO joke_shop flavor text
        }

        private static string ChangeItemUses(Player player, int delta)
        {
            if (delta >= 0)
            {
                delta++;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.ItemsUsedThisTurn += delta;
            playerRepo.SavePlayer(player);

            return "Item use counter changed";  // TODO joke_shop flavor text
        }

        private static string BlockItemUses(Player player)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.ItemsUsedThisTurn = PvPStatics.MaxActionsPerUpdate;
            playerRepo.SavePlayer(player);

            return "Item use counter blocked";  // TODO joke_shop flavor text
        }

        private static string ResetCombatTimer(Player player, double proportionOutOfCombat = 0.0)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            target.LastCombatTimestamp = DateTime.UtcNow.AddMinutes(-proportionOutOfCombat * TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling());
            playerRepo.SavePlayer(player);

            return "A whiff of magic passes under your nose, the acrid smell reminding you of your last battle.  It seems so recent...";
        }

        private static void ResetActivityTimer(Player player, double proportionOutOfOnline = 0.0)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            target.LastActionTimestamp = DateTime.UtcNow.AddMinutes(-proportionOutOfOnline * TurnTimesStatics.GetOfflineAfterXMinutes());
            playerRepo.SavePlayer(player);
        }

        #endregion

        #region Form, name and MC pranks

        private static string MildTransformationPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 55)  // 55%
            {
                return AnimateTransform(player);
            }
            else if (roll < 65)  // 10%
            {
                return TGTransform(player);
            }
            else if (roll < 75)  // 10%
            {
                return BodySwap(player, true);  // clone
            }
            else if (roll < 85)  // 10%
            {
                return RestoreBaseForm(player);
            }
            else  // 15%
            {
                return RestoreName(player);
            }
        }

        private static string MischievousTransformationPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 15)  // 10%
            {
                return ImmobileTransform(player, rand.Next(2) == 0);
            }
            else if (roll < 20)  // 10%
            {
                return MobileInanimateTransform(player);
            }
            else if (roll < 30)  // 10%
            {
                return BodySwap(player, false);
            }
            else if (roll < 40) //  10%
            {
                return ChangeBaseForm(player);
            }
            else if (roll < 45)  // 5%
            {
                return SetBaseFormToCurrent(player);
            }
            else if (roll < 65)  // 20%
            {
                return IdentityChange(player);
            }
            else if (roll < 90)  // 25%
            {
                return TransformToMindControlledForm(player);
            }
            else  // 10%
            {
                return InanimateTransform(player, true);
            }
        }

        private static string MeanTransformationPrank(Player player)
        {
            return InanimateTransform(player, false);
        }

        private static string AnimateTransform(Player player)
        {
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !PlayerHasBeenWarned(player))
            {
                var warning = EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }
            
            var forms = Forms(f => f.Category == PvPStatics.MobilityFull);

            if (forms.IsEmpty())
            {
                return null;
            }

            var index = new Random().Next(forms.Count());
            var form = forms.ElementAt(index);

            if (!TryAnimateTransform(player, form.FormSourceId))
            {
                return null;
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>.");

            return $"You are an animate {form.FriendlyName}.";  // TODO joke_shop flavor text
        }

        private static string ImmobileTransform(Player player, bool temporary)
        {
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !PlayerHasBeenWarned(player))
            {
                var warning = EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }
            
            var forms = Forms(f => f.Category == LIMITED_MOBILITY);

            if (forms.IsEmpty())
            {
                return null;
            }

            FormDetail form;

            if (temporary)
            {
                GiveEffect(player, AUTO_RESTORE_EFFECT, PlayerHasBeenWarnedTwice(player) ? 3 : 2);
            }

            var index = new Random().Next(forms.Count());
            form = forms.ElementAt(index);

            if (!TryAnimateTransform(player, form.FormSourceId))
            {
                return null;
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>.");

            return $"You are an immobile {form.FriendlyName}.";  // TODO joke_shop flavor text
        }

        private static string InanimateTransform(Player player, bool temporary, bool dropInventory = false)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            var forms = Forms(f => f.Category != PvPStatics.MobilityFull && f.Category != LIMITED_MOBILITY);

            if (forms.IsEmpty())
            {
                return null;
            }

            var duration = 0;
            if (temporary)
            {
                duration = PlayerHasBeenWarnedTwice(player) ? 10 : 5;
                temporary = GiveEffect(player, AUTO_RESTORE_EFFECT, duration) != null;
            }

            var index = new Random().Next(forms.Count());
            FormDetail form = forms.ElementAt(index);

            if (!TryInanimateTransform(player, form.FormSourceId, dropInventory: dropInventory, createItem: !temporary, severe: true))
            {
                return null;
            }

            if (temporary)
            {
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You fall into the ether and are stuck as a {form.FriendlyName} for the next {duration} turns!", true);  // TODO joke_shop message
            }
            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>.");

            return $"You are an inanimate {form.FriendlyName}.";  // TODO joke_shop flavor text - must inform player when they will auto revert, if they will
        }

        private static string MobileInanimateTransform(Player player)
        {
            // Turning a player into a rune or consumable is a bit too involved as there are no forms for those items in the DB,
            // however we can make a player inanimate without a player item and pretend they are fully mobile...

            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !PlayerHasBeenWarned(player))
            {
                var warning = EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }
            
            var forms = Forms(f => f.Category != PvPStatics.MobilityFull && f.Category != LIMITED_MOBILITY);

            if (forms.IsEmpty())
            {
                return null;
            }

            var index = new Random().Next(forms.Count());
            FormDetail form = forms.ElementAt(index);

            // Give player an inanimate form without creating a player item
            if (!TryInanimateTransform(player, form.FormSourceId, dropInventory: false, createItem: false, severe: false))
            {
                return null;
            }

            // Coerce player into mobility
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            target.Mobility = PvPStatics.MobilityFull;
            playerRepo.SavePlayer(target);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);

            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into an animate <b>{form.FriendlyName}</b>.");

            return $"You are a mobile inanimate {form.FriendlyName}.";  // TODO joke_shop flavor text - must inform player when they will auto revert, if they will
        }

        private static string TGTransform(Player player)
        {
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !PlayerHasBeenWarned(player))
            {
                var warning = EnsurePlayerIsWarned(player);

                if (!warning.IsNullOrEmpty())
                {
                    return warning;
                }
            }

            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var altForm = formsRepo.DbStaticForms.Where(form => form.Id == player.FormSourceId).Select(form => new {form.AltSexFormSourceId, form.Gender}).FirstOrDefault();

            if (altForm == null || !altForm.AltSexFormSourceId.HasValue)
            {
                return null;
            }

            TryAnimateTransform(player, altForm.AltSexFormSourceId.Value);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You suddenly became {altForm.Gender}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} suddenly became {altForm.Gender}.");

            return "TG";  // TODO joke_shop flavor text
        }

        private static string BodySwap(Player player, bool clone)
        {
            var rand = new Random();
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());
            
            var candidates = PlayerProcedures.GetPlayersAtLocation(LocationsStatics.JOKE_SHOP)
                .Where(p => p.OnlineActivityTimestamp >= cutoff &&
                            p.Id != player.Id &&
                            p.Mobility == PvPStatics.MobilityFull &&
                            p.InDuel <= 0 &&
                            p.InQuest <= 0 &&
                            p.BotId == AIStatics.ActivePlayerBotId)
                .ToList();

            Player victim = null;
            var numCandidates = candidates.Count();

            if (numCandidates == 0)
            {
                return null;
            }

            if (clone)
            {
                var index = rand.Next(numCandidates);
                var candidate = candidates[index];

                TryAnimateTransform(player, candidate.FormSourceId);

                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have become a clone of {victim.GetFullName()}", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> became a clone of <b>{victim.GetFullName()}</b>.");

                return "Clone";  // TODO joke_shop flavor text
            }
            else
            {
                // Find nearboy player with sufficient consent
                do
                {
                    var index = rand.Next(numCandidates);
                    var candidate = candidates[index];

                    if (candidate.GameMode != (int)GameModeStatics.GameModes.Superprotection || PlayerHasBeenWarned(candidate))
                    {
                        victim = candidate;
                    }
                    else
                    {
                        // Shuffle the dismissed candidate out of the way
                        candidates[index] = candidates[numCandidates - 1];
                        candidates[numCandidates - 1] = candidate;
                    }

                    numCandidates--;
                } while (victim == null && numCandidates > 0);

                if (victim == null)
                {
                    return null;
                }

                // Swap forms
                TryAnimateTransform(player, victim.FormSourceId);
                TryAnimateTransform(victim, player.FormSourceId);

                PlayerLogProcedures.AddPlayerLog(victim.Id, $"You have swapped bodies with {player.GetFullName()}", true);  // TODO joke_shop flavor text
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have swapped bodies with {victim.GetFullName()}", false);  // TODO joke_shop flavor text
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> swapped bodies with <b>{victim.GetFullName()}</b>.");

                return "Body swap";  // TODO joke_shop flavor text
            }
        }

        private static void UndoTemporaryForm(int playerId)
        {
            var player = PlayerProcedures.GetPlayer(playerId);

            // Avoid reverting form if player is in a mobile animate form
            var canRemove = false;
            if (player.Mobility != PvPStatics.MobilityFull)
            {
                canRemove = true;
            }
            else
            {
                IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
                var moveActionPointDiscount = formsRepo.DbStaticForms.Where(form => form.Id == player.FormSourceId).Select(form => form.MoveActionPointDiscount).FirstOrDefault();
                canRemove = moveActionPointDiscount < -5;

                if (!canRemove)
                {
                    // Can also revert if player is an inanimate posing as animate
                    var inanimatePlayer = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = playerId });
                    canRemove = inanimatePlayer != null;
                }
            }

            if (canRemove)
            {
                RestoreBaseForm(player, true);
                PlayerLogProcedures.AddPlayerLog(player.Id, $"As your mind begins to clear, the illusion gradually subsides.  It isn't long before you realize you are back in a familiar form.", true);
            }
        }

        private static string RestoreBaseForm(Player player, bool force = false)
        {
            // Require extra warning for SP players, who might want to keep their form
            if (!force && player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !PlayerHasBeenWarned(player))
            {
                return null;
            }

            // Use chaos restore.  Should delete item & reset skills.  Self-restore, struggle restore and classic me do similar
            PlayerProcedures.InstantRestoreToBase(player);

            // No Back On Your Feet, but clear any XP ready for next inanimation - unless Chaos, where people like to lock fast
            if (!PvPStatics.ChaosMode)
            {
                IInanimateXPRepository inanimXpRepo = new EFInanimateXPRepository();
                var inanimXP = inanimXpRepo.InanimateXPs.FirstOrDefault(i => i.OwnerId == player.Id);
                inanimXpRepo.DeleteInanimateXP(inanimXP.Id);
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You returned to base form.", false);

            return "Restore to base";  // TODO joke_shop flavor text
        }

        private static string ChangeBaseForm(Player player)
        {
            var availableForms = STABLE_FORMS.Select(f => f.FormSourceId).Intersect(MISCHIEVOUS_FORMS).ToArray();

            if (availableForms.IsEmpty())
            {
                return null;
            }

            var formSourceId = availableForms[new Random().Next(availableForms.Count())];

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.OriginalFormSourceId = formSourceId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your base form was changed.", false);

            return "Base form changed";  // TODO joke_shop flavor text
        }

        private static string SetBaseFormToCurrent(Player player)
        {
            if (player.OriginalFormSourceId == player.FormSourceId)
            {
                return null;
            }

            var formSourceId = player.FormSourceId;

            // Check we're not setting an inanimate/pet as base form, in case that causes problems...
            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var mobility = formsRepo.DbStaticForms.Where(f => f.Id == formSourceId).Select(f => f.MobilityType).FirstOrDefault();

            if(mobility != PvPStatics.MobilityFull)
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.OriginalFormSourceId = formSourceId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your base form has changed to your current form.", false);

            return "Base form set to current";  // TODO joke_shop flavor text
        }

        private static string RestoreName(Player player)
        {
            if (player.FirstName == player.OriginalFirstName && player.LastName == player.OriginalLastName)
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.FirstName = user.OriginalFirstName;
            user.LastName = user.OriginalLastName;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your name was changed back to {user.GetFullName()}.", false);

            return "Name restored";  // TODO joke_shop flavor text
        }

        private static string IdentityChange(Player player)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            var rand = new Random();

            int[] forms = {};
            var firstName = player.OriginalFirstName;
            var lastName = player.OriginalLastName;
            var mindControl = false;
            var message = "";

            var roll = rand.Next(4);

            // Pick changes to name and form
            if (roll == 0)  // Dogs
            {
                forms = STABLE_FORMS.Select(f => f.FormSourceId).Intersect(DOGS).ToArray();
                mindControl = true;
                message = "";  // TODO joke_shop flavor text

                switch(rand.Next(2))
                {
                    case 0:
                        string[] prefixes = {"Dog", "Doggy", "Canine", "Barker"}; 
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = {"Dogg", "Woof", "Barker"}; 
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                }
            }
            else if (roll == 1)  // Cats
            {
                forms = STABLE_FORMS.Select(f => f.FormSourceId).Intersect(CATS_AND_NEKOS).ToArray();
                mindControl = true;
                message = "";  // TODO joke_shop flavor text

                switch(rand.Next(3))
                {
                    case 0:
                        string[] prefixes = {"Kitty", "Cat", "Neko", "Feline"}; 
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        firstName = prefixes[rand.Next(prefixes.Count())];
                        break;
                    case 1:
                        string[] suffixes = {"Cat", "Neko", "Feline"}; 
                        lastName = suffixes[rand.Next(suffixes.Count())];
                        break;
                    case 2:
                        firstName = firstName.ToLower().Replace("a", "nya");
                        lastName = lastName.ToLower().Replace("a", "nya");
                        firstName = firstName.Substring(0, 1).ToUpper() + firstName.Substring(1);
                        lastName = lastName.Substring(0, 1).ToUpper() + lastName.Substring(1);
                        break;
                }
            }
            else if (roll == 2)  // Drones
            {
                forms = STABLE_FORMS.Select(f => f.FormSourceId).Intersect(DRONES).ToArray();
                mindControl = false;
                message = "";  // TODO joke_shop flavor text

                string[] designators = {"Unit", "Drone", "Clone", "Entity", "Bot"}; 
                var designator = designators[rand.Next(designators.Count())];

                switch(rand.Next(3))
                {
                    case 0:
                        var name = rand.Next(2) == 0 ? firstName : lastName;
                        var order = rand.Next(2);
                        firstName = order == 0 ? designator : name;
                        lastName = order == 0 ? name : designator;
                        break;
                    case 1:
                        firstName = designator;
                        lastName = rand.Next(2) == 0 ? firstName : lastName;
                        lastName = lastName.ToUpper().Replace('I', '1')
                                                     .Replace('L', '1')
                                                     .Replace('Z', '2')
                                                     .Replace('E', '3')
                                                     .Replace('A', '4')
                                                     .Replace('S', '5')
                                                     .Replace('G', '6')
                                                     .Replace('R', '7')
                                                     .Replace('B', '8')
                                                     .Replace('Q', '9')
                                                     .Replace('O', '0');
                        break;
                    case 2:
                        firstName = designator;
                        lastName = $"#{rand.Next(100000)}";
                        break;
                }
            }
            else if (roll == 3)  // Renames
            {
                mindControl = false;
                message = "";  // TODO joke_shop flavor text

                switch(rand.Next(1))
                {
                    case 0:
                        lastName = $"Mc{firstName}face";
                        if (!firstName.EndsWith("y"))
                        {
                            firstName = $"{firstName}y";
                        }
                        break;
                }
            }

            // Change form
            if (forms != null && !forms.IsEmpty())
            {
                var formSourceId = forms[new Random().Next(forms.Count())];
                if (!TryAnimateTransform(player, formSourceId))
                {
                    return null;
                }
            }

            // Change name
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.FirstName = firstName;
            user.LastName = lastName;
            playerRepo.SavePlayer(user);

            // Impose behavior
            if (mindControl)
            {
                GiveEffect(player, INSTINCT_EFFECT);
            }

            if (message.IsNullOrEmpty())
            {
                message = "You have taken on a whole new identity!";  // TODO joke_shop flavor text
            }

            return message;
        }

        private static string TransformToMindControlledForm(Player player)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            var rand = new Random();

            int[][] mcForms = {CATS_AND_NEKOS, DOGS, MAIDS, SHEEP, STRIPPERS};
            var genre = mcForms[rand.Next(mcForms.Count())];
            var forms = STABLE_FORMS.Select(f => f.FormSourceId).Intersect(genre).ToArray();

            if (forms == null || forms.IsEmpty())
            {
                return null;
            }

            if (!TryAnimateTransform(player, forms[rand.Next(forms.Count())]))
            {
                return null;
            }

            GiveEffect(player, INSTINCT_EFFECT);
            return "You are now in a mind controlled form";  // TODO joke_shop flavor text
        }

        private static bool TryAnimateTransform(Player player, int formSourceId)
        {
            // Require extra warning for SP players, who might want to keep their form
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !PlayerHasBeenWarned(player))
            {
                return false;
            }

            PlayerProcedures.InstantChangeToForm(player, formSourceId);
            DomainRegistry.Repository.Execute(new ReadjustMaxes
            {
                playerId = player.Id,
                buffs = ItemProcedures.GetPlayerBuffs(player)
            });

            // TODO joke_shop log tf
            return true;
        }

        private static bool TryInanimateTransform(Player player, int formSourceId, bool dropInventory, bool createItem = true, bool severe = true)
        {
            if ((severe && !PlayerHasBeenWarnedTwice(player)) || (!severe && !PlayerHasBeenWarned(player)))
            {
                return false;
            }

            PlayerProcedures.InstantChangeToForm(player, formSourceId);

            // If item is not created player will have no actions and not be visible to other players,
            // so some external mechanism must be in place to restore the player to animate form.
            if (createItem)
            {
                var form = FormStatics.GetForm(formSourceId);
                var extra = ItemProcedures.PlayerBecomesItem(player, form, null, dropInventory);
                // If inventory isn't dropped at point of TF then it will be dropped if/when player locks.
            }
            else if (dropInventory)
            {
                DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = player.Id, IgnoreRunes = false });
            }
            
            // TODO joke_shop log tf
            return true;
        }

        #endregion

        #region Harmless fun and games

        private static string DiceGame(Player player)
        {
            var target = 69;

            // /role 4d20
            var die1 = PlayerProcedures.RollDie(20);
            var die2 = PlayerProcedures.RollDie(20);
            var die3 = PlayerProcedures.RollDie(20);
            var die4 = PlayerProcedures.RollDie(20);
            var total = die1 + die2 + die3 + die4;

            // Arbitrary score calculation, trying to avoid any big advantage for those who roll more often
            int score;
            if (total == target)
            {
                score = total;
            }
            else
            {
                var distance = Math.Abs(total - target);
                
                if (distance <= 11)
                {
                    score = (11 - distance) * 4;
                }
                else
                {
                    score = (11 - distance) / 10;
                }
            }

            StatsProcedures.AddStat(player.MembershipId, StatsProcedures.Stat__DiceGameScore, score);

            LocationLogProcedures.AddLocationLog(LocationsStatics.JOKE_SHOP, $"{player.GetFullName()} rolls {die1}, {die2}, {die3} and {die4}, giving a total of <b>{total}</b>.");

            return $"You pick up four 20-sided dice and roll {die1}, {die2}, {die3} and {die4}, giving a total of <b>{total}</b>.  You score is <b>{score}</b>.";
        }

        #endregion

    }
}
