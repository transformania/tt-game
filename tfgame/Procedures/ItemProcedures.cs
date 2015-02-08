using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;
using WebMatrix.WebData;

namespace tfgame.Procedures
{
    public class ItemProcedures
    {
     
        public static IEnumerable<ItemViewModel> GetAllPlayerItems(int playerId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> output = from i in itemRepo.Items
                                                      where i.OwnerId == playerId
                                                      join si in itemRepo.DbStaticItems on i.dbName equals si.dbName
                                                      select new ItemViewModel
                                                      {
                                                          dbItem = new Item_VM
                                                          {
                                                             Id = i.Id,
                                                             dbName = i.dbName,
                                                             dbLocationName = i.dbLocationName,
                                                             EquippedThisTurn = i.EquippedThisTurn,
                                                             IsEquipped = i.IsEquipped,
                                                             IsPermanent = i.IsPermanent,
                                                             Level = i.Level,
                                                             OwnerId = i.OwnerId,
                                                             PvPEnabled = i.PvPEnabled,
                                                             TimeDropped = i.TimeDropped,
                                                             TurnsUntilUse = i.TurnsUntilUse,
                                                             VictimName = i.VictimName
                                                             
                                                          },



                                                          Item = new tfgame.ViewModels.StaticItem
                                                          {
                                                            dbName = si.dbName,
                                                            FriendlyName = si.FriendlyName,
                                                            Description = si.Description,
                                                            PortraitUrl = si.PortraitUrl,
                                                            MoneyValue = si.MoneyValue,
                                                            MoneyValueSell = si.MoneyValueSell,
                                                            ItemType = si.ItemType,
                                                            UseCooldown = si.UseCooldown,
                                                            Findable = si.Findable,
                                                            FindWeight = si.FindWeight,
                                                            GivesEffect = si.GivesEffect,

                                                            HealthBonusPercent = si.HealthBonusPercent,
                                                            ManaBonusPercent = si.ManaBonusPercent,
                                                            ExtraSkillCriticalPercent = si.ExtraSkillCriticalPercent,
                                                            HealthRecoveryPerUpdate = si.HealthRecoveryPerUpdate,
                                                            ManaRecoveryPerUpdate = si.ManaRecoveryPerUpdate,
                                                            SneakPercent = si.SneakPercent,
                                                            EvasionPercent = si.EvasionPercent,
                                                            EvasionNegationPercent = si.EvasionNegationPercent,
                                                            MeditationExtraMana = si.MeditationExtraMana,
                                                            CleanseExtraHealth = si.CleanseExtraHealth,
                                                            MoveActionPointDiscount = si.MoveActionPointDiscount,
                                                            SpellExtraHealthDamagePercent = si.SpellExtraHealthDamagePercent,
                                                            SpellExtraTFEnergyPercent = si.SpellExtraTFEnergyPercent,
                                                            CleanseExtraTFEnergyRemovalPercent = si.CleanseExtraTFEnergyRemovalPercent,
                                                            SpellMisfireChanceReduction = si.SpellMisfireChanceReduction,
                                                            SpellHealthDamageResistance = si.SpellHealthDamageResistance,
                                                            SpellTFEnergyDamageResistance = si.SpellTFEnergyDamageResistance,
                                                            ExtraInventorySpace = si.ExtraInventorySpace,


                                                            InstantHealthRestore = si.InstantHealthRestore,
                                                            InstantManaRestore = si.InstantManaRestore,
                                                            ReuseableHealthRestore = si.ReuseableHealthRestore,
                                                            ReuseableManaRestore = si.ReuseableManaRestore,


                                                          }

                                                      };

            return output;
        }

        public static IEnumerable<Item> GetAllPlayerItems_ItemOnly(int playerId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.Items.Where(i => i.OwnerId == playerId && i.IsEquipped == true);
        }

        public static IEnumerable<ItemViewModel> GetAllItemsAtLocation(string dbLocationName, Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> items  = from i in itemRepo.Items
                                                 where i.dbLocationName == dbLocationName
                                                 join si in itemRepo.DbStaticItems on i.dbName equals si.dbName
                                                 select new ItemViewModel
                                                 {
                                                     dbItem = new Item_VM
                                                     {
                                                         Id = i.Id,
                                                         dbName = i.dbName,
                                                         dbLocationName = i.dbLocationName,
                                                         EquippedThisTurn = i.EquippedThisTurn,
                                                         IsEquipped = i.IsEquipped,
                                                         IsPermanent = i.IsPermanent,
                                                         Level = i.Level,
                                                         OwnerId = i.OwnerId,
                                                         PvPEnabled = i.PvPEnabled,
                                                         TimeDropped = i.TimeDropped,
                                                         TurnsUntilUse = i.TurnsUntilUse,
                                                         VictimName = i.VictimName

                                                     },



                                                     Item = new tfgame.ViewModels.StaticItem
                                                     {
                                                         dbName = si.dbName,
                                                         FriendlyName = si.FriendlyName,
                                                         Description = si.Description,
                                                         PortraitUrl = si.PortraitUrl,
                                                         MoneyValue = si.MoneyValue,
                                                         MoneyValueSell = si.MoneyValueSell,
                                                         ItemType = si.ItemType,
                                                         UseCooldown = si.UseCooldown,
                                                         Findable = si.Findable,
                                                         FindWeight = si.FindWeight,
                                                         GivesEffect = si.GivesEffect,

                                                         HealthBonusPercent = si.HealthBonusPercent,
                                                         ManaBonusPercent = si.ManaBonusPercent,
                                                         ExtraSkillCriticalPercent = si.ExtraSkillCriticalPercent,
                                                         HealthRecoveryPerUpdate = si.HealthRecoveryPerUpdate,
                                                         ManaRecoveryPerUpdate = si.ManaRecoveryPerUpdate,
                                                         SneakPercent = si.SneakPercent,
                                                         EvasionPercent = si.EvasionPercent,
                                                         EvasionNegationPercent = si.EvasionNegationPercent,
                                                         MeditationExtraMana = si.MeditationExtraMana,
                                                         CleanseExtraHealth = si.CleanseExtraHealth,
                                                         MoveActionPointDiscount = si.MoveActionPointDiscount,
                                                         SpellExtraHealthDamagePercent = si.SpellExtraHealthDamagePercent,
                                                         SpellExtraTFEnergyPercent = si.SpellExtraTFEnergyPercent,
                                                         CleanseExtraTFEnergyRemovalPercent = si.CleanseExtraTFEnergyRemovalPercent,
                                                         SpellMisfireChanceReduction = si.SpellMisfireChanceReduction,
                                                         SpellHealthDamageResistance = si.SpellHealthDamageResistance,
                                                         SpellTFEnergyDamageResistance = si.SpellTFEnergyDamageResistance,
                                                         ExtraInventorySpace = si.ExtraInventorySpace,


                                                         InstantHealthRestore = si.InstantHealthRestore,
                                                         InstantManaRestore = si.InstantManaRestore,
                                                         ReuseableHealthRestore = si.ReuseableHealthRestore,
                                                         ReuseableManaRestore = si.ReuseableManaRestore,


                                                     }

                                                 };

            List<ItemViewModel> output = new List<ItemViewModel>();

            foreach (ItemViewModel m in items)
            {
                //  if the item is a consumeable and not in the same PvP-nonPvP mode, do not add it
                if (m.Item.ItemType == "consumable" && ((m.dbItem.PvPEnabled == true && player.GameMode < 2) || (m.dbItem.PvPEnabled == false && player.GameMode == 2)))
                {
                    // do nothing
                }
                else
                {
                    output.Add(m);
                }
            }

            return output;
        }

