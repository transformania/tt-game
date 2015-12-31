using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface ILocationInfoRepository
    {

        IQueryable<LocationInfo> LocationInfos { get; }

        void SaveLocationInfo(LocationInfo LocationInfo);

        void DeleteLocationInfo(int LocationInfoId);

    }
}