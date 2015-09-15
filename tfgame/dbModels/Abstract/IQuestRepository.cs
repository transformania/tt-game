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

    }
}