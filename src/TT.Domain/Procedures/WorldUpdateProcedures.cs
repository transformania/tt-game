using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.Items;
using TT.Domain.Commands.Players;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Procedures.BossProcedures;
using TT.Domain.Statics;
using TT.Domain.Utilities;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static  class WorldUpdateProcedures
    {
        /// <summary>
        /// Call the update world method only if enough time has elapsed, otherwise do nothing
        /// </summary>
        public static void UpdateWorldIfReady()
        {
            PvPWorldStat worldStats = PvPWorldStatProcedures.GetWorldStats();
            double secondsSinceUpdate = Math.Abs(Math.Floor(worldStats.LastUpdateTimestamp.Subtract(DateTime.UtcNow).TotalSeconds));

            // if it has been long enough since last update, force an update to occur
            if (secondsSinceUpdate > PvPStatics.TurnSecondLength && !worldStats.WorldIsUpdating && !PvPStatics.AnimateUpdateInProgress)
            {
                WorldUpdateProcedures.UpdateWorld();
            }
        }

        public static void UpdateWorld() 
        {

            PvPWorldStat worldStats = PvPWorldStatProcedures.GetWorldStats();

            int turnNo = worldStats.TurnNumber;
            PvPStatics.LastGameTurn = turnNo;

            if (turnNo < PvPStatics.RoundDuration)
            {

                PvPStatics.AnimateUpdateInProgress = true;

                IServerLogRepository serverLogRepo = new EFServerLogRepository();
                ServerLog log = new ServerLog
                {
                    TurnNumber = turnNo,
                    StartTimestamp = DateTime.UtcNow,
                    FinishTimestamp = DateTime.UtcNow,
                    Errors = 0,
                    FullLog = "",
                    Population = PlayerProcedures.GetWorldPlayerStats().CurrentOnlinePlayers,
                };
                log.AddLog("Started new log for turn " + turnNo + ".");
                serverLogRepo.SaveServerLog(log);
                Stopwatch updateTimer = new Stopwatch();
                updateTimer.Start();

                IPlayerRepository playerRepo = new EFPlayerRepository();

                #region spawn NPCS
                // make sure the NPCs have been spawned early turn
                if (turnNo <= 3)
                {
                    Player lindella = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.LindellaBotId);
                    if (lindella == null)
                    {
                        BossProcedures_Lindella.SpawnLindella();
                    }

                    Player wuffie = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.WuffieBotId);
                    if (wuffie == null)
                    {
                        BossProcedures_PetMerchant.SpawnPetMerchant();
                    }

                    Player fae = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.JewdewfaeBotId);
                    if (fae == null)
                    {
                        BossProcedures_Jewdewfae.SpawnFae();
                    }

                    Player bartender = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.BartenderBotId);
                    if (bartender == null)
                    {
                        BossProcedures_Bartender.SpawnBartender();
                    }

                    Player lorekeeper = playerRepo.Players.FirstOrDefault(p => p.BotId == AIStatics.LoremasterBotId);
                    if (lorekeeper == null)
                    {
                        BossProcedures_Loremaster.SpawnLoremaster();
                    }
                }
                #endregion

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started loading animate players");
                
                IEffectRepository effectRepo = new EFEffectRepository();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started loading effects");
                List<Effect> temporaryEffects = effectRepo.Effects.Where(e => !e.IsPermanent).ToList();
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished loading effects");
                List<Effect> effectsToDelete = new List<Effect>();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating effects");
                foreach (Effect e in temporaryEffects)
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
                foreach (Effect e in effectsToDelete)
                {
                    effectRepo.DeleteEffect(e.Id);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting expired effects");
                serverLogRepo.SaveServerLog(log);

                #region playerExtra / protection cooldown loop
                IPlayerExtraRepository playerExtraRepo = new EFPlayerExtraRepository();
                List<PlayerExtra> extrasToIncrement = playerExtraRepo.PlayerExtras.ToList();
                List<PlayerExtra> extrasToIncrement_SaveList = new List<PlayerExtra>();
                List<PlayerExtra> extrasToIncrement_DeleteList = new List<PlayerExtra>();
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating protection change cooldown (" + extrasToIncrement.Count + ")");

                foreach (PlayerExtra e in extrasToIncrement)
                {
                    Player owner = PlayerProcedures.GetPlayer(e.PlayerId);
                    if (PlayerProcedures.PlayerIsOffline(owner))
                    {
                        extrasToIncrement_SaveList.Add(e);
                    }
                }

                foreach (PlayerExtra e in extrasToIncrement_SaveList)
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

                foreach (PlayerExtra e in extrasToIncrement_DeleteList)
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

                        "UPDATE [dbo].[Players] SET ActionPoints_Refill = ActionPoints_Refill + (ActionPoints % 120 / 2) WHERE ActionPoints >= 120 AND Mobility='full'" +

                        "UPDATE [dbo].[Players] SET ActionPoints = 120 WHERE ActionPoints > 120 AND Mobility='full'" +

                        "UPDATE [dbo].[Players] SET ActionPoints_Refill = 360 WHERE ActionPoints_Refill > 360 AND Mobility='full'" +

                        "UPDATE [dbo].[Players] SET ActionPoints = ActionPoints + 20, ActionPoints_Refill = ActionPoints_Refill - 20 WHERE ActionPoints <= 100 AND ActionPoints_Refill >= 20 AND Mobility='full'");

                        if (PvPStatics.ChaosMode)
                        {
                            context.Database.ExecuteSqlCommand("Update [dbo].[Players] SET ActionPoints = 120, ActionPoints_Refill = 360, TimesAttackingThisUpdate = -999, Mana = MaxMana");
                        }

                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  ANIMATE SQL UPDATE SUCCEEDED!");
                        serverLogRepo.SaveServerLog(log);
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(updateTimer.ElapsedMilliseconds + "ANIMATE SQL UPDATE FAILED.  Reason:  " + e.ToString());
                        serverLogRepo.SaveServerLog(log);
                    }
                }

                DateTime timeCutoff = DateTime.UtcNow.AddHours(-8);

                List<int> playerIdsToSave = playerRepo.Players.Where(p => p.Mobility == PvPStatics.MobilityFull && p.LastActionTimestamp > timeCutoff).Select(p => p.Id).ToList();

                foreach (int i in playerIdsToSave)
                {

                    Player player = playerRepo.Players.FirstOrDefault(p => p.Id == i);

                    // skip players who have not done anything in the past 24 hours
                    double hoursSinceLastAction = Math.Abs(Math.Floor(player.LastActionTimestamp.Subtract(DateTime.UtcNow).TotalHours));
                    if (hoursSinceLastAction > 24)
                    {
                        continue;
                    }

                    BuffBox buffs = ItemProcedures.GetPlayerBuffs(player);
                    player.Health += buffs.HealthRecoveryPerUpdate();
                    player.Mana += buffs.ManaRecoveryPerUpdate();


                    player.ReadjustMaxes(buffs);

                    // extra AP condition checks
                    if (player.Covenant > 0)
                    {
                        CovenantNameFlag playerCov = CovenantDictionary.IdNameFlagLookup.FirstOrDefault(c => c.Key == player.Covenant).Value;

                        // give this player an extra AP refill if they are at their safeground, scaled up by level
                        if (playerCov != null && !playerCov.HomeLocation.IsNullOrEmpty() && player.dbLocationName == playerCov.HomeLocation)
                        {
                            player.ActionPoints_Refill += .25M * playerCov.CovLevel;
                           
                        }

                        // give this player an extra AP refill if they are on a location that their covenane has enchanted
                        Location currentLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == player.dbLocationName);

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
                        if (player.ActionPoints_Refill > PvPStatics.MaximumStoreableActionPoints_Refill)
                        {
                            player.ActionPoints_Refill = PvPStatics.MaximumStoreableActionPoints_Refill;
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

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating animate players (" + playerIdsToSave.Count + ")");
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
                        log.AddLog(updateTimer.ElapsedMilliseconds + "ERROR UPDATING INANIMATE/ANIMAL PLAYERS:  " + e.ToString());
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
                        log.AddLog(updateTimer.ElapsedMilliseconds + "MIND CONTROLL COOLDOWN UPDATE FAILED.  Reason:  " + e.ToString());
                    }
                }
                #endregion

                serverLogRepo.SaveServerLog(log);

                PvPStatics.AnimateUpdateInProgress = false;

                // bump down the timer on all items that are reuseable consumables
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started updating items on cooldown");
                IItemRepository itemsRepo = new EFItemRepository();
                List<Item> itemsToUpdate = itemsRepo.Items.Where(i => i.TurnsUntilUse > 0).ToList();

                foreach (Item item in itemsToUpdate)
                {
                    item.TurnsUntilUse--;
                }

                foreach (Item item in itemsToUpdate)
                {
                    itemsRepo.SaveItem(item);
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished updating items on cooldown");
                serverLogRepo.SaveServerLog(log);

                // find the ids for the merchants Lindella and Skaldyr
                Player merchant = PlayerProcedures.GetPlayerFromBotId(AIStatics.LindellaBotId);
                Player skaldyr = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);
                int merchantId = merchant.Id;
                int skaldyrId = skaldyr.Id;

                // have abandoned items go to Lindella
                if (turnNo % 11 == 3 && merchant.Mobility == PvPStatics.MobilityFull)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started collecting all abandoned items for Lindella");

                    using (var context = new StatsContext())
                    {
                        try
                        {
                            context.Database.ExecuteSqlCommand("UPDATE [dbo].[Items] SET OwnerId = " + merchant.Id + ", dbLocationName = '', PvPEnabled = -1, TimeDropped = '" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + "'  WHERE  dbLocationName <> '' AND dbLocationName IS NOT NULL AND TimeDropped < DATEADD(hour, -8, GETUTCDATE()) AND OwnerId = -1 AND dbName LIKE 'item_%' AND dbName != '" + PvPStatics.ItemType_DungeonArtifact + "'");

                            context.Database.ExecuteSqlCommand("UPDATE [dbo].[Players] SET dbLocationName = '" + merchant.dbLocationName + "' WHERE (FirstName + ' ' + LastName) IN ( SELECT VictimName FROM [dbo].[Items] WHERE  dbLocationName <> '' AND dbLocationName IS NOT NULL AND TimeDropped < DATEADD(hour, -8, GETUTCDATE()) AND OwnerId = -1 AND dbName LIKE 'item_%' AND dbName != '" + PvPStatics.ItemType_DungeonArtifact + "')");

                            log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished collecting all abandoned items for Lindella");

                        }
                        catch (Exception e)
                        {
                            log.Errors++;
                            log.AddLog(updateTimer.ElapsedMilliseconds + ":  ERROR collecting all abandoned items for Lindella:  " + e.ToString());
                        }
                    }


                }


                // delete all consumable type items that have been sitting around on the ground for too long

                DomainRegistry.Repository.Execute(new DeleteExpiredConsumablesOnGround());

                //List<Item> possibleToDelete = itemsRepo.Items.Where(i => (i.dbLocationName != "" && i.OwnerId == -1) || (i.OwnerId == merchantId || i.OwnerId == skaldyrId) && i.dbName != PvPStatics.ItemType_DungeonArtifact).ToList();
                //List<Item> deleteItems = new List<Item>();

                //serverLogRepo.SaveServerLog(log);

                //log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started deleting expired consumables");
                //foreach (Item item in possibleToDelete)
                //{
                //    DbStaticItem temp = ItemStatics.GetStaticItem(item.dbName);
                //    if (temp.ItemType != "consumable")
                //    {
                //        continue;
                //    }
                //    double droppedMinutesAgo = Math.Abs(Math.Floor(item.TimeDropped.Subtract(DateTime.UtcNow).TotalMinutes));

                //    if (droppedMinutesAgo > PvPStatics.MinutesToDroppedItemDelete && item.OwnerId == -1)
                //    {
                //        deleteItems.Add(item);
                //    }
                //    else if (droppedMinutesAgo > PvPStatics.MinutesToDroppedItemDelete * 12)
                //    {
                //        deleteItems.Add(item);
                //    }
                //}

                //foreach (Item item in deleteItems)
                //{
                //    itemsRepo.DeleteItem(item.Id);
                //}
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished deleting expired consumables");
                serverLogRepo.SaveServerLog(log);

                // allow all items that have been recently equipped to be taken back off
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started resetting items that have been recently equipped");
                List<Item> recentlyEquipped = itemsRepo.Items.Where(i => i.EquippedThisTurn).ToList();

                foreach (Item item in recentlyEquipped)
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
                    List<Covenant> covs = covRepo.Covenants.Where(c => c.HomeLocation != null && c.HomeLocation != "").ToList();


                    foreach (Covenant c in covs)
                    {
                        int locationControlledSum = CovenantProcedures.GetLocationControlCount(c);
                        decimal moneyGain = (decimal)Math.Floor(Convert.ToDouble(locationControlledSum));
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
                        int dungeonArtifactCount = itemsRepo.Items.Where(i => i.dbName == PvPStatics.ItemType_DungeonArtifact).Count();
                        for (int x = 0; x < PvPStatics.DungeonArtifact_SpawnLimit - dungeonArtifactCount; x++)
                        {
                            string randDungeon = LocationsStatics.GetRandomLocation_InDungeon();

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
                                VictimName = "",
                                dbName = PvPStatics.ItemType_DungeonArtifact,
                                ItemSourceId = ItemStatics.GetStaticItem(PvPStatics.ItemType_DungeonArtifact).Id
                            };
                            DomainRegistry.Repository.Execute(cmd);
                        }


                        IEnumerable<Player> demons = playerRepo.Players.Where(i => i.Form == PvPStatics.DungeonDemon);
                        int dungeonDemonCount = demons.Count();

                        Random randLevel = new Random(Guid.NewGuid().GetHashCode());

                        var demonNames = XmlResourceLoader.Load<List<string>>("TT.Domain.XMLs.DungeonDemonNames.xml");

                        for (int x = 0; x < PvPStatics.DungeonDemon_Limit - dungeonDemonCount; x++)
                        {
                            string randDungeon = LocationsStatics.GetRandomLocation_InDungeon();
                            Location spawnLocation = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == randDungeon);

                            // pull a random last demon name
                            double maxDemonNameCount = demonNames.Count();
                            double num = randLevel.NextDouble();
                            int demonIndex = Convert.ToInt32(Math.Floor(num * maxDemonNameCount));
                            string demonlastName = demonNames.ElementAt(demonIndex);

                            // if there's already a demon with this last name, reroll and try again
                            if (demons.FirstOrDefault(d => d.LastName == demonlastName) != null)
                            {
                                x--;
                                continue;
                            }


                            double levelRoll = randLevel.NextDouble();
                            int level = (int)Math.Floor(levelRoll * 8 + 3);

                            var cmd = new CreatePlayer
                            {
                                BotId = AIStatics.DemonBotId,
                                FirstName = "Spirit of ",
                                LastName = demonlastName,
                                Mobility = PvPStatics.MobilityFull,
                                Form = PvPStatics.DungeonDemon,
                                Gender = PvPStatics.GenderFemale,
                                GameMode = 2,
                                Health = 1000,
                                Mana = 1000,
                                OriginalForm = PvPStatics.DungeonDemon,
                                Covenant = -1,
                                Location = randDungeon,
                                Level = level,
                                MaxHealth = 500,
                                MaxMana = 500,
                            };

                            var id = DomainRegistry.Repository.Execute(cmd);

                            Player newDemon = new EFPlayerRepository().Players.FirstOrDefault(p => p.Id == id);
                            
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
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  ERROR running dungeon actions:  " + e.ToString());
                }


                #endregion

                serverLogRepo.SaveServerLog(log);
                log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);

                #region forcibly terminate duels that have timed out
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started duel updates");
                try
                {
                    IDuelRepository duelRepo = new EFDuelRepository();
                    List<Duel> duels = duelRepo.Duels.Where(d => d.Status == DuelProcedures.ACTIVE).ToList();

                    foreach (Duel d in duels)
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
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Failed to complete duel updates.  Reason:  " + e.ToString());
                }
                #endregion duel updates


                //TempData["Result"] = "WORLD UPDATED";

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Lindella actions");
                try
                {
                    serverLogRepo.SaveServerLog(log);
                    BossProcedures_Lindella.RunActions(turnNo);
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Lindella actions");
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  FAILED Lindella actions.  Reason:  " + e.ToString());
                }


                // log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);


                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Wuffie actions");
                serverLogRepo.SaveServerLog(log);
                try
                {
                    BossProcedures_PetMerchant.RunPetMerchantActions(turnNo);
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Wuffie actions");
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  ERROR running Wuffie actions:  " + e.ToString());
                }


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
                        log.AddLog(updateTimer.ElapsedMilliseconds + "ERROR MOVING FURNITURE ON MARKET:  " + e.ToString());
                    }

                    // move Jewdewfae to a new location if she has been in one place for more than 48 turns, 8 hours
                    try
                    {
                        Player fae = PlayerProcedures.GetPlayerFromBotId(-6);
                        AIDirective faeAI = AIDirectiveProcedures.GetAIDirective(fae.Id);

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
                        log.AddLog(updateTimer.ElapsedMilliseconds + "ERROR TRYING TO MOVE JEWDEWFAE:  " + e.ToString());
                    }
                }
                #endregion furniture

                #region bosses

                // DONNA
                try
                {
                    // run boss logic if one is active
                    if (worldStats.Boss_Donna == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Donna actions");
                        serverLogRepo.SaveServerLog(log);
                        BossProcedures_Donna.RunDonnaActions();
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Donna actions");
                        serverLogRepo.SaveServerLog(log);
                    }
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  DONNA ERROR:  " + e.InnerException.ToString());
                    serverLogRepo.SaveServerLog(log);
                }


                // VALENTINE
                try
                {
                    // run boss logic if one is active
                    if (worldStats.Boss_Valentine == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Valentine actions");
                        serverLogRepo.SaveServerLog(log);
                        TT.Domain.Procedures.BossProcedures.BossProcedures_Valentine.RunValentineActions();
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Valentine actions");
                    }
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Valentine ERROR:  " + e.InnerException.ToString());
                }

                // BIMBO
                try
                {
                    // run boss logic if one is active
                    if (worldStats.Boss_Bimbo == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Bimbo actions");
                        serverLogRepo.SaveServerLog(log);
                        TT.Domain.Procedures.BossProcedures.BossProcedures_BimboBoss.RunActions(turnNo);
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Bimbo actions");
                    }
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Bimbo ERROR:  " + e.InnerException.ToString());
                }

                // THIEVES
                try
                {
                    // run boss logic if one is active
                    if (worldStats.Boss_Thief == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Thieves actions");
                        serverLogRepo.SaveServerLog(log);
                        TT.Domain.Procedures.BossProcedures.BossProcedures_Thieves.RunThievesAction(turnNo);
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Thieves actions");
                    }
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Bimbo ERROR:  " + e.InnerException.ToString());
                }

                // SISTERS
                try
                {
                    // run boss logic if one is active
                    if (worldStats.Boss_Sisters == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Sisters actions");
                        serverLogRepo.SaveServerLog(log);
                        TT.Domain.Procedures.BossProcedures.BossProcedures_Sisters.RunSistersAction();
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Sisters actions");
                    }
                }
                catch (Exception e)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Sisters ERROR:  " + e.InnerException.ToString());
                }

                // FAEBOSS
                try
                {
                    // run boss logic if one is active
                    if (worldStats.Boss_Faeboss == "active")
                    {
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started Narcissa actions");
                        serverLogRepo.SaveServerLog(log);
                        TT.Domain.Procedures.BossProcedures.BossProcedures_FaeBoss.RunTurnLogic();
                        log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished Narcissa actions");
                    }
                }
                catch (Exception e)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Narcissa ERROR:  " + e.InnerException.ToString());
                }

                #endregion bosses

                // psychopaths
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started psychopath actions");
                serverLogRepo.SaveServerLog(log);

                try
                {
                    AIProcedures.RunPsychopathActions();
                    log = serverLogRepo.ServerLogs.FirstOrDefault(s => s.TurnNumber == turnNo);
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished psychopath actions");
                }
                catch (Exception e)
                {
                    log.Errors++;
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  PSYCHOPATH RUNTIME ERROR:  " + e);
                }


                PvPWorldStatProcedures.UpdateWorldTurnCounter_UpdateDone();

                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Started stored procedure maintenance");
                using (var context = new StatsContext())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[LocationLogs] WHERE Timestamp < DATEADD(hour, -1, GETUTCDATE())");
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Messages] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE()) AND DoNotRecycleMe = 0");
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[TFEnergies] WHERE Amount < .5");
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[PlayerLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[ChatLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[AIDirectives] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE()) AND DoNotRecycleMe = 0");
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[CovenantLogs] WHERE Timestamp < DATEADD(hour, -72, GETUTCDATE())");
                    context.Database.ExecuteSqlCommand("DELETE FROM [dbo].[RPClassifiedAds] WHERE RefreshTimestamp < DATEADD(hour, -72, GETUTCDATE())");
                }
                log.AddLog(updateTimer.ElapsedMilliseconds + ":  Finished stored procedure maintenance");

                #region regenerate dungeon
                if (turnNo % 30 == 7)
                {
                    log.AddLog(updateTimer.ElapsedMilliseconds + ":  Dungeon generation started.");
                    try
                    {
                        DungeonProcedures.GenerateDungeon();
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Dungeon generation completed.");
                    }
                    catch (Exception e)
                    {
                        log.Errors++;
                        log.AddLog(updateTimer.ElapsedMilliseconds + ":  Dungeon generation FAILED.  Reason:  " + e.ToString());
                    }
                }
                #endregion

                log.FinishTimestamp = DateTime.UtcNow;
                serverLogRepo.SaveServerLog(log);


            }
        }

    }
}