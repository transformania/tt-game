using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFFurnitureRepository : IFurnitureRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Furniture> Furnitures
        {
            get { return context.Furnitures; }
        }

        public void SaveFurniture(Furniture Furniture)
        {
            if (Furniture.Id == 0)
            {
                context.Furnitures.Add(Furniture);
            }
            else
            {
                Furniture editMe = context.Furnitures.Find(Furniture.Id);
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

            Furniture dbEntry = context.Furnitures.Find(id);
            if (dbEntry != null)
            {
                context.Furnitures.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}