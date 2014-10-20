using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IFurnitureRepository
    {

        IQueryable<Furniture> Furnitures { get; }

        void SaveFurniture(Furniture Furniture);

        void DeleteFurniture(int FurnitureId);

    }
}