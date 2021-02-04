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
using TT.Domain.Items.Commands;
using TT.Domain.Items.DTOs;
using TT.Domain.Items.Queries;
using TT.Domain.Players.Commands;
using TT.Domain.Players.Queries;

namespace TT.Domain.Procedures
{
    public class ItemProcedures
    {

        private const int Effect_SharpEyeSourceId = 18;
        private const int Effect_Spellsleuth = 194;
        private const int Effect_ApprenticeEnchanterLvl1 = 97;
        private const int Effect_ApprenticeEnchanterLvl2 = 98;
        private const int Effect_ApprenticeEnchanterLvl3 = 99;

        public static IEnumerable<ItemViewModel> GetAllPlayerItems(int playerId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IEnumerable<ItemViewModel> output = from item in itemRepo.Items
                                                      where item.OwnerId == playerId
                                                      join staticItem in itemRepo.DbStaticItems on item.ItemSourceId equals staticItem.Id
                                                      select new ItemViewModel
                                                      {
                                                          dbItem = new Item_VM
                                                          {
                                                             Id = item.Id,
                                                             ItemSourceId = item.ItemSourceId,
                                                             dbLocationName = item.dbLocationName,
                                                             EquippedThisTurn = item.EquippedThisTurn,
                                                             IsEquipped = item.IsEquipped,
                                                             IsPermanent = item.IsPermanent,
                                                             Level = item.Level,
                                                             OwnerId = item.OwnerId,
                                                             PvPEnabled = item.PvPEnabled,
                                                             TimeDropped = item.TimeDropped,
                                                             TurnsUntilUse = item.TurnsUntilUse,
                                                             LastSouledTimestamp = item.LastSouledTimestamp,
                                                             EmbeddedOnItemId = item.EmbeddedOnItemId,
                                                             SoulboundToPlayerId = item.SoulboundToPlayerId,
                                                             FormerPlayerId = item.FormerPlayerId
                                                          },



                                                          Item = new TT.Domain.ViewModels.StaticItem
                                                          {
                                                            Id = staticItem.Id,
                                                            FriendlyName = staticItem.FriendlyName,
                                                            Description = staticItem.Description,
                                                            PortraitUrl = staticItem.PortraitUrl,
                                                            MoneyValue = staticItem.MoneyValue,
                                                            MoneyValueSell = staticItem.MoneyValueSell,
                                                            ItemType = staticItem.ItemType,
                                                            ConsumableSubItemType = staticItem.ConsumableSubItemType,
                                                            UseCooldown = staticItem.UseCooldown,
                                                            UsageMessage_Item = staticItem.UsageMessage_Item,
                                                            UsageMessage_Player = staticItem.UsageMessage_Player,
                                                            Findable = staticItem.Findable,
                                                            FindWeight = staticItem.FindWeight,
                                                            GivesEffectSourceId = staticItem.GivesEffectSourceId,

                                                            HealthBonusPercent = staticItem.HealthBonusPercent,
                                                            ManaBonusPercent = staticItem.ManaBonusPercent,
                                                            ExtraSkillCriticalPercent = staticItem.ExtraSkillCriticalPercent,
                                                            HealthRecoveryPerUpdate = staticItem.HealthRecoveryPerUpdate,
                                                            ManaRecoveryPerUpdate = staticItem.ManaRecoveryPerUpdate,
                                                            SneakPercent = staticItem.SneakPercent,
                                                            EvasionPercent = staticItem.EvasionPercent,
                                                            EvasionNegationPercent = staticItem.EvasionNegationPercent,
                                                            MeditationExtraMana = staticItem.MeditationExtraMana,
                                                            CleanseExtraHealth = staticItem.CleanseExtraHealth,
                                                            MoveActionPointDiscount = staticItem.MoveActionPointDiscount,
                                                            SpellExtraHealthDamagePercent = staticItem.SpellExtraHealthDamagePercent,
                                                            SpellExtraTFEnergyPercent = staticItem.SpellExtraTFEnergyPercent,
                                                            CleanseExtraTFEnergyRemovalPercent = staticItem.CleanseExtraTFEnergyRemovalPercent,
                                                            SpellMisfireChanceReduction = staticItem.SpellMisfireChanceReduction,
                                                            SpellHealthDamageResistance = staticItem.SpellHealthDamageResistance,
                                                            SpellTFEnergyDamageResistance = staticItem.SpellTFEnergyDamageResistance,
                                                            ExtraInventorySpace = staticItem.ExtraInventorySpace,

                                                            Discipline = staticItem.Discipline,
                                                            Perception = staticItem.Perception,
                                                            Charisma = staticItem.Charisma,
                                                            Submission_Dominance = staticItem.Submission_Dominance,

                                                            Fortitude = staticItem.Fortitude,
                                                            Agility = staticItem.Agility,
                                                            Allure = staticItem.Allure,
                                                            Corruption_Purity  = staticItem.Corruption_Purity,

                                                            Magicka = staticItem.Magicka,
                                                            Succour = staticItem.Succour,
                                                            Luck = staticItem.Luck,
                                                            Chaos_Order = staticItem.Chaos_Order,


                                                            InstantHealthRestore = staticItem.InstantHealthRestore,
                                                            InstantManaRestore = staticItem.InstantManaRestore,
                                                            ReuseableHealthRestore = staticItem.ReuseableHealthRestore,
                                                            ReuseableManaRestore = staticItem.ReuseableManaRestore,

                                                            CurseTFFormSourceId = staticItem.CurseTFFormSourceId


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
            IEnumerable<ItemViewModel> output = from item in itemRepo.Items
                                                 where item.Id == id
                                                 join staticItem in itemRepo.DbStaticItems on item.ItemSourceId equals staticItem.Id
                                                 select new ItemViewModel
                                                 {
                                                     dbItem = new Item_VM
                                                     {
                                                         Id = item.Id,
                                                         ItemSourceId = item.ItemSourceId,
                                                         dbLocationName = item.dbLocationName,
                                                         EquippedThisTurn = item.EquippedThisTurn,
                                                         IsEquipped = item.IsEquipped,
                                                         IsPermanent = item.IsPermanent,
                                                         Level = item.Level,
                                                         OwnerId = item.OwnerId,
                                                         PvPEnabled = item.PvPEnabled,
                                                         TimeDropped = item.TimeDropped,
                                                         TurnsUntilUse = item.TurnsUntilUse,
                                                         LastSouledTimestamp = item.LastSouledTimestamp,
                                                         SoulboundToPlayerId = item.SoulboundToPlayerId,
                                                         FormerPlayerId = item.FormerPlayerId
                                                     },



                                                     Item = new TT.Domain.ViewModels.StaticItem
                                                     {
                                                         Id = staticItem.Id,
                                                         FriendlyName = staticItem.FriendlyName,
                                                         Description = staticItem.Description,
                                                         PortraitUrl = staticItem.PortraitUrl,
                                                         MoneyValue = staticItem.MoneyValue,
                                                         ItemType = staticItem.ItemType,
                                                         ConsumableSubItemType = staticItem.ConsumableSubItemType,
                                                         UseCooldown = staticItem.UseCooldown,
                                                         UsageMessage_Item = staticItem.UsageMessage_Item,
                                                         UsageMessage_Player = staticItem.UsageMessage_Player,
                                                         Findable = staticItem.Findable,
                                                         FindWeight = staticItem.FindWeight,
                                                         GivesEffectSourceId = staticItem.GivesEffectSourceId,

                                                         HealthBonusPercent = staticItem.HealthBonusPercent,
                                                         ManaBonusPercent = staticItem.ManaBonusPercent,
                                                         ExtraSkillCriticalPercent = staticItem.ExtraSkillCriticalPercent,
                                                         HealthRecoveryPerUpdate = staticItem.HealthRecoveryPerUpdate,
                                                         ManaRecoveryPerUpdate = staticItem.ManaRecoveryPerUpdate,
                                                         SneakPercent = staticItem.SneakPercent,
                                                         EvasionPercent = staticItem.EvasionPercent,
                                                         EvasionNegationPercent = staticItem.EvasionNegationPercent,
                                                         MeditationExtraMana = staticItem.MeditationExtraMana,
                                                         CleanseExtraHealth = staticItem.CleanseExtraHealth,
                                                         MoveActionPointDiscount = staticItem.MoveActionPointDiscount,
                                                         SpellExtraHealthDamagePercent = staticItem.SpellExtraHealthDamagePercent,
                                                         SpellExtraTFEnergyPercent = staticItem.SpellExtraTFEnergyPercent,
                                                         CleanseExtraTFEnergyRemovalPercent = staticItem.CleanseExtraTFEnergyRemovalPercent,
                                                         SpellMisfireChanceReduction = staticItem.SpellMisfireChanceReduction,
                                                         SpellHealthDamageResistance = staticItem.SpellHealthDamageResistance,
                                                         SpellTFEnergyDamageResistance = staticItem.SpellTFEnergyDamageResistance,
                                                         ExtraInventorySpace = staticItem.ExtraInventorySpace,

                                                         Discipline = staticItem.Discipline,
                                                         Perception = staticItem.Perception,
                                                         Charisma = staticItem.Charisma,
                                                         Submission_Dominance = staticItem.Submission_Dominance,

                                                         Fortitude = staticItem.Fortitude,
                                                         Agility = staticItem.Agility,
                                                         Allure = staticItem.Allure,
                                                         Corruption_Purity = staticItem.Corruption_Purity,

                                                         Magicka = staticItem.Magicka,
                                                         Succour = staticItem.Succour,
                                                         Luck = staticItem.Luck,
                                                         Chaos_Order = staticItem.Chaos_Order,


                                                         InstantHealthRestore = staticItem.InstantHealthRestore,
                                                         InstantManaRestore = staticItem.InstantManaRestore,
                                                         ReuseableHealthRestore = staticItem.ReuseableHealthRestore,
                                                         ReuseableManaRestore = staticItem.ReuseableManaRestore,

                                                         CurseTFFormSourceId = staticItem.CurseTFFormSourceId,
                                                     }

                                                 };

            return output.First();
        }

        public static bool PlayerIsCarryingTooMuch(Player newOwner, bool checkInventoryFull = false)
        {
            var carryWeight = DomainRegistry.Repository.FindSingle(new GetCurrentCarryWeight {PlayerId = newOwner.Id});

            var max = GetInventoryMaxSize(newOwner);

            if (checkInventoryFull)
            {
                return carryWeight >= max;
            }

            return carryWeight > max;
        }

        public static string GiveItemToPlayer(int itemId, int newOwnerId)
        {
            var item = DomainRegistry.Repository.FindSingle(new GetItem { ItemId = itemId });
            var owner = PlayerProcedures.GetPlayer(newOwnerId);

            if (owner.BotId == AIStatics.ActivePlayerBotId && item.PvPEnabled == (int)GameModeStatics.GameModes.Any)
            {
                if (owner.GameMode == (int)GameModeStatics.GameModes.PvP)
                {
                    item.PvPEnabled = (int)GameModeStatics.GameModes.PvP;
                }
                else
                {
                    item.PvPEnabled = (int)GameModeStatics.GameModes.Protection;
                }
            }
            DomainRegistry.Repository.Execute(new ChangeItemOwner {ItemId = item.Id, OwnerId = newOwnerId, GameMode = item.PvPEnabled});
            ItemTransferLogProcedures.AddItemTransferLog(itemId, newOwnerId);

            // if item is not an animal
            if (item.ItemSource.ItemType != PvPStatics.ItemType_Pet) { 
                return "You picked up a " + item.ItemSource.FriendlyName + " and put it into your inventory.";
            }

            // item is an animal
            else
            {
                if (item.FormerPlayer == null)
                {
                    return "You tame a " + item.ItemSource.FriendlyName + " and are now keeping them as a pet.";
                }
                else
                {
                    return "You tame " + item.FormerPlayer.FullName + " the " + item.ItemSource.FriendlyName + " and are now keeping them as a pet.";
                }
            }
        }

        public static string GiveNewItemToPlayer(Player player, DbStaticItem item)
        {

            var cmd = new CreateItem
            {
                OwnerId = player.Id,
                IsEquipped = false,
                dbLocationName = "",
                ItemSourceId = item.Id
            };

            if (player.BotId < AIStatics.ActivePlayerBotId)
            {
                cmd.PvPEnabled = -1;
            }
            else
            {
                if (player.GameMode == (int)GameModeStatics.GameModes.PvP)
                {
                    cmd.PvPEnabled = (int)GameModeStatics.GameModes.PvP;
                }
                else
                {
                    cmd.PvPEnabled = (int)GameModeStatics.GameModes.Protection;
                }
            }
            var newItemId = DomainRegistry.Repository.Execute(cmd);
            ItemTransferLogProcedures.AddItemTransferLog(newItemId, player.Id);
            return "You found a " + item.FriendlyName + "!";
        }

        public static string GiveNewItemToPlayer(Player player, int itemSourceId)
        {
            var i = ItemStatics.GetStaticItem(itemSourceId);
            return GiveNewItemToPlayer(player, i);
        }

        public static string DropItem(int itemId, string locationOverride = null)
        {
            var item = DomainRegistry.Repository.FindSingle( new GetItem { ItemId = itemId });

            var oldOwnerId = item.Owner.Id;

            var itemPlus = ItemStatics.GetStaticItem(item.ItemSource.Id);

            // dropped "item" is a pet so automatically unequip them
            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                EquipItem(itemId, false);
            }

            var cmd = new DropItem
            {
                OwnerId = item.Owner.Id,
                ItemId = item.Id,
                LocationOverride = locationOverride
            };
            DomainRegistry.Repository.Execute(cmd);

            ItemTransferLogProcedures.AddItemTransferLog(itemId, -1);
            SkillProcedures.UpdateItemSpecificSkillsToPlayer(PlayerProcedures.GetPlayer(oldOwnerId));

            // item is an animal
            if (itemPlus.ItemType == PvPStatics.ItemType_Pet)
            {
                if (item.FormerPlayer == null)
                {
                    return "You released the " + itemPlus.FriendlyName + " that you were keeping as a pet.";
                }
                else
                {
                    return "You released " + item.FormerPlayer.FullName + " the " + itemPlus.FriendlyName + " that you were keeping as a pet.";
                }
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
            var item = itemRepo.Items.FirstOrDefault(i => i.Id == itemId);
            var itemPlus = ItemStatics.GetStaticItem(item.ItemSourceId);

            IPlayerRepository playerRepo = new EFPlayerRepository();

            var dbOwner = playerRepo.Players.FirstOrDefault(p => p.Id == item.OwnerId);
            

            if (putOn)
            {
                item.IsEquipped = true;
                item.EquippedThisTurn = true;
                itemRepo.SaveItem(item);

                var targetbuffs = ItemProcedures.GetPlayerBuffs(dbOwner);

                dbOwner = PlayerProcedures.ReadjustMaxes(dbOwner, targetbuffs);
                playerRepo.SavePlayer(dbOwner);

                SkillProcedures.UpdateItemSpecificSkillsToPlayer(dbOwner, item.ItemSourceId);

                return "You put on your " + itemPlus.FriendlyName + ".";
         
            }
            else
            {
                item.IsEquipped = false;
                itemRepo.SaveItem(item);

                var targetbuffs = ItemProcedures.GetPlayerBuffs(dbOwner);

                dbOwner = PlayerProcedures.ReadjustMaxes(dbOwner, targetbuffs);
                playerRepo.SavePlayer(dbOwner);

                SkillProcedures.UpdateItemSpecificSkillsToPlayer(dbOwner);

                return "You took off your " + itemPlus.FriendlyName + ".";
              
            }

        }

        public static void ResetUseCooldown(ItemViewModel input)
        {
            IItemRepository itemRepo = new EFItemRepository();
            var dbItem = itemRepo.Items.FirstOrDefault(i => i.Id == input.dbItem.Id);
            dbItem.TurnsUntilUse = input.Item.UseCooldown;

            // these special reusable consumables can have a lower cooldown at higher levels
            if (dbItem.ItemSourceId == ItemStatics.ButtPlugItemSourceId || dbItem.ItemSourceId == ItemStatics.HotCoffeeMugItemSourceId)
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
            var itemsOfThisType = GetAllPlayerItems(playerId).Where(i => i.Item.ItemType == itemType && i.dbItem.IsEquipped);
            return itemsOfThisType.Count();
        }

        public static int PlayerTotalConsumableCount(int playerID)
        {
            var playerItems = GetAllPlayerItems(playerID).Where(i => i.dbItem.IsEquipped);
            var consumableTotal = playerItems.Count(i => (i.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable || i.Item.ItemType == PvPStatics.ItemType_Consumable) && i.dbItem.IsEquipped);
            return consumableTotal;
        }

        public static int PlayerIsWearingNumberOfThisExactItem(int playerId, int itemSourceId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            var itemsOfThisType = GetAllPlayerItems(playerId).Where(i => i.dbItem.ItemSourceId == itemSourceId && i.dbItem.IsEquipped);
            return itemsOfThisType.Count();
        }

        public static BuffBox GetPlayerBuffs(Player player)
        {
            return GetPlayerBuffs(player.Id);
        }

        public static BuffBox GetPlayerBuffs(int playerId)
        {

            var output = new BuffBox();

            // grab all of the bonuses coming from effects
            var myEffects = EffectProcedures.GetPlayerEffects2(playerId).Where(e => e.dbEffect.Duration > 0);
            var context = new StatsContext();
            var playerparam = new SqlParameter("PlayerId", SqlDbType.Int);
            playerparam.Value = playerId;
            var parameters = new object[] { playerparam };
            var query = context.Database.SqlQuery<BuffStoredProc>("exec [dbo].[GetPlayerBuffs] @PlayerId", parameters);

            foreach (var q in query)
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
            foreach (var eff in myEffects)
            {
                if (eff.dbEffect.EffectSourceId == Effect_SharpEyeSourceId)
                {
                    output.HasSearchDiscount = true;
                    //break;
                }
                else if (eff.dbEffect.EffectSourceId == Effect_Spellsleuth)
                {
                    output.FindSpellsOnly = true;
                }
                else if (eff.dbEffect.EffectSourceId == Effect_ApprenticeEnchanterLvl1)
                {
                    if (output.EnchantmentBoost < 1)
                    {
                        output.EnchantmentBoost = 1;
                    }

                }
                else if (eff.dbEffect.EffectSourceId == Effect_ApprenticeEnchanterLvl2)
                {
                    if (output.EnchantmentBoost < 2)
                    {
                        output.EnchantmentBoost = 2;
                    }
                }
                else if (eff.dbEffect.EffectSourceId == Effect_ApprenticeEnchanterLvl3)
                {
                    if (output.EnchantmentBoost < 3)
                    {
                        output.EnchantmentBoost = 3;
                    }
                }
            }

            return output;
        }

        public static int GetInventoryMaxSize(Player player)
        {
            return player.ExtraInventory + PvPStatics.MaxCarryableItemCountBase;
        }

        public static LogBox PlayerBecomesItem(Player victim, DbStaticForm targetForm, Player attacker)
        {
            var output = DomainRegistry.Repository.Execute(new PlayerBecomesItem { AttackerId = attacker?.Id, VictimId = victim.Id, NewFormId = targetForm.Id });

            IItemRepository itemRepo = new EFItemRepository();
            var item = itemRepo.Items.First(i => i.FormerPlayerId.Value == victim.Id);

            ItemTransferLogProcedures.AddItemTransferLog(item.Id, attacker?.Id);

            SkillProcedures.UpdateItemSpecificSkillsToPlayer(victim);

            return output;
        }

        public static PlayerFormViewModel BeingWornBy(Player player)
        {
            return BeingWornBy(player.Id);
        }

        public static PlayerFormViewModel BeingWornBy(int playerId)
        {
            var item = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer { PlayerId = playerId });

            return item.Owner == null ? null : PlayerProcedures.GetPlayerFormViewModel(item.Owner.Id);
        }

        public static (bool, string) UseItem(int itemId, string membershipId)
        {
            IItemRepository itemRepo = new EFItemRepository();
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var itemPlus = ItemProcedures.GetItemViewModel(itemId);

            var owner = playerRepo.Players.FirstOrDefault(p => p.Id == itemPlus.dbItem.OwnerId);

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

            #region single use consumables

            if (itemPlus.Item.ItemType == PvPStatics.ItemType_Consumable)
            {
                // Assert that the item is currently equipped
                if (!itemPlus.dbItem.IsEquipped)
                {
                    return (false, "You cannot use an item you do not have equipped.");
                }

                // if this is the grenade type, redirect to attack procedure
                if (itemPlus.Item.ConsumableSubItemType == (int)ItemStatics.ConsumableSubItemTypes.WillpowerBomb)
                {

                    if (owner.TimesAttackingThisUpdate >= PvPStatics.MaxAttacksPerUpdate)
                    {
                        return (false,"You have attacked too many times this update.");
                    }

                    // Disable splash orbs during chaos.
                    if (PvPStatics.ChaosMode)
                    {
                        return (false, "Maybe you should try throwing hands instead of throwing orbs.");
                    }

                    var output = "";
                    if (itemPlus.dbItem.ItemSourceId == ItemStatics.WillpowerBombWeakItemSourceId)
                    {
                        output = AttackProcedures.ThrowGrenade(owner, 200, "Weak");
                    }
                    else if (itemPlus.dbItem.ItemSourceId == ItemStatics.WillpowerBombStrongItemSourceId)
                    {
                        output = AttackProcedures.ThrowGrenade(owner, 300, "Strong");
                    }
                    else if (itemPlus.dbItem.ItemSourceId == ItemStatics.WillpowerBombVolatileItemSourceId)
                    {
                        output = AttackProcedures.ThrowGrenade(owner, 400, "Volatile");
                    }

                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return (true,output);

                }

                // if this item has an effect it grants, give that effect to the player
                if (itemPlus.Item.GivesEffectSourceId != null)
                {
                    // check if the player already has this effect or has it in cooldown.  If so, reject usage
                    if (EffectProcedures.PlayerHasEffect(owner, itemPlus.Item.GivesEffectSourceId.Value))
                    {
                        return (false, "You can't use this yet as you already have its effects active on you, or else the effect's cooldown has not expired.");
                    }
                    else
                    {
                        itemRepo.DeleteItem(itemPlus.dbItem.Id);
                        LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
                        return (true,EffectProcedures.GivePerkToPlayer(itemPlus.Item.GivesEffectSourceId.Value, owner));
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
                    return (true,name + " consumed a " + itemPlus.Item.FriendlyName + ", immediately restoring " + itemPlus.Item.InstantHealthRestore + " willpower.  " + owner.Health + "/" + owner.MaxHealth + " WP");

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
                    return (true, name + " consumed a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.InstantManaRestore) + " mana.  " + owner.Mana + "/" + owner.MaxMana + " Mana");
                }

                // special items time!
                if (itemPlus.dbItem.ItemSourceId == ItemStatics.SelfRestoreItemSourceId)
                {

                    if (owner.FormSourceId == owner.OriginalFormSourceId)
                    {
                        return (false,"You could use this lotion, but as you are already in your base form it wouldn't do you any good.");
                    }

                    // assert that this player is not in a duel
                    if (owner.InDuel > 0)
                    {
                        return (false,"You must finish your duel before you can use this item.");
                    }

                    // assert that this player is not in a duel
                    if (owner.InQuest > 0)
                    {
                        return (false, "You must finish your quest before you can use this item.");
                    }

                    PlayerProcedures.InstantRestoreToBase(owner);
                    TFEnergyProcedures.CleanseTFEnergies(owner, 25);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
                    return (true,name + " spread the mana-absorbing " + itemPlus.Item.FriendlyName + " over your skin, quickly restoring you to your original body and cleansing away some additional transformation energies.");
                }

                if (itemPlus.dbItem.ItemSourceId == ItemStatics.LullabyWhistleItemSourceId)
                {
                    AIDirectiveProcedures.DeaggroPsychopathsOnPlayer(owner);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.FirstName + " " + owner.LastName + " used a " + itemPlus.Item.FriendlyName + " here.");
                    return (true,name + " take a quick blow with the whistle, sending a magical melody out into the air that should force any psychopathic spellslingers intent on taking you down to forget all about you, so long as you don't provoke them again...");
                }

                if (itemPlus.dbItem.ItemSourceId == BossProcedures.BossProcedures_BimboBoss.CureItemSourceId)
                {

                    if (!EffectProcedures.PlayerHasEffect(owner, BossProcedures.BossProcedures_BimboBoss.KissEffectSourceId)) {
                        return (false,"Since you are not infected with the bimbonic virus there's no need for you to use this right now.");
                    }

                    PlayerProcedures.InstantRestoreToBase(owner);
                    EffectProcedures.RemovePerkFromPlayer(BossProcedures.BossProcedures_BimboBoss.KissEffectSourceId, owner);
                    EffectProcedures.GivePerkToPlayer(BossProcedures.BossProcedures_BimboBoss.CureEffectSourceId, owner);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return (true,"You inject yourself with the vaccine, returning to your original form and purging the virus out of your body.  Careful though, it may later mutate and be able to infect you once again...");
                }

                if (itemPlus.dbItem.ItemSourceId == ItemStatics.CovenantCrystalItemSourceId)
                {

                    // assert owner is not in the dungeon
                    if (owner.IsInDungeon())
                    {
                        return (false,"The dark magic seeping through the dungeon prevents your crystal from working.");
                    }

                    // assert that this player is not in a duel
                    if (owner.InDuel > 0)
                    {
                        return (false, "You must finish your duel before you can use this item.");
                    }

                    // assert that this player is not in a quest
                    if (owner.InQuest > 0)
                    {
                        return (false, "You must finish your quest before you can use this item.");
                    }
                    // assert owner has not been in combat recently
                    var minutesSinceCombat = Math.Abs(Math.Floor(owner.GetLastCombatTimestamp().Subtract(DateTime.UtcNow).TotalMinutes));
                    if (minutesSinceCombat < TurnTimesStatics.GetMinutesSinceLastCombatBeforeQuestingOrDuelling())
                    {
                        return (false, "Unfortunately you have been in combat too recently for the crystal to work.");
                    }

                    // assert owner is in a covenant
                    if (owner.Covenant == null || owner.Covenant < 1)
                    {
                        return (false, "Unfortunately as you are not in a covenant, you aren't able to use this item.");
                    }

                    // assert covenant has a safeground
                    var myCov = CovenantProcedures.GetDbCovenant((int)owner.Covenant);
                    if (myCov.HomeLocation.IsNullOrEmpty())
                    {
                        return (false, "You are a member of your covenant, but unfortunately your covenant has not yet established a safeground to call home so you are unable to use this item.");
                    }

                    StatsProcedures.AddStat(owner.MembershipId, StatsProcedures.Stat__CovenantCallbackCrystalsUsed, 1);

                    var output = PlayerProcedures.TeleportPlayer(owner, myCov.HomeLocation, true);
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return (true,output);
                }

                // spellbooks; these give the reader some spells they do not know.
                if (itemPlus.Item.ConsumableSubItemType == (int)ItemStatics.ConsumableSubItemTypes.Spellbook)
                {
                    var amount = 4;
                    if (itemPlus.dbItem.ItemSourceId == ItemStatics.SpellbookSmallItemSourceId)
                    {
                        amount = 4;
                    }
                    else if (itemPlus.dbItem.ItemSourceId == ItemStatics.SpellbookMediumItemSourceId)
                    {
                        amount = 8;
                    } if (itemPlus.dbItem.ItemSourceId == ItemStatics.SpellbookLargeItemSourceId)
                    {
                        amount = 12;
                    }
                    if (itemPlus.dbItem.ItemSourceId == ItemStatics.SpellbookGiantItemSourceId)
                    {
                        amount = 16;
                    }
                    var output = $"You learned the spells: {ListifyHelper.Listify(SkillProcedures.GiveRandomFindableSkillsToPlayer(owner, amount), true)} from reading your spellbook before it crumbles and vanishes into dust.";
                    itemRepo.DeleteItem(itemPlus.dbItem.Id);
                    return (true,output);
                }
            }

                #endregion

            #region reuseable consumables

            else if (itemPlus.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable)
            {
                // Assert that the item is currently equipped
                if (!itemPlus.dbItem.IsEquipped)
                {
                    return (false, "You cannot use an item you do not have equipped.");
                }
                // assert item is still not on cooldown
                if (itemPlus.dbItem.TurnsUntilUse > 0)
                {
                    return (false, "You have to wait longer before you can use this item agin.");
                }
                else if (itemPlus.Item.ItemType == PvPStatics.ItemType_Consumable_Reuseable
                    && itemPlus.Item.GivesEffectSourceId != null &&
                    EffectProcedures.PlayerHasEffect(owner, itemPlus.Item.GivesEffectSourceId.Value))
                {
                    return (false, "You can't use this yet as you already have the effect active or on cooldown.");
                }
                else
                {
                    // add waiting time back on to the item
                    var thisdbItem = itemRepo.Items.FirstOrDefault(i => i.Id == itemPlus.dbItem.Id);
                    thisdbItem.TurnsUntilUse = itemPlus.Item.UseCooldown;

                    // there is both a health and mana restoration / loss effect to this spell
                    if (itemPlus.Item.ReuseableHealthRestore != 0 && itemPlus.Item.ReuseableManaRestore != 0)
                    {
                        owner.Health += itemPlus.Item.ReuseableHealthRestore;
                        if (owner.Health > owner.MaxHealth)
                        {
                            owner.Health = owner.MaxHealth;
                        }
                        if (owner.Health < 0)
                        {
                            return (false, "You don't have enough willpower to use this item.");
                        }

                        owner.Mana = owner.Mana += itemPlus.Item.ReuseableManaRestore;
                        if (owner.Mana > owner.MaxMana)
                        {
                            owner.Mana = owner.MaxMana;
                        }
                        if (owner.Mana < 0)
                        {
                            return (false, "You don't have enough mana to use this item.");
                        }

                        itemRepo.SaveItem(thisdbItem);
                        playerRepo.SavePlayer(owner);

                        
                        

                        return (true, name + " used a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.ReuseableHealthRestore) + " willpower and " + (itemPlus.Item.ReuseableManaRestore) + " mana.  " + owner.Health + "/" + owner.MaxHealth + " WP, " + owner.Mana + "/" + owner.MaxMana + " Mana");

                    }

                    // just a health gain
                    else if (itemPlus.Item.ReuseableHealthRestore != 0)
                    {
                        owner.Health = owner.Health += itemPlus.Item.ReuseableHealthRestore;
                        if (owner.Health > owner.MaxHealth)
                        {
                            owner.Health = owner.MaxHealth;
                        }

                        itemRepo.SaveItem(thisdbItem);
                        playerRepo.SavePlayer(owner);

                        if (itemPlus.dbItem.ItemSourceId == ItemStatics.InflatableDollItemSourceId)
                        {
                            StatsProcedures.AddStat(owner.MembershipId, StatsProcedures.Stat__DollsWPRestored,
                                (float) (itemPlus.Item.ReuseableHealthRestore));
                        }

                        return (true, name + " consumed from a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.ReuseableHealthRestore) + " willpower.  " + owner.Health + "/" + owner.MaxHealth + " WP");
                    }

                   // just a mana gain
                    else if (itemPlus.Item.ReuseableManaRestore != 0)
                    {
                        owner.Mana = owner.Mana += itemPlus.Item.ReuseableManaRestore;
                        if (owner.Mana > owner.MaxMana)
                        {
                            owner.Mana = owner.MaxMana;
                        }

                        itemRepo.SaveItem(thisdbItem);
                        playerRepo.SavePlayer(owner);

                        return (true, name + " consumed from a " + itemPlus.Item.FriendlyName + ", immediately restoring " + (itemPlus.Item.ReuseableManaRestore) + " mana.  " + owner.Mana + "/" + owner.MaxMana + " Mana");
                    }

                    else if (itemPlus.Item.GivesEffectSourceId != null)
                    {
                        itemRepo.SaveItem(thisdbItem);
                        EffectProcedures.GivePerkToPlayer(itemPlus.Item.GivesEffectSourceId.Value, owner);
                        var effectPlus = EffectStatics.GetDbStaticEffect(itemPlus.Item.GivesEffectSourceId.Value);
                        if (owner.Gender == PvPStatics.GenderMale && !effectPlus.MessageWhenHit_M.IsNullOrEmpty())
                        {
                            return (true, name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit_M);
                        }
                        else if (owner.Gender == PvPStatics.GenderFemale && !effectPlus.MessageWhenHit_F.IsNullOrEmpty())
                        {
                            return (true, name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit_F);
                        }
                        else
                        {
                            return (true, name + " used a " + itemPlus.Item.FriendlyName + ".  " + effectPlus.MessageWhenHit);
                        }
                        
                    }
                    itemRepo.SaveItem(thisdbItem);
                    LocationLogProcedures.AddLocationLog(owner.dbLocationName, owner.GetFullName() + " used a " + itemPlus.Item.FriendlyName + " here.");

                }
            }

           

          

         

            #endregion

            return (true,"");
        }

        public static DbStaticItem GetRandomFindableItem()
        {

            var eligibleItems = ItemStatics.GetAllFindableItems();

            var RollMax = eligibleItems.Select(i => i.FindWeight).Sum();

            var randomItemIndex2 = new Random(DateTime.Now.Millisecond);
            var roll2 = randomItemIndex2.NextDouble() * RollMax;

            var currentIndex = 0;

            var iteratedThrough = new List<StaticItem>();

            foreach (var item in eligibleItems.ToList())
            {

                var lowRange = eligibleItems.Take(currentIndex).Sum(i => i.FindWeight);
                var highRange = eligibleItems.Take(currentIndex+1).Sum(i => i.FindWeight);

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
            var form = formRepo.DbStaticForms.Where(f => f.ItemSourceId == item.Id).FirstOrDefault();
            return form;
        }

        public static DbStaticItem GetRandomItemOfType(string itemtype)
        {
            if (itemtype == "random") return GetRandomPlayableItem();
            var rand = new Random();
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            IEnumerable<DbStaticItem> item = itemRepo.DbStaticItems.Where(i => i.ItemType == itemtype && !i.IsUnique);
            var iCount = item.Count();
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
            var rand = new Random();
            IDbStaticItemRepository itemRepo = new EFDbStaticItemRepository();
            IEnumerable<DbStaticItem> item = itemRepo.DbStaticItems.Where(i => i.ItemType != PvPStatics.ItemType_Consumable && !i.IsUnique);
            return item.ElementAt(rand.Next(0, item.Count()));
        }

        public static int PlayerHasNumberOfThisItem(Player player, int itemSourceId)
        {
            IItemRepository itemRepo = new EFItemRepository();

            return itemRepo.Items.Count(i => i.ItemSourceId == itemSourceId && i.OwnerId == player.Id);
        }

        public static void DeleteItem(int id)
        {
            IItemRepository itemRepo = new EFItemRepository();

            var item = itemRepo.Items.FirstOrDefault(i => i.Id == id);

            itemRepo.DeleteItem(item.Id);
        }

        public static void DeleteItemOfItemSourceId(Player player, int itemSourceId)
        {
            IItemRepository itemRepo = new EFItemRepository();

            var item = itemRepo.Items.FirstOrDefault(i => i.ItemSourceId == itemSourceId && i.OwnerId == player.Id);

            itemRepo.DeleteItem(item.Id);

        }

        public static decimal GetCostOfItem(ItemDetail item, string buyOrSell)
        {

            if (item.ItemSource.ItemType == PvPStatics.ItemType_Consumable || item.ItemSource.ItemType == PvPStatics.ItemType_Rune)
            {
                if (buyOrSell == "buy")
                {
                    return Math.Ceiling(item.ItemSource.MoneyValue);
                }
                else
                {
                    // override with custom value
                    if (item.ItemSource.MoneyValueSell > 0)
                    {
                        return item.ItemSource.MoneyValueSell;
                    }

                    return Math.Floor(item.ItemSource.MoneyValue * (decimal)ItemStatics.GetSellValueModifier(item.ItemSource.ItemType));
                }
            }

            var runesValue = item.Runes.Select(r => r.ItemSource.MoneyValue).Sum();

            if (buyOrSell == "buy")
            {
                decimal price = 50 + 50 * item.Level;

                // item is not permanent, charge less
                if (!item.IsPermanent)
                {
                    return Math.Floor(price * .85M);
                }


                return Math.Ceiling(price + runesValue);
            }

            // selling, pay less money
            else
            {
                var price = 50 + (10 * item.Level);

                if (price >= Int32.MaxValue)
                {
                    price = 999999999;
                }

                // item is not permanent, charge less
                if (!item.IsPermanent)
                {
                    return Math.Floor(price * .5M);
                }

                return Math.Floor(price + runesValue * (decimal) ItemStatics.GetSellValueModifier(PvPStatics.ItemType_Rune));

            }

        }

        public static bool ItemIncursDungeonPenalty(ItemDetail item)
        {
            // Give items/pets a struggle penalty if their owner is in the dungeon
            if (item.Owner != null)
            {
                var owner = PlayerProcedures.GetPlayer(item.Owner.Id);
                if (owner.IsInDungeon())
                {
                    return true;
                }
            }

            return false;
        }

        public static void LockItem(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
            var itemHack = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = player.Id});
            var item = itemRepo.Items.FirstOrDefault(i => i.Id == itemHack.Id); // TODO: Replace with proper command
            if (item == null)
            {
                return;
            }
            item.IsPermanent = true;
            itemRepo.SaveItem(item);
        }

        public static bool PlayerHasReadBook(Player player, int bookItemSourceId)
        {
            IBookReadingRepository repo = new EFBookReadingRepository();

            var reading = repo.BookReadings.FirstOrDefault(b => b.PlayerId == player.Id && b.BookItemSourceId == bookItemSourceId);

            if (reading == null)
            {
                return false;
            }
            else
            {
                return true;
            }

            
        }

        public static void AddBookReading(Player player, int bookItemSourceId)
        {
            IBookReadingRepository repo = new EFBookReadingRepository();

            var reading = repo.BookReadings.FirstOrDefault(b => b.PlayerId == player.Id && b.BookItemSourceId == bookItemSourceId);

            if (reading == null)
            {
                var newReading = new BookReading
                {
                    PlayerId = player.Id,
                    BookItemSourceId = bookItemSourceId,
                    Timestamp = DateTime.UtcNow,
                };
                repo.SaveBookReading(newReading);
            }
            
        }

        public static void UpdateSouledItem(int id)
        {
            IItemRepository itemRepo = new EFItemRepository();
            var item = itemRepo.Items.FirstOrDefault(i => i.Id == id);
            item.LastSouledTimestamp = DateTime.UtcNow;
            itemRepo.SaveItem(item);
        }

        public static void UpdateSouledItem(Player player)
        {
            IItemRepository itemRepo = new EFItemRepository();
            var itemHack = DomainRegistry.Repository.FindSingle(new GetItemByFormerPlayer {PlayerId = player.Id});
            var item = itemRepo.Items.FirstOrDefault(i => i.Id == itemHack.Id); // TODO: Replace with proper command
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

            var xp = a * level * level + b * level + c;
            var leftover = xp % 10;

            xp = (float)Math.Round(xp / 10) * 10;

            if (leftover != 0)
            {
                xp += 10;
            }

            return xp;
        }

    }

}