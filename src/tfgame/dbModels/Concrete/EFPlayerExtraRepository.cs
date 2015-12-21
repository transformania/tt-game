using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFPlayerExtraRepository : IPlayerExtraRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<PlayerExtra> PlayerExtras
        {
            get { return context.PlayerExtras; }
        }

        public void SavePlayerExtra(PlayerExtra PlayerExtra)
        {
            if (PlayerExtra.Id == 0)
            {
                context.PlayerExtras.Add(PlayerExtra);
            }
            else
            {
                PlayerExtra editMe = context.PlayerExtras.Find(PlayerExtra.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = PlayerExtra.Name;
                    // dbEntry.PlayerExtra = PlayerExtra.PlayerExtra;
                    // dbEntry.TimeStamp = PlayerExtra.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePlayerExtra(int id)
        {

            PlayerExtra dbEntry = context.PlayerExtras.Find(id);
            if (dbEntry != null)
            {
                context.PlayerExtras.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}