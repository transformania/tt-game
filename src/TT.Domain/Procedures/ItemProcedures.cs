using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.ViewModels;
using System.Threading;
using TT.Domain.Commands.Items;
using TT.Domain.Commands.Players;
using TT.Domain.Queries.Item;

namespace TT.Domain.Procedures
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
                                                             VictimName = i.VictimName,
                                                             Nickname = i.Nickname,
                                                             LastSouledTimestamp = i.LastSouledTimestamp,
                                                          },



                                                          Item = new TT.Domain.ViewModels.StaticItem
                                                          {
                                                            dbName = si.dbName,
                                                            FriendlyName = si.FriendlyName,
                                                            Description = si.Description,
                                                            PortraitUrl = si.PortraitUrl,
                                                            MoneyValue = si.MoneyValue,
                                                            MoneyValueSell = si.MoneyValueSell,
                                                            ItemType = si.ItemType,
                                                            UseCooldown = si.UseCooldown,
                                                            UsageMessage_Item = si.UsageMessage_Item,
                                                            UsageMessage_Player = si.UsageMessage_Player,
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

                                                            Discipline = si.Discipline,
                                                            Perception = si.Perception,
                                                            Charisma = si.Charisma,
                                                            Submission_Dominance = si.Submission_Dominance,

                                                            Fortitude = si.Fortitude,
                                                            Agility = si.Agility,
                                                            Allure = si.Allure,
                                                            Corruption_Purity  = si.Corruption_Purity,

                                                            Magicka = si.Magicka,
                                                            Succour = si.Succour,
                                                            Luck = si.Luck,
                                                            Chaos_Order = si.Chaos_Order,


                                                            InstantHealthRestore = si.InstantHealthRestore,
                                                            InstantManaRestore = si.InstantManaRestore,
                                                            ReuseableHealthRestore = si.ReuseableHealthRestore,
                                                            ReuseableManaRestore = si.ReuseableManaRestore,

                                                            CurseTFFormdbName = si.CurseTFFormdbName,


                                                          }

                                                      };

            return output;
        }

        public static IEnumerable<Item> GetAllPlayerItems_ItemOnly(int playerId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.Items.Where(i => i.OwnerId == playerId && i.IsEquipped);
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
                                                         VictimName = i.VictimName,
                                                         Nickname = i.Nickname,
                                                         LastSouledTimestamp = i.LastSouledTimestamp,
                                                     },



                                                     Item = new TT.Domain.ViewModels.StaticItem
                                                     {
                                                         Id = si.Id,
                                                         dbName = si.dbName,
                                                         FriendlyName = si.FriendlyName,
                                                         Description = si.Description,
                                                         PortraitUrl = si.PortraitUrl,
                                                         MoneyValue = si.MoneyValue,
                                                         ItemType = si.ItemType,
                                                         UseCooldown = si.UseCooldown,
                                                         UsageMessage_Item = si.UsageMessage_Item,
                                                         UsageMessage_Player = si.UsageMessage_Player,
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

                                                         Discipline = si.Discipline,
                                                         Perception = si.Perception,
                                                         Charisma = si.Charisma,
                                                         Submission_Dominance = si.Submission_Dominance,

                                                         Fortitude = si.Fortitude,
                                                         Agility = si.Agility,
                                                         Allure = si.Allure,
                                                         Corruption_Purity = si.Corruption_Purity,

                                                         Magicka = si.Magicka,
                                                         Succour = si.Succour,
                                                         Luck = si.Luck,
                                                         Chaos_Order = si.Chaos_Order,


                                                         InstantHealthRestore = si.InstantHealthRestore,
                                                         InstantManaRestore = si.InstantManaRestore,
                                                         ReuseableHealthRestore = si.ReuseableHealthRestore,
                                                         ReuseableManaRestore = si.ReuseableManaRestore,

                                                         CurseTFFormdbName = si.CurseTFFormdbName,
                                                     }

                                                 };

            return output.First();
        }

        public static bool PlayerIsCarryingTooMuch(int newOwnerId, int offset, BuffBox buffs)
        {
            var currentlyOwnedItems = GetAllPlayerItems(newOwnerId);
            var nonWornItemsCarried = currentlyOwnedItems.Count(i => !i.dbItem.IsEquipped) - offset;

            var max = GetInventoryMaxSize(buffs);

            return nonWornItemsCarried >= max;
        }

        public static string GiveItemToPlayer(int itemId, int newOwnerId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == itemId);
            DbStaticItem itemPlus = ItemStatics.GetStaticItem(item.dbName);
            Player owner = PlayerProcedures.GetPlayer(newOwnerId);
            //LogBox log = new LogBox();


            if (!item.dbLocationName.IsNullOrEmpty())
            {
                if (owner.BotId == AIStatics.ActivePlayerBotId && item.PvPEnabled == GameModeStatics.Any)
                {
                    if (owner.GameMode == GameModeStatics.PvP)
                    {
                        item.PvPEnabled = GameModeStatics.PvP;
                    }
                    else
                    {
                        item.PvPEnabled = GameModeStatics.Protection;
                    }
                }
                item.dbLocationName = "";
                item.OwnerId = newOwnerId;
                item.IsEquipped = false;
                ItemTransferLogProcedures.AddItemTransferLog(itemId, newOwnerId);
                itemRepo.SaveItem(item);

                // if item is not an animal
                if (itemPlus.ItemType != PvPStatics.ItemType_Pet) { 
                    return "You picked up a " + itemPlus.FriendlyName + " and put it into your inventory.";
                }

                // item is an animal
                else
                {
                    return "You tame " + item.GetFullName() + " the " + itemPlus.FriendlyName + " and are now keeping them as a pet.";
                }

            }

            return "";
        }

        public static void GiveItemToPlayer_Nocheck(int itemId, int newOwnerId) {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == itemId);
            DbStaticItem itemPlus = ItemStatics.GetStaticItem(item.dbName);
            item.OwnerId = newOwnerId;
            Player owner = PlayerProcedures.GetPlayer(newOwnerId);

            if (owner.BotId == AIStatics.ActivePlayerBotId && item.PvPEnabled == GameModeStatics.Any)
            {
                if (owner.GameMode == GameModeStatics.PvP)
                {
                    item.PvPEnabled = GameModeStatics.PvP;
                }
                else
                {
                    item.PvPEnabled = GameModeStatics.Protection;
                }
            }

            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                item.IsEquipped = true;
            } else {
                item.IsEquipped = false;
                item.dbLocationName = "";
            }
           
            item.TimeDropped = DateTime.UtcNow;
            item.LastSold = item.TimeDropped;
            ItemTransferLogProcedures.AddItemTransferLog(itemId, newOwnerId);
            itemRepo.SaveItem(item);
        }

        public static string GiveNewItemToPlayer(Player player, DbStaticItem item)
        {

            var cmd = new CreateItem
            {
                OwnerId = player.Id,
                dbName = item.dbName,
                IsEquipped = false,
                VictimName = "",
                dbLocationName = "",
                ItemSourceId = item.Id
            };

            if (player.BotId < AIStatics.ActivePlayerBotId)
            {
                cmd.PvPEnabled = -1;
            }
            else
            {
                if (player.GameMode == GameModeStatics.PvP)
                {
                    cmd.PvPEnabled = GameModeStatics.PvP;
                }
                else
                {
                    cmd.PvPEnabled = GameModeStatics.Protection;
                }
            }
            var newItemId = DomainRegistry.Repository.Execute(cmd);
            ItemTransferLogProcedures.AddItemTransferLog(newItemId, player.Id);
            return "You found a " + item.FriendlyName + "!";
        }

        public static string GiveNewItemToPlayer(Player player, string itemName)
        {
            DbStaticItem i = ItemStatics.GetStaticItem(itemName);
            return GiveNewItemToPlayer(player, i);
        }

        public static string DropItem(int itemId, string locationDbName)
        {


            var item = DomainRegistry.Repository.FindSingle( new GetItem { ItemId = itemId });

            int oldOwnerId = item.Owner.Id;

            DbStaticItem itemPlus = ItemStatics.GetStaticItem(item.dbName);

            // dropped "item" is a pet so automatically unequip them
            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                EquipItem(itemId, false);
            }

            var cmd = new DropItem
            {
                OwnerId = item.Owner.Id,
                ItemId = item.Id
            };
            DomainRegistry.Repository.Execute(cmd);


            ItemTransferLogProcedures.AddItemTransferLog(itemId, -1);

            SkillProcedures.UpdateItemSpecificSkillsToPlayer(PlayerProcedures.GetPlayer(oldOwnerId));

            

            // item is an animal
            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                return "You released " + item.GetFullName() + " the " + itemPlus.FriendlyName + " that you were keeping as a pet.";
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
            

            if (putOn)
            {
                item.IsEquipped = true;
                item.EquippedThisTurn = true;
                itemRepo.SaveItem(item);

                BuffBox targetbuffs = ItemProcedures.GetPlayerBuffs(dbOwner);

                dbOwner = PlayerProcedures.ReadjustMaxes(dbOwner, targetbuffs);
                playerRepo.SavePlayer(dbOwner);

                SkillProcedures.UpdateItemSpecificSkillsToPlayer(dbOwner, item.dbName);

                return "You put on your " + itemPlus.FriendlyName + ".";
         
            }
            else
            {
                item.IsEquipped = false;
                itemRepo.SaveItem(item);

                BuffBox targetbuffs = ItemProcedures.GetPlayerBuffs(dbOwner);

                dbOwner = PlayerProcedures.ReadjustMaxes(dbOwner, targetbuffs);
                playerRepo.SavePlayer(dbOwner);

                SkillProcedures.UpdateItemSpecificSkillsToPlayer(dbOwner);

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
            IEnumerable<ItemViewModel> itemsOfThisType = GetAllPlayerItems(playerId).Where(i => i.Item.ItemType == itemType && i.dbItem.IsEquipped);
            return itemsOfThisType.Count();
        }

        public static int PlayerIsWearingNumberOfThisExactItem(int playerId, string itemDbName)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> itemsOfThisType = GetAllPlayerItems(playerId).Where(i => i.Item.dbName == itemDbName && i.dbItem.IsEquipped);
            return itemsOfThisType.Count();
        }

        public static BuffBox GetPlayerBuffs(Player player)
        {
            return GetPlayerBuffs(player.Id);
        }

        public static BuffBox GetPlayerBuffs(int playerId)
        {

            BuffBox output = new BuffBox();

            // grab all of the bonuses coming from effects
            IEnumerable<EffectViewModel2> myEffects = EffectProcedures.GetPlayerEffects2(playerId).Where(e => e.dbEffect.Duration > 0);
            var context = new StatsContext();
            var bonusparam = new SqlParameter("Item_LevelBonusModifier", SqlDbType.Real);
            bonusparam.Value = PvPStatics.Item_LevelBonusModifier;
            var playerparam = new SqlParameter("PlayerId", SqlDbType.Int);
            playerparam.Value = playerId;
            object[] parameters = new object[] { playerparam, bonusparam };
            var query = context.Database.SqlQuery<BuffStoredProc>("exec [dbo].[GetPlayerBuffs] @PlayerId, @Item_LevelBonusModifier", parameters);

            foreach (BuffStoredProc q in query)
            {
                if (q.Type == "Items")
                {
                    output.FromItems_HealthBonusPercent = q.HealthBonusPercent;
                    output.FromItems_ManaBonusPercent = q.ManaBonusPercent;
                    output.FromItems_MeditationExtraMana = q.MeditationExtraMana;
                    output.FromItems_CleanseExtraHealth = q.CleanseExtraHealth;
                    output.FromItems_ExtraSkillCriticalPercent = q.ExtraSkillCriticalPercent;
                    output.FromItems_HealthRecoveryPerUpdate = q.HealthRecoveryPerUpdate;
                    output.FromItems_ManaRecoveryPerUpdate = q.ManaRecoveryPerUpdate;
                    output.FromItems_SneakPercent = q.SneakPercent;
                    output.FromItems_EvasionPercent = q.EvasionPercent;
                    output.FromItems_EvasionNegationPercent = q.EvasionNegationPercent;
                    output.FromItems_MoveActionPointDiscount = q.MoveActionPointDiscount;
                    output.FromItems_SpellExtraTFEnergyPercent = q.SpellExtraTFEnergyPercent;
                    output.FromItems_SpellExtraHealthDamagePercent = q.SpellExtraHealthDamagePercent;
                    output.FromItems_CleanseExtraTFEnergyRemovalPercent = q.CleanseExtraTFEnergyRemovalPercent;
                    output.FromItems_SpellMisfireChanceReduction = q.SpellMisfireChanceReduction;
                    output.FromItems_SpellHealthDamageResistance = q.SpellHealthDamageResistance;
                    output.FromItems_SpellTFEnergyDamageResistance = q.SpellTFEnergyDamageResistance;
                    output.FromItems_ExtraInventorySpace = q.ExtraInventorySpace;

                    output.FromItems_Discipline = q.Discipline;
                    output.FromItems_Perception = q.Perception;
                    output.FromItems_Charisma = q.Charisma;
                    output.FromItems_Submission_Dominance = q.Submission_Dominance;

                    output.FromItems_Fortitude = q.Fortitude;
                    output.FromItems_Agility = q.Agility;
                    output.FromItems_Allure = q.Allure;
                    output.FromItems_Corruption_Purity = q.Corruption_Purity;

                    output.FromItems_Magicka = q.Magicka;
                    output.FromItems_Succour = q.Succour;
                    output.FromItems_Luck = q.Luck;
                    output.FromItems_Chaos_Order = q.Chaos_Order;
                }
                else if (q.Type == "Form")
                {
                    output.FromForm_HealthBonusPercent = q.HealthBonusPercent;
                    output.FromForm_ManaBonusPercent = q.ManaBonusPercent;
                    output.FromForm_MeditationExtraMana = q.MeditationExtraMana;
                    output.FromForm_CleanseExtraHealth = q.CleanseExtraHealth;
                    output.FromForm_ExtraSkillCriticalPercent = q.ExtraSkillCriticalPercent;
                    output.FromForm_HealthRecoveryPerUpdate = q.HealthRecoveryPerUpdate;
                    output.FromForm_ManaRecoveryPerUpdate = q.ManaRecoveryPerUpdate;
                    output.FromForm_SneakPercent = q.SneakPercent;
                    output.FromForm_EvasionPercent = q.EvasionPercent;
                    output.FromForm_EvasionNegationPercent = q.EvasionNegationPercent;
                    output.FromForm_MoveActionPointDiscount = q.MoveActionPointDiscount;
                    output.FromForm_SpellExtraTFEnergyPercent = q.SpellExtraTFEnergyPercent;
                    output.FromForm_SpellExtraHealthDamagePercent = q.SpellExtraHealthDamagePercent;
                    output.FromForm_CleanseExtraTFEnergyRemovalPercent = q.CleanseExtraTFEnergyRemovalPercent;
                    output.FromForm_SpellMisfireChanceReduction = q.SpellMisfireChanceReduction;
                    output.FromForm_SpellHealthDamageResistance = q.SpellHealthDamageResistance;
                    output.FromForm_SpellTFEnergyDamageResistance = q.SpellTFEnergyDamageResistance;
                    output.FromForm_ExtraInventorySpace = q.ExtraInventorySpace;

                    output.FromForm_Discipline = q.Discipline;
                    output.FromForm_Perception = q.Perception;
                    output.FromForm_Charisma = q.Charisma;
                    output.FromForm_Submission_Dominance = q.Submission_Dominance;

                    output.FromForm_Fortitude = q.Fortitude;
                    output.FromForm_Agility = q.Agility;
                    output.FromForm_Allure = q.Allure;
                    output.FromForm_Corruption_Purity = q.Corruption_Purity;

                    output.FromForm_Magicka = q.Magicka;
                    output.FromForm_Succour = q.Succour;
                    output.FromForm_Luck = q.Luck;
                    output.FromForm_Chaos_Order = q.Chaos_Order;
                }
                else if (q.Type == "Effects")
                {
                    output.FromEffects_HealthBonusPercent = q.HealthBonusPercent;
                    output.FromEffects_ManaBonusPercent = q.ManaBonusPercent;
                    output.FromEffects_MeditationExtraMana = q.MeditationExtraMana;
                    output.FromEffects_CleanseExtraHealth = q.CleanseExtraHealth;
                    output.FromEffects_ExtraSkillCriticalPercent = q.ExtraSkillCriticalPercent;
                    output.FromEffects_HealthRecoveryPerUpdate = q.HealthRecoveryPerUpdate;
                    output.FromEffects_ManaRecoveryPerUpdate = q.ManaRecoveryPerUpdate;
                    output.FromEffects_SneakPercent = q.SneakPercent;
                    output.FromEffects_EvasionPercent = q.EvasionPercent;
                    output.FromEffects_EvasionNegationPercent = q.EvasionNegationPercent;
                    output.FromEffects_MoveActionPointDiscount = q.MoveActionPointDiscount;
                    output.FromEffects_SpellExtraTFEnergyPercent = q.SpellExtraTFEnergyPercent;
                    output.FromEffects_SpellExtraHealthDamagePercent = q.SpellExtraHealthDamagePercent;
                    output.FromEffects_CleanseExtraTFEnergyRemovalPercent = q.CleanseExtraTFEnergyRemovalPercent;
                    output.FromEffects_SpellMisfireChanceReduction = q.SpellMisfireChanceReduction;
                    output.FromEffects_SpellHealthDamageResistance = q.SpellHealthDamageResistance;
                    output.FromEffects_SpellTFEnergyDamageResistance = q.SpellTFEnergyDamageResistance;
                    output.FromEffects_ExtraInventorySpace = q.ExtraInventorySpace;

                    output.FromEffects_Discipline = q.Discipline;
                    output.FromEffects_Perception = q.Perception;
                    output.FromEffects_Charisma = q.Charisma;
                    output.FromEffects_Submission_Dominance = q.Submission_Dominance;

                    output.FromEffects_Fortitude = q.Fortitude;
                    output.FromEffects_Agility = q.Agility;
                    output.FromEffects_Allure = q.Allure;
                    output.FromEffects_Corruption_Purity = q.Corruption_Purity;

                    output.FromEffects_Magicka = q.Magicka;
                    output.FromEffects_Succour = q.Succour;
                    output.FromEffects_Luck = q.Luck;
                    output.FromEffects_Chaos_Order = q.Chaos_Order;

                }
            }
            // non-stat buffs

            output.HasSearchDiscount = false;
            foreach (EffectViewModel2 eff in myEffects)
            {
                if (eff.dbEffect.dbName == "perk_sharp_eye")
                {
                    output.HasSearchDiscount = true;
                    //break;
                }
                else if (eff.dbEffect.dbName == "perk_apprentice_enchanter_1_lvl")
                {
                    if (output.EnchantmentBoost < 1)
                    {
                        output.EnchantmentBoost = 1;
                    }

                }
                else if (eff.dbEffect.dbName == "perk_apprentice_enchanter_2_lvl")
                {
                    if (output.EnchantmentBoost < 2)
                    {
                        output.EnchantmentBoost = 2;
                    }
                }
                else if (eff.dbEffect.dbName == "perk_apprentice_enchanter_3_lvl")
                {
                    if (output.EnchantmentBoost < 3)
                    {
                        output.EnchantmentBoost = 3;
                    }
                }
            }

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

            var cmd = new CreateItem
            {
                IsEquipped = false,
                VictimName = victim.FirstName + " " + victim.LastName,
                dbName = targetForm.BecomesItemDbName,
                Level = victim.Level,
                Nickname = victim.Nickname,
                ItemSourceId = ItemStatics.GetStaticItem(targetForm.BecomesItemDbName).Id
            };

            // no attacker, just drop at player's location and return immediately
            if (attacker == null)
            {
                cmd.dbLocationName = victim.dbLocationName;
                cmd.OwnerId = null;
                cmd.PvPEnabled = -1;
                cmd.IsEquipped = false;
                cmd.IsPermanent = false;
                cmd.LastSouledTimestamp = DateTime.UtcNow;

                DomainRegistry.Repository.Execute(cmd);

                DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = victim.Id });
                SkillProcedures.UpdateItemSpecificSkillsToPlayer(victim);

                return output;
            }

            // all bots turn into either game mode
            if (attacker.BotId < AIStatics.ActivePlayerBotId)
            {
                cmd.PvPEnabled = -1;
            }

            // turn victim into attacker's game mode, PvP
            else if (attacker.GameMode == GameModeStatics.PvP)
            {
                cmd.PvPEnabled = GameModeStatics.PvP;
            }

            // turn victim into attacker's game mode, Protection
            else
            {
                cmd.PvPEnabled = GameModeStatics.Protection;
            }

            // victim is a bot; make them permanent immediately
            if (victim.BotId < AIStatics.ActivePlayerBotId)
            {
                cmd.IsPermanent = true;
            }
            else
            {
                cmd.IsPermanent = false;
            }

            BuffBox attackerBuffs = GetPlayerBuffs(attacker);

            DbStaticItem newItemPlus = ItemStatics.GetStaticItem(cmd.dbName);

            int inventoryMax = GetInventoryMaxSize(attackerBuffs);

            // regular item logic
            if (newItemPlus.ItemType != PvPStatics.ItemType_Pet)
            {
                // if the attacking player still has room in their inventory, give it to them.
                if (itemRepo.Items.Count(i => i.OwnerId == attacker.Id && !i.IsEquipped) < inventoryMax)
                {
                    cmd.OwnerId = attacker.Id;
                    cmd.dbLocationName = "";

                // UNLESS the attacker is a boss, then give it to them for free
                } else if (attacker.BotId < AIStatics.PsychopathBotId) {
                    cmd.OwnerId = attacker.Id;
                    cmd.dbLocationName = "";
                }

                // otherwise the item falls to the ground at that location
                else
                {
                    cmd.dbLocationName = victim.dbLocationName;
                    cmd.OwnerId = null;
                }
            }

            // animal item, only allow one at a time of any time
            else if (newItemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                Player dbVictim = playerRepo.Players.FirstOrDefault(p => p.Id == victim.Id);

                IEnumerable<ItemViewModel> AttackerExistingItems = GetAllPlayerItems(attacker.Id);


                // this player currently has no tamed pets, so give it to them auto equipped
                if (AttackerExistingItems.All(e => e.Item.ItemType != PvPStatics.ItemType_Pet) && attacker.BotId >= AIStatics.PsychopathBotId)
                {
                    cmd.OwnerId = attacker.Id;
                    cmd.IsEquipped = true;
                    cmd.dbLocationName = "";
                    SkillProcedures.UpdateItemSpecificSkillsToPlayer(attacker, cmd.dbName);
                }

                // otherwise the animal runs free
                else
                {

                    // bots can keep everything
                    if (attacker.BotId < AIStatics.RerolledPlayerBotId)
                    {
                        cmd.OwnerId = attacker.Id;
                        cmd.IsEquipped = true;
                        cmd.dbLocationName = "";
                    }
                    else
                    {
                        cmd.dbLocationName = victim.dbLocationName;
                        cmd.OwnerId = null;
                    }
                }

                playerRepo.SavePlayer(dbVictim);

            }


            DbStaticItem item = ItemStatics.GetStaticItem(targetForm.BecomesItemDbName);
           
            var newItemId = DomainRegistry.Repository.Execute(cmd);
            var ownerId = cmd.OwnerId;

            if (ownerId == null)
            {
                ownerId = -1;
            }

            ItemTransferLogProcedures.AddItemTransferLog(newItemId, (int)ownerId);

            output.LocationLog = "<br><b>" + victim.GetFullName() + " was completely transformed into a " + item.FriendlyName + " here.</b>";
            output.AttackerLog = "<br><b>You fully transformed " + victim.GetFullName() + " into a " + item.FriendlyName + "</b>!";
            output.VictimLog = "<br><b>You have been fully transformed into a " + item.FriendlyName + "!</b>";

            DomainRegistry.Repository.Execute(new DropAllItems { PlayerId = victim.Id });
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(victim);

            return output;
        }

        public static PlayerFormViewModel BeingWornBy(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            string victimName = player.FirstName + " " + player.LastName;

            Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == victimName);
            Player wearer = playerRepo.Players.FirstOrDefault(p => item.OwnerId == p.Id);

            if (wearer == null)
            {
                return null;
            }

            return PlayerProcedures.GetPlayerFormViewModel(wearer.Id); ;

        }

        public static string PlayerIsItemAtLocation(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
         
            string victimName = player.FirstName + " " + player.LastName;
            Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == victimName);

            Location place = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == item.dbLocationName);
       
            if (place==null) {
                return null;
            } else {
                return place.Name;
            }
            

        }

        public static string UseItem(int itemId, string membershipId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            ItemViewModel itemPlus = ItemProcedures.GetItemViewModel(itemId);

            Player owner = playerRepo.Players.FirstOrDefault(p => p.Id == itemPlus.dbItem.OwnerId);

            string name;

            // get pronoun
            if (membershipId == owner.MembershipId)
            {
                name = "You";
            }
            else
            {
                name = owner.GetFullName();
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
                if (!itemPlus.Item.GivesEffect.IsNullOrEmpty())
                {
                    // check if the player already has this effect or has it in cooldown.  If so, reject usage
                    if (EffectProcedures.PlayerHasEffect(owner, itemPlus.Item.GivesEffect))
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

                    // assert that this player is not in a duel
                    if (owner.InDuel > 0)
                    {
                        return "You must finish your duel before you can use this item.";
                    }

                    // assert that this player is not in a duel
                    if (owner.InQuest > 0)
                    {
                        return "You must finish your quest before you can use this item.";
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
                    return name + " take a quick blow with the whistle, sending a magical melody out into the air that should force any psychopathic spellslingers intent on taking you down to forget all about you, so long as you don't provoke them again...";
                }

                if (itemPlus.Item.dbName == BossProcedures.BossProcedures_BimboBoss.CureItemDbName)
                {

                    if (!EffectProcedures.PlayerHasEffect(owner, BossProcedures.BossProcedures_BimboBoss.KissEffectdbName)) {
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

                    // assert owner is not in the dungeon
                    if (owner.IsInDungeon())
                    {
                        return "The dark magic seeping through the dungeon prevents your crystal from working.";
                    }

                    // assert that this player is not in a duel
                    if (owner.InDuel > 0)
                    {
                        return "You must finish your duel before you can use this item.";
                    }

                    // assert that this player is not in a quest
                    if (owner.InQuest > 0)
                    {
                        return "You must finish your quest before you can use this item.";
                    }
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
                    if (myCov.HomeLocation.IsNullOrEmpty())
                    {
                        return "You are a member of your covenant, but unfortunately your covenant has not yet established a safeground to call home so you are unable to use this item.";
                    }

                    new Thread(() =>
                        StatsProcedures.AddStat(owner.MembershipId, StatsProcedures.Stat__CovenantCallbackCrystalsUsed, 1)
                    ).Start();

                    string output = PlayerProcedures.TeleportPlayer(owner, myCov.HomeLocation, true);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return output;
                }

                // spellbooks; these give the reader some spells they do not know.
                if (itemPlus.Item.dbName.Contains("item_consumable_spellbook_"))
                {
                    int amount = 4;
                    if (itemPlus.Item.dbName.Contains("small"))
                    {
                        amount = 4;
                    }
                    else if (itemPlus.Item.dbName.Contains("medium"))
                    {
                        amount = 8;
                    } if (itemPlus.Item.dbName.Contains("large"))
                    {
                        amount = 12;
                    }
                    if (itemPlus.Item.dbName.Contains("giant"))
                    {
                        amount = 16;
                    }
                    string output = "You learned the spells: " + SkillProcedures.GiveRandomFindableSkillsToPlayer(owner, amount) + " from reading your spellbook before it crumbles and vanishes into dust.";
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
                else if (itemPlus.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable && EffectProcedures.PlayerHasEffect(owner, itemPlus.Item.GivesEffect))
                {
                    return "You can't use this yet as you already have the effect active or on cooldown.";
                }
                else
                {
                    // add waiting time back on to the item
                    Item thisdbItem = itemRepo.Items.FirstOrDefault(i => i.Id == itemPlus.dbItem.Id);
                    thisdbItem.TurnsUntilUse = itemPlus.Item.UseCooldown;

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
                            return "You don't have enough willpower to use this item.";
                        }

                        owner.Mana = owner.Mana += itemPlus.Item.ReuseableManaRestore + bonusFromLevelMana;
                        if (owner.Mana > owner.Mana)
                        {
                            owner.Mana = owner.MaxMana;
                        }
                        if (owner.Mana < 0)
                        {
                            return "You don't have enough mana to use this item.";
                        }

                        itemRepo.SaveItem(thisdbItem);
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

                        itemRepo.SaveItem(thisdbItem);
                        playerRepo.SavePlayer(owner);

                        if (itemPlus.Item.dbName == "item_Inflatable_Sex_Doll_LexamTheGemFox")
                        {
                            new Thread(() =>
                                StatsProcedures.AddStat(owner.MembershipId, StatsProcedures.Stat__DollsWPRestored, (float)(itemPlus.Item.ReuseableHealthRestore + bonusFromLevel))
                            ).Start();
                        }

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

                        itemRepo.SaveItem(thisdbItem);
                        playerRepo.SavePlayer(owner);

                        return name + " consumed from a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.ReuseableManaRestore + bonusFromLevel) + " mana.  " + owner.Mana + "/" + owner.MaxMana + " Mana";
                    }

                    else if (!itemPlus.Item.GivesEffect.IsNullOrEmpty())
                    {
                        itemRepo.SaveItem(thisdbItem);
                        EffectProcedures.GivePerkToPlayer(itemPlus.Item.GivesEffect, owner);
                        DbStaticEffect effectPlus = EffectStatics.GetStaticEffect2(itemPlus.Item.GivesEffect);
                        if (owner.Gender == PvPStatics.GenderMale && !effectPlus.MessageWhenHit_M.IsNullOrEmpty())
                        {
                            return name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit_M;
                        }
                        else if (owner.Gender == PvPStatics.GenderFemale && !effectPlus.MessageWhenHit_F.IsNullOrEmpty())
                        {
                            return name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit_F;
                        }
                        else
                        {
                            return  name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit;
                        }
                        
                    }
                    itemRepo.SaveItem(thisdbItem);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.GetFullName() + " used a " + itemPlus.Item.FriendlyName + " here.");

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

        public static DbStaticForm GetFormFromItem(DbStaticItem item)
        {
            IDbStaticFormRepository formRepo = new EFDbStaticFormRepository();
            DbStaticForm form = formRepo.DbStaticForms.Where(f => f.BecomesItemDbName == item.dbName).FirstOrDefault();
            return form;
        }

        public static DbStaticItem GetRandomItemOfType(string itemtype)
        {
            if (itemtype == "random") return GetRandomPlayableItem();
            Random rand = new Random();
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            IEnumerable<DbStaticItem> item = itemRepo.DbStaticItems.Where(i => i.ItemType == itemtype && !i.IsUnique);
            int iCount = item.Count();
            if (iCount > 0)
            {
                return item.ElementAt(rand.Next(0, iCount));
            }
            else
            {
                return null;
            }
        }

        public static DbStaticItem GetRandomPlayableItem()
        {
            Random rand = new Random();
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            IEnumerable<DbStaticItem> item = itemRepo.DbStaticItems.Where(i => i.ItemType != "consumable" && !i.IsUnique);
            return item.ElementAt(rand.Next(0, item.Count()));
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

            return itemRepo.Items.Count(i => i.dbName == itemdbname && i.OwnerId == player.Id);
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
                if (!item.dbItem.IsPermanent)
                {
                    price *= .85M;
                }
           

                return Math.Ceiling(price);
            }

                // selling, pay less money
            else
            {
                decimal price = 50 + (30 * item.dbItem.Level * .75M);

                // item is not permanent, charge less
                if (!item.dbItem.IsPermanent)
                {
                    price *= .5M;
                }

                // item has custom sell value set and is consumable, use its sell override amount
                if (item.Item.ItemType == "consumable" && item.Item.MoneyValueSell > 0)
                {
                    price = item.Item.MoneyValueSell;
                }

                return Math.Ceiling(price);

            }
            
        }

        public static IEnumerable<SimpleItemLeaderboardViewModel> GetLeadingItemsSimple(int number)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IInanimateXPRepository xpRepo = new EFInanimateXPRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            IEnumerable<Item> items = itemRepo.Items.Where(i => i.VictimName != "" && !i.VictimName.Contains("Psychopath") && !i.VictimName.Contains("Donna Milton") && !i.VictimName.Contains("Lady Lovebringer") && !i.VictimName.Contains("Narcissa the Exiled")).OrderByDescending(i => i.Level).Take(number);

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

                var itemXP = xpRepo.InanimateXPs.FirstOrDefault(p => p.OwnerId == addme.PlayerId);
                if (itemXP != null)
                {
                    addme.ItemXP = itemXP.Amount;
                }
                else
                {
                    addme.ItemXP = 0;
                }

                output.Add(addme);
            }

            return output;

        }

        public static void LockItem(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == player.FirstName + " " + player.LastName);
            if (item == null)
            {
                return;
            }
            item.IsPermanent = true;
            itemRepo.SaveItem(item);
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

        public static IEnumerable<ItemViewModel> SortByItemType(IEnumerable<ItemViewModel> input)
        {
            List<ItemViewModel> inputList = input.ToList();
            List<ItemViewModel> output = new List<ItemViewModel>();


            // consumable types first
            foreach (ItemViewModel i in inputList.Where(i => i.Item.ItemType == "consumable").OrderBy(i => i.Item.FriendlyName))
            {
                output.Add(i);
                inputList.Remove(i);
            }

            // reusable consumable types second
            foreach (ItemViewModel i in inputList.Where(i => i.Item.ItemType == "consumable_reuseable").OrderBy(i => i.Item.FriendlyName))
            {
                output.Add(i);
                inputList.Remove(i);
            }


            // then all the rest
            foreach (ItemViewModel i in inputList.OrderBy(i => i.Item.FriendlyName))
            {
                output.Add(i);
                inputList.Remove(i);
            }

            return output;
        }

        public static void SetNickname(Player player, string nickname)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item playerItem = itemRepo.Items.FirstOrDefault(i => i.VictimName == player.FirstName + " " + player.LastName);
            playerItem.Nickname = nickname;
            itemRepo.SaveItem(playerItem);
        }

        public static void UpdateSouledItem(int id)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.Id == id);
            item.LastSouledTimestamp = DateTime.UtcNow;
            itemRepo.SaveItem(item);
        }

        public static void UpdateSouledItem(string firstName, string lastName)
        {
            IItemRepository itemRepo = new EFItemRepository();
            Item item = itemRepo.Items.FirstOrDefault(i => i.VictimName == firstName + " " + lastName);
            item.LastSouledTimestamp = DateTime.UtcNow;
            itemRepo.SaveItem(item);
        }

        /// <summary>
        /// Get the amount of XP required for an item or pet to reach the next level
        /// </summary>
        /// <param name="level">Current level of the item or pet</param>
        /// <returns></returns>
        public  static float GetXPRequiredForItemPetLevelup(int level)
        {

            float a = 11;
            float b = 0;
            float c = 89;

            float xp = a * level * level + b * level + c;
            float leftover = xp % 10;

            xp = (float)Math.Round(xp / 10) * 10;

            if (leftover != 0)
            {
                xp += 10;
            }

            return xp;
        }

    }

}