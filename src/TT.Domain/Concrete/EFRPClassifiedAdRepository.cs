using System.Linq;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFRPClassifiedAdsRepository : IRPClassifiedAdRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<RPClassifiedAd> RPClassifiedAds
        {
            get { return context.RPClassifiedAds; }
        }

        public void SaveRPClassifiedAd(RPClassifiedAd RPClassifiedAds)
        {
            if (RPClassifiedAds.Id == 0)
            {
                context.RPClassifiedAds.Add(RPClassifiedAds);
            }
            else
            {
                RPClassifiedAd editMe = context.RPClassifiedAds.Find(RPClassifiedAds.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = RPClassifiedAds.Name;
                    // dbEntry.Message = RPClassifiedAds.Message;
                    // dbEntry.TimeStamp = RPClassifiedAds.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteRPClassifiedAd(int id)
        {

            RPClassifiedAd dbEntry = context.RPClassifiedAds.Find(id);
            if (dbEntry != null)
            {
                context.RPClassifiedAds.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}