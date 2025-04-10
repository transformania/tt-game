using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.AI.Commands;
using TT.Domain.Concrete;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;
using TT.Domain.Statics;
using TT.Domain.Items.Queries;
using System;

namespace TT.Domain.Procedures.BossProcedures
{
    public class BossProcedures_Lindella
    {

        private const int LindellaFormId = 400;

        public static void SpawnLindella()
        {
            var merchant = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.LindellaBotId });

            if (merchant == null)
            {
                var cmd = new CreatePlayer
                {
                    BotId = AIStatics.LindellaBotId,
                    Level = 5,
                    FirstName = "Lindella",
                    LastName = "the Soul Vendor",
                    Health = 100000,
                    Mana = 100000,
                    MaxHealth = 100000,
                    MaxMana = 100000,
                    Mobility = PvPStatics.MobilityFull,
                    Money = 1000,
                    FormSourceId = LindellaFormId,
                    Location = "270_west_9th_ave", // Lindella starts her rounds here
                    Gender = PvPStatics.GenderFemale,
                };
                var id = DomainRegistry.Repository.Execute(cmd);

                IPlayerRepository playerRepo = new EFPlayerRepository();
                var newMerchant = playerRepo.Players.FirstOrDefault(p => p.Id == id);
                newMerchant.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(newMerchant));
                playerRepo.SavePlayer(newMerchant);

                AIDirectiveProcedures.GetAIDirective(id);
                AIDirectiveProcedures.SetAIDirective_MoveTo(id, "street_15th_south");
            }
        }

        public static void RunActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var merchant = playerRepo.Players.FirstOrDefault(f => f.BotId == AIStatics.LindellaBotId && f.Mobility == PvPStatics.MobilityFull);

            if (merchant != null && merchant.Mobility == PvPStatics.MobilityFull)
            {

                var directive = AIDirectiveProcedures.GetAIDirective(merchant.Id);

                if(directive.TargetLocation.IsEmpty())
                {
                    AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "270_west_9th_ave");
                }

                if (directive.TargetLocation != merchant.dbLocationName)
                {
                    var newplace = AIProcedures.MoveTo(merchant, directive.TargetLocation, 6);
                    merchant.dbLocationName = newplace;
                }

                // if the merchant has arrived, set a new target for next time.
                // Does this count as turning Lindella into a snail?
                if (directive.TargetLocation == merchant.dbLocationName && turnNumber%2 == 0)
                {
                    if (merchant.dbLocationName == "270_west_9th_ave")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_15th_south");
                    }
                    else if (merchant.dbLocationName == "street_15th_south")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_220_sunnyglade_drive");
                    }
                    else if (merchant.dbLocationName == "street_220_sunnyglade_drive")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_70e9th");
                    }
                    else if (merchant.dbLocationName == "street_70e9th")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "street_130_main");
                    }
                    else if (merchant.dbLocationName == "street_130_main")
                    {
                        AIDirectiveProcedures.SetAIDirective_MoveTo(merchant.Id, "270_west_9th_ave");
                    }
                }

                playerRepo.SavePlayer(merchant);
                var box = ItemProcedures.GetPlayerBuffs(merchant);

                if ((merchant.Health / merchant.MaxHealth) < .75M)
                {
                    if (merchant.Health < merchant.MaxHealth)
                    {
                        DomainRegistry.Repository.Execute(new Cleanse { PlayerId = merchant.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Cleanse { PlayerId = merchant.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = merchant.Id, Buffs = box, NoValidate = true });
                    }
                }
                else
                {
                    if (merchant.Mana < merchant.MaxMana)
                    {
                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = merchant.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Meditate { PlayerId = merchant.Id, Buffs = box, NoValidate = true });
                        DomainRegistry.Repository.Execute(new Cleanse { PlayerId = merchant.Id, Buffs = box, NoValidate = true });
                    }
                }

                if (turnNumber % 16 == 1)
                {
                    DomainRegistry.Repository.Execute(new RestockNPC { BotId = AIStatics.LindellaBotId });
                    DomainRegistry.Repository.Execute(new RestockNPC { BotId = AIStatics.LoremasterBotId });
                }

                if (turnNumber % 12 == 0)
                {
                    // Get a list of all items held that are former players.
                    var lindellaItems = DomainRegistry.Repository.Find(new GetItemsOwnedByPlayer { OwnerId = merchant.Id }).Where(i => i.FormerPlayer != null).ToList();

                    var rand = new Random();
                    int chance = 0;

                    foreach (var p in lindellaItems)
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
                                if (chance <= 8) // Flaunt
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Lindella briefly flaunts you, confident she can sell you soon!", true);
                                }
                                else if (chance <= 16) // Shun
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Lindella sighs and seems to shun you, disappointed she might never be able to sell you.", true);
                                }
                                else // Hush
                                {
                                    PlayerLogProcedures.AddPlayerLog(player.Id, "Lindella hushes you, seemingly tired of still carrying you.", true);
                                }
                            }
                        }                      
                    }
                }
            }
        }
    }
}
