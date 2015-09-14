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

    }
}