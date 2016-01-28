using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFItemRepository : IItemRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Item> Items
        {
            get { return context.Items; }
        }

        public IQueryable<DbStaticItem> DbStaticItems
        {
            get { return context.DbStaticItems; }
        }

        public void SaveItem(Item Item)
        {
            if (Item.Id == 0)
            {
                context.Items.Add(Item);
            }
            else
            {
                Item editMe = context.Items.Find(Item.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Item.Name;
                    // dbEntry.Message = Item.Message;
                    // dbEntry.TimeStamp = Item.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteItem(int id)
        {

            Item dbEntry = context.Items.Find(id);
            if (dbEntry != null)
            {
                context.Items.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void SaveDbStaticItem(DbStaticItem DbStaticItem)
        {
            if (DbStaticItem.Id == 0)
            {
                context.DbStaticItems.Add(DbStaticItem);
            }
            else
            {
                DbStaticItem editMe = context.DbStaticItems.Find(DbStaticItem.Id);
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

            DbStaticItem dbEntry = context.DbStaticItems.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticItems.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}