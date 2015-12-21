using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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