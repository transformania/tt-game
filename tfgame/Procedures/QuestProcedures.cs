﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures
{
    public static class QuestProcedures
    {

        public static IEnumerable<QuestStart> GetAllQuestStarts()
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestStarts;
        }

        public static IEnumerable<QuestStart> GetAllQuestStartsAtLocation(string location)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestStarts.Where(q => q.Location == location);
        }

        public static QuestStart GetQuest(int id)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestStarts.FirstOrDefault(q => q.Id == id);
        }

        public static QuestState GetQuestState(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestState dbQuestState = repo.QuestStates.FirstOrDefault(q => q.Id == Id);

            if (dbQuestState.QuestEnds==null)
            {
                dbQuestState.QuestEnds = new List<QuestEnd>();
            }

            return dbQuestState;
        }

        public static IEnumerable<QuestState> GetChildQuestStates(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestStates.Where(q => q.ParentQuestStateId == Id);
        }

        public static bool PlayerCanBeginQuest(Player player, QuestStart questStart, int gameWorldTurn)
        {
            IQuestRepository repo = new EFQuestRepository();
            IEnumerable<QuestPlayerStatus> questPlayerStatuses = repo.QuestPlayerStatuses.Where(q => q.PlayerId == player.Id);
            return PlayerCanBeginQuest(player, questStart, questPlayerStatuses, gameWorldTurn);
        }

        public static bool PlayerCanBeginQuest(Player player, QuestStart questStart, IEnumerable<QuestPlayerStatus> questPlayerStatuses, int gameWorldTurn)
        {
            if (questStart.IsLive == false)
            {
                return false;
            }
            else if (questStart.MinStartLevel > player.Level)
            {
                return false;
            }
            else if (questStart.MaxStartLevel < player.Level)
            {
                return false;
            }
            else if (questStart.MinStartLevel > gameWorldTurn)
            {
                return false;
            }
            else if (questStart.MaxStartTurn < gameWorldTurn)
            {
                return false;
            }
            else if (questStart.RequiredGender == (int)QuestStatics.Gender.Male && player.Gender != PvPStatics.GenderMale)
            {
                return false;
            }
            else if (questStart.RequiredGender == (int)QuestStatics.Gender.Female && player.Gender != PvPStatics.GenderFemale)
            {
                return false;
            }

            foreach (QuestPlayerStatus q in questPlayerStatuses)
            {
                if (q.QuestId == questStart.Id)
                {

                    // player has already completed this quest so they can't do it again.
                    if (q.Outcome==(int)QuestStatics.QuestOutcomes.Completed)
                    {
                        return false;
                    }

                    // TODO?  Filter out quests whose cooldowns are not yet expired
                    //else if (q.Outcome==(int)QuestStatics.QuestOutcomes.Failed)
                    //{
                    //    if (gameWorldTurn - q.Outcome < QuestStatics.QuestFailCooldownTurnLength)
                    //    {
                    //        return false;
                    //    }
                    //}
                }
            }

            // all checks passed, return true
            return true;
        }

        public static IEnumerable<QuestPlayerStatus> GetQuestPlayerStatuses(Player me)
        {
            IQuestRepository repo = new EFQuestRepository();
            return repo.QuestPlayerStatuses.Where(p => p.PlayerId == me.Id);
        }

        public static IEnumerable<QuestStart> GetAvailableQuestsAtLocation(Player player, int turn)
        {
            IQuestRepository repo = new EFQuestRepository();

            List<QuestStart> quests = repo.QuestStarts.Where(s => s.Location == player.dbLocationName && s.IsLive == true).ToList();
            List<QuestPlayerStatus> playerQuestsRaw = repo.QuestPlayerStatuses.Where(s => s.PlayerId == player.Id).ToList();
            List<QuestStart> eligibleQuests = new List<QuestStart>();


            foreach (QuestStart q in quests)
            {
                if (PlayerCanBeginQuest(player, q, playerQuestsRaw, turn)== true) {
                    eligibleQuests.Add(q);
                }
            }

            return eligibleQuests;
        }

        public static IEnumerable<QuestStart> GetAllAvailableQuestsForPlayer(Player player, int turn)
        {
            IQuestRepository repo = new EFQuestRepository();
            List<QuestStart> quests = repo.QuestStarts.Where(q => q.IsLive == true).ToList();
            List<QuestPlayerStatus> playerQuestsRaw = repo.QuestPlayerStatuses.Where(s => s.PlayerId == player.Id).ToList();
            List<QuestStart> eligibleQuests = new List<QuestStart>();

            foreach (QuestStart q in quests)
            {
                if (PlayerCanBeginQuest(player, q, playerQuestsRaw, turn) == true)
                {
                    eligibleQuests.Add(q);
                }
            }

            return eligibleQuests;
        }

        public static void PlayerBeginQuest(Player player, QuestStart questStart)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = questStart.Id;
            dbPlayer.InQuestState = questStart.StartState;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static void PlayerSetQuestState(Player player, QuestState questState)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            dbPlayer.OnlineActivityTimestamp = DateTime.UtcNow;
            dbPlayer.InQuestState = questState.Id;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static string PlayerEndQuest(Player player, int endType)
        {
            string message = "";
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = 0;
            dbPlayer.InQuestState = 0;
            playerRepo.SavePlayer(dbPlayer);

            IQuestRepository questRepo = new EFQuestRepository();
            QuestPlayerStatus questPlayerStatus = questRepo.QuestPlayerStatuses.FirstOrDefault(q => q.PlayerId == player.Id && q.QuestId == player.InQuest);

            if (questPlayerStatus == null)
            {
                questPlayerStatus = new QuestPlayerStatus
                {
                    PlayerId = player.Id,
                    QuestId = player.InQuest,
                    
                };
            }
            questPlayerStatus.LastEndedTurn = PvPWorldStatProcedures.GetWorldTurnNumber();
            questPlayerStatus.Outcome = endType;

            questRepo.SaveQuestPlayerStatus(questPlayerStatus);

            // assing completion bonuses
            if (endType == (int)QuestStatics.QuestOutcomes.Completed)
            {

                QuestState questState = GetQuestState(player.InQuestState);

                decimal xpGain = 0;

                foreach (QuestEnd q in questState.QuestEnds)
                {
                    // experience gain
                    if (q.RewardType==(int)QuestStatics.RewardType.Experience)
                    {
                        xpGain += Int32.Parse(q.RewardAmount);
                    }

                    // item gain
                    else if (q.RewardType==(int)QuestStatics.RewardType.Item)
                    {
                        DbStaticItem item = ItemStatics.GetStaticItem(q.RewardAmount);
                        ItemProcedures.GiveNewItemToPlayer(player, item);
                        message += "<br/>You received a <b>" + item.FriendlyName + "</b>.";
                    }

                    // effect gain
                    else if (q.RewardType == (int)QuestStatics.RewardType.Effect)
                    {
                        DbStaticEffect effect = EffectStatics.GetEffect(q.RewardAmount);
                        EffectProcedures.GivePerkToPlayer(effect.dbName, player.Id);
                        message += "<br/>You received the effect <b>" + effect.FriendlyName + "</b>.";
                    }
                }

                if (xpGain > 0)
                {
                    message += "<br/>You earned <b>" + xpGain + "</b> XP.";
                }

                PlayerProcedures.GiveXP(player.Id, xpGain);
            }

            return message;

        }

        public static bool QuestStateIsAvailable(QuestState questState, Player player, BuffBox buffs, IEnumerable<QuestPlayerVariable> variables)
        {

            bool isAvailable = true;

            foreach (QuestStateRequirement q in questState.QuestStateRequirements)
            {
                // evaluate variable
                if (q.RequirementType == (int)QuestStatics.RequirementType.Variable) {

                    QuestPlayerVariable var = variables.FirstOrDefault(v => v.VariableName == q.VariabledbName);

                    // variable has never been set, so fail
                    if (var==null)
                    {
                        return false;
                    }

                    isAvailable = ExpressionIsTrue(float.Parse(var.VariableValue), q);
                    return isAvailable;
                }

                else if (q.RequirementType == (int)QuestStatics.RequirementType.Gender)
                {
                    if (q.RequirementValue == PvPStatics.GenderMale && player.Gender != PvPStatics.GenderMale)
                    {
                        return false;
                    }
                    else if (q.RequirementValue == PvPStatics.GenderFemale && player.Gender != PvPStatics.GenderFemale)
                    {
                        return false;
                    }

                    else
                    {
                        return true;
                    }
                }

                // evaluate player buff/ability
                float playerValue = GetValueFromType(q, buffs);
                isAvailable = ExpressionIsTrue(playerValue, q);
                if (isAvailable==false)
                {
                    return false;
                }

            }

            return true;
        }

        private static float GetValueFromType(QuestStateRequirement q, BuffBox buffs)
        {
            float playerValue = 0;

            // get correct ability type requirement
            if (q.RequirementType == (int)QuestStatics.RequirementType.Discipline)
            {
                playerValue = buffs.Discipline();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Perception)
            {
                playerValue = buffs.Perception();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Charisma)
            {
                playerValue = buffs.Charisma();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Fortitude)
            {
                playerValue = buffs.Fortitude();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Agility)
            {
                playerValue = buffs.Agility();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Allure)
            {
                playerValue = buffs.Allure();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Magicka)
            {
                playerValue = buffs.Magicka();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Succour)
            {
                playerValue = buffs.Succour();
            }
            else if (q.RequirementType == (int)QuestStatics.RequirementType.Luck)
            {
                playerValue = buffs.Luck();
            }

            return playerValue;
        }

        private static bool ExpressionIsTrue(float playerValue, QuestStateRequirement q)
        {
            
            float requirementValue = Convert.ToSingle(q.RequirementValue);

            bool isAvailable = false;

            if (q.Operator == (int)QuestStatics.Operator.Less_Than)
            {
                if (playerValue < requirementValue)
                {
                    isAvailable = true;
                }
            }
            else if (q.Operator == (int)QuestStatics.Operator.Less_Than_Or_Equal)
            {
                if (playerValue <= requirementValue)
                {
                    isAvailable = true;
                }
            }
            else if (q.Operator == (int)QuestStatics.Operator.Equal_To)
            {
                if (playerValue == requirementValue)
                {
                    isAvailable = true;
                }
            }
            else if (q.Operator == (int)QuestStatics.Operator.Greater_Than_Or_Equal)
            {
                if (playerValue >= requirementValue)
                {
                    isAvailable = true;
                }
            }
            else if (q.Operator == (int)QuestStatics.Operator.Greater_Than)
            {
                if (playerValue > requirementValue)
                {
                    isAvailable = true;
                }
            }
            else if (q.Operator == (int)QuestStatics.Operator.Not_Equal_To)
            {
                if (playerValue != requirementValue)
                {
                    isAvailable = true;
                }
            }
            return isAvailable;
        }

        public static string GetRequirementsAsString(QuestState q)
        {

            string output = "";

            if (q.QuestStateRequirements.Count()==0)
            {
                return output;
            }

            int len = q.QuestStateRequirements.Count();
            int i = 0;

            output += "[";

            foreach (QuestStateRequirement qs in q.QuestStateRequirements.ToList())
            {
                // don't print anything for variables or gender requirements
                if (qs.RequirementType == (int)QuestStatics.RequirementType.Variable || qs.RequirementType == (int)QuestStatics.RequirementType.Gender)
                {
                    continue;
                }

                output += qs.RequirementValue + " " + Enum.GetName(typeof(QuestStatics.RequirementType), qs.RequirementType);



                if (i < len-1)
                {
                    output += ", ";
                }

                i++;

            }

            output += "]";


            return output;
        }

        public static string Textify(string input, Player player)
        {
            input = input.Replace(Environment.NewLine, "</br>")
                .Replace("[b]", "<b>").Replace("[/b]", "</b>")
                .Replace("[i]", "<i>").Replace("[/i]", "</i>")
                .Replace("$PLAYER_NAME_FIRST$", player.FirstName)
                .Replace("$PLAYER_NAME_LAST$", player.LastName)
                .Replace("$PLAYER_NAME$", player.GetFullName());
            return input;
        }

        public static void PlayerClearAllQuestStatuses(Player player)
        {
            IQuestRepository repo = new EFQuestRepository();
            List<QuestPlayerStatus> statuses = repo.QuestPlayerStatuses.Where(q => q.PlayerId == player.Id).ToList();

            foreach (QuestPlayerStatus s in statuses)
            {
                repo.DeleteQuestPlayerStatus(s.Id);
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = 0;
            dbPlayer.InQuestState = 0;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static Player ProcessQuestStatePreactions(Player player, QuestState questState)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            foreach (QuestStatePreaction p in questState.QuestStatePreactions.ToList())
            {

                // try to parse the value from string into number, if it fails skip this preaction entirely
                decimal valueAsNumber = 0;
                try
                {
                    valueAsNumber = Decimal.Parse(p.ActionValue);
                }
                catch
                {
                    valueAsNumber = 0;
                }

                // change form
                if (p.ActionType==(int)QuestStatics.PreactionType.Form)
                {
                    DbStaticForm newForm = FormStatics.GetForm(p.ActionValue);
                    dbPlayer.Form = newForm.dbName;
                    dbPlayer.Gender = newForm.Gender;
                }

                // move player
                else if (p.ActionType == (int)QuestStatics.PreactionType.MoveToLocation)
                {
                    Location loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == p.ActionValue);
                    if (loc != null)
                    {
                        dbPlayer.dbLocationName = p.ActionValue;
                    }
                }

                // change willpower
                else if (p.ActionType == (int)QuestStatics.PreactionType.Willpower)
                {
                    if (p.AddOrSet == (int)QuestStatics.AddOrSet.Set)
                    {
                        dbPlayer.Health = valueAsNumber;
                    }
                    else if (p.AddOrSet == (int)QuestStatics.AddOrSet.Add_Number)
                    {
                        dbPlayer.Health += valueAsNumber;
                    }
                }

                // change mana
                else if (p.ActionType == (int)QuestStatics.PreactionType.Mana)
                {
                    if (p.AddOrSet == (int)QuestStatics.AddOrSet.Set)
                    {
                        dbPlayer.Mana = valueAsNumber;
                    }
                    else if (p.AddOrSet == (int)QuestStatics.AddOrSet.Add_Number)
                    {
                        dbPlayer.Mana += valueAsNumber;
                    }
                }

                // update or set a variable
                else if (p.ActionType==(int)QuestStatics.PreactionType.Variable)
                {
                    if (p.AddOrSet == (int)QuestStatics.AddOrSet.Set)
                    {
                        QuestProcedures.SetQuestPlayerVariable(p.QuestId, dbPlayer.Id, p.VariableName, p.ActionValue);
                    }
                    else if (p.AddOrSet == (int)QuestStatics.AddOrSet.Add_Number)
                    {
                        QuestProcedures.EditQuestPlayerVariable(p.QuestId, dbPlayer.Id, p.VariableName, p.ActionValue);
                    }
                    
                }
            }

            dbPlayer.ReadjustMaxes(ItemProcedures.GetPlayerBuffs(dbPlayer));
            playerRepo.SavePlayer(dbPlayer);

            return dbPlayer;

        }

        public static void SetQuestPlayerVariable(int questId, int playerId, string variableName, string variableValue)
        {
            IQuestRepository repo = new EFQuestRepository();
            QuestPlayerVariable variable = repo.QuestPlayerVariablees.FirstOrDefault(v => v.PlayerId == playerId && v.QuestId == questId && v.VariableName == variableName);

            if (variable==null)
            {
                variable = new QuestPlayerVariable
                {
                    QuestId  = questId,
                    PlayerId = playerId,
                    VariableName = variableName.ToUpper()
                };
            }

            variable.VariableValue = variableValue;
            repo.SaveQuestPlayerVariable(variable);
        }

        public static void EditQuestPlayerVariable(int questId, int playerId, string variableName, string variableValue)
        {
            IQuestRepository repo = new EFQuestRepository();
            QuestPlayerVariable variable = repo.QuestPlayerVariablees.FirstOrDefault(v => v.PlayerId == playerId && v.QuestId == questId && v.VariableName == variableName);

            if (variable == null)
            {
                variable = new QuestPlayerVariable
                {
                    QuestId = questId,
                    PlayerId = playerId,
                    VariableName = variableName.ToUpper(),
                    VariableValue = "0",
                };
            }

            float oldValueAsFloat = float.Parse(variable.VariableValue);
            float updateValueAsFloat =  float.Parse(variableValue);
            float endValueAsFloat = oldValueAsFloat + updateValueAsFloat;

            variable.VariableValue = endValueAsFloat.ToString();

            repo.SaveQuestPlayerVariable(variable);
        }

        public static QuestPlayerVariable GetQuestPlayerVariable(int questId, int playerId, string variableName)
        {
            IQuestRepository repo = new EFQuestRepository();
            return repo.QuestPlayerVariablees.FirstOrDefault(v => v.PlayerId == playerId && v.QuestId == questId && v.VariableName == variableName);
        }

        public static IEnumerable<QuestPlayerVariable> GetAllQuestPlayerVariablesFromQuest(int questId, int playerId)
        {
            IQuestRepository repo = new EFQuestRepository();
            IEnumerable<QuestPlayerVariable> output = repo.QuestPlayerVariablees.Where(v => v.PlayerId == playerId && v.QuestId == questId);

            if (output == null)
            {
                output = new List<QuestPlayerVariable>();
            }

            return output;
        }

        /// <summary>
        /// Get a list of all of the unique names of variables referenced anywhere in this quest so far to help reduce user error
        /// when entering variable names in to quest state preactions or requirements.
        /// </summary>
        /// <param name="questId"></param>
        /// <returns></returns>
        public static List<string> GetAllPossibleVariablesNamesInQuest(int questId)
        {

            List<string> output = new List<string>();

            IQuestRepository repo = new EFQuestRepository();
            IEnumerable<QuestStatePreaction> allPreactions = repo.QuestStatePreactions.Where(q => q.QuestId == questId);

            foreach(QuestStatePreaction p in allPreactions)
            {
                if (p.ActionType == (int)QuestStatics.PreactionType.Variable)
                {
                    output.Add(p.VariableName);
                }
            }

            IEnumerable<QuestStateRequirement> allRequirements = repo.QuestStateRequirements.Where(q => q.QuestId == questId);

            foreach (QuestStateRequirement p in allRequirements)
            {
                if (p.RequirementType == (int)QuestStatics.RequirementType.Variable)
                {
                    output.Add(p.VariabledbName);
                }
            }

            output = output.Distinct().ToList();
            return output;
        }

        /// <summary>
        /// Delete all of a player's quest variables for a certain quest so that it is a fresh start when they attempt to do the quest
        /// again, or else the quest is complete and there's no more point storing this data in the database
        /// </summary>
        /// <param name="playerId">Id of player</param>
        /// <param name="questId">Id of the quest</param>
        public static void ClearQuestPlayerVariables(int playerId, int questId)
        {
            IQuestRepository repo = new EFQuestRepository();

            foreach (QuestPlayerVariable q in repo.QuestPlayerVariablees.Where(v => v.PlayerId == playerId && v.QuestId == questId).ToList())
            {
                repo.DeleteQuestPlayerVariable(q.Id);
            }

        }

        public static int GetLastTurnQuestEnded(Player player, int questId)
        {
            IQuestRepository repo = new EFQuestRepository();
            IEnumerable<int> turns = repo.QuestPlayerStatuses.Where(p => p.PlayerId == player.Id && p.QuestId == questId).Select(s => s.LastEndedTurn);

            if (turns.Count()==0)
            {
                return -9999;
            } else
            {
                return turns.Max();
            }
           
        }
    }
}