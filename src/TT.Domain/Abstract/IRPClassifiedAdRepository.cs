using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IRPClassifiedAdRepository
    {

        IQueryable<RPClassifiedAd> RPClassifiedAds { get; }

        void SaveRPClassifiedAd(RPClassifiedAd RPClassifiedAd);

        void DeleteRPClassifiedAd(int RPClassifiedAdId);

    }
}