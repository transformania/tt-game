using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Legacy.Procedures.BossProcedures;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.Utilities;
using TT.Domain.World.Queries;

namespace TT.Domain.Procedures
{
    public static class WorldUpdateProcedures
    {
        public static void UpdateWorld()
        {
            var worldStats = DomainRegistry.Repository.FindSingle(new GetWorld());

            var turnNo = worldStats.TurnNumber;
            PvPStatics.LastGameTurn = turnNo;

            if (turnNo < PvPStatics.RoundDuration)
            {

                PvPStatics.AnimateUpdateInProgress = true;

                IServerLogRepository serverLogRepo = new EFServerLogRepository();
                var log = new ServerLog
                {
                    TurnNumber = turnNo,
                    StartTimestamp = DateTime.UtcNow,
                    FinishTimestamp = DateTime.UtcNow,
                    Errors = 0,
                    FullLog = "",
                    Population = PlayerProcedures.GetWorldPlayerStats().CurrentOnlinePlayers,
                };
                log.AddLog($"Started new log for turn {turnNo} at <b>{DateTime.UtcNow}</b>.");
                serverLogRepo.SaveServerLog(log);
                var updateTimer = new Stopwatch();
                updateTimer.Start();

                IPlayerRepository playerRepo = new EFPlayerRepository();

                #region spawn NPCS
                // make sure the NPCs have been spawned early turn
                if (turnNo <= 3)
                {
                    var lindella = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.LindellaBotId);
                    if (lindella == null)
                    {
                        BossProcedures_Lindella.SpawnLindella();
                    }

                    var wuffie = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.WuffieBotId);
                    if (wuffie == null)
                    {
                        BossProcedures_PetMerchant.SpawnPetMerchant();
                    }

                    var fae = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.JewdewfaeBotId);
                    if (fae == null)
                    {
                        BossProcedures_Jewdewfae.SpawnFae();
                    }

                    var bartender = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.BartenderBotId);
                    if (bartender == null)
                    {
                        BossProcedures_Bartender.SpawnBartender();
                    }

