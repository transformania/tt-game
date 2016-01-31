using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
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