using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;


namespace tfgame.dbModels.Concrete
{
    public class EFTFEnergyRepository : ITFEnergyRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<TFEnergy> TFEnergies
        {
            get { return context.TFEnergies; }
        }

        public void SaveTFEnergy(TFEnergy TFEnergy)
        {
            if (TFEnergy.Id == 0)
            {
                context.TFEnergies.Add(TFEnergy);
            }
            else
            {
                TFEnergy editMe = context.TFEnergies.Find(TFEnergy.Id);
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

            TFEnergy dbEntry = context.TFEnergies.Find(id);
            if (dbEntry != null)
            {
                context.TFEnergies.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}