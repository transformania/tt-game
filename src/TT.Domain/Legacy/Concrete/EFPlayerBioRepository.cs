﻿using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
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
                var editMe = context.PlayerBios.Find(PlayerBio.Id);
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

            var dbEntry = context.PlayerBios.Find(id);
            if (dbEntry != null)
            {
                context.PlayerBios.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}