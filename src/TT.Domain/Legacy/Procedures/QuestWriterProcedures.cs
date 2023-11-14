﻿using System;
using System.Collections.Generic;
using System.Linq;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;

namespace TT.Domain.Procedures
{
    public static class QuestWriterProcedures
    {

        public static int SaveQuestStart(QuestStart input)
        {

            IQuestRepository repo = new EFQuestRepository();

            var questStart = repo.QuestStarts.FirstOrDefault(q => q.Id == input.Id);
            var questState = repo.QuestStates.FirstOrDefault(q => q.QuestId == input.Id);

            var newStart = false;

            if (questStart == null)
            {
                questStart = new QuestStart();
                newStart = true;
            }
            questStart.dbName = input.dbName;
            questStart.Location = input.Location;
            questStart.MaxStartLevel = input.MaxStartLevel;
            questStart.MaxStartTurn = input.MaxStartTurn;
            questStart.MinStartLevel = input.MinStartLevel;
            questStart.MinStartTurn = input.MinStartTurn;
            questStart.Name = input.Name;
            questStart.PrerequisiteQuest = input.PrerequisiteQuest;
            questStart.PrerequisiteForm = input.PrerequisiteForm;
            questStart.LockoutQuest = input.LockoutQuest;
            questStart.RequiredGender = input.RequiredGender;
            questStart.StartState = input.StartState;
            questStart.Tags = input.Tags;
            questStart.Description = input.Description;

            repo.SaveQuestStart(questStart);

            // save an additional quest state to start this off
            if (newStart || questState == null)
            {
                questState = new QuestState
                {
                    QuestId = questStart.Id,
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

            var questState = repo.QuestStates.FirstOrDefault(q => q.Id == input.Id);

            if (questState == null)
            {
                questState = new QuestState();
            }

            questState.Id = input.Id;
            questState.QuestEndId = input.QuestEndId;
            questState.QuestStateName = input.QuestStateName;
            questState.Text = input.Text;
            questState.QuestId = input.QuestId;
            questState.HideIfRequirementsNotMet = input.HideIfRequirementsNotMet;
            questState.Notes = input.Notes;
            questState.PinToDiagram = input.PinToDiagram;
            questState.X = input.X;
            questState.Y = input.Y;

            // always set this to something, even if it's just empty string
            if (questState.QuestStateName == null)
            {
                questState.QuestStateName = "-- QUEST STATE NOT NAMED --";
            }

            repo.SaveQuestState(questState);

            return questState.Id;
        }

        public static void DeleteQuestState(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();
            var questState = repo.QuestStates.FirstOrDefault(q => q.Id == Id);

            // dangle any from connections connecting TO this state
            var connections = repo.QuestConnections.Where(q => q.QuestStateToId == questState.Id).ToList();
            foreach(var q in connections)
            {
                q.QuestStateToId = -1;
                repo.SaveQuestConnection(q);
            }

            // dangle any from connections connecting TO this state
            connections = repo.QuestConnections.Where(q => q.QuestStateFromId == questState.Id).ToList();
            foreach (var q in connections)
            {
                q.QuestStateFromId = -1;
                repo.SaveQuestConnection(q);
            }

            // delete any ends on this quest state
            var ends = repo.QuestEnds.Where(q => q.QuestStateId.Id == questState.Id).ToList();
            foreach (var q in ends)
            {
                repo.DeleteQuestEnd(q.Id);
            }

            // delete any quest state preactions on this quest state
            var preactions = repo.QuestStatePreactions.Where(q => q.QuestStateId.Id == questState.Id).ToList();
            foreach (var q in preactions)
            {
                repo.DeleteQuestStatePreaction(q.Id);
            }

            // finally actually delete this state
            repo.DeleteQuestState(Id);

        }

        public static int SaveQuestConnection(QuestConnection input)
        {
            IQuestRepository repo = new EFQuestRepository();

            var questConnection = repo.QuestConnections.FirstOrDefault(q => q.Id == input.Id);

            if (questConnection == null)
            {
                questConnection = new QuestConnection();
            }

            questConnection.ActionName = input.ActionName;
            questConnection.ConnectionName = input.ConnectionName;
            questConnection.HideIfRequirementsNotMet = input.HideIfRequirementsNotMet;
            questConnection.QuestId = input.QuestId;
            questConnection.QuestStateFromId = input.QuestStateFromId;
            questConnection.QuestStateToId = input.QuestStateToId;
            questConnection.QuestStateFailToId = input.QuestStateFailToId;
            questConnection.RankInList = input.RankInList;
            questConnection.Notes = input.Notes;
            questConnection.Text = input.Text;

            // always set this to something, even if it's just empty string
            if (questConnection.ConnectionName == null)
            {
                questConnection.ConnectionName = "-- QUEST CONNECTION NOT NAMED --";
            }

            repo.SaveQuestConnection(questConnection);

            return questConnection.Id;
        }

        public static int SaveQuestConnectionRequirement(QuestConnectionRequirement input, QuestConnection connection)
        {
            IQuestRepository repo = new EFQuestRepository();

            var QuestConnectionRequirement = repo.QuestConnectionRequirements.FirstOrDefault(q => q.Id == input.Id);
           // QuestState dbState = repo.QuestStates.FirstOrDefault(s => s.Id == state.Id);

            if (QuestConnectionRequirement == null)
            {
                QuestConnectionRequirement = new QuestConnectionRequirement
                {
                    QuestId = connection.QuestId,
                };
            }
            QuestConnectionRequirement.Id = input.Id;
            QuestConnectionRequirement.Operator = input.Operator;
            QuestConnectionRequirement.RequirementType = input.RequirementType;
            QuestConnectionRequirement.RequirementValue = input.RequirementValue;
            if (input.VariabledbName != null)
            {
                QuestConnectionRequirement.VariabledbName = input.VariabledbName.ToUpper();
            }
            QuestConnectionRequirement.QuestConnectionId = repo.QuestConnections.FirstOrDefault(q => q.Id == connection.Id);
            QuestConnectionRequirement.QuestConnectionRequirementName = input.QuestConnectionRequirementName;
            QuestConnectionRequirement.RollModifier = input.RollModifier;
            QuestConnectionRequirement.RollOffset = input.RollOffset;
            QuestConnectionRequirement.IsRandomRoll = input.IsRandomRoll;

            repo.SaveQuestConnectionRequirement(QuestConnectionRequirement);

            return QuestConnectionRequirement.Id;
        }

        public static int SaveQuestEnd(QuestEnd input, QuestState state)
        {
            IQuestRepository repo = new EFQuestRepository();

            var questEnd = repo.QuestEnds.FirstOrDefault(q => q.Id == input.Id);
            var dbState = repo.QuestStates.FirstOrDefault(s => s.Id == state.Id);

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

        public static int SaveQuestStatePreaction(QuestStatePreaction input, QuestState state)
        {
            IQuestRepository repo = new EFQuestRepository();

            var questStatePreaction = repo.QuestStatePreactions.FirstOrDefault(q => q.Id == input.Id);
            var dbState = repo.QuestStates.FirstOrDefault(s => s.Id == state.Id);

            if (questStatePreaction == null)
            {
                questStatePreaction = new QuestStatePreaction();
            }

            questStatePreaction.Id = input.Id;
            questStatePreaction.QuestId = input.QuestId;
            questStatePreaction.QuestStateId = dbState;
            questStatePreaction.QuestStatePreactionName = input.QuestStatePreactionName;
            if (input.VariableName != null)
            {
                questStatePreaction.VariableName = input.VariableName.ToUpper();
            }
            questStatePreaction.AddOrSet = input.AddOrSet;
            questStatePreaction.ActionType = input.ActionType;
            questStatePreaction.ActionValue = input.ActionValue;

            repo.SaveQuestStatePreaction(questStatePreaction);

            return questStatePreaction.Id;
        }

        public static void DeleteQuestStatePreaction(int id)
        {
            IQuestRepository repo = new EFQuestRepository();
            repo.DeleteQuestStatePreaction(id);
        }

        public static void DeleteQuestEnd(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();
            repo.DeleteQuestEnd(Id);
        }

        public static void DeleteQuestConnectionRequirement(int id)
        {
            IQuestRepository repo = new EFQuestRepository();
            repo.DeleteQuestConnectionRequirement(id);
        }

        public static void DeleteQuestConnection(int id)
        {
            IQuestRepository repo = new EFQuestRepository();

            var reqs = repo.QuestConnectionRequirements.Where(q => q.QuestConnectionId.Id == id).ToList();

            foreach(var q in reqs)
            {
                repo.DeleteQuestConnectionRequirement(q.Id);
            }

            repo.DeleteQuestConnection(id);
        }

        public static void MarkQuestAsLive(int Id, bool live)
        {
            IQuestRepository repo = new EFQuestRepository();
            var dbQuestStart = repo.QuestStarts.FirstOrDefault(q => q.Id == Id);
            dbQuestStart.IsLive = live;
            repo.SaveQuestStart(dbQuestStart);
        }

        public static IEnumerable<QuestState> GetAllQuestsStatesInQuest(int questId)
        {
            IQuestRepository repo = new EFQuestRepository();
            return repo.QuestStates.Where(q => q.QuestId == questId);
        }

        public static IEnumerable<QuestConnection> GetAllQuestsConnectionsInQuest(int questId)
        {
            IQuestRepository repo = new EFQuestRepository();
            return repo.QuestConnections.Where(q => q.QuestId == questId);
        }

        /// <summary>
        /// Saves a little bit of information about people who have saved / deleted anything to do with quests
        /// </summary>
        /// <param name="username">Account username who performed this action</param>
        /// <param name="questId">Quest Id the change was made in</param>
        /// <param name="logText">Log text to save</param>
        public static void LogQuestWriterAction(string username, int questId, string logText)
        {
            IQuestRepository repo = new EFQuestRepository();
            var log = new QuestWriterLog
            {
                User = username,
                Text = logText,
                Timestamp = DateTime.UtcNow,
                QuestId = questId
            };
            repo.SaveQuestWriterLog(log);
        }

        public static IEnumerable<QuestWriterLog> GetAllQuestWriterLogs(int questId)
        {
            IQuestRepository repo = new EFQuestRepository();
            return repo.QuestWriterLogs.Where(q => q.QuestId == questId);
        }
    }
}