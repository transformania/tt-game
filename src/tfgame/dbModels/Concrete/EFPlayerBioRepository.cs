using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFPlayerBioRepository : IPlayerBioRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<PlayerBio> PlayerBios
        {
            get { return context.PlayerBios; }
        }

        public void SavePlayerBio(PlayerBio PlayerBio)
        {
            if (PlayerBio.Id == 0)
            {
                context.PlayerBios.Add(PlayerBio);
            }
            else
            {
                PlayerBio editMe = context.PlayerBios.Find(PlayerBio.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = PlayerBio.Name;
                    // dbEntry.Message = PlayerBio.Message;
                    // dbEntry.TimeStamp = PlayerBio.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeletePlayerBio(int id)
        {

            PlayerBio dbEntry = context.PlayerBios.Find(id);
            if (dbEntry != null)
            {
                context.PlayerBios.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}