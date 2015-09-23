using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tfgame.dbModels.Models
{
    public class QuestStart
    {
        public int Id { get; set; }
        public string dbName { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public int MinStartTurn { get; set; }
        public int MaxStartTurn { get; set; }
        public int MinStartLevel { get; set; }
        public int MaxStartLevel { get; set; }
        public int PrerequisiteQuest { get; set; }
        public int RequiredGender { get; set; }
        public int StartState { get; set; }
        public bool IsLive { get; set; }

        //[Id] -- int
        //[dbName] - string.  Database name of the quest
        //[Name] - string.  Friendly(human-readable) name of the quest
        //[Location] - string.  Where the quest is launched from.
        //[MinStartTurn] - int.  Minimum turn a player can embark on the quest
        //[MaxStartTurn] - int Maximum turn a player can embark on the quest
        //[RequiredGender] - string, “male”, “female”, or “neutral”.  Gender required to start the quest.  If quests must be started at custom form there will need to be some way to do a gender swap.
        //[MinStartLevel] - int.  Minimum level the player must be to start this quest
        //[MaxStartLevel] - int.  Maximum level the player can be to start the quest.
        //[PrerequisiteQuest] - string.  A previous quest that must be completed before this one can be attempted
        //[StartState] - string.  The first state that the quest immediately launches into
    }

    public class QuestState
    {
        public int Id { get; set; }
        public int ParentQuestStateId { get; set; }
        public string QuestStateName { get; set; }
        public int QuestId { get; set; }
        public string Text { get; set; }
        public virtual List<QuestStateRequirement> QuestStateRequirements { get; set; }
        public virtual List<QuestEnd> QuestEnds { get; set; }
        public int SuccessQuestStateId { get; set; }
        public string ChoiceText { get; set; }
        public int QuestEndId { get; set; }
    }

    //[QuestState]
    //This table has all of the nodes that a quest can contain in addition to the requirements to getting for it.Each state has a block of text and further states to pursue, some of which can be conditional.
    //[Id] -- int
    //[QuestId] -- int.  The Id of the quest this belongs to (FK)
    //[Text] - string.  The text that is displayed when a player is at this state of the quest
    //[Image] - string.  URL of an image to accompany this state if there is one
    //[NextStates] - All of the possible actions that a player can make in this state of the quest.
    //[MandatoryRequirements] - All of the requirements (ie, minimum Luck of 100 and minimum Charisma of 100) necessary to choose this quest state
    //[RollRequirements] - Roll-based requirements for entering this quest state.  This means a requirement is not a hard requirement but can be rolled against.  For example a player with 50 luck
    //[FailStateId] - string.  QuestState to go to instead if the attempt to get to this one failed via a not - met requirement
    //[ChoiceText] - string.The text to be displayed when given the option to enter this quest state, ie “Open the door” or “Run back outside.”
    //[QuestEndId] - int.  The quest end to run when this state is reached (when this quest state is a possible completion state.)



    public class QuestStateRequirement
    {
        public int Id { get; set; }
        public virtual QuestState QuestStateId { get; set; }
        public int RequirementType { get; set; }
        public string VariabledbName { get; set; }
        public int Operator { get; set; }
        public string RequirementValue { get; set; }
        public int QuestId { get; set; }
        public string QuestStateRequirementName { get; set; }

        //    [QuestStateRequirement]
        //    Keeps track of all the requirements in order for a player to see an option to take after reading a quest state’s text.
        //    [Id] -- int
        //    [QuestStateId]  - int.  Id of the quest state this is a requirement for
        //[RequirementType] -- string.  Type of requirement, ie Luck, Charisma, hasItem, variable, etc
        //    [VariabledbName] -- string.  Name of the variable (if used.)
        //[Operator] -- int.  Choices are<. >, ==,  !=, IS, IS NOT
        //[RequirementValue] -- string/int?.  The value to check against.Can be a number (100 Luck) or equality(true)
    }

    public class QuestEnd
    {
        public int Id { get; set; }
        public virtual QuestState QuestStateId { get; set; }
        public int EndType { get; set; }
        public string RewardType { get; set; }
        public string RewardAmount { get; set; }
        public string QuestEndName { get; set; }
        public int QuestId { get; set; }

        //[QuestEnd]
        //A possible ending for a quest, whether good or bad.If the ending is good then the reward can be one of several items.
        //[Id] -- int.
        //[QuestStateId]  - int.  Id of the quest state this is a requirement for
        //[EndType] - int.  How the quest ends.  Choices are “completed ” or “failed”.
        //[RewardType] - string.  What type of reward this quest completion ends (if completed.)  Options include XP, Item, Spell, or GiveEffect.
        //[RewardAmount] - string/int? How much XP / how many items / which spell or effect to give
    }
}

