using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using tfgame.dbModels.Abstract;
using tfgame.dbModels.Concrete;
using tfgame.dbModels.Models;
using tfgame.Statics;
using tfgame.ViewModels;

namespace tfgame.Procedures
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

            QuestState dbQuestState = repo.QuestStates.FirstOrDefault(q => q.Id == Id);

            if (dbQuestState.QuestEnds==null)
            {
                dbQuestState.QuestEnds = new List<QuestEnd>();
            }

            return dbQuestState;
        }

        public static QuestConnection GetQuestConnection(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            QuestConnection dbQuestConnection = repo.QuestConnections.FirstOrDefault(q => q.Id == Id);

            return dbQuestConnection;
        }

        public static IEnumerable<QuestConnection> GetConnectionsFromQuestState(int questStateId)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestConnections.Where(q => q.QuestStateFromId == questStateId);
        }

        public static IEnumerable<QuestConnection> GetConnectionsToQuestState(int questStateId)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestConnections.Where(q => q.QuestStateToId == questStateId);
        }

        public static IEnumerable<QuestConnection> GetChildQuestConnections(int Id)
        {
            IQuestRepository repo = new EFQuestRepository();

            return repo.QuestConnections.Where(q => q.QuestStateFromId == Id);
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

                    // TODO:  Filter out quests that have a prerequisite not yet fulfilled
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
                        message += " < br/>You received a <b>" + item.FriendlyName + "</b>.";
                    }

                    // effect gain
                    else if (q.RewardType == (int)QuestStatics.RewardType.Effect)
                    {
                        DbStaticEffect effect = EffectStatics.GetEffect(q.RewardAmount);
                        EffectProcedures.GivePerkToPlayer(effect.dbName, player.Id);
                        message += "<br/>You received the effect <b>" + effect.FriendlyName + "</b>.";
                    }

                    // spell gain
                    else if (q.RewardType == (int)QuestStatics.RewardType.Effect)
                    {
                        DbStaticSkill spell = SkillStatics.GetStaticSkill(q.RewardAmount);
                        SkillProcedures.GiveSkillToPlayer(player.Id, q.RewardAmount);
                        message += "<br/>You learned the spell <b>" + spell.FriendlyName + "</b>.";
                    }

                }

                if (xpGain > 0)
                {
                    message += "<br/>You earned <b>" + xpGain + "</b> XP.";
                }

                PlayerProcedures.GiveXP(player, xpGain);
            }

            // delete all of the player's quest variables
            List<QuestPlayerVariable> vars = QuestProcedures.GetAllQuestPlayerVariablesFromQuest(player.InQuest, player.Id).ToList();
            foreach (QuestPlayerVariable v in vars)
            {
                questRepo.DeleteQuestPlayerVariable(v.Id);
            }

            return message;

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

            bool isAvailable = true;

            if (questConnection.QuestStateFromId < 0 || questConnection.QuestStateToId < 0)
            {
                return false;
            }

            foreach (QuestConnectionRequirement q in questConnection.QuestConnectionRequirements)
            {

                // skip all roll-based requirements; a player can always attempt a roll
                if (q.IsRandomRoll == true)
                {
                    continue;
                }

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

        public static string GetRequirementsAsString(QuestConnection q, BuffBox buffs)
        {

            string output = "";

            if (q.QuestConnectionRequirements.Count()==0)
            {
                return output;
            }

            int len = q.QuestConnectionRequirements.Count();
            int i = 0;

            output += "[";

            foreach (QuestConnectionRequirement qs in q.QuestConnectionRequirements.ToList())
            {
                // don't print anything for variables or gender requirements
                if (qs.RequirementType == (int)QuestStatics.RequirementType.Variable || qs.RequirementType == (int)QuestStatics.RequirementType.Gender)
                {
                    continue;
                }

                // random roll, calculate % chance and display that
                if (qs.IsRandomRoll == true)
                {
                    float playerValue = GetValueFromType(qs, buffs);

                    double chance = Math.Round(qs.RollModifier * playerValue + qs.RollOffset,1);

                    if (chance < 0)
                    {
                        chance = 0;
                    }
                    else if (chance > 100)
                    {
                        chance = 100;
                    }

                    output += Enum.GetName(typeof(QuestStatics.RequirementType), qs.RequirementType) + " - " + chance + "%";

                }

                // strict requirement
                else
                {
                    output += qs.RequirementValue + " " + Enum.GetName(typeof(QuestStatics.RequirementType), qs.RequirementType);
                }

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
            Match match = Regex.Match(input, ImageRegexPattern);
            string imgName = match.Groups[1].Value;

            Regex rgx = new Regex(ImageRegexPattern);
            input = Regex.Replace(input, ImageRegexPattern, "<img src='/Images/PvP/quests/" + imgName + "' />");

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
                    dbPlayer.Mobility = newForm.MobilityType;
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

        /// <summary>
        /// Returns true if a player passes all rolls for a quest connection.
        /// </summary>
        /// <param name="questConnection"></param>
        /// <param name="player">Player attempting to go down this connection</param>
        /// <param name="buffs">Player's statistics from effects and equipment</param>
        /// <param name="variables">All quest variables created by this player previously in the quest</param>
        /// <returns></returns>
        public static bool RollForQuestConnection(QuestConnection connection, Player player, BuffBox buffs, IEnumerable<QuestPlayerVariable> variables)
        {
           
            foreach(QuestConnectionRequirement q in connection.QuestConnectionRequirements)
            {
                if (q.IsRandomRoll==false)
                {
                    continue;
                }

                float playerValue = GetValueFromType(q, buffs);

                float chance = q.RollModifier * playerValue + q.RollOffset;

                Random r = new Random();
                double roll = r.NextDouble()*100;

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

            IEnumerable<QuestConnectionRequirement> allRequirements = repo.QuestConnectionRequirements.Where(q => q.QuestId == questId);

            foreach (QuestConnectionRequirement p in allRequirements)
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