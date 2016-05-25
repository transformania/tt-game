using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Commands.Items;
using TT.Domain.Commands.Players;
using TT.Domain.Concrete;
using TT.Domain.DTOs.Players;
using TT.Domain.Models;
using TT.Domain.Queries.Item;
using TT.Domain.Queries.Players;
using TT.Domain.Statics;
using TT.Domain.Utilities;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures.BossProcedures
{
    public class BossProcedures_Lindella
    {

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
                merchant.Form = "form_Soul_Item_Vendor_Judoo";
                merchant.Level = 5;

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
                        PlayerProcedures.Cleanse(merchant, box);
                        PlayerProcedures.Cleanse(merchant, box);
                        PlayerProcedures.Meditate(merchant, box);
                    }
                }
                else
                {
                    if (merchant.Mana < merchant.MaxMana)
                    {
                        PlayerProcedures.Meditate(merchant, box);
                        PlayerProcedures.Meditate(merchant, box);
                        PlayerProcedures.Cleanse(merchant, box);
                    }
                }

                #region restock inventory
                if (turnNumber % 16 == 1)
                {

                    Player lorekeeper = PlayerProcedures.GetPlayerFromBotId(AIStatics.LoremasterBotId);

                    IItemRepository itemRepo = new EFItemRepository();

                    var lindellaItemscmd = new GetItemsOwnedByPlayer() { OwnerId = merchant.Id };
                    var lindellasItems = DomainRegistry.Repository.Find(lindellaItemscmd).Where(i => i.Level > 0);

                    var lorekeeperItemscmd = new GetItemsOwnedByPlayer() { OwnerId = lorekeeper.Id };
                    var lorekeeperItems = DomainRegistry.Repository.Find(lorekeeperItemscmd).Where(i => i.Level > 0);

                    var restockItems = XmlResourceLoader.Load<List<RestockListItem>>("TT.Domain.XMLs.RestockList.xml");

                    foreach (RestockListItem item in restockItems)
                    {

                        if (item.Merchant == "Lindella")
                        {
                            int currentCount = lindellasItems.Count(i => i.dbName == item.dbName);
                            if (currentCount < item.AmountBeforeRestock)
                            {
                                for (int x = 0; x < item.AmountToRestockTo - currentCount; x++)
                                {

                                    var cmd = new CreateItem
                                    {
                                        dbName = item.dbName,
                                        dbLocationName = "",
                                        OwnerId = merchant.Id,
                                        IsEquipped = false,
                                        IsPermanent = true,
                                        Level = 0,
                                        PvPEnabled = -1,
                                        TurnsUntilUse = 0,
                                        VictimName = "",
                                        EquippedThisTurn = false,
                                        ItemSourceId = ItemStatics.GetStaticItem(item.dbName).Id
                                    };
                                    DomainRegistry.Repository.Execute(cmd);
                                }

                            }
                        }

                        else if (item.Merchant == "Lorekeeper")
                        {
                            int currentCount = lorekeeperItems.Count(i => i.dbName == item.dbName);
                            if (currentCount < item.AmountBeforeRestock)
                            {
                                for (int x = 0; x < item.AmountToRestockTo - currentCount; x++)
                                {
                                    var cmd = new CreateItem
                                    {
                                        dbName = item.dbName,
                                        dbLocationName = "",
                                        OwnerId = lorekeeper.Id,
                                        IsEquipped = false,
                                        IsPermanent = true,
                                        Level = 0,
                                        PvPEnabled = -1,
                                        TurnsUntilUse = 0,
                                        VictimName = "",
                                        EquippedThisTurn = false,
                                        ItemSourceId = ItemStatics.GetStaticItem(item.dbName).Id
                                    };
                                    DomainRegistry.Repository.Execute(cmd);
                                }

                            }
                        }


                    }
                }
                #endregion

            }

        }

    }
}
