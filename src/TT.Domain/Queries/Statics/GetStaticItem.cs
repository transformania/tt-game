using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Queries.Statics
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