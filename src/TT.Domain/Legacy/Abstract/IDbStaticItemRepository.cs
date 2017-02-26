using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IDbStaticItemRepository
    {

        IQueryable<DbStaticItem> DbStaticItems { get; }

        void SaveDbStaticItem(DbStaticItem DbStaticItem);

        void DeleteDbStaticItem(int DbStaticItemId);

    }
}