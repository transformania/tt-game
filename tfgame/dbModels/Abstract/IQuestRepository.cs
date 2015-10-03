using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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


        IQueryable<QuestStateRequirement> QuestStateRequirements { get; }

        void SaveQuestStateRequirement(QuestStateRequirement quest);

        void DeleteQuestStateRequirement(int QuestStateRequirementId);


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

    }
}