        public static ItemViewModel GetItemViewModel(int id)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> output = from i in itemRepo.Items
                                                 where i.Id == id
                                                 join si in itemRepo.DbStaticItems on i.dbName equals si.dbName
                                                 select new ItemViewModel
                                                 {
                                                     dbItem = new Item_VM
                                                     {
                                                         Id = i.Id,
                                                         dbName = i.dbName,
                                                         dbLocationName = i.dbLocationName,
                                                         EquippedThisTurn = i.EquippedThisTurn,
                                                         IsEquipped = i.IsEquipped,
                                                         IsPermanent = i.IsPermanent,
                                                         Level = i.Level,
                                                         OwnerId = i.OwnerId,
                                                         PvPEnabled = i.PvPEnabled,
                                                         TimeDropped = i.TimeDropped,
                                                         TurnsUntilUse = i.TurnsUntilUse,
                                                         VictimName = i.VictimName

                                                     },



                                                     Item = new tfgame.ViewModels.StaticItem
                                                     {
                                                         dbName = si.dbName,
                                                         FriendlyName = si.FriendlyName,
                                                         Description = si.Description,
                                                         PortraitUrl = si.PortraitUrl,
                                                         MoneyValue = si.MoneyValue,
                                                         ItemType = si.ItemType,
                                                         UseCooldown = si.UseCooldown,
                                                         Findable = si.Findable,
                                                         FindWeight = si.FindWeight,
                                                         GivesEffect = si.GivesEffect,

                                                         HealthBonusPercent = si.HealthBonusPercent,
                                                         ManaBonusPercent = si.ManaBonusPercent,
                                                         ExtraSkillCriticalPercent = si.ExtraSkillCriticalPercent,
                                                         HealthRecoveryPerUpdate = si.HealthRecoveryPerUpdate,
                                                         ManaRecoveryPerUpdate = si.ManaRecoveryPerUpdate,
                                                         SneakPercent = si.SneakPercent,
                                                         EvasionPercent = si.EvasionPercent,
                                                         EvasionNegationPercent = si.EvasionNegationPercent,
                                                         MeditationExtraMana = si.MeditationExtraMana,
                                                         CleanseExtraHealth = si.CleanseExtraHealth,
                                                         MoveActionPointDiscount = si.MoveActionPointDiscount,
                                                         SpellExtraHealthDamagePercent = si.SpellExtraHealthDamagePercent,
                                                         SpellExtraTFEnergyPercent = si.SpellExtraTFEnergyPercent,
                                                         CleanseExtraTFEnergyRemovalPercent = si.CleanseExtraTFEnergyRemovalPercent,
                                                         SpellMisfireChanceReduction = si.SpellMisfireChanceReduction,
                                                         SpellHealthDamageResistance = si.SpellHealthDamageResistance,
                                                         SpellTFEnergyDamageResistance = si.SpellTFEnergyDamageResistance,
                                                         ExtraInventorySpace = si.ExtraInventorySpace,


                                                         InstantHealthRestore = si.InstantHealthRestore,
                                                         InstantManaRestore = si.InstantManaRestore,
                                                         ReuseableHealthRestore = si.ReuseableHealthRestore,
                                                         ReuseableManaRestore = si.ReuseableManaRestore,


                                                     }

                                                 };

            return output.First();
        }

        public static bool PlayerIsCarryingTooMuch(int newOwnerId, int offset, BuffBox buffs)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> currentlyOwnedItems = GetAllPlayerItems(newOwnerId);
            int nonWornItemsCarried = currentlyOwnedItems.Where(i => i.dbItem.IsEquipped == false ).Count() - offset;

            int max = GetInventoryMaxSize(buffs);

            if (nonWornItemsCarried >= max)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string GiveItemToPlayer(int itemId, int newOwnerId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == itemId);
            DbStaticItem itemPlus = ItemStatics.GetStaticItem(item.dbName);
            //LogBox log = new LogBox();


            if (item.dbLocationName != "")
            {
                item.dbLocationName = "";
                item.OwnerId = newOwnerId;
                item.IsEquipped = false;
                itemRepo.SaveItem(item);

                // if item is not an animal
                if (itemPlus.ItemType != PvPStatics.ItemType_Pet) { 
                    return "You picked up a " + itemPlus.FriendlyName + " and put it into your inventory.";
                }

                // item is an animal
                else
                {
                    return "You tame " + item.VictimName + " the " + itemPlus.FriendlyName + " and are now keeping them as a pet.";
                }

            }

