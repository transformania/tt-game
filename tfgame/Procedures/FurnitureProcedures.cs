using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;

namespace tfgame.Procedures
{

    

    public static class FurnitureProcedures
    {

        public const double FurnitureContractVariation = .5D;

        public static IEnumerable<FurnitureViewModel> GetCovenantFurnitureViewModels(int covenantId)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
          //  IDbStaticFurnitureRepository furnStaticRepo = new EFDbStaticFurnitureRepository();

            IEnumerable<FurnitureViewModel> output = from f in furnRepo.Furnitures
                                                     where f.CovenantId == covenantId
                                                     join sf in furnRepo.DbStaticFurniture on f.dbType equals sf.dbType
                                                     select new FurnitureViewModel
                                                     {
                                                         dbFurniture = new Furniture_VM
                                                         {
                                                             Id = f.Id,
                                                             dbType = f.dbType,
                                                             ContractStartTurn = f.ContractStartTurn,
                                                             ContractEndTurn = f.ContractEndTurn,
                                                             CovenantId = f.CovenantId,
                                                             ContractTurnDuration = f.ContractTurnDuration,
                                                             LastUsersIds = f.LastUsersIds,
                                                             LastUseTimestamp = f.LastUseTimestamp,
                                                             Price = f.Price,
                                                             HumanName = f.HumanName,
                                                         },

                                                         FurnitureType = new StaticFurniture
                                                         {
                                                             Id = sf.Id,
                                                             dbType = sf.dbType,
                                                             FriendlyName = sf.FriendlyName,
                                                             APReserveRefillAmount = sf.APReserveRefillAmount,
                                                             BaseContractTurnLength = sf.BaseContractTurnLength,
                                                             BaseCost = sf.BaseCost,
                                                             GivesEffect = sf.GivesEffect,
                                                             GivesItem = sf.GivesItem,
                                                             MinutesUntilReuse = sf.MinutesUntilReuse,
                                                         }

                                                     };

            return output;
        }

        public static IEnumerable<FurnitureViewModel> GetAvailableFurnitureViewModels()
        {
            return GetCovenantFurnitureViewModels(-1);
        }

        public static void AddNewFurnitureToMarket()
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();

            // get a random furniture type
            IEnumerable<DbStaticFurniture> furnitureTypes = furnRepo.DbStaticFurniture;
            double max = furnitureTypes.Count();
            Random rand = new Random();
            double num = rand.NextDouble();

            int index = Convert.ToInt32(Math.Floor(num * max));
            DbStaticFurniture furnitureType = furnitureTypes.ElementAt(index);

            int turn = PvPWorldStatProcedures.GetWorldTurnNumber();
            int contractTurnRandomOffset = (int)(furnitureType.BaseContractTurnLength * ((rand.NextDouble() - .5) * 2) * FurnitureProcedures.FurnitureContractVariation);
            decimal basePriceRandomOffset = furnitureType.BaseCost * (decimal)((rand.NextDouble() - .5) * 2) * (decimal)FurnitureProcedures.FurnitureContractVariation;
            //string firstName = PlayerProcedures.R

            // get a random name
            List<string> names = new List<string>();
            var serializer = new XmlSerializer(typeof(List<string>));
            string path = HttpContext.Current.Server.MapPath("~/XMLs/FirstNames.xml");
            using (var reader = XmlReader.Create(path))
            {
                names = (List<string>)serializer.Deserialize(reader);
            }

            num = rand.NextDouble();

            string firstname = names.ElementAt((int)Math.Floor(num*names.Count()));

            string path2 = HttpContext.Current.Server.MapPath("~/XMLs/LastNames.xml");
            using (var reader = XmlReader.Create(path))
            {
                names = (List<string>)serializer.Deserialize(reader);
            }

            num = rand.NextDouble();

            string lastname = names.ElementAt((int)Math.Floor(num * names.Count()));

            Furniture newfurn = new Furniture
            {
                dbType = furnitureType.dbType,
                ContractTurnDuration = furnitureType.BaseContractTurnLength + contractTurnRandomOffset,
                CovenantId = -1,
                HumanName = firstname + " " + lastname + " the " + furnitureType.FriendlyName,
                Price = Math.Floor(furnitureType.BaseCost + basePriceRandomOffset),
                LastUseTimestamp = DateTime.UtcNow,
                ContractStartTurn = 0,
                ContractEndTurn = 0,
                LastUsersIds = ";",
            };

            furnRepo.SaveFurniture(newfurn);


        }

    }
}