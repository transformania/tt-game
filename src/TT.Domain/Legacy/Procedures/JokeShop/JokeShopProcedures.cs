using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class JokeShopProcedures
    {
        // Category to separate full from limited mobility animate forms
        public const string LIMITED_MOBILITY = "immobile";

        // IDs of some form sources grouped by theme
        public static readonly int[] BIMBOS = { 19, 118, 161, 318, 327, 454, 457, 470, 503, 570, 644, 698, 702, 725, 785, 951, 1003, 1093, 1342, 1389, 1425 };
        public static readonly int[] CATS_AND_NEKOS = { 39, 100, 385, 434, 504, 668, 673, 681, 703, 713, 752, 806, 849, 851, 855, 987, 991, 1060, 1098, 1105, 1188, 1215, 1245, 1247, 1317, 1392, 1422, 1446, 1459, 1488 };
        public static readonly int[] CHRISTMAS_FORMS = { 741, 1248, 1297 };
        public static readonly int[] DOGS = { 34, 359, 552, 667, 911, 912, 995, 1043, 1074, 1123, 1249, 1489 };
        public static readonly int[] DRONES = { 715, 930, 951, 1039, 1050, 1451 };
        public static readonly int[] EASTER_FORMS = { 27, 247, 560, 826, 857, 903, 970, 1102 };
        public static readonly int[] FAIRIES = { 210, 257, 389, 531, 582, 639, 701, 771, 773, 833, 846, 895, 1034, 1093, 1100, 1529 };
        public static readonly int[] GHOSTS = { 300, 633, 1153, 1387 };
        public static readonly int[] HALLOWEEN_FORMS = { 94, 95, 190, 246, 300, 527, 633, 650, 691, 822, 863, 882, 1153 };
        public static readonly int[] MAIDS = { 65, 205, 348, 457, 499, 514, 591, 652, 662, 673, 848, 869, 875, 901, 921, 958, 991, 1045, 1072, 1246, 1343, 1388, 1488, 1489, 1490, 1492, 1541, 1557 };
        public static readonly int[] MANA_FORMS = { 834, 1149 };
        public static readonly int[] MISCHIEVOUS_FORMS = { 215, 221, 438, 1099, 1238 };
        public static readonly int[] RODENTS = { 70, 143, 205, 271, 317, 318, 319, 522, 772, 1237 };
        public static readonly int[] ROMANTIC_FORMS = { 315, 451, 933, 943, 1449 };
        public static readonly int[] SHEEP = { 204, 950, 1035, 1492, 1527};
        public static readonly int[] STRIPPERS = { 153, 719, 880 };
        public static readonly int[] THIEVES = { 143, 149, 271, 849 };
        public static readonly int[] TREES = { 50, 741 };

        // Effect sources supporting the Joke Shop mechanics
        public const int FIRST_WARNING_EFFECT = 199;
        public const int SECOND_WARNING_EFFECT = 200;
        public const int BANNED_FROM_JOKE_SHOP_EFFECT = 201;

        // Specific and behavior-altering effect sources
        public const int ROOT_EFFECT = 250;
        public const int AUTO_RESTORE_EFFECT = 202;
        public const int INSTINCT_EFFECT = 203;
        public const int PSYCHOTIC_EFFECT = 207;
        public const int INVISIBILITY_EFFECT = 208;

        // This list of forms is intended to be 'stable' within a run so that a calulation will always determine the same form.
        private static List<FormDetail> STABLE_FORMS
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

        private static List<FormDetail> StableForms = null;

        #region Core mechanics and utilities

        private static List<FormDetail> CandidateForms()
        {
            IDbStaticSkillRepository skillsRepo = new EFDbStaticSkillRepository();
            var learnableSpells = skillsRepo.DbStaticSkills
                .Where(spell => spell.IsLive == "live" &&
                                spell.IsPlayerLearnable)
                .Select(spell => new { spell.FormSourceId }).ToList();

            IDbStaticFormRepository formsRepo = new EFDbStaticFormRepository();
            var forms = formsRepo.DbStaticForms
                .Where(form => !form.IsUnique)
                .Select(form => new { form.FriendlyName, Immobile = form.MoveActionPointDiscount < -5, form.Id, form.MobilityType, form.ItemSourceId });

            var learnableForms = learnableSpells.Join(forms, spell => spell.FormSourceId, form => form.Id,
                (spell, form) => new { form.Id, form.FriendlyName, form.Immobile, form.MobilityType, form.ItemSourceId });

            IDbStaticItemRepository itemsRepo = new EFDbStaticItemRepository();
            var itemTypes = itemsRepo.DbStaticItems
                .Where(i => !i.IsUnique)
                .Select(i => new { i.Id, i.ItemType }).ToList();

            // Item forms players can become
            var formDetails = learnableForms.Join(itemTypes, form => form.ItemSourceId, itemType => itemType.Id,
                (form, item) => new FormDetail(form.Id, form.FriendlyName, item.ItemType)).ToList();

            // Add full mobility forms players can become
            formDetails.AddRange(learnableForms
                                    .Where(form => form.ItemSourceId == null &&
                                           form.MobilityType == PvPStatics.MobilityFull)
                                    .Select(form => new FormDetail(form.Id, form.FriendlyName, form.Immobile ? LIMITED_MOBILITY : form.MobilityType)));

            return formDetails;
        }

        // Forms here are intended to be stable within a run and only forms players can get through regular spells
        public static List<FormDetail> Forms(Func<FormDetail, bool> predicate)
        {
            return STABLE_FORMS.Where(predicate).ToList();
        }

        public static List<FormDetail> AnimateForms()
        {
            return STABLE_FORMS.Where(f => f.Category == PvPStatics.MobilityFull || f.Category == LIMITED_MOBILITY).ToList();
        }

        public static List<FormDetail> InanimateForms()
        {
            return STABLE_FORMS.Where(f => f.Category != PvPStatics.MobilityFull && f.Category != LIMITED_MOBILITY).ToList();
        }

        internal static void RunEffectExpiryActions(List<Effect> expiringEffects)
        {
            UndoTemporaryTFs(expiringEffects);

            UndoTemporaryPsychopathTFs(expiringEffects);

            UndoTemporaryInvisibility(expiringEffects);

            // Update player challenges (for players on their last chance to pass or fail)
            ChallengeProcedures.CheckExpiringChallenges(expiringEffects);
        }

        private static void UndoTemporaryTFs(List<Effect> expiringEffects)
        {
            var playersToRestore = expiringEffects.Where(e => e.EffectSourceId == AUTO_RESTORE_EFFECT && e.Duration == 0).Select(e => e.OwnerId);
            foreach (var player in playersToRestore)
            {
                CharacterPrankProcedures.UndoTemporaryForm(player);
            }
        }

        private static void UndoTemporaryPsychopathTFs(List<Effect> expiringEffects)
        {
            var playerRepo = new EFPlayerRepository();
            var playersToRestore = expiringEffects.Where(e => e.EffectSourceId == PSYCHOTIC_EFFECT && e.Duration == 0).Select(e => e.OwnerId);

            foreach (var player in playersToRestore)
            {
                CharacterPrankProcedures.UndoPsychotic(player);
            }
        }

        private static void UndoTemporaryInvisibility(List<Effect> expiringEffects)
        {
            var playerRepo = new EFPlayerRepository();
            var playersToRestore = expiringEffects.Where(e => e.EffectSourceId == INVISIBILITY_EFFECT && e.Duration == 0).Select(e => e.OwnerId);

            // Defensively revert any invisible players without the effect too (shouldn't happen, and self-service safety net provides more immediate rescue mechanism)
            if (PvPStatics.LastGameTurn % 20 == 17)
            {
                var invisiblePlayers = playerRepo.Players.Where(p => !playersToRestore.Contains(p.Id) &&
                                                                     p.GameMode == (int)GameModeStatics.GameModes.Invisible)
                                                         .Select(p => p.Id);

                foreach (var invisiblePlayer in invisiblePlayers)
                {
                    if (!EffectProcedures.PlayerHasActiveEffect(invisiblePlayer, INVISIBILITY_EFFECT))
                    {
                        // Player is somehow in invisible limbo
                        playersToRestore = playersToRestore.Append(invisiblePlayer);
                    }
                }
            }

            // Restore visibility
            foreach (var player in playersToRestore)
            {
                CharacterPrankProcedures.MakePlayerVisible(player);
            }

            // Ensure no items have been lost with a weird game mode
            if (playersToRestore.Any())
            {
                CharacterPrankProcedures.EnsureItemsAreVisible();
            }
        }

        internal static void RunEffectActions(List<Effect> effects)
        {
            var playersToControl = effects.Where(e => e.EffectSourceId == INSTINCT_EFFECT).Select(e => e.OwnerId).ToList();
            InstinctProcedures.ActOnInstinct(playersToControl, new Random());
        }

        // Safety net - allows players to revert some effects on them and recover if anything has gone wrong
        public static string Restore(Player player)
        {
            CharacterPrankProcedures.UndoPsychotic(player.Id);
            CharacterPrankProcedures.UndoInvisible(player);

            var message = "You try as best you can to cleanse yourself of the harmful effects of the Joke Shop.";

            if (Forms(f => f.FormSourceId == player.OriginalFormSourceId && f.Category == LIMITED_MOBILITY).Any())
            {
                message += "<br />Your current base form has limited mobility.  You can change it from the Settings page.";
            }

            if (Forms(f => f.FormSourceId == player.FormSourceId && f.Category == LIMITED_MOBILITY).Any())
            {
                message += "<br />Your current form has limited mobility.  You can return to base form using Self Restore from the My Player menu or by using a Classic Me! Restorative Lotion.";
            }

            if (player.FirstName != player.OriginalFirstName || player.LastName != player.OriginalLastName)
            {
                message += "<br />Your name was changed.  You can restore your original name using Self Restore from the My Player menu or by using a Classic Me! Restorative Lotion.";
            }

            return message;
        }

        public static List<Player> ActivePlayersInJokeShopApartFrom(Player player)
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

            var playerRepo = new EFPlayerRepository();
            var candidates = playerRepo.Players
                .Where(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                            p.LastActionTimestamp >= cutoff &&
                            p.Id != player.Id &&
                            p.Mobility == PvPStatics.MobilityFull &&
                            p.InDuel <= 0 &&
                            p.InQuest <= 0 &&
                            p.BotId == AIStatics.ActivePlayerBotId)
                .ToList();

            return candidates;
        }

        public static string Relocate(Random rand = null)
        {
            rand = rand ?? new Random();

            LocationsStatics.MoveJokeShop();
            return JokeShopProcedures.DrainPlayers(rand);
        }

        public static string DrainPlayers(Random rand = null)
        {
            rand = rand ?? new Random();

            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

            var playerRepo = new EFPlayerRepository();
            var players = playerRepo.Players.Where(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                                                        p.LastActionTimestamp >= cutoff &&
                                                        p.Mobility == PvPStatics.MobilityFull &&
                                                        p.InDuel <= 0 &&
                                                        p.InQuest <= 0 &&
                                                        p.BotId == AIStatics.ActivePlayerBotId).ToList();

            var numDrained = 0;
            var numInanimated = 0;

            foreach (var player in players)
            {
                // Drain players of willpower and mana
                var healthDrain = player.MaxHealth / 4;
                var manaDrain = player.MaxMana / 10;
                player.Health = Math.Max(0, player.Health - healthDrain);
                player.Mana = Math.Max(0, player.Mana - manaDrain);
                playerRepo.SavePlayer(player);

                numDrained++;

                // Mobile inanimates at 0 WP may be unable to retain their mobility...
                if (player.Health == 0 && rand.Next(3) == 0 && InanimateForms().Any(f => player.FormSourceId == f.FormSourceId) && PlayerHasBeenWarned(player))
                {
                    if(CharacterPrankProcedures.TryInanimateTransform(player, player.FormSourceId, dropInventory: false, severe: false, logChanges: false))
                    {
                        PlayerLogProcedures.AddPlayerLog(player.Id, $"As the Joke Shop drains more of your willpower you find you no longer have the strength of mind to remain fully mobile!", true);
                        LocationLogProcedures.AddLocationLog(player.dbLocationName, $"<b>{player.GetFullName()}</b> succumbs to the effects of the Joke Shop after losing the will to remain fully mobile!");

                        numInanimated++;
                    }
                }
                else
                {
                    var message = "The Joke Shop drains you of some of your energies!";

                    if ((double)player.Health > (double)healthDrain * 1.5)
                    {
                        // Gentle warning
                        switch (rand.Next(6))
                        {
                            case 0:
                                message = "The ground shakes and your head feels dizzy.  Perhaps you should take a moment to recover your health.";
                                break;
                            case 1:
                                message = "A cloud of dust dances in the air, as if buoyed by some force unseen.  It coats a nearby cobweb and threatens to engulf you, leaving you feeling quite dirty.  Perhaps you should cleanse?";
                                break;
                            case 2:
                                message = "You feel yourself rocked by a sudden force.  Your body has withstood the impact this time, but your mind has taken a dent!";
                                break;
                            case 3:
                                message = "The walls around you shift, as if by some ancient mechanism built to ensnare unwary adventurers.  This is not the place you were just in, and yet it looks identical.  You fear a trap may be at work - you should be on your guard";
                                break;
                            case 4:
                                message = "\"Psst!\"  You hear a voice in your ear.  You turn to see who it is, but immediately find yourself falling through a whirling spiral vortex!  All meaning of space and time dissolves in this trip through some Daliesque wonderland!  Whether or not this pit has a bottom, you know you don't want to end up there.  So you cast a spell and find yourself back where you were, but the experience has taken its toll on you!";
                                break;
                            case 5:
                                message = "You catch a glimpse of some movement out of the corner of your eye.  It's the shopkeeper.  You see them walking among some artifacts, and then they are gone, only to reappear in another part of the shop, still walking.  Again their likeness is obscured, only for it to appear somewhere else - this time behind you.  Conventional space has no meaning in this realm.  You contiue to watch as the proprietor's unbroken steps find passage between disparate spaces, only this time it's you that has moved, and you can feel its effects!";
                                break;
                        }
                    }
                    else
                    {
                        // Severe warning
                        switch (rand.Next(5))
                        {
                            case 0:
                                message = "Your surroundings contort underfoot and your head can't seem to find gravity as the foreboding and sinister vibe of this unholy vault drains you of your precious reserves, threatening to dismantle all you hold dear at any moment...";
                                break;
                            case 1:
                                message = "You feel helpless as you sense your energies being leeched away by this most imposing of liminal spaces.  Perhaps you should leave now, lest you fall permanently under its effects...";
                                break;
                            case 2:
                                message = "Reality itself appears to condense into a cloud of droplets, the fabric of this place's very existence coursing away like rain dripping down a window, your willpower trickling away with it.  Nothing seems real, except the impending threat of losing everythig you are...";
                                break;
                            case 3:
                                message = "Your will begins to falter as your lightheaded resolve threatens to break.  Distracted by the illusion of space itself deconstructing before your eyes, you almsot forget how quickly you must act to save yourself from impending doom...";
                                break;
                            case 4:
                                message = "Coulrophobia may seem irrational.  The fear of that which is just harmless fun.  Why would we dare think it could be so sinister?  Maybe our primal fears know something our conscious minds do not?  You can barely feel the enjoyment of this place eating away your will.  Perhaps it's not just clowns you should fear...";
                                break;
                        }
                    }

                    // Notify player immediately, but don't put them in combat
                    DomainRegistry.AttackNotificationBroker.Notify(player.Id, $"<p>{message}</p>");
                    PlayerLogProcedures.AddPlayerLog(player.Id, message, true);
                }
            }

            return $"{numDrained} players were drained, of which {numInanimated} lost their will to remain animate.";
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
                EmptyJokeShopOnto(streetTile);
            }
        }

        public static string EmptyJokeShopOnto(string dest)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var players = PlayerProcedures.GetPlayersAtLocation(LocationsStatics.JOKE_SHOP).ToList();

            foreach (var player in players)
            {
                var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
                user.dbLocationName = dest;
                playerRepo.SavePlayer(user);
            }

            IItemRepository itemRepo = new EFItemRepository();
            var items = itemRepo.Items.Where(i => i.dbLocationName == LocationsStatics.JOKE_SHOP).ToList();

            foreach (var item in items)
            {
                var streetItem = itemRepo.Items.FirstOrDefault(i => i.Id == item.Id);
                streetItem.dbLocationName = dest;
                itemRepo.SaveItem(streetItem);
            }

            return $"{players.Count()} mobile players and {items.Count()} items moved out of the Joke Shop and on to {dest}.";
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
            if (rand.Next(100) < 80)  // Normal search 80% of the time, else attempt a prank
            {
                return null;
            }

            // Extra pranks if a boss is up
            if (rand.Next(100) < 2)
            {
                var message = CharacterPrankProcedures.BossPrank(player, rand);

                if (message != null)
                {
                    return message;
                }
            }

            // Main pranks
            var roll = rand.Next(100);

            if (roll < 61)  // 61%
            {
                return MildPrank(player, rand);
            }
            else if (roll < 87)  // 26%
            {
                return MischievousPrank(player, rand);
            }
            else if (roll < 97)  // 10%
            {
                return MeanPrank(player, rand);
            }
            else  // 3%
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
                var filteredManaForms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MANA_FORMS).ToArray();
                
                if (CharacterPrankProcedures.TryAnimateTransform(player, filteredManaForms[rand.Next(filteredManaForms.Length)]))
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
                if (others.Any())
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
                return Restore(player);
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
                var filteredMaidForms = JokeShopProcedures.AnimateForms().Select(f => f.FormSourceId).Intersect(JokeShopProcedures.MAIDS).ToArray();

                if (CharacterPrankProcedures.TryAnimateTransform(player, filteredMaidForms[rand.Next(filteredMaidForms.Length)]))
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
                if (others.Any())
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

            // Ensure player can cleanse/self-restore
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
                if (SkillProcedures.AvailableSkills(player, player, true).Any(s => s.StaticSkill.Id == skill.StaticSkill.Id))
                {
                    var attack = AttackProcedures.AttackSequence(player, player, skill);
                    return $"<b>Your victim deftly deflects your spell back at you!</b><br>{attack}";
                }
            }
            else if (roll < 28)  // 7%
            {
                // Coerce victim to self-cast
                if (SkillProcedures.AvailableSkills(victim, victim, true).Any(s => s.StaticSkill.Id == skill.StaticSkill.Id))
                {
                    var attack = AttackProcedures.AttackSequence(victim, victim, skill, false);
                    PlayerLogProcedures.AddPlayerLog(victim.Id, $"<b>Using a trick of the mind, {player.GetFullName()} convinces you to attack yourself!</b><br>{attack}", true);
                    return $"Using a trick of the mind you convince your victim to attack themselves!";
                }
            }
            else if (roll < 35)  // 7%
            {
                // Weaken
                var skillBeingUsed = SkillProcedures.AvailableSkills(player, victim, true).FirstOrDefault(s => s.StaticSkill.Id == PvPStatics.Spell_WeakenId);

                if (skillBeingUsed != null && skillBeingUsed.StaticSkill.Id != skill.StaticSkill.Id)
                {
                    var attack = AttackProcedures.AttackSequence(player, victim, skillBeingUsed);
                    return $"<b>The aura of the Joke Shop messes with your head and you only manage to cast a weakening spell!</b><br>{attack}";
                }
            }
            else if (roll < 42)  // 7%
            {
                // Wrong spell
                var skillsAvailable = SkillProcedures.AvailableSkills(player, victim, true).Where(s => s.MobilityType == skill.MobilityType && s.StaticSkill.Id != skill.StaticSkill.Id).ToArray();

                if (skillsAvailable != null && skillsAvailable.Any())
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
                var skillBeingUsed = SkillProcedures.AvailableSkills(player, victim, true).FirstOrDefault(spell => spell.StaticSkill.FormSourceId == player.FormSourceId);

                if (skillBeingUsed != null && skillBeingUsed.StaticSkill.Id != skill.StaticSkill.Id)
                {
                    var attack = AttackProcedures.AttackSequence(player, victim, skillBeingUsed);
                    return $"<b>Your vanity gets in the way of your spellcasting as you try to turn {victim.GetFullName()} into a clone of yourself!</b><br>{attack}";
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
                BanCharacter(victim);
                return message;
            }

            return null;
        }

        #endregion

        #region Prank selection

        public static string MildPrank(Player player, Random rand = null)
        {
            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 14)  // 14%
            {
                return EnvironmentPrankProcedures.MildResourcePrank(player, rand);
            }
            else if (roll < 23)  // 9%
            {
                return EnvironmentPrankProcedures.MildLocationPrank(player, rand);
            }
            else if (roll < 31)  // 8%
            {
                return EnvironmentPrankProcedures.MildQuotasAndTimerPrank(player, rand);
            }
            else if (roll < 46)  // 15%
            {
                return CharacterPrankProcedures.MildTransformationPrank(player, rand);
            }
            else if (roll < 60)  // 14%
            {
                return CharacterPrankProcedures.MildEffectsPrank(player, rand);
            }
            else if (roll < 65)  // 5%
            {
                return EnvironmentPrankProcedures.RareFind(player, rand);
            }
            else if (roll < 70)  // 5%
            {
                return NovelPrankProcedures.RandomShout(player, rand);
            }
            else if (roll < 80)  // 10%
            {
                return NovelPrankProcedures.LocatePlayerInCombat(player, rand);
            }
            else if (roll < 90)  // 10%
            {
                return NovelPrankProcedures.AwardChallenge(player, 0, 20, false, rand);
            }
            else  // 10%
            {
                return NovelPrankProcedures.DiceGame(player);
            }
        }

        public static string MischievousPrank(Player player, Random rand = null)
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
                return NovelPrankProcedures.AwardChallenge(player, 10, 60, rand: rand);
            }
        }

        public static string MeanPrank(Player player, Random rand = null)
        {
            var warning = EnsurePlayerIsWarnedTwice(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            rand = rand ?? new Random();
            var roll = rand.Next(100);

            if (roll < 12)  // 12%
            {
                return EnvironmentPrankProcedures.MeanResourcePrank(player, rand);
            }
            else if (roll < 24)  // 12%
            {
                return EnvironmentPrankProcedures.MeanLocationPrank(player, rand);
            }
            else if (roll < 33)  // 9%
            {
                return EnvironmentPrankProcedures.MeanQuotasAndTimerPrank(player, rand);
            }
            else if (roll < 45)  // 12%
            {
                return CharacterPrankProcedures.MeanTransformationPrank(player, rand);
            }
            else if (roll < 57)  // 12%
            {
                return CharacterPrankProcedures.MeanEffectsPrank(player, rand);
            }
            else if (roll < 62)  // 5%
            {
                return NovelPrankProcedures.SummonPsychopath(player, rand);
            }
            else if (roll < 66)  // 4%
            {
                return NovelPrankProcedures.SummonDoppelganger(player, rand);
            }
            else if (roll < 70)  // 4%
            {
                return NovelPrankProcedures.TakeIdentity(player, rand);
            }
            else if (roll < 74)  // 4%
            {
                return NovelPrankProcedures.OpenPsychoNip(player);
            }
            else if (roll < 78)  // 4%
            {
                return NovelPrankProcedures.ForceAttack(player, false, rand);
            }
            else if (roll < 82)  // 4%
            {
                return NovelPrankProcedures.Incite(player, rand);
            }
            else if (roll < 89)  // 7%
            {
                return NovelPrankProcedures.AwardChallenge(player, 20, 480, true, rand);
            }
            else  // 11%
            {
                return NovelPrankProcedures.PlaceBountyOnPlayersHead(player, rand);
            }
        }

        #endregion

        #region Warnings and access controls

        public static bool PlayerHasBeenWarned(Player player)
        {
            return EffectProcedures.PlayerHasEffect(player, FIRST_WARNING_EFFECT);
        }

        public static bool PlayerHasBeenWarnedTwice(Player player)
        {
            return EffectProcedures.PlayerHasEffect(player, SECOND_WARNING_EFFECT);
        }

        public static string EnsurePlayerIsWarned(Player player, int? duration = null, int? cooldown = null)
        {
            if (!PlayerHasBeenWarned(player))
            {
                var logMessage = EffectProcedures.GivePerkToPlayer(FIRST_WARNING_EFFECT, player, duration, cooldown);
                PlayerLogProcedures.AddPlayerLog(player.Id, logMessage, false);
                return logMessage;
            }

            // Already recently warned
            return null;
        }

        public static string EnsurePlayerIsWarnedTwice(Player player, int? duration = null, int? cooldown = null)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            if (!PlayerHasBeenWarnedTwice(player))
            {
                var logMessage = EffectProcedures.GivePerkToPlayer(SECOND_WARNING_EFFECT, player, duration, cooldown);
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

        public static string BanCharacter(Player player, int? duration = null, int? cooldown = null)
        {
            var warning = EnsurePlayerIsWarned(player);

            if (!warning.IsNullOrEmpty())
            {
                return warning;
            }

            if (EffectProcedures.PlayerHasEffect(player, BANNED_FROM_JOKE_SHOP_EFFECT))
            {
                return null;
            }

            var message = duration.HasValue ? EffectProcedures.GivePerkToPlayer(BANNED_FROM_JOKE_SHOP_EFFECT, player, duration, cooldown)
                                            : (PvPStatics.ChaosMode ? EffectProcedures.GivePerkToPlayer(BANNED_FROM_JOKE_SHOP_EFFECT, player, 5, 10)  // Faster in chaos
                                                                    : EffectProcedures.GivePerkToPlayer(BANNED_FROM_JOKE_SHOP_EFFECT, player));
            var kickedOutMessage = EjectCharacter(player);

            return $"{message}  {kickedOutMessage}";
        }

        public static string EjectCharacter(Player player)
        {
            if ((player.InDuel > 0 || player.InQuest > 0) &&
                player.LastActionTimestamp > DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes()))
            {
                return null;
            }

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

            if (player.InQuest > 0)
            {
                QuestProcedures.PlayerEndQuest(player, (int)QuestStatics.QuestOutcomes.Failed);
                playerLog += "  You are forced to abandon your quest.";
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var user = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            user.dbLocationName = street;
            playerRepo.SavePlayer(user);

            PlayerLogProcedures.AddPlayerLog(player.Id, playerLog, false);
            LocationLogProcedures.AddLocationLog(player.dbLocationName, leavingMessage);
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
