using System.Linq;
using tfgame.dbModels.Models;

namespace tfgame.dbModels.Abstract
{
    public interface IQuestRepository
    {

        IQueryable<QuestStart> QuestStarts { get; }

        void SaveQuestStart(QuestStart quest);

        void DeleteQuestStart(int QuestStartId);


        IQueryable<QuestState> QuestStates { get; }

        void SaveQuestState(QuestState quest);

        void DeleteQuestState(int QuestStateId);


        IQueryable<QuestConnection> QuestConnections { get; }

        void SaveQuestConnection(QuestConnection quest);

        void DeleteQuestConnection(int QuestConnectionId);


        IQueryable<QuestConnectionRequirement> QuestConnectionRequirements { get; }

        void SaveQuestConnectionRequirement(QuestConnectionRequirement quest);

        void DeleteQuestConnectionRequirement(int QuestConnectionRequirementId);


        IQueryable<QuestEnd> QuestEnds { get; }

        void SaveQuestEnd(QuestEnd quest);

        void DeleteQuestEnd(int QuestEndId);


        IQueryable<QuestStatePreaction> QuestStatePreactions { get; }

        void SaveQuestStatePreaction(QuestStatePreaction quest);

        void DeleteQuestStatePreaction(int QuestStatePreactionId);


        IQueryable<QuestWriterLog> QuestWriterLogs { get; }

        void SaveQuestWriterLog(QuestWriterLog quest);

        void DeleteQuestWriterLog(int QuestWriterLogId);


        IQueryable<QuestPlayerStatus> QuestPlayerStatuses { get; }

        void SaveQuestPlayerStatus(QuestPlayerStatus quest);

        void DeleteQuestPlayerStatus(int QuestPlayerStatusId);


        IQueryable<QuestPlayerVariable> QuestPlayerVariablees { get; }

        void SaveQuestPlayerVariable(QuestPlayerVariable quest);

        void DeleteQuestPlayerVariable(int QuestPlayerVariableId);


        IQueryable<QuestWriterPermission> QuestWriterPermissions { get; }

        void SaveQuestWriterPermission(QuestWriterPermission permission);

        void DeleteQuestWriterPermission(int QuestWriterPermissionId);

    }
}