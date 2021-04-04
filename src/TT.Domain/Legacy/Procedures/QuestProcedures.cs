using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TT.Domain.Abstract;
using TT.Domain.Concrete;
using TT.Domain.Models;
using TT.Domain.Players.Commands;
using TT.Domain.Statics;
using TT.Domain.ViewModels;

namespace TT.Domain.Procedures
{
    public static class QuestProcedures
    {

        public const string ImageRegexPattern = @"\[img\](.*)\[/img\]";

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

            var dbQuestState = repo.QuestStates.FirstOrDefault(q => q.Id == Id);

            if (dbQuestState.QuestEnds==null)
            {
                dbQuestState.QuestEnds = new List<QuestEnd>();
            }

            return dbQuestState;
        }

        public static QuestConnection GetQuestConnection(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            var dbQuestConnection = repo.QuestConnections.FirstOrDefault(q => q.Id == Id);

            return dbQuestConnection;
        }

        public static IEnumerable<QuestConnection> GetChildQuestConnections(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestConnections.Where(q => q.QuestStateFromId == Id);
        }

        public static bool PlayerCanBeginQuest(Player player, QuestStart questStart, IEnumerable<QuestPlayerStatus> questPlayerStatuses, int gameWorldTurn)
        {
            if (!questStart.IsLive)
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
            else if (questStart.MinStartTurn > gameWorldTurn)
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

            foreach (var q in questPlayerStatuses)
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

            // TODO:  Filter out quests that have a prerequisite not yet fulfilled
            if (questStart.PrerequisiteQuest > 0)
            {
                var preReqMet = false;

                foreach (var q in questPlayerStatuses)
                {
                    if (q.QuestId == questStart.PrerequisiteQuest && q.Outcome==(int)QuestStatics.QuestOutcomes.Completed)
                    {
                        preReqMet = true;
                        break;
                    }
                }

                if (!preReqMet)
                {
                    return false;
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

            var quests = repo.QuestStarts.Where(s => s.Location == player.dbLocationName && s.IsLive).ToList();
            var playerQuestsRaw = repo.QuestPlayerStatuses.Where(s => s.PlayerId == player.Id).ToList();
            var eligibleQuests = new List<QuestStart>();


            foreach (var q in quests)
            {
                if (PlayerCanBeginQuest(player, q, playerQuestsRaw, turn)) {
                    eligibleQuests.Add(q);
                }
            }

            return eligibleQuests;
        }

        public static IEnumerable<QuestStart> GetAllAvailableQuestsForPlayer(Player player, int turn)
        {
            IQuestRepository repo = new EFQuestRepository();
            var quests = repo.QuestStarts.Where(q => q.IsLive).ToList();
            var playerQuestsRaw = repo.QuestPlayerStatuses.Where(s => s.PlayerId == player.Id).ToList();
            var eligibleQuests = new List<QuestStart>();

            foreach (var q in quests)
            {
                if (PlayerCanBeginQuest(player, q, playerQuestsRaw, turn))
                {
                    eligibleQuests.Add(q);
                }
            }

            return eligibleQuests;
        }

        public static void PlayerBeginQuest(Player player, QuestStart questStart)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = questStart.Id;
            dbPlayer.InQuestState = questStart.StartState;
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static void PlayerSetQuestState(Player player, QuestState questState)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.LastActionTimestamp = DateTime.UtcNow;
            dbPlayer.OnlineActivityTimestamp = DateTime.UtcNow;
            dbPlayer.InQuestState = questState.Id;
            playerRepo.SavePlayer(dbPlayer);
        }

        public static string PlayerEndQuest(Player player, int endType)
        {
            var message = "";
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = 0;
            dbPlayer.InQuestState = 0;
            playerRepo.SavePlayer(dbPlayer);

            IQuestRepository questRepo = new EFQuestRepository();
            var questPlayerStatus = questRepo.QuestPlayerStatuses.FirstOrDefault(q => q.PlayerId == player.Id && q.QuestId == player.InQuest);

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

                var questState = GetQuestState(player.InQuestState);

                decimal xpGain = 0;

                foreach (var q in questState.QuestEnds)
                {
                    // experience gain
                    if (q.RewardType==(int)QuestStatics.RewardType.Experience)
                    {
                        xpGain += Int32.Parse(q.RewardAmount);
                    }

                    // item gain
                    else if (q.RewardType==(int)QuestStatics.RewardType.Item)
                    {
                        var item = ItemStatics.GetStaticItem(System.Convert.ToInt32(q.RewardAmount));
                        ItemProcedures.GiveNewItemToPlayer(player, item);
                        message += " <br>You received a <b>" + item.FriendlyName + "</b>.";
                    }

                    // effect gain
                    else if (q.RewardType == (int)QuestStatics.RewardType.Effect)
                    {
                        var effect = EffectStatics.GetDbStaticEffect(System.Convert.ToInt32(q.RewardAmount));
                        EffectProcedures.GivePerkToPlayer(effect.Id, player.Id);
                        message += "<br>You received the effect <b>" + effect.FriendlyName + "</b>.";
                    }

                    // spell gain
                    else if (q.RewardType == (int)QuestStatics.RewardType.Spell)
                    {
                        var spell = SkillStatics.GetStaticSkill(System.Convert.ToInt32(q.RewardAmount));
                        SkillProcedures.GiveSkillToPlayer(player.Id, spell.Id);
                        message += "<br>You learned the spell <b>" + spell.FriendlyName + "</b>.";
                    }

                }

                if (xpGain > 0)
                {
                    message += "<br>You earned <b>" + xpGain + "</b> XP.";
                }

                PlayerProcedures.GiveXP(player, xpGain);
            }

            // delete all of the player's quest variables
            var vars = QuestProcedures.GetAllQuestPlayerVariablesFromQuest(player.InQuest, player.Id).ToList();
            foreach (var v in vars)
            {
                questRepo.DeleteQuestPlayerVariable(v.Id);
            }

            return message;

        }

