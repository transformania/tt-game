using System.Linq;
using TT.Domain.Models;

namespace TT.Domain.Abstract
{
    public interface ILocationInfoRepository
    {

        IQueryable<LocationInfo> LocationInfos { get; }

        void SaveLocationInfo(LocationInfo LocationInfo);

        void DeleteLocationInfo(int LocationInfoId);

    }
}