                    var lorekeeper = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.LoremasterBotId);
                    if (lorekeeper == null)
                    {
                        BossProcedures_Loremaster.SpawnLoremaster();
                    }
                }
                #endregion

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started loading animate players");
                
                IEffectRepository effectRepo = new EFEffectRepository();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started loading effects");
                var temporaryEffects = effectRepo.Effects.Where(e => !e.IsPermanent).ToList();
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished loading effects");
                var effectsToDelete = new List<Effect>();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating effects");
                foreach (var e in temporaryEffects)
                {
                    e.Duration--;
                    e.Cooldown--;

                    if (e.Duration < 0)
                    {
                        e.Duration = 0;
                    }

                    if (e.Cooldown <= 0)
                    {
                        effectsToDelete.Add(e);
                    }
                    else
                    {
                        effectRepo.SaveEffect(e);
                    }
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating effects");
                serverLogRepo.SaveServerLog(log);

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started deleting expired effects");
                foreach (var e in effectsToDelete)
                {
                    effectRepo.DeleteEffect(e.Id);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting expired effects");
                serverLogRepo.SaveServerLog(log);

                #region playerExtra / protection cooldown loop
                IPlayerExtraRepository playerExtraRepo = new EFPlayerExtraRepository();
                var extrasToIncrement = playerExtraRepo.PlayerExtras.ToList();
                var extrasToIncrement_SaveList = new List<PlayerExtra>();
                var extrasToIncrement_DeleteList = new List<PlayerExtra>();
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating protection change cooldown (" + extrasToIncrement.Count + ")");

                foreach (var e in extrasToIncrement)
                {
                    var owner = PlayerProcedures.GetPlayer(e.PlayerId);
                    if (PlayerProcedures.PlayerIsOffline(owner))
                    {
                        extrasToIncrement_SaveList.Add(e);
                    }
                }

                foreach (var e in extrasToIncrement_SaveList)
                {
                    if (e.ProtectionToggleTurnsRemaining > 0)
                    {
                        e.ProtectionToggleTurnsRemaining--;
                        playerExtraRepo.SavePlayerExtra(e);
                    }
                    else if (e.ProtectionToggleTurnsRemaining <= 0)
                    {
                        extrasToIncrement_DeleteList.Add(e);
                    }
                }

                foreach (var e in extrasToIncrement_DeleteList)
                {
                    playerExtraRepo.DeletePlayerExtra(e.Id);
                }

                #endregion

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating protection change cooldown.");


                #region main player loop

                using (var context = new StatsContext())
                {
                    try
                    {
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[Players] SET TimesAttackingThisUpdate = 0, CleansesMeditatesThisRound = 0, ShoutsRemaining = 1, ActionPoints = ActionPoints + 10, ItemsUsedThisTurn = 0 WHERE Mobility='full'" +

                        $"UPDATE [dbo].[Players] SET ActionPoints_Refill = ActionPoints_Refill + (ActionPoints % {TurnTimesStatics.GetActionPointLimit()} / 2) WHERE ActionPoints >= {TurnTimesStatics.GetActionPointLimit()} AND Mobility='full'" +

                        $"UPDATE [dbo].[Players] SET ActionPoints = {TurnTimesStatics.GetActionPointLimit()} WHERE ActionPoints > {TurnTimesStatics.GetActionPointLimit()} AND Mobility='full'" +

                        $"UPDATE [dbo].[Players] SET ActionPoints_Refill = {TurnTimesStatics.GetActionPointReserveLimit()} WHERE ActionPoints_Refill > {TurnTimesStatics.GetActionPointReserveLimit()} AND Mobility='full'" +

                        $"UPDATE [dbo].[Players] SET ActionPoints = ActionPoints + 20, ActionPoints_Refill = ActionPoints_Refill - 20 WHERE ActionPoints <= {TurnTimesStatics.GetActionPointLimit()-20} AND ActionPoints_Refill >= 20 AND Mobility='full'");

                        if (PvPStatics.ChaosMode)
                        {
                            context.Database.ExecuteSqlCommand($"Update [dbo].[Players] SET ActionPoints = {TurnTimesStatics.GetActionPointLimit()}, ActionPoints_Refill = {TurnTimesStatics.GetActionPointReserveLimit()}, TimesAttackingThisUpdate = -999, Mana = MaxMana, CleansesMeditatesThisRound = -999");
                        }

                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  ANIMATE SQL UPDATE SUCCEEDED!");
                        serverLogRepo.SaveServerLog(log);
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, " ANIMATE SQL UPDATE FAILED", e));
                        serverLogRepo.SaveServerLog(log);
                    }
                }

                var timeCutoff = DateTime.UtcNow.AddHours(-8);
                var playersToSave = playerRepo.Players
                    .Where(p => p.Mobility == PvPStatics.MobilityFull && p.LastActionTimestamp > timeCutoff).ToList();

                foreach (var player in playersToSave)
                {
                    var buffs = ItemProcedures.GetPlayerBuffs(player);
                    player.Health += buffs.HealthRecoveryPerUpdate();
                    player.Mana += buffs.ManaRecoveryPerUpdate();


                    player.ReadjustMaxes(buffs);

                    // extra AP condition checks
                    if (player.Covenant > 0)
                    {
                        var playerCov = CovenantDictionary.IdNameFlagLookup.FirstOrDefault(c => c.Key == player.Covenant).Value;

                        // give this player an extra AP refill if they are at their safeground, scaled up by level
                        if (playerCov != null && !playerCov.HomeLocation.IsNullOrEmpty() && player.dbLocationName == playerCov.HomeLocation)
                        {
                            player.ActionPoints_Refill += .25M * playerCov.CovLevel;
                           
                        }

                        // give this player an extra AP refill if they are on a location that their covenane has enchanted
                        var currentLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);

                        if (currentLocation != null && currentLocation.CovenantController == player.Covenant)
                        {
                            if (currentLocation.TakeoverAmount < 25)
                            {
                                player.ActionPoints_Refill += .05M;
                            }
                            else if (currentLocation.TakeoverAmount <= 50)
                            {
                                player.ActionPoints_Refill += .10M;
                            }
                            else if (currentLocation.TakeoverAmount <= 75)
                            {
                                player.ActionPoints_Refill += .15M;
                            }
                            else if (currentLocation.TakeoverAmount < 100)
                            {
                                player.ActionPoints_Refill += .20M;
                            }
                            else if (currentLocation.TakeoverAmount >= 100)
                            {
                                player.ActionPoints_Refill += .25M;
                            }
                            
                        }

                        // make sure AP reserve stays within maximum amount
                        if (player.ActionPoints_Refill > TurnTimesStatics.GetActionPointReserveLimit())
                        {
                            player.ActionPoints_Refill = TurnTimesStatics.GetActionPointReserveLimit();
                        }


                    }

                    if (player.MaxHealth < 1)
                    {
                        player.MaxHealth = 1;
                    }

                    if (player.MaxMana < 1)
                    {
                        player.MaxMana = 1;
                    }

                    if (player.Health > player.MaxHealth)
                    {
                        player.Health = player.MaxHealth;
                    }
                    if (player.Mana > player.MaxMana)
                    {
                        player.Mana = player.MaxMana;
                    }
                    if (player.Health < 0)
                    {
                        player.Health = 0;
                    }
                    if (player.Mana < 0)
                    {
                        player.Mana = 0;
                    }

                    playerRepo.SavePlayer(player);

                }

                log.AddLog($"{updateTimer.ElapsedMilliseconds}:  Finished updating animate players ({playersToSave.Count})");
                serverLogRepo.SaveServerLog(log);

                #endregion main player loop

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating inanimate/animal players");


                using (var context = new StatsContext())
                {
                    try
                    {
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[Players] SET TimesAttackingThisUpdate = 0 WHERE (Mobility = 'inanimate' OR Mobility = 'animal') AND BotId = 0");
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating inanimate/animal players");
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, " ERROR UPDATING INANIMATE/ANIMAL PLAYERS", e));
                    }
                }

                serverLogRepo.SaveServerLog(log);

                #region decrement mind control timers
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started mind control cooldown.");

                using (var context = new StatsContext())
                {
                    try
                    {
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[MindControls] SET TurnsRemaining = TurnsRemaining - 1");
                        context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[MindControls] WHERE TurnsRemaining <= 0");
                        context.Database.ExecuteSqlCommand("UPDATE [dbo].[MindControls] SET TimesUsedThisTurn = 0");

                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished mind control cooldown.");
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "MIND CONTROL COOLDOWN UPDATE FAILED", e));
                    }
                }
                #endregion

                serverLogRepo.SaveServerLog(log);

                PvPStatics.AnimateUpdateInProgress = false;

                // bump down the timer on all items that are reuseable consumables
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating items on cooldown");
                IItemRepository itemsRepo = new EFItemRepository();
                var itemsToUpdate = itemsRepo.Items.Where(i => i.TurnsUntilUse > 0).ToList();

                foreach (var item in itemsToUpdate)
                {
                    item.TurnsUntilUse--;
                }

                foreach (var item in itemsToUpdate)
                {
                    itemsRepo.SaveItem(item);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating items on cooldown");
                serverLogRepo.SaveServerLog(log);

                // find the ids for the merchants Lindella and Skaldyr
                var merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);
                var skaldyr = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);
                var merchantId = merchant.Id;
                var skaldyrId = skaldyr.Id;

                // have abandoned items go to Lindella
                if (turnNo % 11 == 3 && merchant.Mobility == PvPStatics.MobilityFull)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started collecting all abandoned items for Lindella");

                    using (var context = new StatsContext())
                    {
                        try
                        {
                            context.Database.ExecuteSqlCommand("UPDATE [dbo].[Items] SET OwnerId = " + merchant.Id + ", dbLocationName = '', PvPEnabled = -1, TimeDropped = '" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "'  WHERE  dbLocationName <> '' AND dbLocationName IS NOT NULL AND TimeDropped < DATEADD(hour, -8, GETUTCDATE()) AND OwnerId IS NULL AND (dbName LIKE 'item_%' OR dbName LIKE 'rune_%') AND dbName != '" + PvPStatics.ItemType_DungeonArtifact + "'");

                            context.Database.ExecuteSqlCommand("UPDATE [dbo].[Players] SET dbLocationName = '" + merchant.dbLocationName + "' WHERE Id IN ( SELECT FormerPlayerId FROM [dbo].[Items] WHERE  dbLocationName <> '' AND dbLocationName IS NOT NULL AND TimeDropped < DATEADD(hour, -8, GETUTCDATE()) AND OwnerId IS NULL AND dbName LIKE 'item_%' AND dbName != '" + PvPStatics.ItemType_DungeonArtifact + "')");

                            log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished collecting all abandoned items for Lindella");

                        }
                        catch (Exception e)
                        {
                            log.Errors++;
                            log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR collecting all abandoned items for Lindella", e));

                        }
                    }


                }

                // delete all consumable type items that have been sitting around on the ground for too long
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started deleting expired consumables");
                DomainRegistry.Repository.Execute(new DeleteExpiredConsumablesOnGround());
                DomainRegistry.Repository.Execute(new DeleteExpiredConsumablesOnMerchants { LindellaId = merchantId, LorekeeperId = skaldyrId });

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting expired consumables");
                serverLogRepo.SaveServerLog(log);

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started deleting expired runes");
                try
                {
                    DomainRegistry.Repository.Execute(new DeleteExpiredRunesOnMerchants());
                }
                catch (Exception e)
                {
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR deleting expired runes", e));
                    log.Errors++;
                }
                
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting expired runes");

                serverLogRepo.SaveServerLog(log);

                // allow all items that have been recently equipped to be taken back off
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started resetting items that have been recently equipped");
                var recentlyEquipped = itemsRepo.Items.Where(i => i.EquippedThisTurn).ToList();

                foreach (var item in recentlyEquipped)
                {
                    item.EquippedThisTurn = false;
                    itemsRepo.SaveItem(item);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished resetting items that have been recently equipped");

                #region give covenants money based on territories
                if (turnNo % 6 == 0)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started giving covenants money from territories");
                    ICovenantRepository covRepo = new EFCovenantRepository();
                    var covs = covRepo.Covenants.Where(c => c.HomeLocation != null && c.HomeLocation != "").ToList();


                    foreach (var c in covs)
                    {
                        var locationControlledSum = CovenantProcedures.GetLocationControlCount(c);
                        var moneyGain = (decimal)Math.Floor(Convert.ToDouble(locationControlledSum));
                        c.Money += moneyGain;

                        if (moneyGain > 0)
                        {
                            CovenantProcedures.WriteCovenantLog("Your covenant collected " + moneyGain + " Arpeyjis from the locations you have enchanted.", c.Id, false);
                        }
                        covRepo.SaveCovenant(c);
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished giving covenants money from territories");

                }
                #endregion

                serverLogRepo.SaveServerLog(log);

                #region drop dungeon artifacts and spawn demons if needed

                try
                {

                    if (turnNo % 7 == 2)
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Starting dungeon item / demon spawning");
                        var dungeonArtifactCount = itemsRepo.Items.Count(i => i.dbName == PvPStatics.ItemType_DungeonArtifact);
                        for (var x = 0; x < PvPStatics.DungeonArtifact_SpawnLimit - dungeonArtifactCount; x++)
                        {
                            var randDungeon = LocationsStatics.GetRandomLocation_InDungeon();

                            var cmd = new CreateItem
                            {
                                dbLocationName = randDungeon,
                                OwnerId = null,
                                EquippedThisTurn = false,
                                IsPermanent = true,
                                Level = 0,
                                PvPEnabled = 2,
                                IsEquipped = false,
                                TurnsUntilUse = 0,
                                dbName = PvPStatics.ItemType_DungeonArtifact,
                                ItemSourceId = ItemStatics.GetStaticItem(PvPStatics.ItemType_DungeonArtifact).Id
                            };
                            DomainRegistry.Repository.Execute(cmd);
                        }


                        IEnumerable<Player> demons = playerRepo.Players.Where(i => i.FormSourceId == PvPStatics.DungeonDemonFormSourceId);
                        var dungeonDemonCount = demons.Count();

                        var randLevel = new Random(Guid.NewGuid().GetHashCode());

                        var demonNames = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.DungeonDemonNames.xml");

                        for (var x = 0; x < PvPStatics.DungeonDemon_Limit - dungeonDemonCount; x++)
                        {
                            var randDungeon = LocationsStatics.GetRandomLocation_InDungeon();

                            // pull a random last demon name
                            double maxDemonNameCount = demonNames.Count();
                            var num = randLevel.NextDouble();
                            var demonIndex = Convert.ToInt32(Math.Floor(num * maxDemonNameCount));
                            var demonlastName = demonNames.ElementAt(demonIndex);

                            // if there's already a demon with this last name, reroll and try again
                            if (demons.FirstOrDefault(d => d.LastName == demonlastName) != null)
                            {
                                x--;
                                continue;
                            }


                            var levelRoll = randLevel.NextDouble();
                            var level = (int)Math.Floor(levelRoll * 8 + 3);

                            var cmd = new CreatePlayer
                            {
                                BotId = AIStatics.DemonBotId,
                                FirstName = "Spirit of ",
                                LastName = demonlastName,
                                Mobility = PvPStatics.MobilityFull,
                                FormSourceId = AIStatics.DungeonDemonFormId,
                                Gender = PvPStatics.GenderFemale,
                                GameMode = 2,
                                Health = 1000,
                                Mana = 1000,
                                Covenant = -1,
                                Location = randDungeon,
                                Level = level,
                                MaxHealth = 500,
                                MaxMana = 500,
                            };

                            var id = DomainRegistry.Repository.Execute(cmd);

                            var newDemon = new EFPlayerRepository().Players.FirstOrDefault(p => p.Id == id);
                            
                            if (cmd.Level <= 5)
                            {
                                ItemProcedures.GiveNewItemToPlayer(newDemon, "item_consumable_spellbook_medium");
                            }
                            else if (cmd.Level <= 7)
                            {
                                ItemProcedures.GiveNewItemToPlayer(newDemon, "item_consumable_spellbook_large");
                            }

                            else if (cmd.Level > 7)
                            {
                                ItemProcedures.GiveNewItemToPlayer(newDemon, "item_consumable_spellbook_giant");
                            }

                        }
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  FINISHED dungeon item / demon spawning");

                    }

                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR running dungeon actions", e));
                }


                #endregion

                serverLogRepo.SaveServerLog(log);

                #region forcibly terminate duels that have timed out
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started duel updates");
                try
                {
                    IDuelRepository duelRepo = new EFDuelRepository();
                    var duels = duelRepo.Duels.Where(d => d.Status == DuelProcedures.ACTIVE).ToList();

                    foreach (var d in duels)
                    {
                        // if the duel has timed out, end it forcibly
                        if ((turnNo - d.StartTurn) >= PvPStatics.MaximumDuelTurnLength)
                        {
                            DuelProcedures.EndDuel(d.Id, DuelProcedures.TIMEOUT);
                        }
                    }

                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Successfully completed duel updates");
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR completing duel updates", e));
                }
                #endregion duel updates

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Lindella actions");
                serverLogRepo.SaveServerLog(log);
                try
                {
                    BossProcedures_Lindella.RunActions(turnNo);
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR running Lindella action", e));
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Lindella actions");

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Wuffie actions");
                serverLogRepo.SaveServerLog(log);
                try
                {
                    BossProcedures_PetMerchant.RunPetMerchantActions(turnNo);
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR running Wuffie actions", e));
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Wuffie actions");

                #region furniture
                if (turnNo % 6 == 0)
                {

                    // move some furniture around on the market
                    try
                    {
                        FurnitureProcedures.MoveFurnitureOnMarket();
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(updateTimer.ElapsedMilliseconds + "ERROR MOVING FURNITURE ON MARKET:  " + e);
                    }

                    // move Jewdewfae to a new location if she has been in one place for more than 48 turns, 8 hours
                    try
                    {
                        var fae = PlayerProcedures.GetPlayerFromBotId(-6);
                        var faeAI = AIDirectiveProcedures.GetAIDirective(fae.Id);

                        // if the turn since her last move has been long enough, relocate her
                        if (turnNo - (int)faeAI.Var2 > 48)
                        {
                            log.AddLog(updateTimer.ElapsedMilliseconds + ":  FORCED JEWDEWFAE TO MOVE.");
                            BossProcedures_Jewdewfae.MoveToNewLocation();
                        }
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(updateTimer.ElapsedMilliseconds + "ERROR TRYING TO MOVE JEWDEWFAE:  " + e);
                    }
                }
                #endregion furniture

                #region bosses

                // DONNA
                if (worldStats.Boss_Donna == AIStatics.ACTIVE)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Donna actions");
                    serverLogRepo.SaveServerLog(log);
                    try
                    {
                        BossProcedures_Donna.RunDonnaActions();
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running Donna actions", e));
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Donna actions");
                }

                // VALENTINE
                if (worldStats.Boss_Valentine == AIStatics.ACTIVE)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Valentine actions");
                    serverLogRepo.SaveServerLog(log);
                    try
                    {
                        BossProcedures_Valentine.RunValentineActions();
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running Valentine actions", e));
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Valentine actions");
                }

                // BIMBO
                if (worldStats.Boss_Bimbo == AIStatics.ACTIVE)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Bimbo actions");
                    serverLogRepo.SaveServerLog(log);
                    try
                    {
                        BossProcedures_BimboBoss.RunActions(turnNo);
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running Bimbo actions", e));
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Bimbo actions");
                }

                // THIEVES                    
                if (worldStats.Boss_Thief == AIStatics.ACTIVE)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Thieves actions");
                    serverLogRepo.SaveServerLog(log);
                    try
                    {
                        BossProcedures_Thieves.RunThievesAction(turnNo);
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running Thieves actions", e));
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Thieves actions");
                }

                // SISTERS
                if (worldStats.Boss_Sisters == AIStatics.ACTIVE)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Sisters actions");
                    serverLogRepo.SaveServerLog(log);
                    try
                    {
                        BossProcedures_Sisters.RunSistersAction();
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running Sisters actions", e));
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Sisters actions");
                }

                // FAEBOSS
                if (worldStats.Boss_Faeboss == AIStatics.ACTIVE)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Narcissa actions");
                    serverLogRepo.SaveServerLog(log);
                    try
                    {
                        BossProcedures_FaeBoss.RunTurnLogic();
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running Narcissa actions", e));
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Narcissa actions");
                }

                // BIKER GANG BOSS
                if (worldStats.Boss_MotorcycleGang == AIStatics.ACTIVE)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started BikerBoss actions");
                    serverLogRepo.SaveServerLog(log);
                    try
                    {
                        BossProcedures_MotorcycleGang.RunTurnLogic();
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  BikerBoss ERROR:  " + e);
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished BikerBoss actions");
                }

                #endregion bosses

                // psychopaths
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started psychopath actions");
                serverLogRepo.SaveServerLog(log);

                var psychoExceptions = AIProcedures.RunPsychopathActions(worldStats);

                foreach (var e in psychoExceptions)
                {
                    log.Errors++;
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running pycho action", e));
                }

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished psychopath actions");

                // minibosses
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started miniboss actions");
                serverLogRepo.SaveServerLog(log);

                var minibossExceptions = BossProcedures_Minibosses.RunAll(worldStats.TurnNumber);

                foreach (var e in minibossExceptions)
                {
                    log.Errors++;
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Error running miniboss action", e));
                }

                serverLogRepo.SaveServerLog(log);
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished miniboss actions");

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Starting setting update status to done");
                try
                {
                    PvPWorldStatProcedures.UpdateWorldTurnCounter_UpdateDone();
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished setting update status to done");
                }
                catch (Exception e)
                {
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR setting update status to done: ", e));
                    log.Errors++;
                }

                serverLogRepo.SaveServerLog(log);

                try
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started stored procedure maintenance");
                    using (var context = new StatsContext())
                    {
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[LocationLogs] WHERE Timestamp < DATEADD(hour, -1, GETUTCDATE())");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[Messages] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE()) AND DoNotRecycleMe = 0");
                        context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[TFEnergies] WHERE Amount < .5");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[PlayerLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[ChatLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[AIDirectives] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE()) AND DoNotRecycleMe = 0");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[CovenantLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[RPClassifiedAds] WHERE RefreshTimestamp < DATEADD(hour, -72, GETUTCDATE())");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[TFEnergies] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                        context.Database.ExecuteSqlCommand(
                            "DELETE FROM [dbo].[SelfRestoreEnergies] WHERE Timestamp < DATEADD(hour, -4, GETUTCDATE())");
                        context.Database.ExecuteSqlCommand(
                            $"UPDATE [dbo].[Items] SET EmbeddedOnItemId = NULL WHERE OwnerId = {merchant.Id} AND EmbeddedOnItemId IS NOT NULL AND LastSold < DATEADD(hour, -12, GETUTCDATE())");
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished stored procedure maintenance");
                }
                catch (Exception e)
                {
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR running stored procedure maintenance: ", e));
                    log.Errors++;
                }
               
                serverLogRepo.SaveServerLog(log);

                #region regenerate dungeon
                if (turnNo % 30 == 7)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Dungeon generation started.");
                    try
                    {
                        DungeonProcedures.GenerateDungeon();
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "Dungeon generation FAILED", e));
                    }
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Dungeon generation completed.");
                    serverLogRepo.SaveServerLog(log);
                }
                #endregion

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started deleting unwanted psycho items/pets on Lindella/Wuffie");
                try
                {
                    DomainRegistry.Repository.Execute(new DeleteUnpurchasedPsychoItems());
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(FormatExceptionLog(updateTimer.ElapsedMilliseconds, "ERROR deleting unwanted psycho items", e));
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting unwanted psycho items/pets on Lindella/Wuffie");
                log.FinishTimestamp = DateTime.UtcNow;
                serverLogRepo.SaveServerLog(log);


            }
        }

        private static string FormatExceptionLog(long elapsedMilliseconds, string header, Exception e)
        {
            var message = String.IsNullOrEmpty(e.Message) ? String.Empty : $"Message: '{e.Message}'.  ";
            var innerException = e.InnerException == null ? String.Empty : $"InnerException: '{e.InnerException}'. ";
            return $"<span class='playerAttackNotification'>{elapsedMilliseconds}: ERROR: {header}. {message}{innerException}</span>";
        }

    }
}