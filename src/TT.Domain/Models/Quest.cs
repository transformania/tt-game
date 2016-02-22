using System;
using System.Collections.Generic;

namespace TT.Domain.Models
{

    /// <summary>
    /// A QuestStart holds the information about the origin of a quest including where the quest can be found, player level / round turn elapsement requirements, and which QuestState to launch into.
    /// </summary>
    public class QuestStart
    {
        public int Id { get; set; }

        /// <summary>
        /// The dbName of this quest
        /// </summary>
        public string dbName { get; set; }

        /// <summary>
        /// The human-readable name of this quest
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The location in which this quest is started
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// The earliest round turn number that must be elapsed for players to be able to begin this quest
        /// </summary>
        public int MinStartTurn { get; set; }

        /// <summary>
        /// The highest round turn number that can be elapsed for players to able to begin this quest
        /// </summary>
        public int MaxStartTurn { get; set; }

        /// <summary>
        /// The lowest level the player can be before this quest becomes unavailable to them.
        /// </summary>
        public int MinStartLevel { get; set; }

        /// <summary>
        /// The highest level the player can be before this quest becomes unavailable to them.
        /// </summary>
        public int MaxStartLevel { get; set; }

        /// <summary>
        /// The Id of the QuestStart of the quest that must be completed before this quest can be started.  Set to 0 if there is no prerequisite.
        /// </summary>
        public int PrerequisiteQuest { get; set; }

        /// <summary>
        /// The sex the player must be in order to begin this quest
        /// </summary>
        public int RequiredGender { get; set; }

        /// <summary>
        /// Id of the first QuestState that the player starts in when beginning this quest
        /// </summary>
        public int StartState { get; set; }

        /// <summary>
        /// Determines whether or not players can begin the quest or not
        /// </summary>
        public bool IsLive { get; set; }

        /// <summary>
        /// Words to describe the themes involved in the quest, ie 'TG', 'inanimate', 'farm', 'vore', etc
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// A short description/introduction of the quest that players can see before engaging on the quest itself
        /// </summary>
        public string Description { get; set; }

    }

    public class QuestState
    {
        public int Id { get; set; }
        public string QuestStateName { get; set; }
        public int QuestId { get; set; }
        public string Text { get; set; }
        public virtual List<QuestConnectionRequirement> QuestConnectionRequirements { get; set; }
        public virtual List<QuestStatePreaction> QuestStatePreactions { get; set; }
        public virtual List<QuestEnd> QuestEnds { get; set; }
        public int QuestEndId { get; set; }
        public bool HideIfRequirementsNotMet { get; set; }

        /// <summary>
        /// Short area the quest writers can use to keep track of elements of this quest state.  Questing players do not see this.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Set whether or not the diagram should pin this quest state for easier viewing on the state/connection  diagram
        /// </summary>
        public bool PinToDiagram { get; set; }

        /// <summary>
        /// X position on the diagram this state is pinned to, if set
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y position on the diagram this state is pinned to, if set
        /// </summary>
        public float Y { get; set; }
    }


    /// <summary>
    ///  A connection between two quest states.
    /// HideIfRequirementsNotMet --  
    /// RankInList -- 
    /// </summary>
    public class QuestConnection
    {
        public int Id { get; set; }

        /// <summary>
        /// Id of the quest state this connection comes from
        /// </summary>
        public int QuestStateFromId { get; set; }

        /// <summary>
        /// Id of the quest state this connection goes to
        /// </summary>
        public int QuestStateToId { get; set; }

        /// <summary>
        /// Id of the quest state this connection goes to if a roll is failed
        /// </summary>
        public int QuestStateFailToId { get; set; }

        /// <summary>
        /// The text shown to the player when given the choice to go down this quest connection, ie "Open the mailbox".
        /// </summary>
        public string ActionName { get; set; }

        /// <summary>
        /// A name given to this quest connection for the author's development and organizational purposes.
        /// </summary>
        public string ConnectionName { get; set; }