        public static bool PlayerHasCompletedQuest(Player player, int questId)
        {
            IQuestRepository questRepo = new EFQuestRepository();
            var matchingQuests = questRepo.QuestPlayerStatuses.Where(q => q.PlayerId == player.Id &&
                                                                          q.QuestId == questId &&
                                                                          q.Outcome == (int)QuestStatics.QuestOutcomes.Completed);

            return matchingQuests.Any();
        }


        /// <summary>
        /// Returns true if a connection is available to the player to take.  
        /// </summary>
        /// <param name="questConnection"></param>
        /// <param name="player">Player attempting to go down this connection</param>
        /// <param name="buffs">Player's statistics from effects and equipment</param>
        /// <param name="variables">All quest variables created by this player previously in the quest</param>
        /// <returns></returns>
        public static bool QuestConnectionIsAvailable(QuestConnection questConnection, Player player, BuffBox buffs, IEnumerable<QuestPlayerVariable> variables)
        {

            var isAvailable = true;

            if (questConnection.QuestStateFromId < 0 || questConnection.QuestStateToId < 0)
            {
                return false;
            }

            foreach (var q in questConnection.QuestConnectionRequirements)
            {

                // skip all roll-based requirements; a player can always attempt a roll
                if (q.IsRandomRoll)
                {
                    continue;
                }

                // evaluate variable
                if (q.RequirementType == (int)QuestStatics.RequirementType.Variable)
                {

                    var var = variables.FirstOrDefault(v => v.VariableName == q.VariabledbName);

                    // variable has never been set, so fail
                    if (var == null)
                    {
                        return false;
                    }

                    isAvailable = ExpressionIsTrue(float.Parse(var.VariableValue), q);

                    if (!isAvailable)
                    {
                        return false;
                    }
                    else
                    {
                        continue;
                    }

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
                        continue;
                    }
                }

                // evaluate player buff/ability
                var playerValue = GetValueFromType(q, buffs);
                isAvailable = ExpressionIsTrue(playerValue, q);
                if (!isAvailable)
                {
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Return the player's stat of a given QuestConnectionRequirement.  Ex:  player has 50 luck
        /// </summary>
        /// <param name="q">QuestConnectionRequirement to be evaluated</param>
        /// <param name="buffs">The player's stats</param>
        /// <returns></returns>
        private static float GetValueFromType(QuestConnectionRequirement q, BuffBox buffs)
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

        private static bool ExpressionIsTrue(float playerValue, QuestConnectionRequirement q)
        {
            
            var requirementValue = Convert.ToSingle(q.RequirementValue);

            var isAvailable = false;

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

        public static string GetRequirementsAsString(QuestConnection q, BuffBox buffs)
        {

            var output = "";

            if (!q.QuestConnectionRequirements.Any())
            {
                return output;
            }

            var len = q.QuestConnectionRequirements.Count();
            var i = 0;

            output += "[";

            foreach (var qs in q.QuestConnectionRequirements.ToList())
            {
                // don't print anything for variables or gender requirements
                if (qs.RequirementType == (int)QuestStatics.RequirementType.Variable || 
                    qs.RequirementType == (int)QuestStatics.RequirementType.Gender || 
                    qs.RequirementType == (int)QuestStatics.RequirementType.Form)
                {
                    continue;
                }

                // random roll, calculate % chance and display that
                if (qs.IsRandomRoll)
                {
                    var playerValue = GetValueFromType(qs, buffs);

                    var chance = Math.Round(qs.RollModifier * playerValue + qs.RollOffset,1);

                    if (chance < 0)
                    {
                        chance = 0;
                    }
                    else if (chance > 100)
                    {
                        chance = 100;
                    }

                    output += qs.PrintRequirementStatAsString() + " - " + chance + "%";

                }

                // strict requirement
                else
                {
                    output += qs.PrintOperatorAsString() + " " + qs.RequirementValue + " " + qs.PrintRequirementStatAsString();
                }

                if (i < len-1)
                {
                    output += ", ";
                }

                i++;

            }

            output += "]";

            // no requirements were found, so clear up
            if (output == "[]")
            {
                output = "";
            }


            return output;
        }

        public static string Textify(string input, Player player)
        {
            if (input == null)
            {
                input = "";
            }

            input = input.Replace(Environment.NewLine, "</br>")
                .Replace("[b]", "<b>").Replace("[/b]", "</b>")
                .Replace("[i]", "<i>").Replace("[/i]", "</i>")
                .Replace("$PLAYER_NAME_FIRST$", player.FirstName)
                .Replace("$PLAYER_NAME_LAST$", player.LastName)
                .Replace("$PLAYER_NAME$", player.GetFullName());


            // replace [img]filename[/img] with proper html image links
            var match = Regex.Match(input, ImageRegexPattern);
            var imgName = match.Groups[1].Value;

            var rgx = new Regex(ImageRegexPattern);
            input = Regex.Replace(input, ImageRegexPattern, "<img src='/Images/PvP/quests/" + imgName + "' style='max-width: 100%;'>");

            return input;
        }

        public static void PlayerClearAllQuestStatuses(Player player)
        {
            IQuestRepository repo = new EFQuestRepository();
            var statuses = repo.QuestPlayerStatuses.Where(q => q.PlayerId == player.Id).ToList();

            foreach (var s in statuses)
            {
                repo.DeleteQuestPlayerStatus(s.Id);
            }

            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);
            dbPlayer.InQuest = 0;
            dbPlayer.InQuestState = 0;
            playerRepo.SavePlayer(dbPlayer);

        }

        public static Player ProcessQuestStatePreactions(Player player, QuestState questState)
        {
            IPlayerRepository playerRepo = new EFPlayerRepository();
            var dbPlayer = playerRepo.Players.FirstOrDefault(p => p.Id == player.Id);

            foreach (var p in questState.QuestStatePreactions.ToList())
            {

                // try to parse the value from string into number, if it fails, default to 0
                decimal valueAsNumber = 0;
                if (!Decimal.TryParse(p.ActionValue, out valueAsNumber))
                {
                    valueAsNumber = 0;
                }

                // change form
                if (p.ActionType==(int)QuestStatics.PreactionType.Form)
                {
                    DomainRegistry.Repository.Execute(new ChangeForm
                    {
                        PlayerId = dbPlayer.Id,
                        FormSourceId = System.Convert.ToInt32(p.ActionValue)
                    });
                }

                // move player
                else if (p.ActionType == (int)QuestStatics.PreactionType.MoveToLocation)
                {
                    var loc = LocationsStatics.LocationList.GetLocation.FirstOrDefault(l => l.dbName == p.ActionValue);
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

        /// <summary>
        /// Returns true if a player passes all rolls for a quest connection.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="player">Player attempting to go down this connection</param>
        /// <param name="buffs">Player's statistics from effects and equipment</param>
        /// <param name="variables">All quest variables created by this player previously in the quest</param>
        /// <returns></returns>
        public static bool RollForQuestConnection(QuestConnection connection, Player player, BuffBox buffs, IEnumerable<QuestPlayerVariable> variables)
        {
           
            foreach(var q in connection.QuestConnectionRequirements)
            {
                if (!q.IsRandomRoll)
                {
                    continue;
                }

                var playerValue = GetValueFromType(q, buffs);

                var chance = q.RollModifier * playerValue + q.RollOffset;

                var r = new Random();
                var roll = r.NextDouble()*100;

                if (roll > chance)
                {
                    return false;
                }

            }

            return true;
        }

        public static void SetQuestPlayerVariable(int questId, int playerId, string variableName, string variableValue)
        {
            IQuestRepository repo = new EFQuestRepository();
            var variable = repo.QuestPlayerVariablees.FirstOrDefault(v => v.PlayerId == playerId && v.QuestId == questId && v.VariableName == variableName);

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
            var variable = repo.QuestPlayerVariablees.FirstOrDefault(v => v.PlayerId == playerId && v.QuestId == questId && v.VariableName == variableName);

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

            var oldValueAsFloat = float.Parse(variable.VariableValue);
            var updateValueAsFloat =  float.Parse(variableValue);
            var endValueAsFloat = oldValueAsFloat + updateValueAsFloat;

            variable.VariableValue = endValueAsFloat.ToString();

            repo.SaveQuestPlayerVariable(variable);
        }

        /// <summary>
        /// 
        /// Returns all of the player quest variables for a given player and quest
        /// 
        /// </summary>
        /// <param name="questId">Id of the quest whose variables should be deleted</param>
        /// <param name="playerId">Id of the player who owns the variables that should be deleted</param>
        /// <returns></returns>
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

            var output = new List<string>();

            IQuestRepository repo = new EFQuestRepository();
            IEnumerable<QuestStatePreaction> allPreactions = repo.QuestStatePreactions.Where(q => q.QuestId == questId);

            foreach(var p in allPreactions)
            {
                if (p.ActionType == (int)QuestStatics.PreactionType.Variable)
                {
                    output.Add(p.VariableName);
                }
            }

            IEnumerable<QuestConnectionRequirement> allRequirements = repo.QuestConnectionRequirements.Where(q => q.QuestId == questId);

            foreach (var p in allRequirements)
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

            foreach (var q in repo.QuestPlayerVariablees.Where(v => v.PlayerId == playerId && v.QuestId == questId).ToList())
            {
                repo.DeleteQuestPlayerVariable(q.Id);
            }

        }

        public static int GetLastTurnQuestEnded(Player player, int questId)
        {
            IQuestRepository repo = new EFQuestRepository();
            IEnumerable<int> turns = repo.QuestPlayerStatuses.Where(p => p.PlayerId == player.Id && p.QuestId == questId).Select(s => s.LastEndedTurn);

            if (!turns.Any())
            {
                return -9999;
            } else
            {
                return turns.Max();
            }
           
        }
    }
}