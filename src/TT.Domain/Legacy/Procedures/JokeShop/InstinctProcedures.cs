using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Procedures;
using TT.Domain.Statics;

namespace TT.Domain.Legacy.Procedures.JokeShop
{
    static class InstinctProcedures
    {

        internal static void ActOnInstinct(List<int> playersToControl, Random rand = null)
        {
            if (playersToControl == null || playersToControl.IsEmpty())
            {
                return;
            }

            rand = rand ?? new Random();
            var cutoff = DateTime.UtcNow.AddMinutes(-TurnTimesStatics.GetOfflineAfterXMinutes());

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var activePlayers = playerRepo.Players
                                          .Where(p => p.OnlineActivityTimestamp >= cutoff &&
                                                      p.Mobility == PvPStatics.MobilityFull &&
                                                      p.InDuel <= 0 &&
                                                      p.InQuest <= 0 &&
                                                      p.BotId == AIStatics.ActivePlayerBotId &&
                                                      !p.dbLocationName.StartsWith("dungeon_"))
                                          .Select(p => new { p.Id, p.FormSourceId, p.dbLocationName });

            var mcPlayers = activePlayers.Where(p => playersToControl.Any(victim => p.Id == victim));
            var freePlayers = activePlayers.Where(p => !playersToControl.Any(victim => p.Id == victim));

            // Sheep - find another sheep and follow it
            var mcSheep = mcPlayers.Where(p => JokeShopProcedures.SHEEP.Any(sheepForm => p.FormSourceId == sheepForm)).ToList();
            if (!mcSheep.IsEmpty())
            {
                var freeSheep = freePlayers.Where(p => JokeShopProcedures.SHEEP.Any(sheepForm => p.FormSourceId == sheepForm)).ToList();
                var flockToPlayer = -1;
                var flockToLocation = "";

                if (!freeSheep.IsEmpty())
                {
                    var target = freeSheep[rand.Next(freeSheep.Count())];
                    flockToPlayer = target.Id;
                    flockToLocation = target.dbLocationName;
                }
                else
                {
                    flockToLocation = LocationsStatics.GetRandomLocationNotInDungeonOr(LocationsStatics.JOKE_SHOP);
                }

                foreach (var sheep in mcSheep)
                {
                    var sheepPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == sheep.Id);
                    var stoppedAt = EnvironmentPrankProcedures.MovePlayer(sheepPlayer, flockToLocation, 15, (p, loc) =>
                    {
                        if (rand.Next(3) == 0)
                        {
                            LocationLogProcedures.AddLocationLog(loc, $"{p.GetFullName()} bleated here:  <b>Baaaaa!</b>");
                        }
                    });

                    if (stoppedAt == flockToLocation)
                    {
                        if (flockToPlayer >= 0)
                        {
                            PlayerLogProcedures.AddPlayerLog(flockToPlayer, $"{sheepPlayer.GetFullName()} bleated at you:  <b>Baaaaa!</b>", true);
                        }

                        LocationLogProcedures.AddLocationLog(stoppedAt, $"{sheepPlayer.GetFullName()} bleated here:  <b>Baaaaa!</b>");
                    }

                    if (stoppedAt != null)
                    {
                        var here = LocationsStatics.GetConnectionName(stoppedAt);
                        PlayerLogProcedures.AddPlayerLog(sheepPlayer.Id, $"You followed your flock to <b>{here}</b>.", true);
                    }
                }

                // Don't consider affected players for any more actions this turn
                mcPlayers = mcPlayers.Where(p => mcSheep.All(s => p.Id != s.Id));

                if (flockToPlayer >= 0)
                {
                    freePlayers = freePlayers.Where(p => p.Id != flockToPlayer);
                }
            }

