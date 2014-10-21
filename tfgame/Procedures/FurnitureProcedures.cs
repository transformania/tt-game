using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.ViewModels;

namespace tfgame.Procedures
{
    public class FurnitureProcedures
    {
        public IEnumerable<FurnitureViewModel> GetFurnitureViewModel(int furnitureId)
        {
            IFurnitureRepository furnRepo = new EFFurnitureRepository();
            IDbStaticFurnitureRepository furnStaticRepo = new EFDbStaticFurnitureRepository();

            IEnumerable<FurnitureViewModel> output = from f in furnRepo.Furnitures
                                                      where f.Id == furnitureId
                                                     join sf in furnStaticRepo.DbStaticFurnitures on f.dbType equals sf.dbType
                                                      select new FurnitureViewModel
                                                      {
                                                          dbFurniture = new Furniture
                                                          {
                                                              Id = f.Id,
                                                           
                                                          },

                                                          FurnitureType = new DbStaticFurniture
                                                          {
                                                              Id = sf.Id,
                                                             
                                                          }

                                                      };

            return output;
        }
    }
}