using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class JokeShopProcedures
    {
        // These fields back properies populated by the database
        private static int? FirstWarningEffect = null;
        private static int? SecondWarningEffect = null;
        private static int? BannedFromJokeShopEffect = null;

        private static int? RootEffect = null;

        private static int? AutoRestoreEffect = null;
        private static int? InstinctEffect = null;
        private static int? PsychoticEffect = null;
        private static int? InvisibilityEffect = null;

        private static List<FormDetail> StableForms = null;

        // Category to separate full from limited mobility animate forms
        public const string LIMITED_MOBILITY = "immobile";

        // IDs of some form sources grouped by theme
        public static readonly int[] MISCHIEVOUS_FORMS = { 215, 221, 438 };
        public static readonly int[] CATS_AND_NEKOS = { 39, 100, 385, 434, 504, 575, 668, 673, 681, 703, 713, 733, 752, 761, 806, 849, 851, 855, 987, 991, 1034, 1060, 1098, 1105, 1188, 1202 };
        public static readonly int[] DOGS = { 34, 359, 552, 667, 911, 912, 995, 1043, 1074, 1108, 1123, 1187 };
        public static readonly int[] RODENTS = { 70, 143, 205, 271, 278, 279, 317, 318, 319, 522, 772, 1077 };
        public static readonly int[] TREES = { 50, 741 };
        public static readonly int[] STRIPPERS = { 153, 719, 880 };
        public static readonly int[] DRONES = { 715, 930, 951, 1039, 1050 };
        public static readonly int[] SHEEP = { 204, 950, 1022, 1035, 1198 };
        public static readonly int[] MAIDS = { 65, 205, 305, 348, 457, 499, 514, 591, 652, 662, 673, 848, 869, 875, 901, 921, 958, 991, 1001, 1040, 1041, 1045, 1058, 1072, 1073, 1076, 1110, 1117, 1188, 1193, 1203, 1207 };
        public static readonly int[] GHOSTS = { 300, 456, 633, 1153, 1210 };
        public static readonly int[] MANA_FORMS = { 834, 1149 };

        // Effects supporting the Joke Shop mechanics
        // Try to contact DB once in a threadsafe way and cache results/failure
        public static int? FIRST_WARNING_EFFECT
        {
            get
            {
                if (!FirstWarningEffect.HasValue)
                {
                    FirstWarningEffect = EffectWithName("effect_Joke_Shop_Warned") ?? -1;
                }

                return FirstWarningEffect.Value == -1 ? null : FirstWarningEffect;
            }
            set
            {
                FirstWarningEffect = value;
            }
        }

        public static int? SECOND_WARNING_EFFECT
        {
            get
            {
                if (!SecondWarningEffect.HasValue)
                {
                    SecondWarningEffect = EffectWithName("effect_Joke_Shop_Warned_Twice") ?? -1;
                }

                return SecondWarningEffect.Value == -1 ? null : SecondWarningEffect;
            }
            set
            {
                SecondWarningEffect = value;
            }
        }

        public static int? BANNED_FROM_JOKE_SHOP_EFFECT
        {
            get
            {
                if (!BannedFromJokeShopEffect.HasValue)
                {
                    BannedFromJokeShopEffect = EffectWithName("effect_Joke_Shop_Banned") ?? -1;
                }

                return BannedFromJokeShopEffect.Value == -1 ? null : BannedFromJokeShopEffect;
            }
            set
            {
                BannedFromJokeShopEffect = value;
            }
        }

        // Specific and behavior-altering effects
        public static int? ROOT_EFFECT
        {
            get
            {
                if (!RootEffect.HasValue)
                {
                    RootEffect = EffectWithName("effect_Joke_Shop_Penalty_Mobility") ?? -1;
                }

                return RootEffect.Value == -1 ? null : RootEffect;
            }
            set
            {
                RootEffect = value;
            }
        }

        public static int? AUTO_RESTORE_EFFECT
        {
            get
            {
                if (!AutoRestoreEffect.HasValue)
                {
                    AutoRestoreEffect = EffectWithName("effect_Joke_Shop_Auto_Restore") ?? -1;
                }

                return AutoRestoreEffect.Value == -1 ? null : AutoRestoreEffect;
            }
            set
            {
                AutoRestoreEffect = value;
            }
        }

        public static int? INSTINCT_EFFECT
        {
            get
            {
                if (!InstinctEffect.HasValue)
                {
                    InstinctEffect = EffectWithName("effect_Joke_Shop_MC_Instinct") ?? -1;
                }

                return InstinctEffect.Value == -1 ? null : InstinctEffect;
            }
            set
            {
                InstinctEffect = value;
            }
        }

        public static int? PSYCHOTIC_EFFECT
        {
            get
            {
                if (!PsychoticEffect.HasValue)
                {
                    PsychoticEffect = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Psychotic") ?? -1;
                }

                return PsychoticEffect == -1 ? null : PsychoticEffect;
            }
            set
            {
                PsychoticEffect = value;
            }
        }

        public static int? INVISIBILITY_EFFECT
        {
            get
            {
                if (!InvisibilityEffect.HasValue)
                {
                    InvisibilityEffect = JokeShopProcedures.EffectWithName("effect_Joke_Shop_Invisible_PvP") ?? -1;
                }

                return InvisibilityEffect == -1 ? null : InvisibilityEffect;
            }
            set
            {
                InvisibilityEffect = value;
            }
        }

        // This list of forms is intended to be 'stable' within a run so that a calulation will always determine the same form.
        internal static List<FormDetail> STABLE_FORMS
        {
            get
            {
                if (StableForms == null)
                {
                    StableForms = CandidateForms() ?? new List<FormDetail>();
                }

                return StableForms;
            }
            set
            {
                StableForms = value;
            }
        }

        #region Core mechanics and utilities

        private static List<FormDetail> CandidateForms()
        {
            IDbStaticSkillRepository skillsRepo = new EFDbStaticSkillRepository();
            var learnableSpells = skillsRepo.DbStaticSkills.Where(spell => spell.IsLive == "live" && spell.IsPlayerLearnable).Select(spell => new { spell.FormSourceId }).ToList();

            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var forms = formsRepo.DbStaticForms.Select(form => new { form.FriendlyName, Immobile = form.MoveActionPointDiscount < -5, form.Id, form.MobilityType, form.ItemSourceId });

            var learnableForms = learnableSpells.Join(forms, spell => spell.FormSourceId, form => form.Id, (spell, form) => new { form.Id, form.FriendlyName, form.Immobile, form.MobilityType, form.ItemSourceId });

            IDbStaticItemRepository itemsRepo = new EFDbStaticItemRepository();
            var itemTypes = itemsRepo.DbStaticItems.Select(i => new { i.Id, i.ItemType }).ToList();

            var formDetails = learnableForms.Join(itemTypes, form => form.ItemSourceId, itemType => itemType.Id, (form, item) => new FormDetail(form.Id, form.FriendlyName, item.ItemType)).ToList();
            formDetails.AddRange(learnableForms.Where(form => form.ItemSourceId == null && form.MobilityType == PvPStatics.MobilityFull).Select(form => new FormDetail(form.Id, form.FriendlyName, form.Immobile ? LIMITED_MOBILITY : form.MobilityType)));

            return formDetails;
        }

        public static List<FormDetail> Forms(Func<FormDetail, bool> predicate)
        {
            return STABLE_FORMS.Where(predicate).ToList();
        }

        public static int? EffectWithName(string dbName)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.dbName == dbName).FirstOrDefault()?.Id;
        }

        public static List<int> EffectsWithNamesStarting(string prefix)
        {
            IEffectRepository effectRepo = new EFEffectRepository();
            return effectRepo.DbStaticEffects.Where(e => e.dbName.StartsWith(prefix)).Select(e => e.Id).ToList();
        }

        internal static void RunEffectExpiryActions(List<Effect> expiringEffects)
        {
            // Undo temporary TFs
            if (AUTO_RESTORE_EFFECT.HasValue)
            {
                var playersToRestore = expiringEffects.Where(e => e.EffectSourceId == AUTO_RESTORE_EFFECT.Value && e.Duration == 0).Select(e => e.OwnerId);
                foreach (var player in playersToRestore)
                {
                    CharacterPrankProcedures.UndoTemporaryForm(player);
                }
            }

            // Undo temporary psychopath TF
            if (PSYCHOTIC_EFFECT.HasValue)
            {
                var playerRepo = new EFPlayerRepository();
                var playersToRestore = expiringEffects.Where(e => e.EffectSourceId == PSYCHOTIC_EFFECT.Value && e.Duration == 0).Select(e => e.OwnerId);

                foreach (var player in playersToRestore)
                {
                    var user = playerRepo.Players.FirstOrDefault(p => p.Id == player);
                    user.BotId = AIStatics.ActivePlayerBotId;
                    playerRepo.SavePlayer(user);

                    AIDirectiveProcedures.DeleteAIDirectiveByPlayerId(player);

                    PlayerLogProcedures.AddPlayerLog(player, "The psychotic voices leave your head.. for now..", true);
                }
            }

            // Undo temporary invisibility
            if (INVISIBILITY_EFFECT.HasValue)
            {
                var playerRepo = new EFPlayerRepository();
                var playersToRestore = expiringEffects.Where(e => e.EffectSourceId == INVISIBILITY_EFFECT.Value && e.Duration == 0).Select(e => e.OwnerId);

                // Defensively revert any invisible players without the effect too
                if (PvPStatics.LastGameTurn % 5 == 2)
                {
                    var invisiblePlayers = playerRepo.Players.Where(p => !playersToRestore.Contains(p.Id) &&
                                                                         p.GameMode == (int)GameModeStatics.GameModes.Invisible)
                                                             .Select(p => p.Id);

                    foreach (var invisiblePlayer in invisiblePlayers)
                    {
                        if (!EffectProcedures.PlayerHasActiveEffect(invisiblePlayer, PSYCHOTIC_EFFECT.Value))
                        {
                            // Player is somehow in invisible limbo
                            playersToRestore = playersToRestore.Append(invisiblePlayer);
                        }
                    }

                }

                // Restore visibility
                foreach (var player in playersToRestore)
                {
                    var user = playerRepo.Players.FirstOrDefault(p => p.Id == player);

                    DomainRegistry.Repository.Execute(new ChangeGameMode
                    {
                        MembershipId = user.MembershipId,
                        GameMode = (int)GameModeStatics.GameModes.PvP,
                        Force = true
                    });

                    PlayerLogProcedures.AddPlayerLog(player, "Your cloak of invisibility starts to fade, leaving you visible to the world once more.", true);
                    LocationLogProcedures.AddLocationLog(user.dbLocationName, $"{user.GetFullName()} seems to appear from nowhere!");
                }

                // Ensure no items have been lost with a weird game mode
                if (playersToRestore.Any())
                {
                    var itemRepo = new EFItemRepository();
                    var itemsInLimbo = itemRepo.Items.Where(i => i.PvPEnabled == (int)GameModeStatics.GameModes.Invisible);

                    foreach (var item in itemsInLimbo)
                    {
                        item.PvPEnabled = (int)GameModeStatics.GameModes.PvP;
                        itemRepo.SaveItem(item);
                    }
                }
            }

            // Update player challenges
            ChallengeProcedures.CheckExpiringChallenges(expiringEffects);
        }

        internal static void RunEffectActions(List<Effect> effects)
        {
            if (INSTINCT_EFFECT.HasValue)
            {
                var playersToControl = effects.Where(e => e.EffectSourceId == INSTINCT_EFFECT.Value).Select(e => e.OwnerId).ToList();
                InstinctProcedures.ActOnInstinct(playersToControl, new Random());
            }
        }

        public static List<Player> ActivePlayersInJokeShopApartFrom(Player player)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

            var playerRepo = new EFPlayerRepository();
            var candidates = playerRepo.Players
                .Where(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                            p.OnlineActivityTimestamp >= cutoff &&
                            p.Id != player.Id &&
                            p.Mobility == PvPStatics.MobilityFull &&
                            p.InDuel <= 0 &&
                            p.InQuest <= 0 &&
                            p.BotId == AIStatics.ActivePlayerBotId)
                .ToList();

            return candidates;
        }

        public static bool IsJokeShopActive()
        {
            return LocationsStatics.LocationList.GetLocation.Any(l => l.dbName == LocationsStatics.JOKE_SHOP);
        }

        public static void SetJokeShopActive(bool active)
        {
            // Work on a copy of the map to avoid concurrency issues
            var newMap = LocationsStatics.LocationList.GetLocation.Select(l => l.Clone()).ToList();
            var jokeShopTile = newMap.FirstOrDefault(l => l.dbName == LocationsStatics.JOKE_SHOP);
            var initiallyActive = jokeShopTile != null;

            if (initiallyActive == active)
            {
                return;
            }

            if (active == true)
            {
                // Activate joke shop

                // Add tile to map
                jokeShopTile = new Location
                {
                    dbName = LocationsStatics.JOKE_SHOP,
                    Name = "Cursed Joke Shop",
                    Region = "limbo"
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

                foreach (var item in itemRepo.Items.Where(i => i.dbLocationName == LocationsStatics.JOKE_SHOP).ToList())
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

        public static string Search(Player player, Random rand = null)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            rand = rand ?? new Random();

            // Decide whether this is a regular or a prank search
            if (rand.Next(5) < 3)  // Normal search 60% of the time, else attempt a prank
            {
                return null;
            }

            var roll = rand.Next(100);

            if (roll < 65)  // 65%
            {
                return MildPrank(player, rand);
            }
            else if (roll < 90)  // 25%
            {
                return MischievousPrank(player, rand);
            }
            else if (roll < 95)  // 5%
            {
                return MeanPrank(player, rand);
            }
            else  // 5%
            {
                return BanCharacter(player);
            }
        }

        public static string Meditate(Player player, Random rand = null)
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

            rand = rand ?? new Random();

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
                if (CharacterPrankProcedures.TryAnimateTransform(player, MANA_FORMS[rand.Next(MANA_FORMS.Count())]))
                {
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    message = "Oh dear, it looks like you've been meditating too hard!";
                }
            }
            else if (roll < 28)  // 25%
            {
                // Toy with mana
                message = EnvironmentPrankProcedures.ChangeMana(player, rand.Next(200) - 100);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 38)  // 10%
            {
                // Toy with health
                message = EnvironmentPrankProcedures.ChangeHealth(player, rand.Next(400) - 200);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 48)  // 10%
            {
                // Mana recharge
                EnvironmentPrankProcedures.ChangeMana(player, (int)(player.MaxMana - player.Mana + 1));
                EnvironmentPrankProcedures.BlockCleanseMeditates(player);
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
                    EnvironmentPrankProcedures.ChangeMana(player, (int)amount);
                    EnvironmentPrankProcedures.ChangeMana(other, (int)-amount);
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    PlayerLogProcedures.AddPlayerLog(other.Id, $"{player.GetFullName()} steals {amount} mana from you!", true);
                    message = $"You steal {amount} mana from {other.GetFullName()}!";
                }
            }
            else if (roll < 68)  // 10%
            {
                // Block cleans/meditate
                message = EnvironmentPrankProcedures.BlockCleanseMeditates(player);
            }
            else if (roll < 93)  // 25%
            {
                // Other resource/effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MeanEffectsPrank(player, rand) : EnvironmentPrankProcedures.MeanResourcePrank(player, rand);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MischievousEffectsPrank(player, rand) : EnvironmentPrankProcedures.MischievousResourcePrank(player, rand);
                }
                else
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MildEffectsPrank(player, rand) : EnvironmentPrankProcedures.MildResourcePrank(player, rand);
                }
            }
            else if (roll < 98)  // 5%
            {
                // Put in combat
                message = EnvironmentPrankProcedures.ResetCombatTimer(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else  // 2%
            {
                message = BanCharacter(player);
            }

            // Charge player AP for the prank
            if (message != null)
            {
                EnvironmentPrankProcedures.ChangeActionPoints(player, (int)-PvPStatics.MeditateCost);
            }

            return message;
        }

        public static string Cleanse(Player player, Random rand = null)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            // Mechanism through which a player can elect to restore their original name
            if (player.Health >= player.MaxHealth)
            {
                return CharacterPrankProcedures.RestoreName(player);
            }

            // Ensure player can cleanse
            if (player.ActionPoints < PvPStatics.CleanseCost || player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate)
            {
                return null;
            }

            rand = rand ?? new Random();

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
                if (CharacterPrankProcedures.TryAnimateTransform(player, MAIDS[rand.Next(MAIDS.Count())]))
                {
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    message = "You've been cleansing a lot.  Maybe you would like to clean the shop while you're at it?";
                }
            }
            else if (roll < 10)  // 5%
            {
                // Change to base form
                if (player.FormSourceId != player.OriginalFormSourceId && CharacterPrankProcedures.TryAnimateTransform(player, player.OriginalFormSourceId))
                {
                    CharacterPrankProcedures.RestoreBaseForm(player);
                    EnvironmentPrankProcedures.BlockCleanseMeditates(player);
                    message = "Oops.. You might have just cleansed a little too much!";
                }
            }
            else if (roll < 15)  // 5%
            {
                message = CharacterPrankProcedures.SetBaseFormToRegular(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 20)  // 5%
            {
                message = CharacterPrankProcedures.RestoreName(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 36)  // 16%
            {
                // Toy with health
                message = EnvironmentPrankProcedures.ChangeHealth(player, rand.Next(400) - 200);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 46)  // 10%
            {
                // Health recharge
                EnvironmentPrankProcedures.ChangeHealth(player, (int)(player.MaxHealth - player.Health + 1));
                PlayerProcedures.AddCleanseMeditateCount(player);
                EnvironmentPrankProcedures.BlockAttacks(player);
                CharacterPrankProcedures.GiveEffect(player, ROOT_EFFECT, 1);
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
                    EnvironmentPrankProcedures.ChangeHealth(player, (int)amount);
                    EnvironmentPrankProcedures.ChangeHealth(other, (int)-amount);
                    PlayerProcedures.AddCleanseMeditateCount(player);
                    PlayerLogProcedures.AddPlayerLog(other.Id, $"{player.GetFullName()} steals {amount} of your willpower!", true);
                    message = $"You steal {amount} willpower from {other.GetFullName()}!";
                }
            }
            else if (roll < 68)  // 12%
            {
                // Block cleans/meditate
                message = EnvironmentPrankProcedures.BlockCleanseMeditates(player);
            }
            else if (roll < 93)  // 25%
            {
                // Other resource/effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MeanEffectsPrank(player, rand) : EnvironmentPrankProcedures.MeanResourcePrank(player, rand);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MischievousEffectsPrank(player, rand) : EnvironmentPrankProcedures.MischievousResourcePrank(player, rand);
                }
                else
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MildEffectsPrank(player, rand) : EnvironmentPrankProcedures.MildResourcePrank(player, rand);
                }
            }
            else if (roll < 98)  // 5%
            {
                // Put in combat
                message = EnvironmentPrankProcedures.ResetCombatTimer(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else  // 2%
            {
                message = BanCharacter(player);
            }

            // Charge player AP for the prank
            if (message != null)
            {
                EnvironmentPrankProcedures.ChangeActionPoints(player, (int)-PvPStatics.CleanseCost);
            }

            return message;
        }

        public static string SelfRestore(Player player, Random rand = null)
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

            rand = rand ?? new Random();

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
                if (CharacterPrankProcedures.TryAnimateTransform(player, player.OriginalFormSourceId))
                {
                    EnvironmentPrankProcedures.BlockCleanseMeditates(player);
                    EnvironmentPrankProcedures.BlockAttacks(player);
                    CharacterPrankProcedures.GiveEffect(player, ROOT_EFFECT, 1);
                    message = "You find yourself instantly in your base form!  The shock leaves you momentarily stunned!";
                }
            }
            else if (roll < 25)  // 10%
            {
                // Regular base form
                message = CharacterPrankProcedures.SetBaseFormToRegular(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 35)  // 10%
            {
                message = CharacterPrankProcedures.RestoreName(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else if (roll < 50)  // 15%
            {
                // Block cleans/meditate
                message = EnvironmentPrankProcedures.BlockCleanseMeditates(player);
            }
            else if (roll < 80)  // 30%
            {
                // Other resource/effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MeanEffectsPrank(player, rand) : EnvironmentPrankProcedures.MeanResourcePrank(player, rand);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MischievousEffectsPrank(player, rand) : EnvironmentPrankProcedures.MischievousResourcePrank(player, rand);
                }
                else
                {
                    message = rand.Next(2) == 0 ? CharacterPrankProcedures.MildEffectsPrank(player, rand) : EnvironmentPrankProcedures.MildResourcePrank(player, rand);
                }
            }
            else if (roll < 95)  // 15%
            {
                // Put in combat
                message = EnvironmentPrankProcedures.ResetCombatTimer(player);
                PlayerProcedures.AddCleanseMeditateCount(player);
            }
            else  // 5%
            {
                message = BanCharacter(player);
            }

            // Charge player AP for the prank
            if (message != null)
            {
                EnvironmentPrankProcedures.ChangeActionPoints(player, (int)-PvPStatics.CleanseCost);
            }

            return message;
        }

        public static string Drop(Player player, int itemId, Random rand = null)
        {
            // Don't return without dropping (or doing something similar).

            // Ensure player can cleanse
            if (player.ActionPoints < PvPStatics.CleanseCost || player.CleansesMeditatesThisRound >= PvPStatics.MaxCleansesMeditatesPerUpdate || player.FormSourceId == player.OriginalFormSourceId)
            {
                return null;
            }

            rand = rand ?? new Random();

            if (rand.Next(2) != 0)  // Prank 1 drop in 2
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

                if (message.IsNullOrEmpty() && rollAgain < 2 && (!item.dbItem.FormerPlayerId.HasValue || item.dbItem.IsPermanent && !item.dbItem.SoulboundToPlayerId.HasValue))
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
                var nearby = LocationsStatics.LocationList.GetLocation.Where(l => l.X >= loc.X - 2 && l.X <= loc.X + 2 && l.Y >= loc.Y - 2 && l.Y <= loc.Y + 2 && l.Region != "dungeon").ToArray();
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

        public static string Attack(Player player, Player victim, SkillViewModel skill, Random rand = null)
        {
            if (CharacterIsBanned(player))
            {
                return EjectCharacter(player);
            }

            rand = rand ?? new Random();

            if (rand.Next(5) != 0)  // Prank 1 attack in 5
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
                EnvironmentPrankProcedures.BlockAttacks(player);
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
                    return rand.Next(2) == 0 ? EnvironmentPrankProcedures.MeanQuotasAndTimerPrank(player, rand) : EnvironmentPrankProcedures.MeanResourcePrank(player, rand);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    return rand.Next(2) == 0 ? EnvironmentPrankProcedures.MischievousQuotasAndTimerPrank(player, rand) : EnvironmentPrankProcedures.MischievousResourcePrank(player, rand);
                }
                else
                {
                    return rand.Next(2) == 0 ? EnvironmentPrankProcedures.MildQuotasAndTimerPrank(player, rand) : EnvironmentPrankProcedures.MildResourcePrank(player, rand);
                }
            }
            else if (roll < 98)  // 14%
            {
                // Effects prank
                PlayerProcedures.AddCleanseMeditateCount(player);

                var prankRoll = rand.Next(4);
                if (prankRoll < 1 && PlayerHasBeenWarnedTwice(player))
                {
                    return CharacterPrankProcedures.MeanEffectsPrank(player, rand);
                }
                else if (prankRoll < 2 && PlayerHasBeenWarned(player))
                {
                    return CharacterPrankProcedures.MischievousEffectsPrank(player, rand);
                }
                else
                {
                    return CharacterPrankProcedures.MildEffectsPrank(player, rand);
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

        private static string MildPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 15)  // 15%
            {
                return EnvironmentPrankProcedures.MildResourcePrank(player, rand);
            }
            else if (roll < 25)  // 10%
            {
                return EnvironmentPrankProcedures.MildLocationPrank(player, rand);
            }
            else if (roll < 35)  // 10%
            {
                return EnvironmentPrankProcedures.MildQuotasAndTimerPrank(player, rand);
            }
            else if (roll < 50)  // 15%
            {
                return CharacterPrankProcedures.MildTransformationPrank(player, rand);
            }
            else if (roll < 65)  // 15%
            {
                return CharacterPrankProcedures.MildEffectsPrank(player, rand);
            }
            else if (roll < 70)  // 5%
            {
                return EnvironmentPrankProcedures.RareFind(player, rand);
            }
            else if (roll < 75)  // 5%
            {
                return NovelPrankProcedures.RandomShout(player, rand);
            }
            else if (roll < 80)  // 5%
            {
                return NovelPrankProcedures.LocatePlayerInCombat(player, rand);
            }
            else if (roll < 90)  // 10%
            {
                return NovelPrankProcedures.AwardChallenge(player, 0, 20, false);
            }
            else  // 10%
            {
                return NovelPrankProcedures.DiceGame(player);
            }
        }

        private static string MischievousPrank(Player player, Random rand = null)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 15)  // 15%
            {
                return EnvironmentPrankProcedures.MischievousResourcePrank(player, rand);
            }
            else if (roll < 30)  // 15%
            {
                return EnvironmentPrankProcedures.MischievousLocationPrank(player, rand);
            }
            else if (roll < 45)  // 15%
            {
                return EnvironmentPrankProcedures.MischievousQuotasAndTimerPrank(player, rand);
            }
            else if (roll < 65)  // 20%
            {
                return CharacterPrankProcedures.MischievousTransformationPrank(player, rand);
            }
            else if (roll < 85)  // 20%
            {
                return CharacterPrankProcedures.MischievousEffectsPrank(player, rand);
            }
            else  // 15%
            {
                return NovelPrankProcedures.AwardChallenge(player, 10, 60);
            }
        }

        private static string MeanPrank(Player player, Random rand = null)
        {
            var warning = EnsurePlayerIsWarnedTwice(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 15)  // 15%
            {
                return EnvironmentPrankProcedures.MeanResourcePrank(player, rand);
            }
            else if (roll < 30)  // 15%
            {
                return EnvironmentPrankProcedures.MeanLocationPrank(player, rand);
            }
            else if (roll < 40)  // 10%
            {
                return EnvironmentPrankProcedures.MeanQuotasAndTimerPrank(player, rand);
            }
            else if (roll < 50)  // 10%
            {
                return CharacterPrankProcedures.MeanTransformationPrank(player, rand);
            }
            else if (roll < 65)  // 15%
            {
                return CharacterPrankProcedures.MeanEffectsPrank(player, rand);
            }
            else if (roll < 69)  // 4%
            {
                return NovelPrankProcedures.SummonPsychopath(player, rand);
            }
            else if (roll < 73)  // 4%
            {
                return NovelPrankProcedures.SummonDoppelganger(player, rand);
            }
            else if (roll < 77)  // 4%
            {
                return NovelPrankProcedures.OpenPsychoNip(player);
            }
            else if (roll < 81)  // 4%
            {
                return NovelPrankProcedures.ForceAttack(player, false, rand);
            }
            else if (roll < 85)  // 4%
            {
                return NovelPrankProcedures.Incite(player, rand);
            }
            else if (roll < 92)  // 7%
            {
                return NovelPrankProcedures.AwardChallenge(player, 20, 480, true);
            }
            else  // 8%
            {
                return NovelPrankProcedures.PlaceBountyOnPlayersHead(player, rand);
            }
        }

        #endregion

        #region Warnings and access controls

        public static bool PlayerHasBeenWarned(Player player)
        {
            return FIRST_WARNING_EFFECT.HasValue && EffectProcedures.PlayerHasEffect(player, FIRST_WARNING_EFFECT.Value);
        }

        public static bool PlayerHasBeenWarnedTwice(Player player)
        {
            return SECOND_WARNING_EFFECT.HasValue && EffectProcedures.PlayerHasEffect(player, SECOND_WARNING_EFFECT.Value);
        }

        public static string EnsurePlayerIsWarned(Player player)
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

        public static string EnsurePlayerIsWarnedTwice(Player player)
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
                    EnvironmentPrankProcedures.ResetActivityTimer(player);
                    EnvironmentPrankProcedures.ChangeHealth(player, -50);
                    EnvironmentPrankProcedures.ChangeMana(player, -10);
                }
                else if (PlayerHasBeenWarned(player))
                {
                    EnvironmentPrankProcedures.ResetActivityTimer(player, 0.5);
                    EnvironmentPrankProcedures.ChangeHealth(player, -25);
                    EnvironmentPrankProcedures.ChangeMana(player, -5);
                }
            }
        }

        #endregion

    }
}
