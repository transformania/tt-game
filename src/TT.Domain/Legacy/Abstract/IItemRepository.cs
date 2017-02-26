using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IItemRepository
    {

        IQueryable<Item> Items { get; }

        IQueryable<DbStaticItem> DbStaticItems { get; }

        void SaveItem(Item Item);

        void DeleteItem(int ItemId);

        void SaveDbStaticItem(DbStaticItem Item);

        void DeleteDbStaticItem(int DbStaticItemId);

    }
}