            // Dogs - chase cats, possiby up a tree
            var mcDogs = mcPlayers.Where(p => JokeShopProcedures.DOGS.Any(dogForm => p.FormSourceId == dogForm)).ToList();
            if (!mcDogs.IsEmpty())
            {
                var mcCats = mcPlayers.Where(p => JokeShopProcedures.CATS_AND_NEKOS.Any(catForm => p.FormSourceId == catForm)).ToList();
                var freeCats = freePlayers.Where(p => JokeShopProcedures.CATS_AND_NEKOS.Any(catForm => p.FormSourceId == catForm)).ToList();

                while (!mcDogs.IsEmpty() && mcCats.Count() + freeCats.Count() > 0)
                {
                    // Find a dog
                    var dogIndex = rand.Next(mcDogs.Count());
                    var dog = mcDogs[dogIndex];
                    mcDogs.RemoveAt(dogIndex);
                    mcPlayers = mcPlayers.Where(p => p.Id != dog.Id);

                    int catId;
                    string catLoc;
                    bool catIsMindControlled;

                    // Find a cat for them to chase
                    if (!mcCats.IsEmpty())
                    {
                        var catIndex = rand.Next(mcCats.Count());
                        var cat = mcCats[catIndex];
                        mcCats.RemoveAt(catIndex);

                        catId = cat.Id;
                        catLoc = cat.dbLocationName;
                        catIsMindControlled = true;
                        mcPlayers = mcPlayers.Where(p => p.Id != catId);
                    }
                    else  // !freeCats.IsEmpty()
                    {
                        var catIndex = rand.Next(freeCats.Count);
                        var cat = freeCats[catIndex];
                        freeCats.RemoveAt(catIndex);

                        catId = cat.Id;
                        catLoc = cat.dbLocationName;
                        catIsMindControlled = false;
                        freePlayers = freePlayers.Where(p => p.Id != catId);
                    }

                    var dogPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == dog.Id);
                    var catPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == catId);

                    // Move dog
                    var stoppedAt = EnvironmentPrankProcedures.MovePlayer(dogPlayer, catLoc, 15, (p, loc) =>
                    {
                        var roll = rand.Next(4);
                        if (roll == 0)
                        {
                            LocationLogProcedures.AddLocationLog(loc, $"{dogPlayer.GetFullName()} barked here as they catch the scent of a cat, {catPlayer.GetFullName()}:  <b>Woof woof!</b>");
                        }
                        else if (roll == 1)
                        {
                            LocationLogProcedures.AddLocationLog(loc, $"{dogPlayer.GetFullName()} growled here as they get closer to {catPlayer.GetFullName()}, the cat:  <b>Grrrrrrrrr!</b>");
                        }
                    });

