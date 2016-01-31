using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDbStaticFurnitureRepository
    {

        IQueryable<DbStaticFurniture> DbStaticFurnitures { get; }

        void SaveDbStaticFurniture(DbStaticFurniture DbStaticFurniture);

        void DeleteDbStaticFurniture(int DbStaticFurnitureId);

    }
}