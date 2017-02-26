using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface IRPClassifiedAdRepository
    {

        IQueryable<RPClassifiedAd> RPClassifiedAds { get; }

        void SaveRPClassifiedAd(RPClassifiedAd RPClassifiedAd);

        void DeleteRPClassifiedAd(int RPClassifiedAdId);

    }
}