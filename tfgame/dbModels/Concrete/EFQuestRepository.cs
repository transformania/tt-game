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

        #region QuestStarts
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

        #endregion

        #region QuestStates
        public IQueryable<QuestState> QuestStates
        {
            get { return context.QuestStates; }
        }

        public void SaveQuestState(QuestState QuestState)
        {
            if (QuestState.Id == 0)
            {
                context.QuestStates.Add(QuestState);
            }
            else
            {
                QuestState editMe = context.QuestStates.Find(QuestState.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestState.Name;
                    // dbEntry.Message = QuestState.Message;
                    // dbEntry.TimeStamp = QuestState.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestState);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestState(int id)
        {

            QuestState dbEntry = context.QuestStates.Find(id);
            if (dbEntry != null)
            {
                context.QuestStates.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestStateRequirements
        public IQueryable<QuestStateRequirement> QuestStateRequirements
        {
            get { return context.QuestStateRequirements; }
        }

        public void SaveQuestStateRequirement(QuestStateRequirement QuestStateRequirement)
        {
            if (QuestStateRequirement.Id == 0)
            {
                context.QuestStateRequirements.Add(QuestStateRequirement);
            }
            else
            {
                QuestStateRequirement editMe = context.QuestStateRequirements.Find(QuestStateRequirement.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestStateRequirement.Name;
                    // dbEntry.Message = QuestStateRequirement.Message;
                    // dbEntry.TimeStamp = QuestStateRequirement.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestStateRequirement);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestStateRequirement(int id)
        {

            QuestStateRequirement dbEntry = context.QuestStateRequirements.Find(id);
            if (dbEntry != null)
            {
                context.QuestStateRequirements.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestEnd
        public IQueryable<QuestEnd> QuestEnds
        {
            get { return context.QuestEnds; }
        }

        public void SaveQuestEnd(QuestEnd QuestEnd)
        {
            if (QuestEnd.Id == 0)
            {
                context.QuestEnds.Add(QuestEnd);
            }
            else
            {
                QuestEnd editMe = context.QuestEnds.Find(QuestEnd.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestEnd.Name;
                    // dbEntry.Message = QuestEnd.Message;
                    // dbEntry.TimeStamp = QuestEnd.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestEnd);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestEnd(int id)
        {

            QuestEnd dbEntry = context.QuestEnds.Find(id);
            if (dbEntry != null)
            {
                context.QuestEnds.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestWriterLog
        public IQueryable<QuestWriterLog> QuestWriterLogs
        {
            get { return context.QuestWriterLogs; }
        }

        public void SaveQuestWriterLog(QuestWriterLog QuestWriterLog)
        {
            if (QuestWriterLog.Id == 0)
            {
                context.QuestWriterLogs.Add(QuestWriterLog);
            }
            else
            {
                QuestWriterLog editMe = context.QuestWriterLogs.Find(QuestWriterLog.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestWriterLog.Name;
                    // dbEntry.Message = QuestWriterLog.Message;
                    // dbEntry.TimeStamp = QuestWriterLog.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestWriterLog);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestWriterLog(int id)
        {

            QuestWriterLog dbEntry = context.QuestWriterLogs.Find(id);
            if (dbEntry != null)
            {
                context.QuestWriterLogs.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestPlayerStatus
        public IQueryable<QuestPlayerStatus> QuestPlayerStatuses
        {
            get { return context.QuestPlayerStatuses; }
        }

        public void SaveQuestPlayerStatus(QuestPlayerStatus QuestPlayerStatus)
        {
            if (QuestPlayerStatus.Id == 0)
            {
                context.QuestPlayerStatuses.Add(QuestPlayerStatus);
            }
            else
            {
                QuestPlayerStatus editMe = context.QuestPlayerStatuses.Find(QuestPlayerStatus.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestPlayerStatus.Name;
                    // dbEntry.Message = QuestPlayerStatus.Message;
                    // dbEntry.TimeStamp = QuestPlayerStatus.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestPlayerStatus);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestPlayerStatus(int id)
        {

            QuestPlayerStatus dbEntry = context.QuestPlayerStatuses.Find(id);
            if (dbEntry != null)
            {
                context.QuestPlayerStatuses.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

    }
}