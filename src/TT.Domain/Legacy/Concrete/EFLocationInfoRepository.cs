﻿using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
{
    public class EFLocationInfoRepository : ILocationInfoRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<LocationInfo> LocationInfos
        {
            get { return context.LocationInfos; }
        }

        public void SaveLocationInfo(LocationInfo LocationInfo)
        {
            if (LocationInfo.Id == 0)
            {
                context.LocationInfos.Add(LocationInfo);
            }
            else
            {
                var editMe = context.LocationInfos.Find(LocationInfo.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = LocationInfo.Name;
                    // dbEntry.Message = LocationInfo.Message;
                    // dbEntry.TimeStamp = LocationInfo.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteLocationInfo(int id)
        {

            var dbEntry = context.LocationInfos.Find(id);
            if (dbEntry != null)
            {
                context.LocationInfos.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}