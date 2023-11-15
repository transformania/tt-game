﻿using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Legacy.Services;
using TT.Domain.Models;
using TT.Domain.Statics;
using TT.Domain.Utilities;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{

    

    public static class FurnitureProcedures
    {

        public const double FurnitureContractVariation = .15D;

        public static IEnumerable<FurnitureViewModel> GetCovenantFurnitureViewModels(int covenantId = -1)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
          //  IDbStaticFurnitureRepository furnStaticRepo = new EFDbStaticFurnitureRepository();

            IEnumerable<FurnitureViewModel> output = from furniture in furnRepo.Furnitures
                                                     where furniture.CovenantId == covenantId
                                                     join staticFurniture in furnRepo.DbStaticFurniture on furniture.dbType equals staticFurniture.dbType
                                                     select new FurnitureViewModel
                                                     {
                                                         dbFurniture = new Furniture_VM
                                                         {
                                                             Id = furniture.Id,
                                                             dbType = furniture.dbType,
                                                             ContractStartTurn = furniture.ContractStartTurn,
                                                             ContractEndTurn = furniture.ContractEndTurn,
                                                             CovenantId = furniture.CovenantId,
                                                             ContractTurnDuration = furniture.ContractTurnDuration,
                                                             LastUsersIds = furniture.LastUsersIds,
                                                             LastUseTimestamp = furniture.LastUseTimestamp,
                                                             Price = furniture.Price,
                                                             HumanName = furniture.HumanName,
                                                         },

                                                         FurnitureType = new StaticFurniture
                                                         {
                                                             Id = staticFurniture.Id,
                                                             dbType = staticFurniture.dbType,
                                                             FriendlyName = staticFurniture.FriendlyName,
                                                             APReserveRefillAmount = staticFurniture.APReserveRefillAmount,
                                                             BaseContractTurnLength = staticFurniture.BaseContractTurnLength,
                                                             BaseCost = staticFurniture.BaseCost,
                                                             GivesEffectSourceId = staticFurniture.GivesEffectSourceId,
                                                             GivesItemSourceId = staticFurniture.GivesItemSourceId,
                                                             MinutesUntilReuse = staticFurniture.MinutesUntilReuse,
                                                             Description = staticFurniture.Description,
                                                             PortraitUrl = staticFurniture.PortraitUrl,
                                                             
                                                         }

                                                     };

            return output;
        }

        public static IEnumerable<FurnitureViewModel> GetAvailableFurnitureViewModels()
        {
            return GetCovenantFurnitureViewModels();
        }

        public static Furniture GetdbFurniture(int id)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
            return furnRepo.Furnitures.FirstOrDefault(f => f.Id == id);
        }

        public static void AddNewFurnitureToMarket(int count)
        {
            var furnRepo = new EFFurnitureRepository();

            var rand = new Random();

            for (var i = 0; i < count; i++)
            {
                var furnitureTypes = furnRepo.DbStaticFurniture.ToList();
                var max = furnitureTypes.Count();
                var num = rand.NextDouble();

                var index = Convert.ToInt32(Math.Floor(num * max));
                var furnitureType = furnitureTypes[index];

                var contractTurnRandomOffset = (int)(furnitureType.BaseContractTurnLength * ((rand.NextDouble() - .5) * 2) * FurnitureContractVariation);
                var basePriceRandomOffset = furnitureType.BaseCost * (decimal)((rand.NextDouble() - .5) * 2) * (decimal)FurnitureContractVariation;

                var newfurn = new Furniture
                {
                    dbType = furnitureType.dbType,
                    ContractTurnDuration = furnitureType.BaseContractTurnLength + contractTurnRandomOffset,
                    CovenantId = -1,
                    HumanName = $"{NameService.GetRandomFirstName()} {NameService.GetRandomLastName()} the {furnitureType.FriendlyName}",
                    Price = Math.Floor(furnitureType.BaseCost + basePriceRandomOffset),
                    LastUseTimestamp = DateTime.UtcNow,
                    ContractStartTurn = 0,
                    ContractEndTurn = 0,
                    LastUsersIds = ";",
                };

                furnRepo.SaveFurniture(newfurn);
            }
        }

        public static void GiveFurnitureToCovenant(Furniture furniture, Covenant covenant)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
            ICovenantRepository covRepo = new EFCovenantRepository();

            var dbCovenant = covRepo.Covenants.FirstOrDefault(c => c.Id == covenant.Id);
            var dbFurniture = furnRepo.Furnitures.First(f => f.Id == furniture.Id);

            dbCovenant.Money -= furniture.Price;
            dbFurniture.CovenantId = covenant.Id;

            // update the contract begin/end dates for this furniture
            dbFurniture.ContractStartTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            dbFurniture.ContractEndTurn = dbFurniture.ContractStartTurn + dbFurniture.ContractTurnDuration;

            covRepo.SaveCovenant(dbCovenant);
            furnRepo.SaveFurniture(dbFurniture);

            var message = "The leader of the covenant has purchased the furniture contract for " + dbFurniture.HumanName + ".";
            CovenantProcedures.WriteCovenantLog(message, covenant.Id, true);

        }

        public static void RemoveFurnitureToCovenant(Furniture furniture, Covenant covenant)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
            ICovenantRepository covRepo = new EFCovenantRepository();

            var dbCovenant = covRepo.Covenants.FirstOrDefault(c => c.Id == covenant.Id);
            var dbFurniture = furnRepo.Furnitures.First(f => f.Id == furniture.Id);

            dbCovenant.Money += (furniture.Price) / 4;
            dbFurniture.CovenantId = -1;

            // update the contract begin/end dates for this furniture
            dbFurniture.ContractStartTurn = 0;
            dbFurniture.ContractEndTurn = 0;

            covRepo.SaveCovenant(dbCovenant);
            furnRepo.SaveFurniture(dbFurniture);

            var message = "The leader of the covenant has sold the furniture contract for " + dbFurniture.HumanName + " for " + ((furniture.Price) / 4) + "ARP.";
            CovenantProcedures.WriteCovenantLog(message, covenant.Id, true);
        }

        public static double GetMinutesUntilReuse(FurnitureViewModel furniture) {

            var rechargeTime = (double)furniture.FurnitureType.MinutesUntilReuse;
            var minutesSinceLastUse = furniture.dbFurniture.LastUseTimestamp.Subtract(DateTime.UtcNow).TotalMinutes;
            return rechargeTime + minutesSinceLastUse;
        }

        public static double GetMinutesUntilReuse(Furniture furniture)
        {
             IFurnitureRepository furnRepo = new EFFurnitureRepository();
             var staticFurniture = furnRepo.DbStaticFurniture.FirstOrDefault(f => f.dbType == furniture.dbType);
             var rechargeTime = (double)staticFurniture.MinutesUntilReuse;
             var minutesSinceLastUse = furniture.LastUseTimestamp.Subtract(DateTime.UtcNow).TotalMinutes;
             return rechargeTime + minutesSinceLastUse;
        }

        public static string UseFurniture(int furnitureId, Player user)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
            var dbFurniture = furnRepo.Furnitures.FirstOrDefault(f => f.Id == furnitureId);
            dbFurniture.LastUseTimestamp = DateTime.UtcNow;
            dbFurniture.LastUsersIds = user.Id.ToString();
            furnRepo.SaveFurniture(dbFurniture);

            var furnitureStatic = furnRepo.DbStaticFurniture.FirstOrDefault(f => f.dbType == dbFurniture.dbType);

            var logMessage = "<b>" + user.GetFullName() + "</b> used <b>" + dbFurniture.HumanName + "</b>.";
            CovenantProcedures.WriteCovenantLog(logMessage, (int)user.Covenant, false);

            // furniture gives AP reserve bonus
            if (furnitureStatic.APReserveRefillAmount > 0)
            {
                IPlayerRepository playerRepo = new EFPlayerRepository();
                var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == user.Id);
                dbPlayer.ActionPoints_Refill += furnitureStatic.APReserveRefillAmount;
                if (dbPlayer.ActionPoints_Refill > TurnTimesStatics.GetActionPointReserveLimit())
                {
                    dbPlayer.ActionPoints_Refill = TurnTimesStatics.GetActionPointReserveLimit();
                }
                dbPlayer.LastActionTimestamp = DateTime.UtcNow;
                playerRepo.SavePlayer(dbPlayer);

                return "You used " + dbFurniture.HumanName + ", a human voluntarily transformed into furniture and leased by your covenant, and gained " + furnitureStatic.APReserveRefillAmount + " reserve action points.";
            }

            // furniture gives effect
            else if (furnitureStatic.GivesEffectSourceId != null)
            {
                EffectProcedures.GivePerkToPlayer(furnitureStatic.GivesEffectSourceId.Value, user);
                PlayerProcedures.SetTimestampToNow(user);
                return "You used " + dbFurniture.HumanName + ", a human voluntarily transformed into furniture and leased by your covenant, and gained the " + EffectStatics.GetDbStaticEffect(furnitureStatic.GivesEffectSourceId.Value).FriendlyName + " effect.";
            }

            //furniture gives item
            else if (furnitureStatic.GivesItemSourceId != null)
            {
                ItemProcedures.GiveNewItemToPlayer(user, furnitureStatic.GivesItemSourceId.Value);
                PlayerProcedures.SetTimestampToNow(user);
                var itemGained = ItemStatics.GetStaticItem(furnitureStatic.GivesItemSourceId.Value);
                return "You used " + dbFurniture.HumanName + ", a human voluntarily transformed into furniture and leased by your covenant, gaining a " + itemGained.FriendlyName + ".";
            }

            return "ERROR";
        }

        public static int MoveFurnitureOnMarket()
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();

            IEnumerable<Furniture> furnitureOnMarket = furnRepo.Furnitures.Where(f => f.CovenantId == -1).ToList();

            var rand = new Random();
            var amountToDelete = (int)Math.Floor(rand.NextDouble() * 2 + 1);
            var amountToAdd = (int)Math.Floor(rand.NextDouble() * 3 + 1);

            // delete some of the furniture currently available on the market if 
            IEnumerable<Furniture> furnitureToDelete;
            if (furnitureOnMarket.Count() > 5)
            {
                furnitureToDelete = furnitureOnMarket.Take(amountToDelete);
                foreach (var f in furnitureToDelete)
                {
                    furnRepo.DeleteFurniture(f.Id);
                }
            }

            // get the new count of furniture on market now
            furnitureOnMarket = furnRepo.Furnitures.Where(f => f.CovenantId == -1).ToList();

            if (furnitureOnMarket.Count() < 15)
            {
                AddNewFurnitureToMarket(amountToAdd);
            }


            return 1;
        }

    }
}