        /// <summary>
        ///  Id of the quest this connection belongs to
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// A list of all the requirements a player must pass or roll for to be eligible for going down this quest connection
        /// </summary>
        public virtual List<QuestConnectionRequirement> QuestConnectionRequirements { get; set; }

        /// <summary>
        /// Do not show this option to the player if the requirements for this connection have not been met.
        /// </summary>
        public bool HideIfRequirementsNotMet { get; set; }

        /// <summary>
        /// A number to sort  this option from others.  Ie, a connection with rank 10 will appear above rank 8.
        /// </summary>
        public int RankInList { get; set; }

        /// <summary>
        /// Short area the quest writers can use to keep track of elements of this quest connection.  Questing players do not see this.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Optional text to display to the user when traversing down this connection.  This text will print out above the QuestState text immediately after and only immediately after performing the previous choice; consecutive reloads will no longer display it.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Returns true if at least one of the connection requirements is a random roll
        /// </summary>
        /// <returns></returns>
        public bool RequiresRolls()
        {
            if (this.QuestConnectionRequirements == null)
            {
                return false;
            } 

            foreach (QuestConnectionRequirement q in this.QuestConnectionRequirements)
            {
                if (q.IsRandomRoll==true)
                {
                    return true;
                }
            }

            return false;
        }

    }


    /// <summary>
    /// A condition that must be fulfilled for a quest connection to be available for a player.
    /// </summary>
    public class QuestConnectionRequirement
    {
        public int Id { get; set; }

        /// <summary>
        /// Quest Connection this requirement is for
        /// </summary>
        public virtual QuestConnection QuestConnectionId { get; set; }
        public int RequirementType { get; set; }

        /// <summary>
        /// Name of the variable to use when variable is selected
        /// </summary>
        public string VariabledbName { get; set; }

        /// <summary>
        /// Mathematical comparison for the RequirementValue to check against, ie greater than, less than, equal
        /// </summary>
        public int Operator { get; set; }

        /// <summary>
        /// Value the operator is comparing against.
        /// </summary>
        public string RequirementValue { get; set; }

        /// <summary>
        /// Id of the Quest this connection requirement belongs to
        /// </summary>
        public int QuestId { get; set; }

        /// <summary>
        /// Name given to this requirement by the quest author for internal organizational purposes
        /// </summary>
        public string QuestConnectionRequirementName { get; set; }

        /// <summary>
        /// This boolean determines if this requirement is a strict requirement or a chance-based dice roll.
        /// </summary>
        public bool IsRandomRoll { get; set; }

        /// <summary>
        /// A numerical value that is used to weight against a given requirement type (such as Luck).  Ie, .75 * Luck = % chance of success
        /// </summary>
        public float RollModifier { get; set; }

        /// <summary>
        /// A numerical offset when making a requirement roll to make the base chance easier or harder.  Ie, an offset of 10 gives a 10% boost to any roll the player makes.
        /// </summary>
        public float RollOffset { get; set; }
        public object QuestStatics { get; private set; }

        /// <summary>
        /// Constructor; initialized a few defaults
        /// </summary>
        public QuestConnectionRequirement()
        {
            this.RollModifier = 1;
            this.RollOffset = 0;
        }

        /// <summary>
        /// Return the operator this requirement uses as a string
        /// </summary>
        /// <returns></returns>
        public string PrintOperatorAsString()
        {

            switch (this.Operator)
            {
                case (int)TT.Domain.Statics.QuestStatics.Operator.Less_Than:
                    {
                        return "<";
                    }
                case (int)TT.Domain.Statics.QuestStatics.Operator.Less_Than_Or_Equal:
                    {
                        return "<=";
                    }
                case (int)TT.Domain.Statics.QuestStatics.Operator.Equal_To:
                    {
                        return "=";
                    }
                case (int)TT.Domain.Statics.QuestStatics.Operator.Greater_Than:
                    {
                        return ">";
                    }
                case (int)TT.Domain.Statics.QuestStatics.Operator.Greater_Than_Or_Equal:
                    {
                        return ">=";
                    }
                case (int)TT.Domain.Statics.QuestStatics.Operator.Not_Equal_To:
                    {
                        return "!=";
                    }
            }
            return "";
        }

