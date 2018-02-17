using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFDbStaticItemRepository : IDbStaticItemRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<DbStaticItem> DbStaticItems
        {
            get { return context.DbStaticItems; }
        }

        public void SaveDbStaticItem(DbStaticItem DbStaticItem)
        {
            if (DbStaticItem.Id == 0)
            {
                context.DbStaticItems.Add(DbStaticItem);
            }
            else
            {
                var editMe = context.DbStaticItems.Find(DbStaticItem.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = DbStaticItem.Name;
                    // dbEntry.Message = DbStaticItem.Message;
                    // dbEntry.TimeStamp = DbStaticItem.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticItem(int id)
        {

            var dbEntry = context.DbStaticItems.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticItems.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}