﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFServerLogRepository : IServerLogRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<ServerLog> ServerLogs
        {
            get { return context.ServerLogs; }
        }

        public void SaveServerLog(ServerLog ServerLog)
        {
            if (ServerLog.Id == 0)
            {
                context.ServerLogs.Add(ServerLog);
            }
            else
            {
                ServerLog editMe = context.ServerLogs.Find(ServerLog.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = ServerLog.Name;
                    // dbEntry.Message = ServerLog.Message;
                    // dbEntry.TimeStamp = ServerLog.TimeStamp;

                }
            }
            context.SaveChanges();
        }

        public void DeleteServerLog(int id)
        {

            ServerLog dbEntry = context.ServerLogs.Find(id);
            if (dbEntry != null)
            {
                context.ServerLogs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}