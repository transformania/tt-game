using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.AI;
using TT.Domain.Commands.Players;
using TT.Domain.Concrete;
using TT.Domain.DTOs.Players;
using TT.Domain.Models;
using TT.Domain.Queries.Players;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures.BossProcedures
{
    public class BossProcedures_Lindella
    {

        private const int LindellaFormId = 400;

        public static void SpawnLindella()
        {
            PlayerDetail merchant = DomainRegistry.Repository.FindSingle(new GetPlayerByBotId { BotId = AIStatics.LindellaBotId });

            if (merchant == null)
            {
                var cmd = new CreatePlayer
                {
                    BotId = AIStatics.LindellaBotId,
                    Level = 5,
                    FirstName = "Lindella",
                    LastName = "the Soul Vendor",
                    Health = 5000,
                    Mana = 5000,
                    MaxHealth = 500,
                    MaxMana = 500,
                    Mobility = PvPStatics.MobilityFull,
                    Money = 1000,
                    Form = "form_Soul_Item_Vendor_Judoo",
                    FormSourceId = LindellaFormId,
                    Location = "270_west_9th_ave", // Lindella starts her rounds here
                    Gender = PvPStatics.GenderFemale,
                };
                var id = DomainRegistry.Repository.Execute(cmd);


                AIDirectiveProcedures.GetAIDirective(id);
                AIDirectiveProcedures.SetAIDirective_MoveTo(id, "street_15th_south");
            }
        }

        public static void RunActions(int turnNumber)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player merchant = playerRepo.Players.FirstOrDefault(f => f.FirstName == "Lindella" && f.LastName == "the Soul Vendor" && f.Mobility == PvPStatics.MobilityFull);

            if (merchant != null && merchant.Mobility == PvPStatics.MobilityFull)
            {

                AIDirective directive = AIDirectiveProcedures.GetAIDirective(merchant.Id);

                if (directive.TargetLocation != merchant.dbLocationName)
                {
                    string newplace = AIProcedures.MoveTo(merchant, directive.TargetLocation, 5);
                    merchant.dbLocationName = newplace;
                }

                // if the merchant has arrived, set a new target for next time.
                if (directive.TargetLocation == merchant.dbLocationName)
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
                BuffBox box = ItemProcedures.GetPlayerBuffs(merchant);

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
            }
        }
    }
}
