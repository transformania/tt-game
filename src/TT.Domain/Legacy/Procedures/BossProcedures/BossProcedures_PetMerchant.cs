using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;
using TT.Domain.Items.Queries;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_PetMerchant
    {

        private const int WuffieFormId = 286;

        public static void SpawnPetMerchant()
        {
            var petMerchant = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.WuffieBotId });

            if (petMerchant == null)
            {

                var cmd = new CreatePlayer
                {
                    BotId = AIStatics.WuffieBotId,
                    Level = 5,
                    FirstName = "Wüffie",
                    LastName = "the Soul Pet Vendor",
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    Mobility = PvPStatics.MobilityFull,
                    Money = 1000,
                    FormSourceId = WuffieFormId,
                    Location = "270_west_9th_ave", // Lindella starts her rounds here
                    Gender = PvPStatics.GenderFemale,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                var petMerchantEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                petMerchantEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(petMerchantEF));
                playerRepo.SavePlayer(petMerchantEF);
            }
        }

        public static void RunPetMerchantActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var petMerchant = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.WuffieBotId);


            if (petMerchant.Mobility == PvPStatics.MobilityFull)
            {
                if (petMerchant.Health < petMerchant.MaxHealth || petMerchant.Mana < petMerchant.MaxMana)
                {
                    var buffs = ItemProcedures.GetPlayerBuffs(petMerchant);
                    if (petMerchant.Health < petMerchant.MaxHealth)
                    {
                        petMerchant.Health += 200;
                        var logmessage = "<span class='playerCleansingNotification'>" + petMerchant.GetFullName() + " cleansed here.</span>";
                        LocationLogProcedures.AddLocationLog(petMerchant.dbLocationName, logmessage);
                    }
                    if (petMerchant.Mana < petMerchant.MaxMana)
                    {
                        petMerchant.Mana += 200;
                        var logmessage = "<span class='playerMediatingNotification'>" + petMerchant.GetFullName() + " meditated here.</span>";
                        LocationLogProcedures.AddLocationLog(petMerchant.dbLocationName, logmessage);
                    }

                    petMerchant.NormalizeHealthMana();

                }

                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();

                var turnMod = turnNumber % (24 * 4);
                var regionIndex = turnMod / 24;

                var newLocation = "";
                switch (regionIndex)
                {
                    case 0:
                        newLocation = LocationsStatics.GetRandomLocation_InRegion("ranch_outside");
                        break;
                    case 1:
                        newLocation = LocationsStatics.GetRandomLocation_InRegion("forest");
                        break;
                    case 2:
                        newLocation = LocationsStatics.GetRandomLocation_InRegion("campground");
                        break;
                    default:
                        newLocation = LocationsStatics.GetRandomLocation_InRegion("park");
                        break;
                }

                var actualNewLocation = AIProcedures.MoveTo(petMerchant, newLocation, 6);
                petMerchant.dbLocationName = actualNewLocation;
                playerRepo.SavePlayer(petMerchant);

                if (turnNumber % 11 == 5)
                {
                    DomainRegistry.Repository.Execute(new MoveAbandonedPetsToWuffie {WuffieId = petMerchant.Id });
                }

                if (turnNumber % 12 == 0)
                {
                    // Get a list of all pets held that are former players.
                    var petItems = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = petMerchant.Id }).Where(i => i.FormerPlayer != null).Where(i => i.ItemSource.ItemType == PvPStatics.ItemType_Pet).ToList();

                    var rand = new Random();
                    int chance = 0;

                    foreach (var p in petItems)
                    {
                        var player = PlayerProcedures.GetPlayer(p.FormerPlayer.Id);

                        if (!player.MembershipId.IsNullOrEmpty())
                        {
                            // Roll the dice.
                            chance = rand.Next(0, 100);

                            // About 1/4 of a chance every 12 turns to be interacted with.
                            if (chance <= 24)
                            {
                                // Depending on the roll determines the interaction.
                                if (chance <= 8) // Praise
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Wüffie briefly praises you, confident she can find you a good home!", true);
                                }
                                else if (chance <= 16) // Scold
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Wüffie sighs as she scolds you for misbehaving.", true);
                                }
                                else // Restrain
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Wüffie restrains you, making sure you can't run off.", true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}