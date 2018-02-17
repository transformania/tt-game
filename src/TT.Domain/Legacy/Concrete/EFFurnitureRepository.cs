using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFFurnitureRepository : IFurnitureRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Furniture> Furnitures
        {
            get { return context.Furnitures; }
        }

        public IQueryable<DbStaticFurniture> DbStaticFurniture
        {
            get { return context.DbStaticFurniture; }
        }

        public void SaveFurniture(Furniture Furniture)
        {
            if (Furniture.Id == 0)
            {
                context.Furnitures.Add(Furniture);
            }
            else
            {
                var editMe = context.Furnitures.Find(Furniture.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Furniture.Name;
                    // dbEntry.Furniture = Furniture.Furniture;
                    // dbEntry.TimeStamp = Furniture.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteFurniture(int id)
        {

            var dbEntry = context.Furnitures.Find(id);
            if (dbEntry != null)
            {
                context.Furnitures.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void SaveDbStaticFurniture(DbStaticFurniture StaticFurniture)
        {
            if (StaticFurniture.Id == 0)
            {
                context.DbStaticFurniture.Add(StaticFurniture);
            }
            else
            {
                var editMe = context.DbStaticFurniture.Find(StaticFurniture.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = Furniture.Name;
                    // dbEntry.Furniture = Furniture.Furniture;
                    // dbEntry.TimeStamp = Furniture.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteDbStaticFurniture(int id)
        {

            var dbEntry = context.DbStaticFurniture.Find(id);
            if (dbEntry != null)
            {
                context.DbStaticFurniture.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}