using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;


namespace tfgame.dbModels.Concrete
{
    public class EFGameshowStatsRepository : IGameshowStatsRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<GameshowStats> GameshowStatss
        {
            get { return context.GameshowStats; }
        }

        public void SaveGameshowStats(GameshowStats GameshowStats)
        {
            if (GameshowStats.Id == 0)
            {
                context.GameshowStats.Add(GameshowStats);
            }
            else
            {
                GameshowStats editMe = context.GameshowStats.Find(GameshowStats.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = GameshowStats.Name;
                    // dbEntry.Message = GameshowStats.Message;
                    // dbEntry.TimeStamp = GameshowStats.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteGameshowStats(int id)
        {

            GameshowStats dbEntry = context.GameshowStats.Find(id);
            if (dbEntry != null)
            {
                context.GameshowStats.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}