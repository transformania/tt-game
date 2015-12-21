using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
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