using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Concrete
{
    public class EFQuestRepository : IQuestRepository
    {
        private StatsContext context = new StatsContext();

        public IQueryable<QuestStart> QuestStarts
        {
            get { return context.QuestStarts; }
        }

        public void SaveQuestStart(QuestStart QuestStart)
        {
            if (QuestStart.Id == 0)
            {
                context.QuestStarts.Add(QuestStart);
            }
            else
            {
                QuestStart editMe = context.QuestStarts.Find(QuestStart.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestStart.Name;
                    // dbEntry.Message = QuestStart.Message;
                    // dbEntry.TimeStamp = QuestStart.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestStart);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestStart(int id)
        {

            QuestStart dbEntry = context.QuestStarts.Find(id);
            if (dbEntry != null)
            {
                context.QuestStarts.Remove(dbEntry);
                context.SaveChanges();
            }
        }

    }
}