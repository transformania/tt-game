﻿using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;


namespace TT.Domain.Concrete
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
                var editMe = context.GameshowStats.Find(GameshowStats.Id);
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

            var dbEntry = context.GameshowStats.Find(id);
            if (dbEntry != null)
            {
                context.GameshowStats.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}