            return "";
        }

        public static void GiveItemToPlayer_Nocheck(int itemId, int newOwnerId) {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == itemId);
            DbStaticItem itemPlus = ItemStatics.GetStaticItem(item.dbName);
            item.OwnerId = newOwnerId;

            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                item.IsEquipped = true;
            } else {
                item.IsEquipped = false;
                item.dbLocationName = "";
            }
           
            item.TimeDropped = DateTime.UtcNow;
            itemRepo.SaveItem(item);
        }

        public static string GiveNewItemToPlayer(Player player, DbStaticItem item)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item newitem = new Item
            {
                OwnerId = player.Id,
                dbName = item.dbName,
                IsEquipped = false,
                VictimName = "",
                dbLocationName = "",
                TimeDropped = DateTime.UtcNow,
               // PvPEnabled = player.InPvP,
            };

            if (player.GameMode == 2)
            {
                newitem.PvPEnabled = true;
            }
            else
            {
                newitem.PvPEnabled = false;
            }

            itemRepo.SaveItem(newitem);
            return "You found a " + item.FriendlyName + "!";
        }

        public static string GiveNewItemToPlayer(Player player, string itemName)
        {
            DbStaticItem i = ItemStatics.GetStaticItem(itemName);
            return GiveNewItemToPlayer(player, i);
        }

        public static string DropItem(int itemId, string locationDbName)
        {

         
           // EquipItem(itemId, false);

            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == itemId);

            int oldOwnerId = item.OwnerId;

            DbStaticItem itemPlus = ItemStatics.GetStaticItem(item.dbName);

            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                EquipItem(itemId, false);
            }

            item.OwnerId = -1;
            item.dbLocationName = locationDbName;
            item.IsEquipped = false;
            item.TimeDropped = DateTime.UtcNow;
            itemRepo.SaveItem(item);

            SkillProcedures.UpdateItemSpecificSkillsToPlayer(oldOwnerId);

            // item is an animal
            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                AnimalProcedures.ChangeOwner(item, -1);
                return "You released " + item.VictimName + " the " + itemPlus.FriendlyName + " that you were keeping as a pet.";
            }

            // item is a regular item
            else
            {
                return "You dropped a " + itemPlus.FriendlyName + " that you were carrying.";
            }
            
        }

        public static string EquipItem(int itemId, bool putOn)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == itemId);
            DbStaticItem itemPlus = ItemStatics.GetStaticItem(item.dbName);

            IPlayerRepository playerRepo = new EFPlayerRepository();

            Player dbOwner = playerRepo.Players.FirstOrDefault(p => p.Id == item.OwnerId);
            

            if (putOn == true)
            {
                item.IsEquipped = true;
                item.EquippedThisTurn = true;
                itemRepo.SaveItem(item);

                BuffBox targetbuffs = ItemProcedures.GetPlayerBuffs(dbOwner);

                dbOwner = PlayerProcedures.ReadjustMaxes(dbOwner, targetbuffs);
                playerRepo.SavePlayer(dbOwner);

                SkillProcedures.UpdateItemSpecificSkillsToPlayer(dbOwner.Id, item.dbName);

                return "You put on your " + itemPlus.FriendlyName + ".";
         
            }
            else
            {
                item.IsEquipped = false;
                itemRepo.SaveItem(item);

                BuffBox targetbuffs = ItemProcedures.GetPlayerBuffs(dbOwner);

                dbOwner = PlayerProcedures.ReadjustMaxes(dbOwner, targetbuffs);
                playerRepo.SavePlayer(dbOwner);

                SkillProcedures.UpdateItemSpecificSkillsToPlayer(dbOwner.Id);

                return "You took off your " + itemPlus.FriendlyName + ".";
              
            }

        }

        public static void ResetUseCooldown(ItemViewModel input)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item dbItem = itemRepo.Items.FirstOrDefault(i => i.Id == input.dbItem.Id);
            dbItem.TurnsUntilUse = input.Item.UseCooldown;

            // these special reusable consumables can have a lower cooldown at higher levels
            if (dbItem.dbName == "item_Butt_Plug_Hanna" || dbItem.dbName == "item_Hot_Coffee_Mug_Alex_Wise")
            {
                dbItem.TurnsUntilUse -= dbItem.Level;
                if (dbItem.TurnsUntilUse < 0)
                {
                    dbItem.TurnsUntilUse = 0;
                }
            }

            itemRepo.SaveItem(dbItem);
        }

        public static int PlayerIsWearingNumberOfThisType(int playerId, string itemType)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> itemsOfThisType = GetAllPlayerItems(playerId).Where(i => i.Item.ItemType == itemType && i.dbItem.IsEquipped == true);
            return itemsOfThisType.Count();
        }

        public static int PlayerIsWearingNumberOfThisExactItem(int playerId, string itemDbName)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> itemsOfThisType = GetAllPlayerItems(playerId).Where(i => i.Item.dbName == itemDbName && i.dbItem.IsEquipped == true);
            return itemsOfThisType.Count();
        }

        public static BuffBox GetPlayerBuffs(Player player)
        {

            BuffBox output = new BuffBox();

            // grab all of the bonuses coming from worn equipment
            IEnumerable<ItemViewModel> wornItems = GetAllPlayerItems(player.Id).Where(i => i.dbItem.IsEquipped == true);

            // grab all of the bonuses coming from effects
            IEnumerable<EffectViewModel2> myEffects = EffectProcedures.GetPlayerEffects2(player.Id).Where(e => e.dbEffect.Duration > 0);

            // formula:  bonus = amount * (itemlevel - 1) * PvPStatics.Item_LevelBonusModifier

            DbStaticForm myform = FormStatics.GetForm(player.Form);

            output.FromItems_HealthBonusPercent = wornItems.Sum(x => x.Item.HealthBonusPercent + x.Item.HealthBonusPercent * ( (x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_ManaBonusPercent = wornItems.Sum(x => x.Item.ManaBonusPercent + x.Item.ManaBonusPercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_MeditationExtraMana = wornItems.Sum(x => x.Item.MeditationExtraMana + x.Item.MeditationExtraMana * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_CleanseExtraHealth = wornItems.Sum(x => x.Item.CleanseExtraHealth + x.Item.CleanseExtraHealth * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_ExtraSkillCriticalPercent = wornItems.Sum(x => x.Item.ExtraSkillCriticalPercent + x.Item.ExtraSkillCriticalPercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_HealthRecoveryPerUpdate = wornItems.Sum(x => x.Item.HealthRecoveryPerUpdate + x.Item.HealthRecoveryPerUpdate * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_ManaRecoveryPerUpdate = wornItems.Sum(x => x.Item.ManaRecoveryPerUpdate + x.Item.ManaRecoveryPerUpdate * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_SneakPercent = wornItems.Sum(x => x.Item.SneakPercent + x.Item.SneakPercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_EvasionPercent = wornItems.Sum(x => x.Item.EvasionPercent + x.Item.EvasionPercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_EvasionNegationPercent = wornItems.Sum(x => x.Item.EvasionNegationPercent + x.Item.EvasionNegationPercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));


          //  output.FromItems_SpellResistancePercent = wornItems.Sum(x => x.Item.SpellResistancePercent + x.Item.SpellResistancePercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_MoveActionPointDiscount = wornItems.Sum(x => x.Item.MoveActionPointDiscount + x.Item.MoveActionPointDiscount * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_SpellExtraTFEnergyPercent = wornItems.Sum(x => x.Item.SpellExtraTFEnergyPercent + x.Item.SpellExtraTFEnergyPercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_SpellExtraHealthDamagePercent = wornItems.Sum(x => x.Item.SpellExtraHealthDamagePercent + x.Item.SpellExtraHealthDamagePercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_CleanseExtraTFEnergyRemovalPercent = wornItems.Sum(x => x.Item.CleanseExtraTFEnergyRemovalPercent + x.Item.CleanseExtraTFEnergyRemovalPercent * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_SpellMisfireChanceReduction = wornItems.Sum(x => x.Item.SpellMisfireChanceReduction + x.Item.SpellMisfireChanceReduction * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_SpellHealthDamageResistance = wornItems.Sum(x => x.Item.SpellHealthDamageResistance + x.Item.SpellHealthDamageResistance * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_SpellTFEnergyDamageResistance = wornItems.Sum(x => x.Item.SpellTFEnergyDamageResistance + x.Item.SpellTFEnergyDamageResistance * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

            output.FromItems_ExtraInventorySpace = wornItems.Sum(x => x.Item.ExtraInventorySpace + x.Item.ExtraInventorySpace * ((x.dbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier));

        

            output.FromForm_HealthBonusPercent = myform.HealthBonusPercent;
            output.FromForm_ManaBonusPercent = myform.ManaBonusPercent;
            output.FromForm_MeditationExtraMana = myform.MeditationExtraMana;
            output.FromForm_CleanseExtraHealth = myform.CleanseExtraHealth;
            output.FromForm_ExtraSkillCriticalPercent = myform.ExtraSkillCriticalPercent;
            output.FromForm_HealthRecoveryPerUpdate = myform.HealthRecoveryPerUpdate;
            output.FromForm_ManaRecoveryPerUpdate = myform.ManaRecoveryPerUpdate;
            output.FromForm_SneakPercent = myform.SneakPercent;
            output.FromForm_EvasionPercent = myform.EvasionPercent;
            output.FromForm_EvasionNegationPercent = myform.EvasionNegationPercent;
            output.FromForm_MoveActionPointDiscount = myform.MoveActionPointDiscount;
            output.FromForm_SpellExtraTFEnergyPercent = myform.SpellExtraTFEnergyPercent;
            output.FromForm_SpellExtraHealthDamagePercent = myform.SpellExtraHealthDamagePercent;
            output.FromForm_CleanseExtraTFEnergyRemovalPercent = myform.CleanseExtraTFEnergyRemovalPercent;
            output.FromForm_SpellMisfireChanceReduction = myform.SpellMisfireChanceReduction;
            output.FromForm_SpellHealthDamageResistance = myform.SpellHealthDamageResistance;
            output.FromForm_SpellTFEnergyDamageResistance = myform.SpellTFEnergyDamageResistance;
            output.FromForm_ExtraInventorySpace = myform.ExtraInventorySpace;

            output.FromEffects_HealthBonusPercent = myEffects.Sum(e => e.Effect.HealthBonusPercent);
            output.FromEffects_ManaBonusPercent = myEffects.Sum(e => e.Effect.ManaBonusPercent);
            output.FromEffects_MeditationExtraMana = myEffects.Sum(e => e.Effect.MeditationExtraMana);
            output.FromEffects_CleanseExtraHealth = myEffects.Sum(e => e.Effect.CleanseExtraHealth);
            output.FromEffects_ExtraSkillCriticalPercent = myEffects.Sum(e => e.Effect.ExtraSkillCriticalPercent);
            output.FromEffects_HealthRecoveryPerUpdate = myEffects.Sum(e => e.Effect.HealthRecoveryPerUpdate);
            output.FromEffects_ManaRecoveryPerUpdate = myEffects.Sum(e => e.Effect.ManaRecoveryPerUpdate);
            output.FromEffects_SneakPercent = myEffects.Sum(e => e.Effect.SneakPercent);
            output.FromEffects_EvasionPercent = myEffects.Sum(e => e.Effect.EvasionPercent);
            output.FromEffects_EvasionNegationPercent = myEffects.Sum(e => e.Effect.EvasionNegationPercent);
            output.FromEffects_MoveActionPointDiscount = myEffects.Sum(e => e.Effect.MoveActionPointDiscount);
            output.FromEffects_SpellExtraTFEnergyPercent = myEffects.Sum(e => e.Effect.SpellExtraTFEnergyPercent);
            output.FromEffects_SpellExtraHealthDamagePercent = myEffects.Sum(e => e.Effect.SpellExtraHealthDamagePercent);
            output.FromEffects_CleanseExtraTFEnergyRemovalPercent = myEffects.Sum(e => e.Effect.CleanseExtraTFEnergyRemovalPercent);
            output.FromEffects_SpellMisfireChanceReduction = myEffects.Sum(e => e.Effect.SpellMisfireChanceReduction);
            output.FromEffects_SpellHealthDamageResistance = myEffects.Sum(e => e.Effect.SpellHealthDamageResistance);
            output.FromEffects_SpellTFEnergyDamageResistance = myEffects.Sum(e => e.Effect.SpellTFEnergyDamageResistance);
            output.FromEffects_ExtraInventorySpace = myEffects.Sum(e => e.Effect.ExtraInventorySpace);

            // non-stat buffs

            output.HasSearchDiscount = false;
            foreach (EffectViewModel2 eff in myEffects)
            {
                if (eff.dbEffect.dbName == "perk_sharp_eye")
                {
                    output.HasSearchDiscount = true;
                    break;
                }
            }

            return output;
        }

        public static BuffBox GetPlayerBuffsRAM(Player player)
        {
            BuffBox output = new BuffBox();

            // form portion
            RAMBuffBox temp_form = null;
            try
            {
                temp_form = FormStatics.FormRAMBuffBoxes.First(x => x.dbName == player.Form.ToLower());
            }
            catch
            {
                PlayerProcedures.LoadFormRAMBuffBox();
                temp_form = FormStatics.FormRAMBuffBoxes.First(x => x.dbName == player.Form.ToLower());
            }

            output.FromForm_HealthBonusPercent = (decimal)temp_form.HealthBonusPercent;
            output.FromForm_ManaBonusPercent = (decimal)temp_form.ManaBonusPercent;
            output.FromForm_HealthRecoveryPerUpdate = (decimal)temp_form.HealthRecoveryPerUpdate;
            output.FromForm_ManaRecoveryPerUpdate = (decimal)temp_form.ManaRecoveryPerUpdate;


            // items portion
            IEnumerable<Item> wornItems = GetAllPlayerItems_ItemOnly(player.Id);
            foreach (Item i in wornItems)
            {
                RAMBuffBox temp = null;
                try
                {
                    temp = ItemStatics.ItemRAMBuffBoxes.First(x => x.dbName == i.dbName.ToLower());
                   // if (temp == null)
                }
                catch
                {
                    LoadItemRAMBuffBox();
                    temp = ItemStatics.ItemRAMBuffBoxes.FirstOrDefault(x => x.dbName == i.dbName.ToLower());
                }

                output.FromItems_HealthBonusPercent += (decimal)temp.HealthBonusPercent + (decimal)temp.HealthBonusPercent * (((i.Level - 1) * (decimal)PvPStatics.Item_LevelBonusModifier));
                output.FromItems_ManaBonusPercent += (decimal)temp.ManaBonusPercent + (decimal)temp.ManaBonusPercent * (((i.Level - 1) * (decimal)PvPStatics.Item_LevelBonusModifier));

                output.FromItems_HealthRecoveryPerUpdate += (decimal)temp.HealthRecoveryPerUpdate + (decimal)temp.HealthRecoveryPerUpdate * (((i.Level - 1) * (decimal)PvPStatics.Item_LevelBonusModifier));
                output.FromItems_ManaRecoveryPerUpdate += (decimal)temp.ManaRecoveryPerUpdate + (decimal)temp.ManaRecoveryPerUpdate * (((i.Level - 1) * (decimal)PvPStatics.Item_LevelBonusModifier));
            }
            
            // effects portion
            IEnumerable<EffectViewModel2> myEffects = EffectProcedures.GetPlayerEffects2(player.Id).Where(e => e.dbEffect.Duration > 0);

            IEnumerable<Effect> myEffects2 = EffectProcedures.GetPlayerEffects_EffectOnly(player.Id).Where(e => e.Duration > 0);

            foreach (Effect e in myEffects2)
            {
                RAMBuffBox temp = null;
                try
                {
                    temp = EffectStatics.EffectRAMBuffBoxes.First(x => x.dbName == e.dbName.ToLower());
                    // if (temp == null)
                }
                catch
                {
                    EffectProcedures.LoadEffectRAMBuffBox();
                    temp = EffectStatics.EffectRAMBuffBoxes.First(x => x.dbName == e.dbName.ToLower());
                }

                output.FromEffects_HealthBonusPercent += (decimal)temp.HealthBonusPercent;
                output.FromEffects_ManaBonusPercent += (decimal)temp.ManaBonusPercent;
                output.FromEffects_HealthRecoveryPerUpdate += (decimal)temp.HealthRecoveryPerUpdate;
                output.FromEffects_ManaRecoveryPerUpdate += (decimal)temp.ManaRecoveryPerUpdate;
            }

            //output.FromEffects_HealthBonusPercent = myEffects.Sum(e => e.Effect.HealthBonusPercent);
            //output.FromEffects_ManaBonusPercent = myEffects.Sum(e => e.Effect.ManaBonusPercent);
            //output.FromEffects_HealthRecoveryPerUpdate = myEffects.Sum(e => e.Effect.HealthRecoveryPerUpdate);
            //output.FromEffects_ManaRecoveryPerUpdate = myEffects.Sum(e => e.Effect.ManaRecoveryPerUpdate);

            //output.FromEffects_HealthRecoveryPerUpdate = 

            // formula:  bonus = amount * (itemlevel - 1) * PvPStatics.Item_LevelBonusModifier

            return output;
        }

        public static int GetInventoryMaxSize(BuffBox box)
        {
            return (int)Math.Floor(box.ExtraInventorySpace()) + PvPStatics.MaxCarryableItemCountBase;
        }

        public static LogBox PlayerBecomesItem(Player victim, DbStaticForm targetForm, Player attacker)
        {
            LogBox output = new LogBox();
            IItemRepository itemRepo = new EFItemRepository();

            bool permanent = false;

            if (victim.MembershipId < 0)
            {
                permanent = true;
            }

            Item newItem = new Item
            {
                IsEquipped = false,
                VictimName = victim.FirstName + " " + victim.LastName,
                dbName = targetForm.BecomesItemDbName,
                Level = victim.Level,
                TimeDropped = DateTime.UtcNow,
                IsPermanent = permanent,
            };

            if (victim.GameMode == 2)
            {
                newItem.PvPEnabled = true;
            }
            else
            {
                newItem.PvPEnabled = false;
            }

            BuffBox attackerBuffs = ItemProcedures.GetPlayerBuffs(attacker);
            decimal maxInventorySize = PvPStatics.MaxCarryableItemCountBase + attackerBuffs.ExtraInventorySpace();

            DbStaticItem newItemPlus = ItemStatics.GetStaticItem(newItem.dbName);

            int inventoryMax = GetInventoryMaxSize(attackerBuffs);

            // regular item logic
            if (newItemPlus.ItemType != PvPStatics.ItemType_Pet)
            {
                // if the attacking player still has room in their inventory, give it to them.
                if (itemRepo.Items.Where(i => i.OwnerId == attacker.Id && i.IsEquipped == false).Count() < inventoryMax)
                {
                    newItem.OwnerId = attacker.Id;
                    newItem.dbLocationName = "";

                    // UNLESS the attacker is a boss, then give it to them for free
                } else if (attacker.MembershipId <= -3) {
                    newItem.OwnerId = attacker.Id;
                    newItem.dbLocationName = "";
                }
                // otherwise the item falls to the ground at that location
                else
                {
                    newItem.dbLocationName = victim.dbLocationName;
                    newItem.OwnerId = -1;

                }
            }

            // animal item, only allow one at a time of any time
            else if (newItemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                Player dbVictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);

                IEnumerable<ItemViewModel> AttackerExistingItems = ItemProcedures.GetAllPlayerItems(attacker.Id);


                // this player currently has no tamed pets, so give it to them auto equipped
                if (AttackerExistingItems.Where(e => e.Item.ItemType == PvPStatics.ItemType_Pet).Count() == 0 && attacker.MembershipId >= -2)
                {
                    newItem.OwnerId = attacker.Id;
                    newItem.IsEquipped = true;
                    newItem.dbLocationName = "";
                    dbVictim.IsPetToId = attacker.Id;
                    SkillProcedures.UpdateItemSpecificSkillsToPlayer(attacker.Id, newItem.dbName);
                }

                // otherwise the animal runs free
                else
                {

                    // bots can keep everything
                    if (attacker.MembershipId < -1)
                    {
                        newItem.OwnerId = attacker.Id;
                        newItem.IsEquipped = true;
                        newItem.dbLocationName = "";
                        dbVictim.IsPetToId = attacker.Id;
                    }
                    else
                    {

                        newItem.dbLocationName = victim.dbLocationName;
                        newItem.OwnerId = -1;
                        dbVictim.IsPetToId = -1;
                    }
                }

                playerRepo.SavePlayer(dbVictim);

                // release any pets the victim currently has (the item is already considered dropeed)
                Player pet = playerRepo.Players.FirstOrDefault(p => p.IsPetToId == victim.Id);
                if (pet != null)
                {
                    pet.IsPetToId = -1;
                    playerRepo.SavePlayer(pet);
                }

            }


            DbStaticItem item = ItemStatics.GetStaticItem(targetForm.BecomesItemDbName);
            Location here = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == victim.dbLocationName);

            itemRepo.SaveItem(newItem);

            output.LocationLog = "<br><b>" + victim.FirstName + " " + victim.LastName + " was completely transformed into a " + item.FriendlyName + " here.</b>";
            output.AttackerLog = "<br><b>You fully transformed " + victim.FirstName + " " + victim.LastName + " into a " + item.FriendlyName + "</b>!";
            output.VictimLog = "<br><b>You have been fully transformed into a " + item.FriendlyName + "!</b>";

            DropAllItems(victim);


            return output;


        }

        public static void DropAllItems(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
            List<Item> itemsToDrop = itemRepo.Items.Where(i => i.OwnerId == player.Id).ToList();

            foreach (Item item in itemsToDrop)
            {
                item.IsEquipped = false;
                item.OwnerId = -1;
                item.dbLocationName = player.dbLocationName;
                item.TimeDropped = DateTime.UtcNow;

            }

            foreach (Item item in itemsToDrop)
            {
                itemRepo.SaveItem(item);

            }

            SkillProcedures.UpdateItemSpecificSkillsToPlayer(player.Id);

        }

        public static PlayerFormViewModel BeingWornBy(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            string victimName = player.FirstName + " " + player.LastName;

            Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == victimName);
            Player wearer = playerRepo.Players.FirstOrDefault(p => item.OwnerId == p.Id);



            return PlayerProcedures.GetPlayerFormViewModel(wearer.Id); ;

        }

        public static string PlayerIsItemAtLocation(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
         
            string victimName = player.FirstName + " " + player.LastName;
            Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == victimName);

            Location place = LocationsStatics.GetLocation.FirstOrDefault(l => l.dbName == item.dbLocationName);
       
            if (place==null) {
                return null;
            } else {
                return place.Name;
            }
            

        }

        public static string UseItem(int itemId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            ItemViewModel itemPlus = ItemProcedures.GetItemViewModel(itemId);

            Player owner = playerRepo.Players.FirstOrDefault(p => p.Id == itemPlus.dbItem.OwnerId);

            string name;

            // get pronoun
            if (WebSecurity.CurrentUserId == owner.MembershipId)
            {
                name = "You";
            }
            else
            {
                name = owner.FirstName + " " + owner.LastName;
            }

            #region single use consumeables

            if (itemPlus.Item.ItemType == "consumable")
            {

                // if this is the grenade type, redirect to attack procedure
                if (itemPlus.dbItem.dbName.Contains("willpower_bomb"))
                {

                    if (owner.TimesAttackingThisUpdate >= PvPStatics.MaxAttacksPerUpdate)
                    {
                        return "You have attacked too many times this update.";
                    }
                  

                    string output = "";
                    if (itemPlus.dbItem.dbName == "item_consumeable_willpower_bomb_weak")
                    {
                        output = AttackProcedures.ThrowGrenade(owner, 10, "Weak");
                    }
                    else if (itemPlus.dbItem.dbName == "item_consumeable_willpower_bomb_strong")
                    {
                        output = AttackProcedures.ThrowGrenade(owner, 25, "Strong");
                    }
                    else if (itemPlus.dbItem.dbName == "item_consumeable_willpower_bomb_volatile")
                    {
                        output = AttackProcedures.ThrowGrenade(owner, 50, "Volatile");
                    }

                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return output;

                }

                // if this item has an effect it grants, give that effect to the player
                if (itemPlus.Item.GivesEffect != null && itemPlus.Item.GivesEffect != "")
                {
                    // check if the player already has this effect or has it in cooldown.  If so, reject usage
                    if (EffectProcedures.PlayerHasEffect(owner, itemPlus.Item.GivesEffect) == true)
                    {
                        return "You can't use this yet as you already have its effects active on you, or else the effect's cooldown has not expired.";
                    }
                    else
                    {
                        itemRepo.DeleteItem(itemPlus.dbItem.Id);
                        LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
                        return EffectProcedures.GivePerkToPlayer(itemPlus.Item.GivesEffect, owner);
                    }
                }
                

                
                if (itemPlus.Item.InstantHealthRestore > 0)
                {
                    owner.Health += itemPlus.Item.InstantHealthRestore;
                    if (owner.Health > owner.MaxHealth)
                    {
                        owner.Health = owner.MaxHealth;
                    }

                    playerRepo.SavePlayer(owner);

                    itemRepo.DeleteItem(itemPlus.dbItem.Id);

                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " consumed a " + itemPlus.Item.FriendlyName + " here.");
                    return name + " consumed a " + itemPlus.Item.FriendlyName + ", immediately restoring " + itemPlus.Item.InstantHealthRestore + " willpower.  " + owner.Health + "/" + owner.MaxHealth + " WP";

                }

                if (itemPlus.Item.InstantManaRestore > 0)
                {
                    owner.Mana += itemPlus.Item.InstantManaRestore;
                    if (owner.Mana > owner.MaxMana)
                    {
                        owner.Mana = owner.MaxMana;
                    }
                    playerRepo.SavePlayer(owner);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);

                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " consumed a " + itemPlus.Item.FriendlyName + " here.");
                    return name + " consumed a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.InstantManaRestore) + " mana.  " + owner.Mana + "/" + owner.MaxMana + " Mana";
                }

                // special items time!
                if (itemPlus.Item.dbName == "item_consumeable_ClassicMeLotion")
                {

                    if (owner.Form == owner.OriginalForm)
                    {
                        return "You could use this lotion, but as you are already in your base form it wouldn't do you any good.";
                    }

                    PlayerProcedures.InstantRestoreToBase(owner);
                    TFEnergyProcedures.CleanseTFEnergies(owner, 25);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
                    return name + " spread the mana-absorbing " + itemPlus.Item.FriendlyName + " over your skin, quickly restoring you to your original body and cleansing away some additional transformation energies.";
                }

                if (itemPlus.Item.dbName == "item_consumeable_Lullaby_Whistle")
                {
                    AIDirectiveProcedures.DeaggroPsychopathsOnPlayer(owner);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
                    return name + "You take a quick blow with the whistle, sending a magical melody out into the air that should force any psychopathic spellslingers intend on taking you down forget all about you, so long as you don't provoke them again...";
                }

                if (itemPlus.Item.dbName == BossProcedures.BossProcedures_BimboBoss.CureItemDbName)
                {

                    if (EffectProcedures.PlayerHasEffect(owner, BossProcedures.BossProcedures_BimboBoss.KissEffectdbName) == false) {
                        return "Since you are not infected with the bimbonic virus there's no need for you to use this right now.";
                    }

                    PlayerProcedures.InstantRestoreToBase(owner);
                    EffectProcedures.RemovePerkFromPlayer(BossProcedures.BossProcedures_BimboBoss.KissEffectdbName, owner);
                    EffectProcedures.GivePerkToPlayer(BossProcedures.BossProcedures_BimboBoss.CureEffectdbName, owner);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return "You inject yourself with the vaccine, returning to your original form and purging the virus out of your body.  Careful though, it may later mutate and be able to infect you once again...";
                }

                if (itemPlus.Item.dbName == "item_consumeable_covenant_crystal")
                {

                    // assert owner has not been in combat recently
                    double minutesSinceCombat = Math.Abs(Math.Floor(owner.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
                    if (minutesSinceCombat < 30)
                    {
                        return "Unfortunately you have been in combat too recently for the crystal to work.";
                    }

                    // assert owner is in a covenant
                    if (owner.Covenant < 1)
                    {
                        return "Unfortunately as you are not in a covenant, you aren't able to use this item.";
                    }

                    // assert covenant has a safeground
                    Covenant myCov = CovenantProcedures.GetDbCovenant(owner.Covenant);
                    if (myCov.HomeLocation == null || myCov.HomeLocation == "")
                    {
                        return "You are a member of your covenant, but unfortunately your covenant has not yet established a safeground to call home so you are unable to use this item.";
                    }

                    string output = PlayerProcedures.TeleportPlayer(owner, myCov.HomeLocation, true);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return output;
                }

                // spellbooks; these give the reader some spells they do not know.
                if (itemPlus.Item.dbName.Contains("item_consumable_spellbook_"))
                {
                    int amount = 3;
                    if (itemPlus.Item.dbName.Contains("small"))
                    {
                        amount = 3;
                    }
                    else if (itemPlus.Item.dbName.Contains("medium"))
                    {
                        amount = 6;
                    } if (itemPlus.Item.dbName.Contains("large"))
                    {
                        amount = 9;
                    }
                    if (itemPlus.Item.dbName.Contains("giant"))
                    {
                        amount = 12;
                    }
                    string output = SkillProcedures.GiveRandomFindableSkillsToPlayer(owner, amount);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return output;
                }

               // if (itemPlus.Item.dbName == "item_consumeable_")



            }

                #endregion

            #region reuseable consumeables

            else if (itemPlus.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable)
            {

                // assert item is still not on cooldown
                if (itemPlus.dbItem.TurnsUntilUse > 0)
                {
                    return "You have to wait longer before you can use this item agin.";
                }
                else if (itemPlus.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable && EffectProcedures.PlayerHasEffect(owner, itemPlus.Item.GivesEffect) == true)
                {
                    return "You can't use this yet as you already have the effect active or on cooldown.";
                }
                else
                {
                    // add waiting time back on to the item
                    Item thisdbItem = itemRepo.Items.FirstOrDefault(i => i.Id == itemPlus.dbItem.Id);
                    thisdbItem.TurnsUntilUse = itemPlus.Item.UseCooldown;
                    itemRepo.SaveItem(thisdbItem);

                    // formula:  bonus = amount * (itemlevel - 1) * PvPStatics.Item_LevelBonusModifier

                    // there is both a health and mana restoration / loss effect to this spell
                    if (itemPlus.Item.ReuseableHealthRestore != 0 && itemPlus.Item.ReuseableManaRestore != 0)
                    {

                        // get the bonus from this item being leveled
                        

                        decimal bonusFromLevelHealth = itemPlus.Item.ReuseableHealthRestore * (thisdbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier;
                        decimal bonusFromLevelMana = itemPlus.Item.ReuseableManaRestore * (thisdbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier;

                        owner.Health = owner.Health += itemPlus.Item.ReuseableHealthRestore + bonusFromLevelHealth;
                        if (owner.Health > owner.MaxHealth)
                        {
                            owner.Health = owner.MaxHealth;
                        }
                        if (owner.Health < 0)
                        {
                            owner.Health = 0;
                        }

                        owner.Mana = owner.Mana += itemPlus.Item.ReuseableManaRestore + bonusFromLevelMana;
                        if (owner.Mana > owner.Mana)
                        {
                            owner.Mana = owner.MaxMana;
                        }
                        if (owner.Mana < 0)
                        {
                            owner.Mana = 0;
                        }


                        playerRepo.SavePlayer(owner);

                        return name + " used a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.ReuseableHealthRestore + bonusFromLevelHealth) + " willpower and " + (itemPlus.Item.ReuseableManaRestore + bonusFromLevelMana) + " mana.  " + owner.Health + "/" + owner.MaxHealth + " WP, " + owner.Mana + "/" + owner.MaxMana + " Mana";

                    }

                    // just a health gain
                    else if (itemPlus.Item.ReuseableHealthRestore != 0)
                    {

                          // get the bonus from this item being leveled
                        decimal bonusFromLevel = itemPlus.Item.ReuseableHealthRestore * (thisdbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier;

                        owner.Health = owner.Health += itemPlus.Item.ReuseableHealthRestore + bonusFromLevel;
                        if (owner.Health > owner.MaxHealth)
                        {
                            owner.Health = owner.MaxHealth;
                        }
                        playerRepo.SavePlayer(owner);

                        return name + " consumed from a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.ReuseableHealthRestore + bonusFromLevel) + " willpower.  " + owner.Health + "/" + owner.MaxHealth + " WP";
                    }

                   // just a mana gain
                    else if (itemPlus.Item.ReuseableManaRestore != 0)
                    {

                        // get the bonus from this item being leveled
                        decimal bonusFromLevel = itemPlus.Item.ReuseableManaRestore * (thisdbItem.Level - 1) * PvPStatics.Item_LevelBonusModifier;

                        owner.Mana = owner.Mana += itemPlus.Item.ReuseableManaRestore + bonusFromLevel;
                        if (owner.Mana > owner.MaxMana)
                        {
                            owner.Mana = owner.MaxMana;
                        }
                        playerRepo.SavePlayer(owner);

                        return name + " consumed from a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.ReuseableManaRestore + bonusFromLevel) + " mana.  " + owner.Mana + "/" + owner.MaxMana + " Mana";
                    }

                    else if (itemPlus.Item.GivesEffect != null && itemPlus.Item.GivesEffect != "")
                    {
                        EffectProcedures.GivePerkToPlayer(itemPlus.Item.GivesEffect, owner);
                        DbStaticEffect effectPlus = EffectStatics.GetStaticEffect2(itemPlus.Item.GivesEffect);
                        if (owner.Gender == "male" && effectPlus.MessageWhenHit_M != null && effectPlus.MessageWhenHit_M != "")
                        {
                            return name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit_M;
                        }
                        else if (owner.Gender == "female" && effectPlus.MessageWhenHit_F != null && effectPlus.MessageWhenHit_F != "")
                        {
                            return name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit_F;
                        }
                        else
                        {
                            return  name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit;
                        }
                        
                    }

                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");

                }
            }

           

          

         

            #endregion

            return "";
        }

        public static Item GetItemByVictimName(string firstName, string lastName)
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.Items.FirstOrDefault(i => i.VictimName == (firstName + " " + lastName));
        }

        public static DbStaticItem GetRandomFindableItem()
        {

            IEnumerable<DbStaticItem> eligibleItems = ItemStatics.GetAllFindableItems();

            double RollMax = eligibleItems.Select(i => i.FindWeight).Sum();

            Random randomItemIndex2 = new Random(DateTime.Now.Millisecond);
            double roll2 = randomItemIndex2.NextDouble() * RollMax;

            int currentIndex = 0;

            List<StaticItem> iteratedThrough = new List<StaticItem>();

            foreach (DbStaticItem item in eligibleItems.ToList())
            {

                double lowRange = eligibleItems.Take(currentIndex).Sum(i => i.FindWeight);
                double highRange = eligibleItems.Take(currentIndex+1).Sum(i => i.FindWeight);

                if (roll2 >= lowRange && roll2 < highRange)
                {
                    return item;
                }

             
                currentIndex++;
            }

            DbStaticItem foundItem = null;
            return foundItem;
        }

        public static void MoveAnimalItem(Player player, string destination)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item animalItem = itemRepo.Items.FirstOrDefault(i => i.VictimName == (player.FirstName + " " + player.LastName));
            animalItem.dbLocationName = destination;
            itemRepo.SaveItem(animalItem);
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player me = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            me.dbLocationName = destination;
            playerRepo.SavePlayer(me);
        }

        public static int PlayerHasNumberOfThisItem(Player player, string itemdbname)
        {
            IItemRepository itemRepo = new EFItemRepository();

            return itemRepo.Items.Where(i => i.dbName == itemdbname && i.OwnerId == player.Id).Count();
        }

        public static void DeleteItem(int id)
        {
            IItemRepository itemRepo = new EFItemRepository();

            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == id);

            itemRepo.DeleteItem(item.Id);
        }

        public static void DeleteItemOfName(Player player, string itemdbname)
        {
            IItemRepository itemRepo = new EFItemRepository();

            Item item = itemRepo.Items.FirstOrDefault(i => i.dbName == itemdbname && i.OwnerId == player.Id);

            itemRepo.DeleteItem(item.Id);

        }

        public static decimal GetCostOfItem(ItemViewModel item, string buyOrSell)
        {

            if (item.Item.ItemType == "consumable")
            {
                if (buyOrSell == "buy")
                {
                    return item.Item.MoneyValue;
                }
                else
                {
                    return item.Item.MoneyValue * .5M;
                }
            }

            if (buyOrSell == "buy")
            {
                decimal price = 50 + 50 * item.dbItem.Level;

                // item is not permanent, charge less
                if (item.dbItem.IsPermanent == false)
                {
                    price *= .85M;
                }
           

                return Math.Ceiling(price);
            }

                // selling, pay less money
            else
            {
                decimal price = 50 + 50 * item.dbItem.Level * .75M;

                // item is not permanent, charge less
                if (item.dbItem.IsPermanent == false)
                {
                    price *= .5M;
                }

                return Math.Ceiling(price);

            }
            
        }

        public static IEnumerable<ItemLeaderboardViewModel> GetLeadingItems(int number)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IInanimateXPRepository xpRepo = new EFInanimateXPRepository();


            IEnumerable<ItemLeaderboardViewModel> output = from i in itemRepo.Items
                                                where i.VictimName != null && i.VictimName != ""
                                                join ix in xpRepo.InanimateXPs on i.OwnerId equals ix.Amount
                                                select new ItemLeaderboardViewModel
                                                {
                                                   dbName = i.dbName,
                                                   VictimName = i.VictimName,
                                                   Level = i.Level,
                                                   XP = ix.Amount,
                                                };

            output = output.OrderByDescending(i => i.Level).ThenByDescending(i => i.XP).Take(number);

            foreach (ItemLeaderboardViewModel i in output) {
                i.StaticItem = ItemStatics.GetStaticItem(i.dbName);
            }

            return output;

        }

        public static IEnumerable<SimpleItemLeaderboardViewModel> GetLeadingItemsSimple(int number)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IInanimateXPRepository xpRepo = new EFInanimateXPRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<Item> items = itemRepo.Items.Where(i => i.VictimName != "" && !i.VictimName.Contains("Psychopath") && !i.VictimName.Contains("Donna Milton") && !i.VictimName.Contains("Lady Lovebringer")).OrderByDescending(i => i.Level).Take(number);

            List<SimpleItemLeaderboardViewModel> output = new List<SimpleItemLeaderboardViewModel>();
            foreach (Item i in items)
            {
                SimpleItemLeaderboardViewModel addme = new SimpleItemLeaderboardViewModel
                {
                    Item = i,
                    StaticItem = ItemStatics.GetStaticItem(i.dbName),
                   
                };

                Player player = playerRepo.Players.FirstOrDefault(p => p.FirstName + " " + p.LastName == i.VictimName);
                addme.PlayerId = player.Id;
                addme.Gender = player.Gender;

                try
                {
                    addme.ItemXP = xpRepo.InanimateXPs.FirstOrDefault(p => p.OwnerId == addme.PlayerId).Amount;
                }
                catch
                {
                    addme.ItemXP = 0;
                }

                output.Add(addme);
            }

            return output;

        }

        public static void LoadItemRAMBuffBox()
        {
            IDbStaticItemRepository dbStaticItemRepo = new EFDbStaticItemRepository();

            ItemStatics.ItemRAMBuffBoxes = new List<RAMBuffBox>();

            IEnumerable<DbStaticItem> statics = dbStaticItemRepo.DbStaticItems.Where(c => c.dbName != null && c.dbName != "").ToList();

            foreach (DbStaticItem i in statics)
            {
                RAMBuffBox temp = new RAMBuffBox
                {
                    dbName = i.dbName.ToLower(),

                    HealthBonusPercent = (float)i.HealthBonusPercent,
                    ManaBonusPercent = (float)i.ManaBonusPercent,
                    HealthRecoveryPerUpdate = (float)i.HealthRecoveryPerUpdate,
                    ManaRecoveryPerUpdate = (float)i.ManaRecoveryPerUpdate,

                };
                ItemStatics.ItemRAMBuffBoxes.Add(temp);
            }
        }

        public static bool PlayerHasReadBook(Player player, string bookDbName)
        {
            IBookReadingRepository repo = new EFBookReadingRepository();

            BookReading reading = repo.BookReadings.FirstOrDefault(b => b.PlayerId == player.Id && b.BookDbName == bookDbName);

            if (reading == null)
            {
                return false;
            }
            else
            {
                return true;
            }

            
        }

        public static void AddBookReading(Player player, string bookDbName)
        {
            IBookReadingRepository repo = new EFBookReadingRepository();

            BookReading reading = repo.BookReadings.FirstOrDefault(b => b.PlayerId == player.Id && b.BookDbName == bookDbName);

            if (reading == null)
            {
                BookReading newReading = new BookReading
                {
                    PlayerId = player.Id,
                    BookDbName = bookDbName,
                    Timestamp = DateTime.UtcNow,
                };
                repo.SaveBookReading(newReading);
            }
            
        }



        

    }

}