        /// <summary>
        /// Print the name of the requirement as a string
        /// </summary>
        /// <returns></returns>
        public string PrintRequirementStatAsString()
        {
            switch (this.RequirementType)
            {
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Agility:
                    {
                        return "Agility";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Allure:
                    {
                        return "Restoration";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Charisma:
                    {
                        return "Charisma";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Discipline:
                    {
                        return "Discipline";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Fortitude:
                    {
                        return "Fortitude";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Luck:
                    {
                        return "Luck";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Magicka:
                    {
                        return "Magicka";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Perception:
                    {
                        return "Perception";
                    }
                case (int)TT.Domain.Statics.QuestStatics.RequirementType.Succour:
                    {
                        return "Regeneration";
                    }
            }
            return "";
        }

    }

    public class QuestEnd
    {
        public int Id { get; set; }
        public virtual QuestState QuestStateId { get; set; }

        /// <summary>
        /// End Type; currently only Passed or Failed
        /// </summary>
        public int EndType { get; set; }

        /// <summary>
        /// Type of the reward this QuestEnd gives:  XP, an effect, item, or spell
        /// </summary>
        public int RewardType { get; set; }

        /// <summary>
        /// The amount of XP to be given, or else the name of the item/effect/spell
        /// </summary>
        public string RewardAmount { get; set; }

        /// <summary>
        /// Name of the Quest End, used only for internal author purposes
        /// </summary>
        public string QuestEndName { get; set; }

        /// <summary>
        /// Id of the Quest this end belongs to
        /// </summary>
        public int QuestId { get; set; }

    }

    /// <summary>
    /// Something that is done immediately when a player enters a quest state, ie change their form, alter their willpower or mana, set a variable, etc.
    /// </summary>
    public class QuestStatePreaction
    {
        public int Id { get; set; }
        public virtual QuestState QuestStateId { get; set; }
        public int QuestId { get; set; }
        public string QuestStatePreactionName { get; set; }
        public int ActionType { get; set; }
        public string ActionValue { get; set; }
        public int AddOrSet { get; set; }
        public string VariableName { get; set; }
    }

    //[QuestStatePreAction]
    //Something to do before the start of a state, such as setting a variable, giving an item, etc.
    //[Id] -- int
    //[QuestId] -- int.  The Id of the quest this belongs to
    //[QuestStateId]  - int.  Id of the quest state this is a variable for
    //[ActionType] - string.  What action type to take, ie “setVariable”
    //[VariabledbName] - string.  What variable name to alter (if altering a variable)
    //[ActionSetValue] -string  Sets the value of a variable
    //[ActionAlterValue] - int.  Amount to alter (raise or lower) the value of a variable

    public class QuestWriterLog
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public int QuestId { get; set; }

    }

    public class QuestPlayerStatus
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int QuestId { get; set; }
        public int Outcome { get; set; }
        public int StartedTurn { get; set; }
        public int LastEndedTurn { get; set; }
    }

//    [PlayerQuests]
//    This table is used to keep track of which players have done which quests and their outcomes.
//    [Id] -- int
//    [PlayerId] - int.  Database name of the quest
//    [QuestId] -- int.  The quest that this refers to
//    [Outcome] - string.  Either “completed” or “failed.”
//[StartedTurn] - int.  The game turn that the player began this quest.
//    [CompletedOrFailedTurn] - int.  The game turn that the player either last failed or completed this quest.

    public class QuestPlayerVariable
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int QuestId { get; set; }
        public string VariableName { get; set; }
        public string VariableValue { get; set; }
    }

    public class QuestWriterPermission
    {
        public int Id { get; set; }
        public int QuestId { get; set; }
        public string PlayerMembershipId { get; set; }
        public int PermissionType { get; set; }
    }
}

