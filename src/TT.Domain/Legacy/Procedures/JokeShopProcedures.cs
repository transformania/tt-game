﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Queries;
using TT.Domain.Legacy.Services;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

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

        internal static readonly List<FormDetail> STABLE_FORMS = CandidateForms();

        public static readonly int[] MISCHIEVOUS_FORMS = {215, 221, 438};
        public static readonly int[] CATS_AND_NEKOS = {39, 100, 385, 434, 504, 575, 668, 673, 681, 703, 713, 733, 752, 761, 806, 849, 851, 855, 987, 991, 1034, 1060, 1098, 1105, 1188, 1202};
        public static readonly int[] DOGS = {34, 359, 552, 667, 911, 912, 995, 1043, 1074, 1108, 1123, 1187};
        public static readonly int[] RODENTS = {70, 143, 205, 271, 278, 279, 317, 318, 319, 522, 772, 1077};
        public static readonly int[] TREES = {50, 741};
        public static readonly int[] STRIPPERS = {153, 719, 880};
        public static readonly int[] DRONES = {715, 930, 951, 1039, 1050};
        public static readonly int[] SHEEP = {204, 950, 1022, 1035, 1198};
        public static readonly int[] MAIDS = {65, 205, 305, 348, 457, 499, 514, 652, 662, 673, 848, 869, 875, 901, 921, 951, 958, 991, 1001, 1040, 1041, 1045, 1058, 1072, 1073, 1076, 1110, 1117, 1188, 1193, 1203, 1207};
        public static readonly int[] MANA_FORMS = {834, 1149};

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

        #region Core mechanics and utilities

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

            // TODO joke_shop call this later on in world update (after AP update, psycho actions)?
            if (INSTINCT_EFFECT.HasValue)
            {
                var playersToControl = temporaryEffects.Where(e => e.EffectSourceId == INSTINCT_EFFECT.Value).Select(e => e.OwnerId).ToList();
                InstinctProcedures.ActOnInstinct(playersToControl);
            }
        }

        private static List<Player> ActivePlayersInJokeShopApartFrom(Player player)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

            var candidates = PlayerProcedures.GetPlayersAtLocation(LocationsStatics.JOKE_SHOP)
                .Where(p => p.OnlineActivityTimestamp >= cutoff &&
                            p.Id != player.Id &&
                            p.Mobility == PvPStatics.MobilityFull &&
                            p.InDuel <= 0 &&
                            p.InQuest <= 0 &&
                            p.BotId == AIStatics.ActivePlayerBotId)
                .ToList();

            return candidates;
        }

        public static void SetJokeShopActive(bool active)
        {
            // Work on a copy of the map to avoid concurrency issues
            var newMap = LocationsStatics.LocationList.GetLocation.Select(l => l.Clone()).ToList();
            var jokeShopTile = newMap.FirstOrDefault(l => l.dbName == LocationsStatics.JOKE_SHOP);
            var initiallyActive = (jokeShopTile != null);

            if (initiallyActive == active)
            {
                return;
            }

            if (active == true)
            {
                // Activate joke shop

                // Add tile to map
                jokeShopTile = new Location {
                    dbName = LocationsStatics.JOKE_SHOP,
                    Name = "Cursed Joke Shop",
                    Region="limbo"
                };
                newMap.Add(jokeShopTile);

                // Give it a location and connect it to the streets
                LocationsStatics.MoveJokeShop(newMap);

                // Commit new map (reference assignment is atomic)
                LocationsStatics.LocationList.GetLocation = newMap;
            }
            else
            {
                // Deactivate joke shop

                // Find somewhere to relocate things to
                var streetTile = jokeShopTile.Name_North ?? jokeShopTile.Name_East ??
                                 jokeShopTile.Name_South ?? jokeShopTile.Name_West ??
                                 LocationsStatics.GetRandomLocationNotInDungeonOr(LocationsStatics.JOKE_SHOP);

                // Disable wayfinding into joke shop
                IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
                
                foreach (var directive in directiveRepo.AIDirectives.Where(d => d.TargetLocation == LocationsStatics.JOKE_SHOP).ToList())
                {
                    var botDirective = directiveRepo.AIDirectives.FirstOrDefault(d => d.Id == directive.Id);
                    botDirective.TargetLocation = streetTile;
                    directiveRepo.SaveAIDirective(botDirective);
                }

                // Remove from map (reference assignment is atomic)
                LocationsStatics.UnlinkLocation(jokeShopTile, newMap);
                LocationsStatics.LocationList.GetLocation = newMap.Where(l => l.dbName != LocationsStatics.JOKE_SHOP).ToList();

                // Everybody out
                IPlayerRepository playerRepo = new EFPlayerRepository();
    
                foreach (var player in PlayerProcedures.GetPlayersAtLocation(LocationsStatics.JOKE_SHOP).ToList())
                {
                    var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
                    user.dbLocationName = streetTile;
                    playerRepo.SavePlayer(user);
                }

                IItemRepository itemRepo = new EFItemRepository();
    
                foreach (var item in itemRepo.Items.Where(i => i.dbLocationName ==LocationsStatics.JOKE_SHOP).ToList())
                {
                    var streetItem = itemRepo.Items.FirstOrDefault(i => i.Id == item.Id);
                    streetItem.dbLocationName = streetTile;
                    itemRepo.SaveItem(streetItem);
                }
            }
        }

        #endregion

        #region Location action hooks

        // Returning null from hooks opts to not override the default action

        public static string Search(Player player)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            var rand = new Random();

            // Decide whether this is a regular or a prank search
            if (rand.Next(5) < 3)  // Normal search 60% of the time, else attempt a prank
            {
                return null;
            }

            var roll = rand.Next(100);

            if (roll < 65)  // 65%
            {
                return MildPrank(player);
            }
            else if (roll < 90)  // 25%
            {
                 return MischievousPrank(player);
            }
            else if (roll < 95)  // 5%
            {
                return MeanPrank(player);
            }
            else  // 5%
            {
                return BanCharacter(player);
            }
        }

        public static string Meditate(Player player)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            // Ensure player can meditate
            if (player.ActionPoints < PvPStatics.MeditateCost || player.Mana >= player.MaxMana || player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
            {
                return null;
            }

            var rand = new Random();

            // Decide whether this is a regular or a prank meditation
            if (rand.Next(10) < 9)  // Prank 10% of the time
            {
                return null;
            }

            // Issue warnings
            var message = EnsurePlayerIsWarned(player);

            if (!message.IsNullOrEmpty())
            {
                return message;
            }

            if (rand.Next(10) == 0)
            {
                message = EnsurePlayerIsWarnedTwice(player);

                if (!message.IsNullOrEmpty())
                {
                    return message;
                }
            }

            // Pick a prank
            var roll = rand.Next(100);

            if (roll < 3)  // 3%
            {
                // Change to mana form
                if (TryAnimateTransform(player, MANA_FORMS[rand.Next(MANA_FORMS.Count())]))
                {
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    message = "Oh dear, it looks like you've been meditating too hard!";
                }
            }
            else if (roll < 28)  // 25%
            {
                // Toy with mana
                message = ChangeMana(player, rand.Next(200) - 100);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 38)  // 10%
            {
                // Toy with health
                message = ChangeHealth(player, rand.Next(400) - 200);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 48)  // 10%
            {
                // Mana recharge
                ChangeMana(player, (int)(player.MaxMana - player.Mana + 1));
                BlockCleanseMeditates(player);
                message = "You feel full of magic, but it might take a moment to absorb all that new mana.";
            }
            else if (roll < 58)  // 10%
            {
                // Mana steal
                var others = ActivePlayersInJokeShopApartFrom(player);
                if (others.Count() > 0)
                {
                    var other = others[rand.Next(others.Count())];
                    var proportion = rand.NextDouble() * 0.6 + 0.2;
                    var amount = Math.Floor(other.Mana * (decimal)proportion);
                    ChangeMana(player, (int)(amount));
                    ChangeMana(other, (int)(-amount));
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    PlayerLogProcedures.AddPlayerLog(other.Id, $"{player.GetFullName()} steals {amount} mana from you!", true);
                    message = $"You steal {amount} mana from {other.GetFullName()}!";
                }
            }
            else if (roll < 68)  // 10%
            {
                // Block cleans/meditate
                message = BlockCleanseMeditates(player);
            }
            else if (roll < 93)  // 25%
            {
                // Other resource/effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    message = rand.Next(2) == 0 ? MeanEffectsPrank(player) : MeanResourcePrank(player);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    message = rand.Next(2) == 0 ? MischievousEffectsPrank(player) : MischievousResourcePrank(player);
                }
                else
                {
                    message = rand.Next(2) == 0 ? MildEffectsPrank(player) : MildResourcePrank(player);
                }
            }
            else if (roll < 98)  // 5%
            {
                // Put in combat
                message = ResetCombatTimer(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else  // 2%
            {
                message = BanCharacter(player);
            }

            // Charge player AP for the prank
            if (message != null)
            {
                ChangeActionPoints(player, (int)(-PvPStatics.MeditateCost));
            }

            return message;
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

            // Ensure player can cleanse
            if (player.ActionPoints < PvPStatics.CleanseCost || player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
            {
                return null;
            }

            var rand = new Random();

            // Decide whether this is a regular or a prank cleanse
            if (rand.Next(10) < 9)  // Prank 10% of the time
            {
                return null;
            }

            // Issue warnings
            var message = EnsurePlayerIsWarned(player);

            if (!message.IsNullOrEmpty())
            {
                return message;
            }

            if (rand.Next(10) == 0)
            {
                message = EnsurePlayerIsWarnedTwice(player);

                if (!message.IsNullOrEmpty())
                {
                    return message;
                }
            }

            // Pick a prank
            var roll = rand.Next(100);

            if (roll < 5)  // 5%
            {
                // Change to cleanse form
                if (TryAnimateTransform(player, MAIDS[rand.Next(MAIDS.Count())]))
                {
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    message = "You've been cleansing a lot.  Maybe you would like to clean the shop while you're at it?";
                }
            }
            else if (roll < 10)  // 5%
            {
                // Change to base form
                if (player.FormSourceId != player.OriginalFormSourceId && TryAnimateTransform(player, player.OriginalFormSourceId))
                {
                    RestoreBaseForm(player);
                    BlockCleanseMeditates(player);
                    message = "Oops.. You might have just cleansed a little too much!";
                }
            }
            else if (roll < 15)  // 5%
            {
                message = SetBaseFormToRegular(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 20)  // 5%
            {
                message = RestoreName(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 36)  // 16%
            {
                // Toy with health
                message = ChangeHealth(player, rand.Next(400) - 200);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 46)  // 10%
            {
                // Health recharge
                ChangeHealth(player, (int)(player.MaxHealth - player.Health + 1));
                PlayerProcedures.AddCleanseMeditateCount(player);
                BlockAttacks(player);
                GiveEffect(player, ROOT_EFFECT, 1);
                message = "You feel completely rejuvenated, but the cleansing has drained you and you need a moment to recover.";
            }
            else if (roll < 56)  // 10%
            {
                // Health steal
                var others = ActivePlayersInJokeShopApartFrom(player);
                if (others.Count() > 0)
                {
                    var other = others[rand.Next(others.Count())];
                    var proportion = rand.NextDouble() * 0.3 + 0.1;
                    var amount = Math.Floor(other.Health * (decimal)proportion);
                    ChangeHealth(player, (int)(amount));
                    ChangeHealth(other, (int)(-amount));
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    PlayerLogProcedures.AddPlayerLog(other.Id, $"{player.GetFullName()} steals {amount} of your willpower!", true);
                    message = $"You steal {amount} willpower from {other.GetFullName()}!";
                }
            }
            else if (roll < 68)  // 12%
            {
                // Block cleans/meditate
                message = BlockCleanseMeditates(player);
            }
            else if (roll < 93)  // 25%
            {
                // Other resource/effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    message = rand.Next(2) == 0 ? MeanEffectsPrank(player) : MeanResourcePrank(player);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    message = rand.Next(2) == 0 ? MischievousEffectsPrank(player) : MischievousResourcePrank(player);
                }
                else
                {
                    message = rand.Next(2) == 0 ? MildEffectsPrank(player) : MildResourcePrank(player);
                }
            }
            else if (roll < 98)  // 5%
            {
                // Put in combat
                message = ResetCombatTimer(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else  // 2%
            {
                message = BanCharacter(player);
            }

            // Charge player AP for the prank
            if (message != null)
            {
                ChangeActionPoints(player, (int)(-PvPStatics.CleanseCost));
            }

            return message;
        }

        public static string SelfRestore(Player player)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            // Ensure player can cleanse
            if (player.ActionPoints < PvPStatics.CleanseCost || player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate || player.FormSourceId == player.OriginalFormSourceId)
            {
                return null;
            }

            var rand = new Random();

            // Decide whether this is a regular or a prank self-restore
            if (rand.Next(25) < 24)  // Prank 4% of the time
            {
                return null;
            }

            // Issue warnings
            var message = EnsurePlayerIsWarned(player);

            if (!message.IsNullOrEmpty())
            {
                return message;
            }

            if (rand.Next(10) == 0)
            {
                message = EnsurePlayerIsWarnedTwice(player);

                if (!message.IsNullOrEmpty())
                {
                    return message;
                }
            }

            // Pick a prank
            var roll = rand.Next(100);

            if (roll < 15)  // 15%
            {
                // Instantly change to base form
                if (TryAnimateTransform(player, player.OriginalFormSourceId))
                {
                    BlockCleanseMeditates(player);
                    BlockAttacks(player);
                    GiveEffect(player, ROOT_EFFECT, 1);
                    message = "You find yourself instantly in your base form!  The shock leaves you momentarily stunned!";
                }
            }
            else if (roll < 25)  // 10%
            {
                // Regular base form
                message = SetBaseFormToRegular(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 35)  // 10%
            {
                message = RestoreName(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 50)  // 15%
            {
                // Block cleans/meditate
                message = BlockCleanseMeditates(player);
            }
            else if (roll < 80)  // 30%
            {
                // Other resource/effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    message = rand.Next(2) == 0 ? MeanEffectsPrank(player) : MeanResourcePrank(player);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    message = rand.Next(2) == 0 ? MischievousEffectsPrank(player) : MischievousResourcePrank(player);
                }
                else
                {
                    message = rand.Next(2) == 0 ? MildEffectsPrank(player) : MildResourcePrank(player);
                }
            }
            else if (roll < 95)  // 15%
            {
                // Put in combat
                message = ResetCombatTimer(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else  // 5%
            {
                message = BanCharacter(player);
            }

            // Charge player AP for the prank
            if (message != null)
            {
                ChangeActionPoints(player, (int)(-PvPStatics.CleanseCost));
            }

            return message;
        }

        public static string Drop(Player player, int itemId)
        {
            // Don't return without dropping (or doing something similar).

            // Ensure player can cleanse
            if (player.ActionPoints < PvPStatics.CleanseCost || player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate || player.FormSourceId == player.OriginalFormSourceId)
            {
                return null;
            }

            var rand = new Random();

            if (rand.Next(2) !=  0)  // Prank 1 drop in 2
            {
                return null;
            }

            // Pick a prank
            var roll = rand.Next(4);
            var message = "";

            if (roll < 1 && PlayerHasBeenWarnedTwice(player))
            {
                // Mean
                var item = ItemProcedures.GetItemViewModel(itemId);
                var rollAgain = rand.Next(10);

                if (rollAgain < 1 && !item.dbItem.FormerPlayerId.HasValue && item.dbItem.Level < player.Level)
                {
                    ItemProcedures.DeleteItem(itemId);
                    message = $"You drop your {item.Item.FriendlyName}, but it vanishes into thin air before it can hit the ground!";
                }

                if (message.IsNullOrEmpty() && rollAgain < 2 && (!item.dbItem.FormerPlayerId.HasValue || (item.dbItem.IsPermanent && !item.dbItem.SoulboundToPlayerId.HasValue) ))
                {
                    var merchant = PlayerProcedures.GetPlayerFromBotId(item.Item.ItemType == PvPStatics.ItemType_Pet ? AIStatics.WuffieBotId : AIStatics.LindellaBotId);
                    if (merchant != null)
                    {
                        ItemProcedures.GiveItemToPlayer(itemId, merchant.Id);
                        message = $"You drop your {item.Item.FriendlyName}, but somebody quickly scoops it up and sells it to {merchant.GetFullName()}!";
                    }
                }

                if (message.IsNullOrEmpty())
                {
                    var location = LocationsStatics.GetRandomLocationNotInDungeon();
                    message = ItemProcedures.DropItem(itemId, location);
                    message = $"{message}  It falls through this realm and lands somewhere in town!";
                }

            }
            else if (roll < 2 && PlayerHasBeenWarned(player))
            {
                // Mischievous
                var location = LocationsStatics.GetRandomLocationNotInDungeon();
                var loc = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == location);
                var nearby = LocationsStatics.LocationList.GetLocation.Where(l => l.X >= loc.X -2 && l.X <= loc.X + 2 && l.Y >= loc.Y - 2 && l.Y <= loc.Y + 2 && l.Region != "dungeon").ToArray();
                var anchor = nearby[rand.Next(nearby.Count())];

                message = ItemProcedures.DropItem(itemId, location);
                message = $"{message}  It falls through this realm and lands somewhere in the vicinity of {anchor.Name}!";
            }
            else if (roll < 4)
            {
                // Mild
                var location = LocationsStatics.GetRandomLocationNotInDungeon();
                var loc = LocationsStatics.LocationList.GetLocation.First(l => l.dbName == location);
                message = ItemProcedures.DropItem(itemId, location);
                message = $"{message}  It falls through this realm and into {loc.Name}!";
            }

            if (message.IsNullOrEmpty())  // 50% chance of no prank
            {
                message = ItemProcedures.DropItem(itemId);
            }

            // Issue warnings/bans (after any actions that check player has been warned)
            if (CharacterIsBanned(player))
            {
                message = $"{message}  {EjectCharacter(player)}";
            }
            else
            {
                var warning = EnsurePlayerIsWarned(player);

                if (warning.IsNullOrEmpty() && rand.Next(10) == 0)
                {
                    warning = EnsurePlayerIsWarnedTwice(player);
                }

                if (!warning.IsNullOrEmpty())
                {
                    message = $"{warning}<br>{message}";
                }
            }

            return message;
        }

        public static string Attack(Player player, Player victim, SkillViewModel skill)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            var rand = new Random();

            if (rand.Next(5) !=  0)  // Prank 1 attack in 5
            {
                return null;
            }

            // Issue warnings
            var message = EnsurePlayerIsWarned(player);

            if (!message.IsNullOrEmpty())
            {
                return message;
            }

            if (rand.Next(10) == 0)
            {
                message = EnsurePlayerIsWarnedTwice(player);

                if (!message.IsNullOrEmpty())
                {
                    return message;
                }
            }

            // Pick a prank
            var roll = rand.Next(100);

            if (roll < 7)  // 7%
            {
                // Delay
                PlayerProcedures.AddAttackCount(player);
                return "You try to cast your spell, but it just fizzles away!";
            }
            else if (roll < 14)  // 7%
            {
                // Double
                var attacks = player.TimesAttackingThisUpdate;
                var attack1 = AttackProcedures.AttackSequence(player, victim, skill);
                var attack2 = AttackProcedures.AttackSequence(player, victim, skill);
                PlayerProcedures.SetAttackCount(player, attacks + 1);
                return $"<b>You accidentally fire off two spells instead of one!</b><br>{attack1}<br>{attack2}";
            }
            else if (roll < 21)  // 7%
            {
                // Deflect back at self
                var attack = AttackProcedures.AttackSequence(player, player, skill);
                return $"<b>Your victim deftly deflects your spell back at you!</b><br>{attack}";
            }
            else if (roll < 28)  // 7%
            {
                // Coerce victim to self-cast
                var attack = AttackProcedures.AttackSequence(victim, victim, skill);
                PlayerLogProcedures.AddPlayerLog(victim.Id, $"<b>Using a trick of the mind, {player.GetFullName()} convinces you to attack yourself!</b><br>{attack}", true);
                return $"Using a trick of the mind you convince your victim to attack themselves!";
            }
            else if (roll < 35)  // 7%
            {
                // Weaken
                var skillBeingUsed = SkillProcedures.GetSkillViewModel(PvPStatics.Spell_WeakenId, player.Id);

                if (skillBeingUsed != null && skillBeingUsed.StaticSkill.Id != skill.StaticSkill.Id)
                {
                    var attack = AttackProcedures.AttackSequence(player, victim, skillBeingUsed);
                    return $"<b>The aura of the Joke Shop messes with your head and you only manage to cast a weakening spell!</b><br>{attack}";
                }
            }
            else if (roll < 42)  // 7%
            {
                // Wrong spell
                var skillsAvailable = SkillProcedures.GetSkillViewModelsOwnedByPlayer(player.Id).Where(s => s.MobilityType == skill.MobilityType && s.StaticSkill.Id != skill.StaticSkill.Id).ToArray();
                
                if (skillsAvailable != null && skillsAvailable.Count() > 0)
                {
                    var skillToUse = skillsAvailable[rand.Next(skillsAvailable.Count())];
                    var attack = AttackProcedures.AttackSequence(player, victim, skillToUse);
                    return $"<b>You find yourself casting the wrong spell at your opponent!</b><br>{attack}";
                }
            }
            else if (roll < 49)  // 7%
            {
                // Block attacks
                BlockAttacks(player);
                return $"A magical shield prevents you casting any more attacks this turn!";
            }
            else if (roll < 56)  // 7%
            {
                // Free attack
                var attacks = player.TimesAttackingThisUpdate;
                var attack = AttackProcedures.AttackSequence(player, victim, skill);
                PlayerProcedures.SetAttackCount(player, attacks);
                return $"<b>You attack your enemy but don't feel drained at all!</b><br>{attack}";
            }
            else if (roll < 63)  // 7%
            {
                // Double cost attack
                var attacks = player.TimesAttackingThisUpdate;
                var attack = AttackProcedures.AttackSequence(player, victim, skill);
                PlayerProcedures.SetAttackCount(player, attacks + 2);
                return $"{attack}<br /><b>That attack cost a lot of energy, and you feel less able to cast another.</b>";
            }
            else if (roll < 70)  // 7%
            {
                // Make victim into attacker form
                IDbStaticSkillRepository skillsRepo = new EFDbStaticSkillRepository();
                var selfSkill = skillsRepo.DbStaticSkills.FirstOrDefault(spell => spell.FormSourceId == player.FormSourceId);

                if (selfSkill != null)
                {
                    var skillBeingUsed = SkillProcedures.GetSkillViewModel(selfSkill.Id, player.Id);

                    if (skillBeingUsed != null && skillBeingUsed.StaticSkill.Id != skill.StaticSkill.Id)
                    {
                        var attack = AttackProcedures.AttackSequence(player, victim, skillBeingUsed);
                        return $"<b>Your vanity gets in the way of your spellcasting as you try to turn {victim.GetFullName()} into a clone of yourself!</b><br>{attack}";
                    }
                }
            }
            else if (roll < 84)  // 14%
            {
                // Resource/times/quotas prank
                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    return rand.Next(2) == 0 ? MeanQuotasAndTimerPrank(player) : MeanResourcePrank(player);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    return rand.Next(2) == 0 ? MischievousQuotasAndTimerPrank(player) : MischievousResourcePrank(player);
                }
                else
                {
                    return rand.Next(2) == 0 ? MildQuotasAndTimerPrank(player) : MildResourcePrank(player);
                }
            }
            else if (roll < 98)  // 14%
            {
                // Effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    return MeanEffectsPrank(player);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    return MischievousEffectsPrank(player);
                }
                else
                {
                    return MildEffectsPrank(player);
                }
            }
            else  // 2%
            {
                message = BanCharacter(player);
            }

            return null;
        }

        #endregion

        #region Prank selection

        private static string MildPrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 20)  // 20%
            {
                return MildResourcePrank(player);
            }
            else if (roll < 30)  // 10%
            {
                return MildLocationPrank(player);
            }
            else if (roll < 50)  // 20%
            {
                return MildQuotasAndTimerPrank(player);
            }
            else if (roll < 65)  // 15%
            {
                return MildTransformationPrank(player);
            }
            else if (roll < 80)  // 15%
            {
                return MildEffectsPrank(player);
            }
            else  // 20%
            {
                return DiceGame(player);
            }
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

            if (roll < 20)  // 20%
            {
                return MischievousResourcePrank(player);
            }
            else if (roll < 40)  // 20%
            {
                return MischievousLocationPrank(player);
            }
            else if (roll < 60)  // 20%
            {
                return MischievousQuotasAndTimerPrank(player);
            }
            else if (roll < 80)  // 20%
            {
                return MischievousTransformationPrank(player);
            }
            else  // 20%
            {
                return MischievousEffectsPrank(player);
            }
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

            if (roll < 15)  // 15%
            {
                return MeanResourcePrank(player);
            }
            else if (roll < 35)  // 20%
            {
                return MeanLocationPrank(player);
            }
            else if (roll < 50)  // 15%
            {
                return MeanQuotasAndTimerPrank(player);
            }
            else if (roll < 60)  // 10%
            {
                return MeanTransformationPrank(player);
            }
            else if (roll < 75)  // 15%
            {
                return MeanEffectsPrank(player);
            }
            else if (roll < 80)  // 5%
            {
                return SummonPsychopath(player);
            }
            else if (roll < 85)  // 5%
            {
                return SummonDoppelganger(player);
            }
            else if (roll < 90)  // 5%
            {
                return OpenPsychoNip(player);
            }
            else  // 10%
            {
                return PlaceBountyOnPlayersHead(player);
            }
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

                var logMessage = EffectProcedures.GivePerkToPlayer(FIRST_WARNING_EFFECT.Value, player);
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

                var logMessage = EffectProcedures.GivePerkToPlayer(SECOND_WARNING_EFFECT.Value, player);
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
                return null;
            }

            var message = EffectProcedures.GivePerkToPlayer(BANNED_FROM_JOKE_SHOP_EFFECT.Value, player);
            var kickedOutMessage = EjectCharacter(player);

            return $"{message}  {kickedOutMessage}";
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
            user.dbLocationName = street;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, playerLog, false);
            LocationLogProcedures.AddLocationLog(LocationsStatics.JOKE_SHOP, leavingMessage);
            LocationLogProcedures.AddLocationLog(street, enteringMessage);

            return playerLog;
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
            if (amount >= 0)
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
            if (amount >= 0)
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
            if (amount >= 0)
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
            if (amount >= 0)
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

            if (amount >= 0)
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
                return false;
            }
            
            if (ItemProcedures.GetAllPlayerItems(player.Id).Count(i => !i.dbItem.IsEquipped) > PvPStatics.MaxCarryableItemCountBase + player.ExtraInventory)
            {
                // Carryiing too much
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
            return $"The bejeweled eyes of a strage ornament begin to glow as a raspy sucking voice echoes throughout the room:  \"Maybe you should talk to <b>{npcPlayer.GetFullName()}</b>?\"  The room then fades away.";
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

            if (!Teleport(player, bar, true))
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
                PlayerLogProcedures.AddPlayerLog(player.Id, $"You fall into the ether and are stuck as a {form.FriendlyName} for the next {duration} turns!", true);  // TODO joke_shop flavor text - ensure this message and effect message are consistent
            }

            return $"You are an inanimate {form.FriendlyName}.";  // TODO joke_shop flavor text - must inform player when they will auto revert, if they will
        }

        private static string MobileInanimateTransform(Player player)
        {
            // Turning a player into a rune or consumable is a bit too involved as there are no static forms for those items in the DB,
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

            return $"You are a mobile inanimate {form.FriendlyName}.";  // TODO joke_shop flavor text
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
            List<Player> candidates = ActivePlayersInJokeShopApartFrom(player);

            if (candidates.Count() == 0)
            {
                return null;
            }

            if (clone)
            {
                var victim = candidates[rand.Next(candidates.Count())];

                TryAnimateTransform(player, victim.FormSourceId);

                PlayerLogProcedures.AddPlayerLog(player.Id, $"You have become a clone of {victim.GetFullName()}", false);
                LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> became a clone of <b>{victim.GetFullName()}</b>.");

                return "Clone";  // TODO joke_shop flavor text
            }
            else
            {
                // Find nearboy player with sufficient consent
                Player victim = null;

                do
                {
                    var index = rand.Next(candidates.Count());
                    var candidate = candidates[index];

                    if (candidate.GameMode != (int)GameModeStatics.GameModes.Superprotection || PlayerHasBeenWarned(candidate))
                    {
                        victim = candidate;
                    }
                    else
                    {
                        candidates.RemoveAt(index);
                    }
                } while (victim == null && candidates.Count() > 0);

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

            return ChangeBaseForm(player, availableForms);
        }

        private static string ChangeBaseForm(Player player, int[] availableForms)
        {
            var formSourceId = availableForms[new Random().Next(availableForms.Count())];

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.OriginalFormSourceId = formSourceId;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"Your base form was changed.", false);

            return "Base form changed";  // TODO joke_shop flavor text
        }

        private static string SetBaseFormToRegular(Player player)
        {
            string message;
            var formRepo = new EFDbStaticFormRepository();
            var baseForms = formRepo.DbStaticForms.Where(f => (f.FriendlyName == "Regular Guy" || f.FriendlyName == "Regular Girl") &&
                                                               f.Id != player.OriginalFormSourceId)
                                                  .Select(f => f.Id).ToArray();

            if (baseForms.Count() > 0)
            {
                ChangeBaseForm(player, baseForms);
            }
            message = "You spot a fortune cookie and open it to see the message inside:  \"True purity can only come from the deepest of cleanses.\"  Let's hope the shopkeeper didn't see you!";
            return message;
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

            if (mobility != PvPStatics.MobilityFull)
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
            var rand = new Random();

            int[] forms = Array.Empty<int>();
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

            var form = FormStatics.GetForm(formSourceId);
            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>");

            return true;
        }

        private static bool TryInanimateTransform(Player player, int formSourceId, bool dropInventory, bool createItem = true, bool severe = true)
        {
            if ((severe && !PlayerHasBeenWarnedTwice(player)) || (!severe && !PlayerHasBeenWarned(player)))
            {
                return false;
            }

            PlayerProcedures.InstantChangeToForm(player, formSourceId);
            var form = FormStatics.GetForm(formSourceId);

            // If item is not created player will have no actions and not be visible to other players,
            // so some external mechanism must be in place to restore the player to animate form.
            if (createItem)
            {
                ItemProcedures.PlayerBecomesItem(player, form, null, dropInventory);
                // If inventory isn't dropped at point of TF then it will be dropped if/when player locks.
            }
            else if (dropInventory)
            {
                DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = player.Id, IgnoreRunes = false });
            }
            
            PlayerLogProcedures.AddPlayerLog(player.Id, $"You spontaneously turned into a {form.FriendlyName}.", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} spontaneously turned into a <b>{form.FriendlyName}</b>");

            return true;
        }

        #endregion

        #region Novel pranks

        private static string DiceGame(Player player)
        {
            var target = 69;

            // /roll 4d20
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

        private static string PlaceBountyOnPlayersHead(Player player)
        {
            // Only place bounties on PvP players
            if (player.GameMode != (int)GameModeStatics.GameModes.PvP)
            {
                return null;
            }

            var bountyEffect = BountyProcedures.PlaceBounty(player);

            if (!bountyEffect.HasValue)
            {
                return null;
            }

            var details = BountyProcedures.BountyDetails(player, bountyEffect.Value);

            if (details == null)
            {
                return null;
            }

            StatsProcedures.AddStat(player.MembershipId, StatsProcedures.Stat__BountyCount, 1);

            var rand = new Random();
            var locations = LocationsStatics.LocationList.GetLocation.Select(l => l.dbName).ToList();
            var locationMessage = $"<b>Wanted:</b>  A reward is on offer to whoever turns <b>{player.GetFullName()}</b> into a <b>{details.Form?.FriendlyName}</b>!";

            for (var i = 0; i < 5; i++)
            {
                var loc = locations[rand.Next(locations.Count())];
                LocationLogProcedures.AddLocationLog(loc, locationMessage);
                locations.Remove(loc);
            }

            var playerMessage = $"A bounty has been placed on your head!  Players will be trying to turn you into a <b>{details.Form?.FriendlyName}</b>!";  // TODO joke_shop flavor text
            PlayerLogProcedures.AddPlayerLog(player.Id, playerMessage, true);

            return playerMessage;
        }

        private static string SummonPsychopath(Player player)
        {
            var rand = new Random();

            var baseStrength = (int)Math.Min (Math.Max(0, player.Level / 3), 4);
            var strength = baseStrength + rand.Next(3);
            var prefix = "";
            int level;
            int perk;
            int? extraPerk = null;
            var gender = rand.Next(2);
            int form;

            if (strength <= 0)
            {
                level = 1;
                perk = AIProcedures.PsychopathicForLevelOneEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho1MId : AIProcedures.Psycho1FId;
            }
            else if (strength == 1)
            {
                level = 3;
                prefix = "Fierce";
                perk = AIProcedures.PsychopathicForLevelThreeEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho3MId : AIProcedures.Psycho3FId;
            }
            else if (strength == 2)
            {
                level = 5;
                prefix = "Wrathful";
                perk = AIProcedures.PsychopathicForLevelFiveEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho5MId : AIProcedures.Psycho5FId;
            }
            else if (strength == 3)
            {
                level = 6;
                prefix = "Loathful";
                perk = AIProcedures.PsychopathicForLevelSevenEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho7MId : AIProcedures.Psycho7FId;
            }
            else if (strength == 4)
            {
                level = 7;
                prefix = "Soulless";
                perk = AIProcedures.PsychopathicForLevelNineEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho9MId : AIProcedures.Psycho9FId;
            }
            else if (strength == 5)
            {
                level = 8;
                prefix = "Ruthless";
                perk = AIProcedures.PsychopathicForLevelNineEffectSourceId;
                extraPerk = AIProcedures.PsychopathicForLevelOneEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho9MId : AIProcedures.Psycho9FId;
            }
            else
            {
                level = 9;
                prefix = "Eternal";
                perk = AIProcedures.PsychopathicForLevelNineEffectSourceId;
                extraPerk = AIProcedures.PsychopathicForLevelThreeEffectSourceId;
                form = gender == 0 ? AIProcedures.Psycho9MId : AIProcedures.Psycho9FId;
            }

            var firstName = "Psychopath";
            var lastName = NameService.GetRandomLastName();

            if (!prefix.IsEmpty())
            {
                firstName = $"{prefix} {firstName}";
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cmd = new CreatePlayer
            {
                FirstName = firstName,
                LastName = lastName,
                Location = LocationsStatics.JOKE_SHOP,
                FormSourceId = form,
                Level = level,
                Health = 100000,
                MaxHealth = 100000,
                Mana = 100000,
                MaxMana = 100000,
                BotId = AIStatics.PsychopathBotId,
                UnusedLevelUpPerks = 0,
                XP = 0,
                Money = (strength + 1) * 50,
                Gender = gender == 0 ? PvPStatics.GenderMale : PvPStatics.GenderFemale,
            };

            var botId = DomainRegistry.Repository.Execute(cmd);

            // Give spells
            var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();
            SkillProcedures.GiveSkillToPlayer(botId, eligibleSkills[rand.Next(eligibleSkills.Count())].Id);

            if (strength >= 5)
            {
                SkillProcedures.GiveSkillToPlayer(botId, PvPStatics.Spell_WeakenId);
            }

            if (strength >= 6)
            {
                var limitedMobilityForms = STABLE_FORMS.Where(f => f.Category == LIMITED_MOBILITY).ToArray();

                if (limitedMobilityForms.Any())
                {
                    IDbStaticSkillRepository skillsRepo = new EFDbStaticSkillRepository();

                    var formId = limitedMobilityForms[rand.Next(limitedMobilityForms.Count())].FormSourceId;
                    var immobileSkill = skillsRepo.DbStaticSkills.FirstOrDefault(spell => spell.FormSourceId == formId);

                    if (immobileSkill != null)
                    {
                        SkillProcedures.GiveSkillToPlayer(botId, immobileSkill.Id);
                    }
                }
            }

            // Give bonuses
            EffectProcedures.GivePerkToPlayer(perk, botId);

            if (extraPerk.HasValue)
            {
                EffectProcedures.GivePerkToPlayer(extraPerk.Value, botId);
            }

            // Give a rune
            var runeId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = strength * 2 + 1, Random = rand });
            DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = runeId, PlayerId = botId });

            // Balance stats
            var psychoEF = playerRepo.Players.FirstOrDefault(p => p.Id == botId);
            psychoEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(psychoEF));
            playerRepo.SavePlayer(psychoEF);

            // Tell the bot to attack the player
            AIDirectiveProcedures.SetAIDirective_Attack(botId, player.Id);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"<b>You have summoned {firstName} {lastName}!</b>  Beware!  They are not friendly!!", true);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} has summoned <b>{firstName} {lastName}</b>!");

            return "You have summoned a psychopath!";  // TODO joke_shop flavor text
        }

        private static string SummonDoppelganger(Player player)
        {
            var rand = new Random();

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var cmd = new CreatePlayer
            {
                FirstName = $"Evil {player.FirstName}",
                LastName = player.LastName,
                Location = player.dbLocationName,
                FormSourceId = player.FormSourceId,
                Level = (int)Math.Min(9, player.Level),
                Health = 100000,
                MaxHealth = player.MaxHealth,
                Mana = 100000,
                MaxMana = player.MaxMana,
                BotId = AIStatics.PsychopathBotId,
                UnusedLevelUpPerks = 0,
                XP = 0,
                Money = 100 + player.Money / 10,
                Gender = player.Gender,
            };

            var botId = DomainRegistry.Repository.Execute(cmd);

            // Give spells
            var eligibleSkills = SkillStatics.GetLearnablePsychopathSkills().ToList();
            SkillProcedures.GiveSkillToPlayer(botId, eligibleSkills[rand.Next(eligibleSkills.Count())].Id);
            SkillProcedures.GiveSkillToPlayer(botId, PvPStatics.Spell_WeakenId);

            // Give bonuses
            var sourcePerks = EffectProcedures.GetPlayerEffects2(player.Id);

            foreach (var sourcePerk in sourcePerks)
            {
                 EffectProcedures.GivePerkToPlayer(sourcePerk.dbEffect.EffectSourceId, botId, sourcePerk.dbEffect.Duration, sourcePerk.dbEffect.Cooldown);
            }

            // Give a rune (round level down to odd)
            var runeLevel = cmd.Level - 1;
            runeLevel = runeLevel - (runeLevel % 2) + 1;
            var runeId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = runeLevel, Random = rand });
            DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = runeId, PlayerId = botId });

            // Balance stats
            var psychoEF = playerRepo.Players.FirstOrDefault(p => p.Id == botId);
            psychoEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(psychoEF));
            playerRepo.SavePlayer(psychoEF);

            // Tell the bot to attack the player
            AIDirectiveProcedures.SetAIDirective_Attack(botId, player.Id);

            PlayerLogProcedures.AddPlayerLog(player.Id, $"<b>You have summoned your evil twin!</b>  Beware!  They are not friendly!", true);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} has summoned their evil twin!");

            return "You have summoned a doppelganger!";  // TODO joke_shop flavor text
        }

        private static string OpenPsychoNip(Player player)
        {
            IAIDirectiveRepository directiveRepo = new EFAIDirectiveRepository();
            var idleBots = directiveRepo.AIDirectives.Where(d => d.State == "idle")
                                                     .Select(d => d.OwnerId)
                                                     .ToArray();

            // Set three lowest level psychos without targets on the player
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var botsToAttract = playerRepo.Players.Where(p => idleBots.Contains(p.Id)
                                                           && p.BotId == AIStatics.PsychopathBotId
                                                           && p.Mobility == PvPStatics.MobilityFull)
                                                  .OrderBy(p => p.Level)
                                                  .Select(p => p.Id)
                                                  .Take(3)
                                                  .ToArray();

            foreach (var botId in botsToAttract)
            {
                AIDirectiveProcedures.SetAIDirective_Attack(botId, player.Id);
            }

            PlayerLogProcedures.AddPlayerLog(player.Id, $"You open a tin pf PsychoNip", false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, $"{player.GetFullName()} opened a tin of PsychoNip!");

            return "You spot a tin with a colorful insignia on a shelf.  You move over to take a closer look, but accidentally knock the tin to the floor and spill its contents!  You gather up the fallen leaves and place them back in the tin.  \"PsychoNip,\" it reads.  Perhaps you should stay alert for the next few turns in case the scent has caught anyone's attention...";
        }

        #endregion

    }
}
