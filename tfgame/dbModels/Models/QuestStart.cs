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
        public int StartState { get; set; }
    }
}

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