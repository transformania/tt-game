using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;

namespace tfgame.Procedures
{
    public static class QuestWriterProcedures
    {

        public static int SaveQuestStart(QuestStart input)
        {

            IQuestRepository repo = new EFQuestRepository();

            QuestStart questStart = repo.QuestStarts.FirstOrDefault(q => q.Id == input.Id);

            if (questStart == null)
            {
                questStart = new QuestStart();
            }
            questStart.dbName = input.dbName;
            questStart.IsLive = false;
            questStart.Location = input.Location;
            questStart.MaxStartLevel = input.MaxStartLevel;
            questStart.MaxStartTurn = input.MaxStartTurn;
            questStart.MinStartLevel = input.MinStartLevel;
            questStart.MinStartTurn = input.MinStartTurn;
            questStart.Name = input.Name;
            questStart.PrerequisiteQuest = input.RequiredGender;
            questStart.RequiredGender = input.RequiredGender;
            questStart.StartState = input.StartState;

            repo.SaveQuestStart(questStart);

            return questStart.Id;
        }

        public static int SaveQuestState(QuestState input)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestState questState = repo.QuestStates.FirstOrDefault(q => q.Id == input.Id);

            if (questState == null)
            {
                questState = new QuestState();
            }

            questState.Id = input.Id;
            questState.ChoiceText = input.ChoiceText;
            questState.ParentQuestStateId = input.ParentQuestStateId;
            questState.QuestEndId = input.QuestEndId;
            questState.QuestStateName = input.QuestStateName;
            questState.Text = input.Text;
            questState.QuestId = input.QuestId;

            repo.SaveQuestState(questState);

            return -1;
        }

    }
}