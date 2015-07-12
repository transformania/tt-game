using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Queries.Statics
{
    public class GetStaticItem : QuerySingle<DbStaticItem>
    {
        public string DbName { get; set; }
        
        internal override DbStaticItem FindSingle()
        {
            IItemRepository itemRepo = new EFItemRepository();
            return itemRepo.DbStaticItems.FirstOrDefault(i => i.dbName == DbName);
        }
    }
}