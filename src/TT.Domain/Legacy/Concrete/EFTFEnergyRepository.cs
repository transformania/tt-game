﻿using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;


namespace TT.Domain.Concrete
{
    public class EFTFEnergyRepository : ITFEnergyRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<Models.TFEnergy> TFEnergies
        {
            get { return context.TFEnergies; }
        }

        public void SaveTFEnergy(Models.TFEnergy TFEnergy)
        {
            if (TFEnergy.Id == 0)
            {
                context.TFEnergies.Add(TFEnergy);
            }
            else
            {
                var editMe = context.TFEnergies.Find(TFEnergy.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = TFEnergies.Name;
                    // dbEntry.Message = TFEnergies.Message;
                    // dbEntry.TimeStamp = TFEnergies.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteTFEnergy(int id)
        {

            var dbEntry = context.TFEnergies.Find(id);
            if (dbEntry != null)
            {
                context.TFEnergies.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}