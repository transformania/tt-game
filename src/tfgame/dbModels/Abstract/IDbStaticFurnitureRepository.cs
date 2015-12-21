using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDbStaticFurnitureRepository
    {

        IQueryable<DbStaticFurniture> DbStaticFurnitures { get; }

        void SaveDbStaticFurniture(DbStaticFurniture DbStaticFurniture);

        void DeleteDbStaticFurniture(int DbStaticFurnitureId);

    }
}