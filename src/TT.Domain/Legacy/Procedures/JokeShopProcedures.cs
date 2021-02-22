﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures;
using TT.Domain.Procedures.BossProcedures;
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
            if (rand.Next(3) != 0)  // Attempt a prank 1 time in 3, else normal search
            {
                return null;
            }
            */

            var roll = rand.Next(100);

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
            var roll = rand.Next(100);

            if (roll < 100)
            {
                // return MildResourcePrank(player);
                // return MildLocationPrank(player);
                // return MildQuotasAndTimerPrank(player);
                return DiceGame(player);
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
                return MischievousQuotasAndTimerPrank(player);
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
                return MeanQuotasAndTimerPrank(player);
            }

            // TODO joke_shop return value
            return "Mean prank";
            //return null;
        }

        #endregion

        #region Warnings and access controls

        private static bool PlayerHasBeenWarned(Player player)
        {
            return EffectProcedures.PlayerHasEffect(player, FIRST_WARNING_EFFECT);
        }

        private static bool PlayerHasBeenWarnedTwice(Player player)
        {
            return EffectProcedures.PlayerHasEffect(player, SECOND_WARNING_EFFECT);
        }

        private static string EnsurePlayerIsWarned(Player player)
        {
            if (!PlayerHasBeenWarned(player))
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

            if (!PlayerHasBeenWarnedTwice(player))
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

        public static void EjectOfflineCharacters()
        {
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

            var playerRepo = new EFPlayerRepository();
            var players = playerRepo.Players.Where(p => p.dbLocationName == LocationsStatics.JOKE_SHOP &&
                                                        p.Mobility == PvPStatics.MobilityFull &&
                                                        p.LastActionTimestamp < cutoff);

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
                return TeleportToOverworld(player, false);
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
                return TeleportToOverworld(player, true);
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

        private static string TeleportToOverworld(Player player, bool root)
        {
            var location = LocationsStatics.GetRandomLocationNotInDungeonOr(LocationsStatics.JOKE_SHOP);

            if (!Teleport(player, location, new Random().Next(2) == 0))
            {
                return null;
            }

            if (root)
            {
                Root(player);
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
                Root(player);
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
            if (hostiles == null || hostiles.Count() == 0)
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
            if(!Teleport(player, "tavern_counter", true))
            {
                return null;
            }

            if (root)
            {
                Root(player);
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

            var start = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == LocationsStatics.JOKE_SHOP);
            var end = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == destination);

            if (destination == null || start == null || end == null)
            {
                return null;
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var target = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            var pathTiles = PathfindingProcedures.GetMovementPath(start, end);
            var costPerTile = 1 - target.MoveActionPointDiscount;

            // Cap distance, plus don't excees number of tiles or available AP
            var maxDistance = Math.Floor(target.ActionPoints / costPerTile);
            var spacesToMove = (int)(Math.Min(maxDistance, pathTiles.Count()));
            spacesToMove = Math.Min(15, spacesToMove);

            if (spacesToMove == 0)
            {
                return null;
            }

            PlayerProcedures.MovePlayerMultipleLocations(player, pathTiles[spacesToMove - 1], spacesToMove * costPerTile);

            return "Run away";  // TODO joke_shop flavor text
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

        private static bool Root(Player player)
        {
            // TODO joke_shop implement
            return false;
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

        private static bool TryAnimateTransform(Player player, int formSourceId)
        {
            // Give extra warning for SP players, who might want to keep their form
            if (player.GameMode == (int)GameModeStatics.GameModes.Superprotection && !PlayerHasBeenWarned(player))
            {
                return false;
            }

            PlayerProcedures.InstantChangeToForm(player, formSourceId);

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
