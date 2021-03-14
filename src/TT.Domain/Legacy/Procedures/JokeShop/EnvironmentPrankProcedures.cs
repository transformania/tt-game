using System;
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

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    public static class EnvironmentPrankProcedures
    {

        #region Resource pranks

        public static string MildResourcePrank(Player player)
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
            else if (roll < 90) // 30%
            {
                return ChangeMoney(player, rand.Next(-2, 3));
            }
            else  // 10%
            {
                return LearnSpell(player);
            }
        }

        public static string MischievousResourcePrank(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(100);

            if (roll < 30)  // 30%
            {
                return ChangeHealth(player, rand.Next(-125, 125));
            }
            else if (roll < 55)  // 25%
            {
                return ChangeMana(player, rand.Next(-20, 20));
            }
            else if (roll < 60)  // 5%
            {
                return ChangeActionPoints(player, rand.Next(-10, 20));
            }
            else if (roll < 85)  // 25%
            {
                return ChangeMoney(player, rand.Next(-5, 5));
            }
            else if (roll < 95)  // 10%
            {
                return UnlearnSpell(player);
            }
            else  // 5%
            {
                return ChangeDungeonPoints(player, 1);
            }
        }

        public static string MeanResourcePrank(Player player)
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

        public static string ChangeHealth(Player player, int amount)
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

        public static string ChangeMana(Player player, int amount)
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

        public static string ChangeActionPoints(Player player, int amount)
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

        public static string RareFind(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(3);

            if (roll < 1)
            {
                // Quality consumable
                int[] itemTypes = { ItemStatics.CurseLifterItemSourceId,
                                    ItemStatics.AutoTransmogItemSourceId,
                                    ItemStatics.WillpowerBombVolatileItemSourceId,
                                    ItemStatics.SelfRestoreItemSourceId,
                                    ItemStatics.LullabyWhistleItemSourceId,
                                    ItemStatics.SpellbookLargeItemSourceId,
                                    ItemStatics.TeleportationScrollItemSourceId };

                var itemType = itemTypes[rand.Next(itemTypes.Count())];

                return ItemProcedures.GiveNewItemToPlayer(player, itemType);
            }
            else if (roll < 2)
            {
                // Regular item
                var level = (int)(rand.NextDouble() * rand.NextDouble() * 6 + 1);
                var itemType = ItemProcedures.GetRandomPlayableItem();
                var message = ItemProcedures.GiveNewItemToPlayer(player, itemType, level);

                // Don't permit player to carry untamed pet
                if (itemType.ItemType == PvPStatics.ItemType_Pet)
                {
                    var unequippedPet = ItemProcedures.GetAllPlayerItems(player.Id).FirstOrDefault(i => i.Item.ItemType == PvPStatics.ItemType_Pet && !i.dbItem.IsEquipped);

                    if (unequippedPet != null)
                    {
                        ItemProcedures.DropItem(unequippedPet.dbItem.Id);
                    }
                }

                return message;
            }
            else
            {
                // Rune
                var levelRoll = rand.Next(10);
                int level;

                if (levelRoll < 2)  // 20%
                {
                    level = 3;  // Standard
                }
                else if (levelRoll < 9)  // 70%
                {
                    level = 5;  // Great
                }
                else  // 10%
                {
                    level = 7;  // Major
                }

                var runeId = DomainRegistry.Repository.FindSingle(new GetRandomRuneAtLevel { RuneLevel = level, Random = rand });
                DomainRegistry.Repository.Execute(new GiveRune { ItemSourceId = runeId, PlayerId = player.Id });

                return "You have found a rune!";
            }
        }

        private static string LearnSpell(Player player)
        {
            var rand = new Random();
            var num = rand.Next(3) + 1;
            var learnt = SkillProcedures.GiveRandomFindableSkillsToPlayer(player, num);

            if (learnt.IsEmpty())
            {
                return null;
            }

            return $"You discover the secrets of {ListifyHelper.Listify(learnt)}!";  // TODO joke_shop flavor text
        }

        private static string UnlearnSpell(Player player)
        {
            var rand = new Random();

            var skills = SkillProcedures.GetSkillViewModelsOwnedByPlayer(player.Id)
                .Where(s => s.StaticSkill.IsPlayerLearnable &&
                            !s.StaticSkill.ExclusiveToFormSourceId.HasValue &&
                            !s.StaticSkill.ExclusiveToItemSourceId.HasValue &&
                            s.StaticSkill.Id != PvPStatics.Spell_WeakenId &&
                            s.StaticSkill.Id != PvPStatics.Dungeon_VanquishSpellSourceId &&
                            s.StaticSkill.Id != BossProcedures_FaeBoss.SpellUsedAgainstNarcissaSourceId &&
                            (s.StaticSkill.LearnedAtLocation ?? s.StaticSkill.LearnedAtRegion) != null);

            if (skills.IsEmpty())
            {
                return null;
            }

            var skill = skills.ElementAt(rand.Next(skills.Count()));
            var message = $"The strange aura of the Joke Shop causes you to forget how to cast {skill.StaticSkill.FriendlyName}!";

            ISkillRepository skillRepo = new EFSkillRepository();
            skillRepo.DeleteSkill(skill.dbSkill.Id);

            return message;
        }

        #endregion

        #region Location pranks

        public static string MildLocationPrank(Player player)
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

        public static string MischievousLocationPrank(Player player)
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

        public static string MeanLocationPrank(Player player)
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
                PlayerPrankProcedures.GiveEffect(player, JokeShopProcedures.ROOT_EFFECT);
                root = JokeShopProcedures.ROOT_EFFECT.HasValue && EffectProcedures.PlayerHasActiveEffect(player.Id, JokeShopProcedures.ROOT_EFFECT.Value);
            }

            if (curse && !root)
            {
                PlayerPrankProcedures.ApplyLocalCurse(player, location);
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
                PlayerPrankProcedures.GiveEffect(player, JokeShopProcedures.ROOT_EFFECT);
            }

            return "Teleport to dungeon"; // TODO joke_shop flavor text
        }

        private static string TeleportToFriendlyNPC(Player player)
        {
            var rand = new Random();
            var roll = rand.Next(6);

            int npc = 0;
            // View view = null;

            switch (roll)
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
                PlayerPrankProcedures.TryAnimateTransform(player, encounter.RequiredFormSourceId);
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
                PlayerPrankProcedures.TryAnimateTransform(player, targetForm);
            }

            // Move to same tile as NPC
            if (!Teleport(player, npcPlayer.dbLocationName, rand.Next(2) == 0))
            {
                return null;
            }

            var message = "Teleport to hostile NPC";  // TODO joke_shop Add flavor text;

            if (attack)
            {
                var spells = SkillProcedures.AvailableSkills(player, npcPlayer, true);
                if (spells != null && spells.Any())
                {
                    var spellList = spells.ToArray();
                    var spell = spellList[rand.Next(spellList.Count())];

                    // Note we do not apply the full gamut of preconditions of a manual attack present in the controller
                    var attackMessage = AttackProcedures.AttackSequence(player, npcPlayer, spell);
                    message = $"{message}<br />{attackMessage}";
                }
            }

            return message;
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
                PlayerPrankProcedures.GiveEffect(player, JokeShopProcedures.ROOT_EFFECT);
            }
            else
            {
                PlayerPrankProcedures.ApplyLocalCurse(player, bar);
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
            var spacesToMove = (int)Math.Min(maxDistance, pathTiles.Count());
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

        #region Quotas and timers pranks

        public static string MildQuotasAndTimerPrank(Player player)
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

        public static string MischievousQuotasAndTimerPrank(Player player)
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

        public static string MeanQuotasAndTimerPrank(Player player)
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

        public static string BlockAttacks(Player player)
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

        public static string BlockCleanseMeditates(Player player)
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

        public static string ResetCombatTimer(Player player, double proportionOutOfCombat = 0.0)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            target.LastCombatTimestamp = DateTime.UtcNow.AddMinutes(-proportionOutOfCombat * TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling());
            playerRepo.SavePlayer(player);

            return "A whiff of magic passes under your nose, the acrid smell reminding you of your last battle.  It seems so recent...";
        }

        public static void ResetActivityTimer(Player player, double proportionOutOfOnline = 0.0)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            target.LastActionTimestamp = DateTime.UtcNow.AddMinutes(-proportionOutOfOnline * TurnTimesStatics.GetOfflineAfterXMinutes());
            playerRepo.SavePlayer(player);
        }

        #endregion

    }
}
