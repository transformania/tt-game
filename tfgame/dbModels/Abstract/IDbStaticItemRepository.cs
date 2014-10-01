using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IDbStaticItemRepository
    {

        IQueryable<DbStaticItem> DbStaticItems { get; }

        void SaveDbStaticItem(DbStaticItem DbStaticItem);

        void DeleteDbStaticItem(int DbStaticItemId);

    }
}