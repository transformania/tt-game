using System;
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

        public static QuestStart GetQuest(int id)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestStarts.FirstOrDefault(q => q.Id == id);
        }

        public static QuestState GetQuestState(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestStates.FirstOrDefault(q => q.Id == Id);
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

        public static void PlayerBeginQuest(Player player, QuestStart questStart)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = questStart.Id;
            dbPlayer.InQuestState = questStart.StartState;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static void PlayerEndQuest(Player player, int endType)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            Player dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = 0;
            dbPlayer.InQuestState = 0;
            playerRepo.SavePlayer(dbPlayer);

            IQuestRepository questRepo = new EFQuestRepository();
            QuestPlayerStatus questPlayerStatus = questRepo.QuestPlayerStatuses.FirstOrDefault(q => q.PlayerId == player.Id);

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

            if (endType == (int)QuestStatics.QuestOutcomes.Completed)
            {
                // TODO:  assign rewards if quest passed
            }


        }

        public static bool QuestStateIsAvailable(QuestState questState, Player player, BuffBox buffs)
        {

            bool isAvailable = false;

            foreach (QuestStateRequirement q in questState.QuestStateRequirements)
            {
                float playerValue = GetValueFromType(q, buffs);
                isAvailable = ExpressionIsTrue(playerValue, q);
                if (isAvailable==false)
                {
                    return false;
                }

            }

            return isAvailable;
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
    }
}