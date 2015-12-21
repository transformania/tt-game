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

        IQueryable<DbStaticFurniture> DbStaticFurniture { get; }

        void SaveFurniture(Furniture Furniture);

        void DeleteFurniture(int FurnitureId);

        void SaveDbStaticFurniture(DbStaticFurniture StaticFurniture);

        void DeleteDbStaticFurniture(int StaticFurnitureId);

    }
}