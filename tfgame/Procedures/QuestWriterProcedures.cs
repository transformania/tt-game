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
            QuestState questState = repo.QuestStates.FirstOrDefault(q => q.QuestId == input.Id);

            bool newStart = false;

            if (questStart == null)
            {
                questStart = new QuestStart();
                newStart = true;
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

            // save an additional quest state to start this off
            if (newStart == true || questState == null)
            {
                questState = new QuestState
                {
                    QuestId = questStart.Id,
                    ParentQuestStateId = -1,
                    QuestStateName = "[ STARTING QUEST STATE ]",
                };
                repo.SaveQuestState(questState);

                questStart.StartState = questState.Id;
                repo.SaveQuestStart(questStart);
            }

           

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

            // always set this to something, even if it's just empty string
            if (questState.QuestStateName == null)
            {
                questState.QuestStateName = "-- QUEST STATE NOT NAMED --";
            }

            repo.SaveQuestState(questState);

            return -1;
        }

        public static int SaveQuestStateRequirement(QuestStateRequirement input, QuestState state)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestStateRequirement questStateRequirement = repo.QuestStateRequirements.FirstOrDefault(q => q.Id == input.Id);
            QuestState dbState = repo.QuestStates.FirstOrDefault(s => s.Id == state.Id);

            if (questStateRequirement == null)
            {
                questStateRequirement = new QuestStateRequirement();
            }

            questStateRequirement.Id = input.Id;
            questStateRequirement.Operator = input.Operator;
            questStateRequirement.RequirementType = input.RequirementType;
            questStateRequirement.RequirementValue = input.RequirementValue;
            questStateRequirement.VariabledbName = input.VariabledbName;
            questStateRequirement.QuestStateId = dbState;
            questStateRequirement.QuestStateRequirementName = input.QuestStateRequirementName;

            repo.SaveQuestStateRequirement(questStateRequirement);

            return questStateRequirement.Id;
        }

        public static int SaveQuestEnd(QuestEnd input, QuestState state)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestEnd questEnd = repo.QuestEnds.FirstOrDefault(q => q.Id == input.Id);
            QuestState dbState = repo.QuestStates.FirstOrDefault(s => s.Id == state.Id);

            if (questEnd == null)
            {
                questEnd = new QuestEnd();
            }

            questEnd.QuestId = input.QuestId;
            questEnd.QuestEndName = input.QuestEndName;
            questEnd.RewardAmount = input.RewardAmount;
            questEnd.RewardType = input.RewardType;
            questEnd.EndType = input.EndType;
            questEnd.QuestStateId = dbState;

            repo.SaveQuestEnd(questEnd);

            return questEnd.Id;
        }

        public static void DeleteQuestStateRequirement(int id)
        {
            IQuestRepository repo = new EFQuestRepository();
            repo.DeleteQuestStateRequirement(id);
        }

        public static void AddQuestWriterLog(string writer, string message)
        {
            IQuestRepository repo = new EFQuestRepository();
            QuestWriterLog log = new QuestWriterLog
            {
                Timestamp = DateTime.UtcNow,
                Text = message,
                User = writer

            };
            repo.SaveQuestWriterLog(log);
        }
    }
}