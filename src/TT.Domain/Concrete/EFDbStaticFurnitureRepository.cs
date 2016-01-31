using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFDbStaticFurnitureRepository : IDbStaticFurnitureRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<DbStaticFurniture> DbStaticFurnitures
        {
            get { return context.DbStaticFurniture; }
        }

        public void SaveDbStaticFurniture(DbStaticFurniture DbStaticFurniture)
        {
            if (DbStaticFurniture.Id == 0)
            {
                context.DbStaticFurniture.Add(DbStaticFurniture);
            }
            else
            {
                DbStaticFurniture editMe = context.DbStaticFurniture.Find(DbStaticFurniture.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = DbStaticFurniture.Name;
                    // dbEntry.Message = DbStaticFurniture.Message;
                    // dbEntry.TimeStamp = DbStaticFurniture.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticFurniture(int id)
        {

            DbStaticFurniture dbEntry = context.DbStaticFurniture.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticFurniture.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}