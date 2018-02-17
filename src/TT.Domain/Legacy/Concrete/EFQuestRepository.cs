using System.Data.Entity.Core;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Models;

namespace TT.Domain.Concrete
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
                var editMe = context.QuestStarts.Find(QuestStart.Id);
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

            var dbEntry = context.QuestStarts.Find(id);
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
                var editMe = context.QuestStates.Find(QuestState.Id);
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

            var dbEntry = context.QuestStates.Find(id);
            if (dbEntry != null)
            {
                context.QuestStates.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestConnections
        public IQueryable<QuestConnection> QuestConnections
        {
            get { return context.QuestConnections; }
        }

        public void SaveQuestConnection(QuestConnection QuestConnection)
        {
            if (QuestConnection.Id == 0)
            {
                context.QuestConnections.Add(QuestConnection);
            }
            else
            {
                var editMe = context.QuestConnections.Find(QuestConnection.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestConnection.Name;
                    // dbEntry.Message = QuestConnection.Message;
                    // dbEntry.TimeStamp = QuestConnection.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestConnection);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestConnection(int id)
        {

            var dbEntry = context.QuestConnections.Find(id);
            if (dbEntry != null)
            {
                context.QuestConnections.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestConnectionRequirements
        public IQueryable<QuestConnectionRequirement> QuestConnectionRequirements
        {
            get { return context.QuestConnectionRequirements; }
        }

        public void SaveQuestConnectionRequirement(QuestConnectionRequirement QuestConnectionRequirement)
        {
            if (QuestConnectionRequirement.Id == 0)
            {
                context.QuestConnectionRequirements.Add(QuestConnectionRequirement);
            }
            else
            {
                var editMe = context.QuestConnectionRequirements.Find(QuestConnectionRequirement.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestConnectionRequirement.Name;
                    // dbEntry.Message = QuestConnectionRequirement.Message;
                    // dbEntry.TimeStamp = QuestConnectionRequirement.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestConnectionRequirement);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestConnectionRequirement(int id)
        {

            var dbEntry = context.QuestConnectionRequirements.Find(id);
            if (dbEntry != null)
            {
                context.QuestConnectionRequirements.Remove(dbEntry);
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
                var editMe = context.QuestEnds.Find(QuestEnd.Id);
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

            var dbEntry = context.QuestEnds.Find(id);
            if (dbEntry != null)
            {
                context.QuestEnds.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestStatePreactions
        public IQueryable<QuestStatePreaction> QuestStatePreactions
        {
            get { return context.QuestStatePreactions; }
        }

        public void SaveQuestStatePreaction(QuestStatePreaction QuestStatePreaction)
        {
            if (QuestStatePreaction.Id == 0)
            {
                context.QuestStatePreactions.Add(QuestStatePreaction);
            }
            else
            {
                var editMe = context.QuestStatePreactions.Find(QuestStatePreaction.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestStatePreaction.Name;
                    // dbEntry.Message = QuestStatePreaction.Message;
                    // dbEntry.TimeStamp = QuestStatePreaction.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestStatePreaction);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestStatePreaction(int id)
        {

            var dbEntry = context.QuestStatePreactions.Find(id);
            if (dbEntry != null)
            {
                context.QuestStatePreactions.Remove(dbEntry);
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
                var editMe = context.QuestWriterLogs.Find(QuestWriterLog.Id);
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

            var dbEntry = context.QuestWriterLogs.Find(id);
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
                var editMe = context.QuestPlayerStatuses.Find(QuestPlayerStatus.Id);
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

            var dbEntry = context.QuestPlayerStatuses.Find(id);
            if (dbEntry != null)
            {
                context.QuestPlayerStatuses.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

        #region QuestPlayerVariable
        public IQueryable<QuestPlayerVariable> QuestPlayerVariablees
        {
            get { return context.QuestPlayerVariables; }
        }

        public void SaveQuestPlayerVariable(QuestPlayerVariable QuestPlayerVariable)
        {
            if (QuestPlayerVariable.Id == 0)
            {
                context.QuestPlayerVariables.Add(QuestPlayerVariable);
            }
            else
            {
                var editMe = context.QuestPlayerVariables.Find(QuestPlayerVariable.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestPlayerVariable.Name;
                    // dbEntry.Message = QuestPlayerVariable.Message;
                    // dbEntry.TimeStamp = QuestPlayerVariable.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestPlayerVariable);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestPlayerVariable(int id)
        {

            var dbEntry = context.QuestPlayerVariables.Find(id);
            if (dbEntry != null)
            {
                context.QuestPlayerVariables.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

         #region QuestWriterPermission
        public IQueryable<QuestWriterPermission> QuestWriterPermissions
        {
            get { return context.QuestWriterPermissions; }
        }

        public void SaveQuestWriterPermission(QuestWriterPermission QuestWriterPermission)
        {
            if (QuestWriterPermission.Id == 0)
            {
                context.QuestWriterPermissions.Add(QuestWriterPermission);
            }
            else
            {
                var editMe = context.QuestWriterPermissions.Find(QuestWriterPermission.Id);
                if (editMe != null)
                {
                    // dbEntry.Name = QuestWriterPermission.Name;
                    // dbEntry.Message = QuestWriterPermission.Message;
                    // dbEntry.TimeStamp = QuestWriterPermission.TimeStamp;

                }
            }

            try
            {
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                //context.(RefreshMode.ClientWins, dbModels.Models.QuestWriterPermission);
                //context.SaveChanges();
            }

            // context.SaveChanges();
        }

        public void DeleteQuestWriterPermission(int id)
        {

            var dbEntry = context.QuestWriterPermissions.Find(id);
            if (dbEntry != null)
            {
                context.QuestWriterPermissions.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        #endregion

    }
}