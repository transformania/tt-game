using System;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Items.Commands;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Players.DTOs;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures.BossProcedures
{
    public static class BossProcedures_PetMerchant
    {

        private const int WuffieFormId = 286;

        public static void SpawnPetMerchant()
        {
            PlayerDetail petMerchant = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.WuffieBotId });

            if (petMerchant == null)
            {

                var cmd = new CreatePlayer
                {
                    BotId = AIStatics.WuffieBotId,
                    Level = 5,
                    FirstName = "Wüffie",
                    LastName = "the Soul Pet Vendor",
                    Health = 5000,
                    Mana = 5000,
                    MaxHealth = 500,
                    MaxMana = 500,
                    Mobility = PvPStatics.MobilityFull,
                    Money = 1000,
                    Form = "form_Soul_Pet_Vendor_Judoo",
                    FormSourceId = WuffieFormId,
                    Location = "270_west_9th_ave", // Lindella starts her rounds here
                    Gender = PvPStatics.GenderFemale,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                var playerRepo = new EFPlayerRepository();
                Player petMerchantEF = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                petMerchantEF.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(petMerchantEF));
                playerRepo.SavePlayer(petMerchantEF);
            }
        }

        public static void CounterAttack(Player attacker)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player petMerchant = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.WuffieBotId);

            if (petMerchant.Mobility == PvPStatics.MobilityFull && attacker.Mobility == PvPStatics.MobilityFull)
            {
                AttackProcedures.Attack(petMerchant, attacker, "skill_Sarmoti_Zatur");
            }

        }

        public static void RunPetMerchantActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player petMerchant = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.WuffieBotId);


            if (petMerchant.Mobility == PvPStatics.MobilityFull)
            {
                if (petMerchant.Health < petMerchant.MaxHealth || petMerchant.Mana < petMerchant.MaxMana)
                {
                    BuffBox buffs = ItemProcedures.GetPlayerBuffs(petMerchant);
                    if (petMerchant.Health < petMerchant.MaxHealth)
                    {
                        petMerchant.Health += 200;
                        string logmessage = "<span class='playerCleansingNotification'>" + petMerchant.GetFullName() + " cleansed here.</span>";
                        LocationLogProcedures.AddLocationLog(petMerchant.dbLocationName, logmessage);
                    }
                    if (petMerchant.Mana < petMerchant.MaxMana)
                    {
                        petMerchant.Mana += 200;
                        string logmessage = "<span class='playerMediatingNotification'>" + petMerchant.GetFullName() + " meditated here.</span>";
                        LocationLogProcedures.AddLocationLog(petMerchant.dbLocationName, logmessage);
                    }

                    petMerchant.NormalizeHealthMana();

                }

                IAIDirectiveRepository aiRepo = new EFAIDirectiveRepository();

                int turnMod = turnNumber % (24 * 4);

                string newLocation = "";
                if (turnMod < 24 * 1)
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("ranch_outside");
                }
                else if (turnMod > 24 * 1 && turnMod <= 24 * 2)
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("forest");
                }
                else if (turnMod > 24 * 2 && turnMod <= 24 * 3)
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("campground");
                }
                else
                {
                    newLocation = LocationsStatics.GetRandomLocation_InRegion("campground");
                }

                string actualNewLocation = AIProcedures.MoveTo(petMerchant, newLocation, 5);
                petMerchant.dbLocationName = actualNewLocation;
                playerRepo.SavePlayer(petMerchant);

                if (turnNumber % 11 == 5)
                {
                    DomainRegistry.Repository.Execute(new MoveAbandonedPetsToWuffie {WuffieId = petMerchant.Id });
                }
            }
        }
    }
}