                    // Dog has arrived at the cat's location
                    if (stoppedAt == catLoc)
                    {
                        var here = LocationsStatics.GetConnectionName(stoppedAt);
                        LocationLogProcedures.AddLocationLog(stoppedAt, $"{dogPlayer.GetFullName()} barked at {catPlayer.GetFullName()}:  <b>Woof woof!</b>");

                        // If cat is mind controlled we can send them up a tree
                        if (catIsMindControlled)
                        {
                            var treeLoc = mcPlayers.FirstOrDefault(p => JokeShopProcedures.TREES.Any(treeForm => p.FormSourceId == treeForm))?.dbLocationName;

                            if (treeLoc == null)
                            {
                                treeLoc = freePlayers.FirstOrDefault(p => JokeShopProcedures.TREES.Any(treeForm => p.FormSourceId == treeForm))?.dbLocationName;
                            }

                            if (treeLoc == null)
                            {
                                treeLoc = "forest_ancestor_tree";
                            }

                            var catStoppedAt = EnvironmentPrankProcedures.MovePlayer(catPlayer, treeLoc, 15, (p, loc) =>
                            {
                                var roll = rand.Next(3);
                                if (roll == 0)
                                {
                                    LocationLogProcedures.AddLocationLog(loc, $"<b>Meoooww!</b> yowls {catPlayer.GetFullName()} as they quickly flee from {dogPlayer.GetFullName()}, the dog who is chasing them.");
                                }
                            });

                            if (catStoppedAt == treeLoc)
                            {
                                LocationLogProcedures.AddLocationLog(catLoc, $"{catPlayer.GetFullName()} runs to hide from {dogPlayer.GetFullName()}!");
                                LocationLogProcedures.AddLocationLog(treeLoc, $"{catPlayer.GetFullName()} leaps into a tree to hide from {dogPlayer.GetFullName()}, the dog who is chasing them.");
                                PlayerLogProcedures.AddPlayerLog(catId, $"<b>Woof woof!</b>  {dogPlayer.GetFullName()} barks at you, and you run off to the nearest tree!", true);
                                PlayerLogProcedures.AddPlayerLog(dogPlayer.Id, $"You chased a cat to <b>{here}</b> and started barking at {catPlayer.GetFullName()}:  <b>Woof woof!</b>.  They run off to the nearest tree to hide from you!", true);
                            }
                            else if (catStoppedAt == null)
                            {
                                PlayerLogProcedures.AddPlayerLog(catId, $"<b>Woof woof!</b>  {dogPlayer.GetFullName()} barks at you, but you can't seem to escape!", true);
                                PlayerLogProcedures.AddPlayerLog(dogPlayer.Id, $"You chased a cat to <b>{here}</b> and started barking at {catPlayer.GetFullName()}:  <b>Woof woof!</b>.", true);
                            }
                            else
                            {
                                LocationLogProcedures.AddLocationLog(catLoc, $"{catPlayer.GetFullName()} runs to hide from {dogPlayer.GetFullName()}!");
                                PlayerLogProcedures.AddPlayerLog(catId, $"<b>Woof woof!</b>  {dogPlayer.GetFullName()} barks at you, and you run away to try and hide!", true);
                                PlayerLogProcedures.AddPlayerLog(dogPlayer.Id, $"You chased a cat to <b>{here}</b> and started barking at {catPlayer.GetFullName()}:  <b>Woof woof!</b>.  They run off to try and hide from you!", true);
                            }
                        }
                        else  // Cat not mind controlled, dog can only bark
                        {
                            PlayerLogProcedures.AddPlayerLog(catId, $"{dogPlayer.GetFullName()} barked at you:  <b>Woof woof!</b>", true);
                            PlayerLogProcedures.AddPlayerLog(dogPlayer.Id, $"You chased a cat to <b>{here}</b> and started barking at {catPlayer.GetFullName()}:  <b>Woof woof!</b>", true);
                        }
                    }
                    else if (stoppedAt != null)  // Dog moved, but didn't get to the cat's location
                    {
                        var here = LocationsStatics.GetConnectionName(stoppedAt);
                        PlayerLogProcedures.AddPlayerLog(dogPlayer.Id, $"You caught scent of a cat and ran to <b>{here}</b>", true);
                    }
                    else if (dog.dbLocationName == catLoc)
                    {
                        PlayerLogProcedures.AddPlayerLog(catId, $"{dogPlayer.GetFullName()} barked at you:  <b>Woof woof!</b>", true);
                        PlayerLogProcedures.AddPlayerLog(dogPlayer.Id, $"You barked at {catPlayer.GetFullName()}:  <b>Woof woof!</b>", true);
                    }

                }
            }

            // Cats - chase rodents
            var mcCats2 = mcPlayers.Where(p => JokeShopProcedures.CATS_AND_NEKOS.Any(catForm => p.FormSourceId == catForm)).ToList();
            if (!mcCats2.IsEmpty())
            {
                var mcRodents = mcPlayers.Where(p => JokeShopProcedures.RODENTS.Any(rodentForm => p.FormSourceId == rodentForm)).ToList();
                var freeRodents = freePlayers.Where(p => JokeShopProcedures.RODENTS.Any(rodentForm => p.FormSourceId == rodentForm)).ToList();

                while (!mcCats2.IsEmpty() && mcRodents.Count() + freeRodents.Count() > 0)
                {
                    // Find a cat
                    var catIndex = rand.Next(mcCats2.Count());
                    var cat = mcCats2[catIndex];
                    mcCats2.RemoveAt(catIndex);
                    mcPlayers = mcPlayers.Where(p => p.Id != cat.Id);

                    int rodentId;
                    string rodentLoc;

                    // Find a rodent for them to chase
                    if (!mcRodents.IsEmpty())
                    {
                        var rodentIndex = rand.Next(mcRodents.Count());
                        var rodent = mcRodents[rodentIndex];
                        mcRodents.RemoveAt(rodentIndex);

                        rodentId = rodent.Id;
                        rodentLoc = rodent.dbLocationName;
                        mcPlayers = mcPlayers.Where(p => p.Id != rodentId);
                    }
                    else  // !freeRodents.IsEmpty()
                    {
                        var rodentIndex = rand.Next(freeRodents.Count);
                        var rodent = freeRodents[catIndex];
                        freeRodents.RemoveAt(rodentIndex);

                        rodentId = rodent.Id;
                        rodentLoc = rodent.dbLocationName;
                        freePlayers = freePlayers.Where(p => p.Id != rodentId);
                    }

                    var catPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == cat.Id);
                    var rodentPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == rodentId);

                    // Move cat
                    var stoppedAt = EnvironmentPrankProcedures.MovePlayer(catPlayer, rodentLoc, 15, (p, loc) =>
                    {
                        var roll = rand.Next(4);
                        if (roll == 0)
                        {
                            LocationLogProcedures.AddLocationLog(loc, $"{catPlayer.GetFullName()} stealthily prowls the area, on the hunt for {rodentPlayer.GetFullName()}");
                        }
                    });

                    if (stoppedAt == rodentLoc)
                    {
                        var here = LocationsStatics.GetConnectionName(stoppedAt);
                        LocationLogProcedures.AddLocationLog(stoppedAt, $"{catPlayer.GetFullName()} lunges at a rodent, but {rodentPlayer.GetFullName()} is too quick and evades the cat's attack!</b>");

                        PlayerLogProcedures.AddPlayerLog(rodentId, $"{catPlayer.GetFullName()} jumps out at you, but you leap from their paws and deprive them of an easy snack!", true);
                        PlayerLogProcedures.AddPlayerLog(catPlayer.Id, $"You prowl to <b><{here}</b>, creep up on {rodentPlayer.GetFullName()} and pounce!  But they're too quick and evade your clutches!", true);
                    }
                    else if (stoppedAt != null)  // Cat moved, but didn't get to the rodent's location
                    {
                        var here = LocationsStatics.GetConnectionName(stoppedAt);
                        PlayerLogProcedures.AddPlayerLog(catPlayer.Id, $"You prowl to <b>{here}</b>, being careful not to get too close to your prey as you prepare for the hunt...", true);
                    }
                    else if (cat.dbLocationName == rodentLoc)
                    {
                        PlayerLogProcedures.AddPlayerLog(rodentId, $"{catPlayer.GetFullName()} hides in a bush, stealthily watching you, getting ready to pounce...", true);
                        PlayerLogProcedures.AddPlayerLog(catPlayer.Id, $"You slink behind a bush and focus your eyes on {rodentPlayer.GetFullName()}, getting ready to strike...", true);
                    }
                }
            }

            // Maids - find places to clean
            var mcMaids = mcPlayers.Where(p => JokeShopProcedures.MAIDS.Any(maidForm => p.FormSourceId == maidForm)).ToList();
            foreach (var maid in mcMaids)
            {
                var maidPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == maid.Id);
                var maidLoc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == maidPlayer.dbLocationName);

                if (maidLoc != null)
                {
                    string nextLoc;
                    string newContract = "";

                    string[] cleaningContracts = { "coffee_shop", "tavern", "oldoak_apartments", "mansion", "ranch_inside", "castle", "salon" };
                    if (cleaningContracts.Contains(maidLoc.Region))
                    {
                        nextLoc = LocationsStatics.GetRandomLocation_InRegion(maidLoc.Region);
                    }
                    else
                    {
                        newContract = "One of the local facilities has just signed a new contract with you!  ";
                        nextLoc = LocationsStatics.GetRandomLocation_InRegion(cleaningContracts[rand.Next(cleaningContracts.Count())]);
                    }

                    var stoppedAt = EnvironmentPrankProcedures.MovePlayer(maidPlayer, nextLoc, 20);

                    if (stoppedAt == nextLoc || maidPlayer.dbLocationName == nextLoc)
                    {
                        var here = LocationsStatics.GetConnectionName(nextLoc);
                        string[] activities = { "dusting away the cobwebs", "vaccuuming the floor", "washing up the dishes", "doing the laundry", "serving refreshments", "sweeping the trash", "handing out freshly baked cupcakes" };
                        var activity = activities[rand.Next(activities.Count())];

                        PlayerLogProcedures.AddPlayerLog(maidPlayer.Id, $"{newContract}You arrive at <b>{here}</b> and start {activity}!", true);
                        LocationLogProcedures.AddLocationLog(nextLoc, $"{maidPlayer.GetFullName()} arrives here and starts <b>{activity}</b>.");
                    }
                    else if (stoppedAt != null)
                    {
                        var here = LocationsStatics.GetConnectionName(stoppedAt);
                        PlayerLogProcedures.AddPlayerLog(maidPlayer.Id, $"{newContract}You quickly head to your new job but only get as far as <b>{here}</b>.", true);
                    }
                }

                mcPlayers = mcPlayers.Where(p => p.Id != maid.Id);
            }

            // Strippers - remove random item
            var mcStrippers = mcPlayers.Where(p => JokeShopProcedures.STRIPPERS.Any(stripperForm => p.FormSourceId == stripperForm)).ToList();
            foreach (var stripper in mcStrippers)
            {
                var stripperPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == stripper.Id);
                var stripperItems = ItemProcedures.GetAllPlayerItems(stripperPlayer.Id)
                                                  .Where(i => i.dbItem.IsEquipped &&
                                                              i.Item.ItemType != PvPStatics.ItemType_Pet &&
                                                              i.Item.ItemType != PvPStatics.ItemType_Consumable &&
                                                              i.Item.ItemType != PvPStatics.ItemType_Consumable_Reuseable).ToList();

                if (stripperItems.Any())
                {
                    string[] adverbs = { "seductively", "cautiously", "flirtatiously", "cheekily", "obediently", "enthusiastically", "reluctantly", "hesitantly", "energetically", "sheepishly" };
                    var adverb = adverbs[rand.Next(adverbs.Count())];
                    var itemToDrop = stripperItems[rand.Next(stripperItems.Count())];

                    PlayerLogProcedures.AddPlayerLog(stripper.Id, $"You {adverb} remove your <b>{itemToDrop.Item.FriendlyName}</b>!", true);
                    LocationLogProcedures.AddLocationLog(stripper.dbLocationName, $"{stripperPlayer.GetFullName()} {adverb} removes their <b>{itemToDrop.Item.FriendlyName}</b>!");
                    ItemProcedures.DropItem(itemToDrop.dbItem.Id);
                }

                mcPlayers = mcPlayers.Where(p => p.Id != stripper.Id);
            }

            // Ghosts - haunt old buildings
            var mcGhosts = mcPlayers.Where(p => JokeShopProcedures.GHOSTS.Any(ghostForm => p.FormSourceId == ghostForm)).ToList();
            foreach (var ghost in mcGhosts)
            {
                var ghostPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == ghost.Id);
                var ghostLoc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == ghostPlayer.dbLocationName);

                if (ghostLoc != null)
                {
                    string nextLoc;

                    string[] haunts = { "mansion", "castle" };
                    if (haunts.Contains(ghostLoc.Region))
                    {
                        nextLoc = LocationsStatics.GetRandomLocation_InRegion(ghostLoc.Region);
                    }
                    else
                    {
                        nextLoc = LocationsStatics.GetRandomLocation_InRegion(haunts[rand.Next(haunts.Count())]);
                    }

                    // Check whether we can haunt the current tile
                    // An enhancement would be to allow ghosts to move through walls
                    var stoppedAt = EnvironmentPrankProcedures.MovePlayer(ghostPlayer, nextLoc, 20);

                    var canHaunt = stoppedAt == nextLoc || ghostPlayer.dbLocationName == nextLoc;

                    if (!canHaunt && stoppedAt != null)
                    {
                        var stoppedAtTile = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == stoppedAt);
                        canHaunt = haunts.Contains(stoppedAtTile.Region);
                    }

                    if (canHaunt)
                    {
                        var here = LocationsStatics.GetConnectionName(nextLoc);

                        var candidates = playerRepo.Players
                            .Where(p => p.dbLocationName == nextLoc &&
                                        p.OnlineActivityTimestamp >= cutoff &&
                                        p.Id != ghostPlayer.Id &&
                                        p.Mobility == PvPStatics.MobilityFull &&
                                        p.InDuel <= 0 &&
                                        p.InQuest <= 0 &&
                                        p.BotId == AIStatics.ActivePlayerBotId)
                            .ToList();

                        if (candidates.Any())
                        {
                            var victim = candidates[rand.Next(candidates.Count())];
                            string[] calls = { "Boo!", "Whooo!" };
                            var call = calls[rand.Next(calls.Count())];

                            PlayerLogProcedures.AddPlayerLog(ghostPlayer.Id, $"After entering {here} you tap {victim.GetFullName()} on the shoulder and shout <b>\"{call}\"</b>!", true);
                            PlayerLogProcedures.AddPlayerLog(victim.Id, $"{ghostPlayer.GetFullName()} taps you on the shoulder and shouts <b>\"{call}\"</b>!", true);
                            LocationLogProcedures.AddLocationLog(nextLoc, $"{ghostPlayer.GetFullName()} shouts <b>\"{call}\"</b> at {victim.GetFullName()}.");
                        }
                        else
                        {
                            string[] activities = { "rattles some chains", "wails like a banshee", "floats through a wall", "lights a candle", "makes the lights flicker", "fades into the background", "knocks a book off a shelf", "melts into a pool of ectoplasm", "sends a a chill through the air" };
                            var activity = activities[rand.Next(activities.Count())];

                            PlayerLogProcedures.AddPlayerLog(ghostPlayer.Id, $"You begin haunting {here}!", true);
                            LocationLogProcedures.AddLocationLog(nextLoc, $"<b>{ghostPlayer.GetFullName()} {activity}</b>.");
                        }
                    }
                    else if (stoppedAt != null)
                    {
                        var here = LocationsStatics.GetConnectionName(stoppedAt);
                        PlayerLogProcedures.AddPlayerLog(ghostPlayer.Id, $"The spirits call you to <b>{here}</b>.", true);
                    }
                }

                mcPlayers = mcPlayers.Where(p => p.Id != ghost.Id);
            }


        }

    }
}